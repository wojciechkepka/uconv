using System;
using System.IO;
using UConv.Core;

namespace UConv.Client
{
    internal class ConvClient : UTcpClient
    {
        public ConvClient(
            string hostname,
            int port
        ) : base(hostname, port)
        {
        }

        private Response HandleRequest<I, O>(string path, Request req)
            where I : Request
            where O : Response
        {
            Connect();
            using (var ns = GetStream())
            {
                if (!ns.CanRead || !ns.CanWrite) throw new IOException("Can't write to or read from tcp connection");
                var data = writeRequest<I>(ns, path, req);
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
    }
}