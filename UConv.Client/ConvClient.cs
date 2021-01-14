using UConv.Core;
using System.Net.Sockets;

namespace UConv.Client
{
    class ConvClient : UTcpClient
    {
        public ConvClient(
            string hostname,
            int port
        ) : base(hostname, port)
        { }

        public ConvResponse ConvertRequest(string converter, string inputUnit, string outputUnit, string value)
        {
            using (NetworkStream ns = GetStream())
            {
                var resp = writeRequest<ConvRequest, ConvResponse>(ns, "/convert", new ConvRequest(converter, inputUnit, outputUnit, value));
                return resp;
            }
        }

        public ConvListResponse ConverterListRequest()
        {
            using (NetworkStream ns = GetStream())
            {
                var resp = writeRequest<ConvListRequest, ConvListResponse>(ns, "/converters", new ConvListRequest());
                return resp;
            }
        }
    }
}
