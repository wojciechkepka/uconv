using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace UConv.Server
{
    internal class UTcpServer
    {
        public delegate void ConnectionHandlerDelegate(NetworkStream ns);

        protected readonly ConnectionHandlerDelegate _OnHandleConnection;
        protected readonly string Hostname;
        protected readonly TcpListener Listener;
        protected readonly int MaxConcurrentListeners;
        protected readonly int Port;
        protected readonly int Timeout;
        private volatile bool _ExitSignal;
        protected bool IsRunning;

        protected List<Task> TcpClientTasks = new();

        public UTcpServer(
            string hostname,
            int port,
            int timeout,
            int maxListeners
        )
        {
            Hostname = hostname;
            Port = port;
            Timeout = timeout;
            MaxConcurrentListeners = maxListeners;
            var addr = IPAddress.Parse(Hostname);
            Listener = new TcpListener(addr, Port);
            _OnHandleConnection += OnHandleConnection;
        }

        public UTcpServer(
            ConnectionHandlerDelegate connectionHandler,
            string hostname,
            int port,
            int timeout,
            int maxListeners
        )
        {
            Hostname = hostname;
            Port = port;
            Timeout = timeout;
            MaxConcurrentListeners = maxListeners;
            Listener = new TcpListener(IPAddress.Parse(Hostname), Port);
            _OnHandleConnection += connectionHandler;
        }

        public virtual bool ExitSignal
        {
            get => _ExitSignal;
            set => _ExitSignal = value;
        }

        public virtual void Stop()
        {
            if (!IsRunning) return;
            ExitSignal = true;
        }

        public virtual void Listen()
        {
            if (IsRunning) return;
            Listener.Start(MaxConcurrentListeners);

            IsRunning = true;
            while (!ExitSignal) ConnectionLoop();
            IsRunning = false;
        }

        protected virtual void ConnectionLoop()
        {
            while (TcpClientTasks.Count <= MaxConcurrentListeners)
            {
                var AwaiterTask = Task.Run(async () =>
                {
                    ProcessMessagesFromClient(await Listener.AcceptTcpClientAsync());
                });
                TcpClientTasks.Add(AwaiterTask);
            }

            var RemoveAtIndex = Task.WaitAny(TcpClientTasks.ToArray(), Timeout);

            if (RemoveAtIndex > 0)
                TcpClientTasks.RemoveAt(RemoveAtIndex);
        }

        protected virtual void ProcessMessagesFromClient(TcpClient connection)
        {
            if (!connection.Connected) return;

            using (var ns = connection.GetStream())
            {
                _OnHandleConnection.Invoke(ns);
            }

            connection.Client.Close();
            connection.Close();
        }

        // Do nadpisania przez nadrzędną klasę
        protected virtual void OnHandleConnection(NetworkStream ns)
        {
        }
    }
}