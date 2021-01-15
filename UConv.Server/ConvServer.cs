using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UConv.Core;
using static UConv.Core.Units;

namespace UConv.Server
{
    internal class ConvServer : MessageServer
    {
        protected List<IConverter<double, Unit>> converters = new()
        {
            new DistanceConverter(),
            new MassConverter(),
            new CurrencyConverter(),
            new MassConverter(),
            new SpeedConverter()
        };

        protected TimeConverter tConv = new();

        public ConvServer(string hostname, int port) : base(hostname, port, 500, 10)
        {
        }


        protected override void OnHandleMessage(NetworkStream ns, string message)
        {
            var sw = new Stopwatch();
            sw.Start();
            var dataStart = message.IndexOf('<');
            var path = message[..dataStart];
            message = message[dataStart..];
            byte[] data;
            var isErr = false;


            void HandleFunc<I, O>(Func<I, Response> method)
                where I : Request
                where O : Response
            {
                var req = Request.FromData<I>(message);
                var resp = method(req);
                data = resp.ToXmlBinary<O>();
                if (resp.GetType() == typeof(ErrResponse))
                {
                    data = resp.ToXmlBinary<ErrResponse>();
                    isErr = true;
                }
                else
                {
                    data = resp.ToXmlBinary<O>();
                }
            }

            Console.Write(
                $"[{DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)}][{path}]"); // iso-8601 timestamp
            try
            {
                switch (path)
                {
                    case "/convert":
                        HandleFunc<ConvRequest, ConvResponse>(convertMethod);
                        break;
                    case "/converters":
                        HandleFunc<ConvListRequest, ConvListResponse>(converterListMethod);
                        break;
                    case "/rateme":
                        HandleFunc<RateMeRequest, RateMeResponse>(rateMeMethod);
                        break;
                    case "/last_rating":
                        HandleFunc<LastRatingRequest, LastRatingResponse>(lastRatingMethod);
                        break;
                    case "/clear_data":
                        HandleFunc<ClearDataRequest, ClearDataResponse>(clearDataMethod);
                        break;
                    default:
                        var resp = new ErrResponse($"Invalid route to `{path}`");
                        data = resp.ToXmlBinary<Response>();
                        isErr = true;
                        break;
                }

                var writer = new StreamWriter(ns)
                {
                    AutoFlush = true
                };
                writer.WriteLine(Encoding.ASCII.GetString(data));

                sw.Stop();
                Console.Write($"[{sw.ElapsedMilliseconds} ms]");
                if (isErr) Console.Write(" ERR\n");
                else Console.Write(" OK\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ;
            }
        }

        public static void Main(string[] args)
        {
            var addr = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
            var port = 7001;
            var server = new ConvServer(addr, port);
            Console.WriteLine($"Listening on {addr}:{port}");
            server.Listen();
        }

        private Response convertMethod(ConvRequest request)
        {
            try
            {
                switch (request.converter)
                {
                    case "Time":
                        var ret = tConv.Convert(
                            request.value,
                            TimeFormatFromString(request.inputUnit),
                            TimeFormatFromString(request.outputUnit)
                        );
                        return new ConvResponse(ret.Item1);
                    default:
                        var val = double.Parse(request.value);
                        foreach (var iconv in converters)
                            if (iconv.Name == request.converter)
                            {
                                var ret2 = iconv.Convert(
                                    val,
                                    UnitFromName(request.inputUnit),
                                    UnitFromName(request.outputUnit)
                                );
                                var t = new Thread(() =>
                                {
                                    try
                                    {
                                        using (var context = new UConvDbContext())
                                        {
                                            context.Records.Add(new Record
                                            {
                                                hostname = Dns.GetHostName(),
                                                date = DateTime.Now,
                                                converter = iconv.Name,
                                                inputValue = val,
                                                inputUnit = request.inputUnit,
                                                outputValue = ret2.Item1,
                                                outputUnit = request.outputUnit
                                            });
                                            context.SaveChanges();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Failed to save record to db - {ex.Message}");
                                    }
                                });
                                t.Start();

                                return new ConvResponse(ret2.Item1.ToString());
                            }

                        break;
                }

                return new ErrResponse($"Converter {request.converter} not found.");
            }
            catch (FormatException ex)
            {
                return new ErrResponse($"Invalid input number - {ex.Message}");
            }
            catch (Exception ex)
            {
                return new ErrResponse($"Unhandled exception - {ex.Message}");
            }
        }

        private Response exchangeMethod(ExchangeRateRequest _)
        {
            return new ErrResponse("unimplemented");
        }

        private Response converterListMethod(Request _)
        {
            var convs = new Dictionary<string, List<Unit>>();

            foreach (var iconv in converters)
            {
                if (convs.ContainsKey(iconv.Name)) continue;
                convs.Add(iconv.Name, iconv.SupportedUnits);
            }

            // TODO: fixme
            convs.Add(tConv.Name, new List<Unit>());

            return new ConvListResponse(convs);
        }

        private Response rateMeMethod(RateMeRequest request)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var context = new UConvDbContext())
                    {
                        var record = context.Ratings.First(r => r.name == request.hostname);
                        if (record == null)
                            context.AddRating(new Rating {name = request.hostname, rating = request.rating});
                        else
                            record.rating = request.rating;
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save rating to db - {ex.Message}");
                }
            });
            t.Start();

            return new RateMeResponse();
        }

        private Response lastRatingMethod(LastRatingRequest request)
        {
            try
            {
                using (var context = new UConvDbContext())
                {
                    var record = context.Ratings.First(r => r.name == request.hostname);
                    if (record != null) return new LastRatingResponse(record.date, record.name, record.rating);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save rating to db - {ex.Message}");
            }

            return new LastRatingResponse(DateTime.Now, Dns.GetHostName(), 0);
        }

        private Response clearDataMethod(ClearDataRequest request)
        {
            try
            {
                using (var context = new UConvDbContext())
                {
                    context.Ratings.RemoveRange(context.Ratings.Where(r => r.name == request.hostname));
                    context.Records.RemoveRange(context.Records.Where(r => r.hostname == request.hostname));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save rating to db - {ex.Message}");
            }

            return new LastRatingResponse(DateTime.Now, Dns.GetHostName(), 0);

        }
    }
}