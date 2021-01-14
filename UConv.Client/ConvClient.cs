using UConv.Core;
using System.Net.Sockets;
using System;

namespace UConv.Client
{
    class ConvClient : UTcpClient
    {
        public ConvClient(
            string hostname,
            int port
        ) : base(hostname, port)
        { }

        public Response ConvertRequest(string converter, string inputUnit, string outputUnit, string value)
        {
            using (NetworkStream ns = GetStream())
            {
                var resp = writeRequest<ConvRequest>(ns, "/convert", new ConvRequest(converter, inputUnit, outputUnit, value));
                try
                {
                    return (ConvResponse)Response.FromData<ConvResponse>(resp);
                }
                catch (Exception _)
                {
                    return (ErrResponse)Response.FromData<ErrResponse>(resp);
                }
            }
        }

        public Response ConverterListRequest()
        {
            using (NetworkStream ns = GetStream())
            {
                var resp = writeRequest<ConvListRequest>(ns, "/converters", new ConvListRequest());
                try
                {
                    return (ConvListResponse)Response.FromData<ConvListResponse>(resp);
                }
                catch (Exception _)
                {
                    return (ErrResponse)Response.FromData<ErrResponse>(resp);
                }
            }
        }
    }
}
