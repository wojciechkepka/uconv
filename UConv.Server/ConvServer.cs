using System;
using System.Collections.Generic;
using System.Text;
using UConv.Core;
using static UConv.Core.Units;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace UConv.Server
{
    class ConvServer : MessageServer
    {
        protected List<IConverter<double, Unit>> converters = new List<IConverter<double, Unit>>
        {
            new DistanceConverter(),
            new MassConverter(),
            new CurrencyConverter(),
            new MassConverter(),
            new SpeedConverter(),
        };
        protected TimeConverter tConv = new TimeConverter();

        public ConvServer(string hostname, int port) : base(hostname, port, 500, 10)
        { }

        protected override void OnHandleMessage(NetworkStream ns, string message)
        {
            int dataStart = message.IndexOf('<');
            string path = message[..dataStart];
            message = message[dataStart..];
            byte[] data;

            Console.WriteLine($"[{DateTime.Now}] {path}");
            switch (path)
            {
                case "/convert":
                    var convReq = Request.FromData<ConvRequest>(message);
                    var convResp = convertMethod(convReq);
                    data = convResp.ToXmlBinary<ConvResponse>();
                    break;
                case "/converters":
                    var convlReq = Request.FromData<ConvListRequest>(message);
                    var convlResp = converterListMethod(convlReq);
                    data = convlResp.ToXmlBinary<ConvListResponse>();
                    break;
                default:
                    data = new byte[1];
                    break;
            }

            var writer = new StreamWriter(ns);
            writer.WriteLine(Encoding.ASCII.GetString(data));
            writer.Flush();
            ExitSignal = true;
        }

        public static void Main(string[] args)
        {
            var addr = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
            var port = 7001;
            ConvServer server = new ConvServer(addr, port);
            Console.WriteLine($"Listening on {addr}:{port}");
            server.Listen();
        }

        private ConvResponse convertMethod(ConvRequest request)
        {
            switch (request.converter)
            {
                case "Time":
                    Tuple<string, TimeFormat> ret = tConv.Convert(
                        request.value,
                        TimeFormatFromString(request.inputUnit),
                        TimeFormatFromString(request.outputUnit)
                    );
                    return new ConvResponse { status = true, value = ret.Item1 };
                default:
                    Double val = Double.Parse(request.value);
                    foreach (IConverter<double, Unit> iconv in converters)
                    {
                        if (iconv.Name == request.converter)
                        {
                            Tuple<double, Unit> ret2 = iconv.Convert(
                                val,
                                UnitFromString(request.inputUnit),
                                UnitFromString(request.outputUnit)
                             );

                            return new ConvResponse { status = true, value = ret2.Item1.ToString() };
                        }
                    }
                    break;
            }
            return new ConvResponse { status = false, value = $"Converter {request.converter} not found." };
        }

        private ExchangeRateResponse exchangeMethod(ExchangeRateRequest _)
        {
            return new ExchangeRateResponse { status = false, rates = null };
        }

        private ConvListResponse converterListMethod(Request _)
        {
            var convs = new Dictionary<String, List<Unit>> { };

            foreach (IConverter<double, Unit> iconv in converters)
            {
                if (convs.ContainsKey(iconv.Name)) continue;
                convs.Add(iconv.Name, iconv.SupportedUnits);
            }
            // TODO: fixme
            convs.Add(tConv.Name, new List<Unit> { });

            return new ConvListResponse { status = true, converters = convs };
        }
    }
}
