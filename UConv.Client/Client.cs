using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UConv.Core;
using System.Net.Sockets;
using System.Threading;

namespace UConv.Client
{
    class ConvClient : SocketEntity
    {

        public ConvClient(string hostname, int port) : base(hostname, port)
        { socket.Connect(endpoint); }

        private T readResponse<T>()
        where T : Response
        {
            while (true)
            {
                byte[] response = ReadBytes(socket);
                if (response.Length == 0)
                {
                    Thread.Sleep(2);
                    continue;
                }
                String data = Encoding.ASCII.GetString(response, 0, response.Length);
                return Response.FromData<T>(data[..(data.Length - 5)]); // - <EOF>
            }

        }


        private void writeRequest<T>(Socket handler, string route, Request req)
        where T : Message
        {
            var data = req.ToXmlBinary<T>();
            WriteBytes(handler, route, data);
        }

        public ConvResponse ConvertRequest(string converter, string inputUnit, string outputUnit, string value)
        {
            writeRequest<ConvRequest>(socket, "/convert", new ConvRequest(converter, inputUnit, outputUnit, value));
            var resp = readResponse<ConvResponse>();
            return resp;
        }

        public ConvListResponse ConverterListRequest()
        {
            writeRequest<ConvListRequest>(socket, "/converters", new ConvListRequest());
            var resp = readResponse<ConvListResponse>();
            return resp;
        }
    }
}
