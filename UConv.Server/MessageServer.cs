using System.IO;
using System.Net.Sockets;

namespace UConv.Server
{


    class MessageServer : UTcpServer
    {
        protected readonly MessageDelegate OnMessage;
        public delegate void MessageDelegate(NetworkStream ns, string message);

        public MessageServer(string hostname, int port, int timeout, int maxListeners) : base(hostname, port, timeout, maxListeners)
        {
            OnMessage += OnHandleMessage;
        }

        public MessageServer(MessageDelegate onMessageHandler, string hostname, int port, int timeout, int maxListeners) : base(hostname, port, timeout, maxListeners)
        {
            OnMessage += onMessageHandler;
        }

        protected override void OnHandleConnection(NetworkStream ns)
        {
            var isOk = ns.CanRead && ns.CanWrite;
            if (!isOk) return;
            var reader = new StreamReader(ns);
            string data;

            try
            {
                data = reader.ReadLine();
            }
            catch (IOException ex)
            {
                _ = ex;
                return;
            }

            OnMessage.Invoke(ns, data);
        }

        // Do nadpisania przed nadrzędną klasę
        protected virtual void OnHandleMessage(NetworkStream ns, string message)
        { }
    }
}
