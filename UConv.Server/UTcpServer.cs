using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace UConv.Server
{
    class UTcpServer
    {
        public delegate void ConnectionHandlerDelegate(NetworkStream ns);

        protected readonly ConnectionHandlerDelegate _OnHandleConnection;

        protected List<Task> TcpClientTasks = new List<Task>();
        protected readonly int MaxConcurrentListeners;
        protected readonly int Timeout;
        protected readonly string Hostname;
        protected readonly int Port;
        protected readonly TcpListener Listener;
        protected bool IsRunning;
        private volatile bool _ExitSignal;
        public virtual bool ExitSignal
        {
            get => this._ExitSignal;
            set => this._ExitSignal = value;
        }

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
            while (!ExitSignal)
            {
                ConnectionLoop();
            }
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

            int RemoveAtIndex = Task.WaitAny(TcpClientTasks.ToArray(), Timeout);

            if (RemoveAtIndex > 0)
                TcpClientTasks.RemoveAt(RemoveAtIndex);
        }

        protected virtual void ProcessMessagesFromClient(TcpClient connection)
        {
            if (!connection.Connected) return;

            using (NetworkStream ns = connection.GetStream())
            {
                _OnHandleConnection.Invoke(ns);
            }
        }

        // Do nadpisania przez nadrzędną klasę
        protected virtual void OnHandleConnection(NetworkStream ns)
        {
        }

    }
}
