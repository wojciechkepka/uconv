using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UConv.Core
{
    public abstract class SocketEntity
    {
        public SocketEntity(string hostname, int port)
        {
            host = Dns.GetHostEntry(hostname);
            addr = host.AddressList[0];
            this.port = port;
            endpoint = new IPEndPoint(addr, port);
            socket = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public IPHostEntry host { get; set; }
        public IPAddress addr { get; set; }
        public IPEndPoint endpoint { get; set; }
        public Socket socket { get; set; }
        public int port { get; set; }

        public static byte[] ReadBytes(Socket socket)
        {
            int bufSize = 1024, total = 0, size = bufSize;
            var bytes = new byte[size];
            var buf = new byte[bufSize];
            int received;
            while (true)
                try
                {
                    received = socket.Receive(buf);

                    if (total != 0 && received == 0) break;
                    if (received < bufSize && total == 0) return buf;

                    if (total + received >= size)
                    {
                        size *= 2;
                        Array.Resize(ref bytes, size);
                    }

                    for (var i = 0; i < bufSize; i++) bytes[total + i] = buf[i];

                    total += received;
                }
                catch (SocketException)
                {
                }

            return bytes.Take(total).ToArray();
        }

        public static void WriteBytes(Socket socket, string route, byte[] bytes)
        {
            var ro = Encoding.ASCII.GetBytes(route);
            var eof = Encoding.ASCII.GetBytes("<EOF>");
            var payload = new byte[ro.Length + bytes.Length + eof.Length];
            Buffer.BlockCopy(ro, 0, payload, 0, ro.Length);
            Buffer.BlockCopy(bytes, 0, payload, ro.Length, bytes.Length);
            Buffer.BlockCopy(eof, 0, payload, ro.Length + bytes.Length, eof.Length);
            socket.Send(payload);
        }

        public static void WriteBytes(Socket socket, byte[] bytes)
        {
            var eof = Encoding.ASCII.GetBytes("<EOF>");
            var payload = new byte[bytes.Length + eof.Length];
            Buffer.BlockCopy(bytes, 0, payload, 0, bytes.Length);
            Buffer.BlockCopy(eof, 0, payload, bytes.Length, eof.Length);
            socket.Send(payload);
        }
    }
}