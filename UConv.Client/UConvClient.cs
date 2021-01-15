using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UConv.Core;
using UConv.Core.Net;

namespace UConv.Client
{
    internal class UConvClient
    {
        public readonly string Hostname;
        public readonly int Port;
        private Queue<UTcpClient> clients;

        public UConvClient(
            string hostname,
            int port
        )
        {
            Hostname = hostname;
            Port = port;
            clients = new Queue<UTcpClient>();
        }

        private Response HandleRequest<I, O>(string path, Request req)
            where I : Request
            where O : Response
        {
            var client = new UTcpClient(Hostname, Port);
            client.Connect();
            using (var ns = client.GetStream())
            {
                if (!ns.CanRead || !ns.CanWrite) throw new IOException("Can't write to or read from tcp connection");
                var data = client.writeRequest<I>(ns, path, req);
                try
                {
                    return Response.FromData<O>(data);
                }
                catch (Exception)
                {
                    try
                    {
                        return Response.FromData<ErrResponse>(data);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidResponse(ex.Message, data);
                    }
                }
            }
        }

        public Response ConvertRequest(string converter, string inputUnit, string outputUnit, string value)
        {
            return HandleRequest<ConvRequest, ConvResponse>("/convert",
                new ConvRequest(converter, inputUnit, outputUnit, value));
        }

        public Response ConverterListRequest()
        {
            return HandleRequest<ConvListRequest, ConvListResponse>("/converters", new ConvListRequest());
        }

        public Response RateMeRequest(string hostname, int rating)
        {
            return HandleRequest<RateMeRequest, RateMeResponse>("/rateme", new RateMeRequest(hostname, rating));
        }

        public Response LastRatingRequest(string hostname)
        {
            return HandleRequest<LastRatingRequest, LastRatingResponse>("/last_rating",
                new LastRatingRequest(hostname));
        }

        public Response ClearDataRequest()
        {
            return HandleRequest<ClearDataRequest, ClearDataResponse>("/clear_data",
                new ClearDataRequest(Dns.GetHostName()));
        }

        public Response ExchangeRatesRequest(string currency)
        {
            return HandleRequest<ExchangeRateRequest, ExchangeRateResponse>("/exchange_rates",
                new ExchangeRateRequest(currency));
        }

        public Response CurrenciesListRequest()
        {
            return HandleRequest<CurrencyListRequest, CurrencyListResponse>("/currencies",
                new CurrencyListRequest());
        }

        public Response StatisticsRequest()
        {
            return HandleRequest<StatisticsRequest, StatisticsResponse>("/stats",
                new StatisticsRequest());
        }
    }
}