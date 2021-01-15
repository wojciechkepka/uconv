using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace UConv.Core.Net
{
    public class UTcpClient
    {
        protected readonly string Hostname;
        protected readonly int Port;
        protected TcpClient client;

        public UTcpClient(
            string hostname,
            int port
        )
        {
            Hostname = hostname;
            Port = port;
            client = new TcpClient();
        }

        public void Connect()
        {
            client.Connect(Hostname, Port);
        }

        public NetworkStream GetStream()
        {
            return client.GetStream();
        }

        public string writeRequest<I>(NetworkStream ns, string route, Request req)
            where I : Request
        {
            var data = req.ToXmlBinary<I>();
            using (var reader = new StreamReader(ns))
            {
                var ro = Encoding.ASCII.GetBytes(route);
                var payload = new byte[ro.Length + data.Length];
                Buffer.BlockCopy(ro, 0, payload, 0, ro.Length);
                Buffer.BlockCopy(data, 0, payload, ro.Length, data.Length);
                var writer = new StreamWriter(ns);
                writer.AutoFlush = true;
                writer.WriteLine(Encoding.ASCII.GetString(payload));
                var resp = reader.ReadLine();
                ns.Close();
                client.Dispose();
                client = new TcpClient();
                return resp;
            }
        }
    }
}