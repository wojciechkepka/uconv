using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UConv.Core;

namespace UConv.Client
{
    class UTcpClient
    {
        protected readonly string Hostname;
        protected readonly int Port;
        protected readonly TcpClient client;

        public UTcpClient(
            string hostname,
            int port
        )
        {
            Hostname = hostname;
            Port = port;
            client = new TcpClient();
        }

        protected NetworkStream GetStream()
        {
            client.Connect(Hostname, Port);
            return client.GetStream();
        }

        protected R writeRequest<T, R>(NetworkStream ns, string route, Request req)
        where T : Request
        where R : Response
        {
            var data = req.ToXmlBinary<T>();
            using (StreamReader reader = new StreamReader(ns))
            {
                byte[] ro = Encoding.ASCII.GetBytes(route);
                byte[] payload = new byte[ro.Length + data.Length];
                Buffer.BlockCopy(ro, 0, payload, 0, ro.Length);
                Buffer.BlockCopy(data, 0, payload, ro.Length, data.Length);
                var writer = new StreamWriter(ns);
                writer.WriteLine(Encoding.ASCII.GetString(payload));
                writer.Flush();
                var resp = reader.ReadLine();
                ns.Close();
                return (R)Response.FromData<R>(resp);
            }
        }
    }
}
