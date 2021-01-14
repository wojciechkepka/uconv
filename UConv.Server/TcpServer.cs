using System;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using static UConv.Core.Units;
using UConv.Core;
using System.Threading.Tasks;
using System.Net;

namespace UConv.Server
{

    class ConvServer
    {
        protected TcpServer server;

        public ConvServer(string hostname, int port, int timeout, int maxisteners)
        {
            server = new TcpServer(MessageHandler, ConnectionHandler, hostname, port, timeout, maxisteners);
        }

        public void Start()
        {
            server.Listen();
        }

        protected virtual void ConnectionHandler(NetworkStream ns)
        { }

        protected virtual void MessageHandler(string message)
        { }
    }

    class TcpServer
    {
        public delegate void ConnectionHandlerDelegate(NetworkStream connectedAutoDisposedNetStream);
        public delegate void MessageDelegate(string message);

        protected readonly ConnectionHandlerDelegate OnHandleConnection;
        protected readonly MessageDelegate OnMessage;

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


        List<IConverter<double, Unit>> converters = new List<IConverter<double, Unit>>
        {
            new DistanceConverter(),
            new MassConverter(),
            new CurrencyConverter(),
            new MassConverter(),
            new SpeedConverter(),
        };
        TimeConverter tConv = new TimeConverter();

        public static void Main(string[] args)
        {
            //TcpServer serv = new TcpServer("localhost", 3693);
            //serv.Listen();
        }

        public TcpServer(
            MessageDelegate onMessage,
            ConnectionHandlerDelegate connectionHandler,
            string hostname,
            int port,
            int timeout,
            int maxListeners
        )
        {
            Hostname = hostname ?? throw new ArgumentNullException(nameof(host));
            Port = port;
            Timeout = timeout;
            MaxConcurrentListeners = maxListeners;
            Listener = new TcpListener(IPAddress.Parse(this.Hostname), this.Port);
        }

        public virtual void Listen()
        {
            if (IsRunning) return;

            IsRunning = true;
            while (!ExitSignal)
            {
                ConnectionLoop();
            }
            IsRunning = false;
        }

        protected virtual void ConnectionLoop()
        {
            while (TcpClientTasks.Count < MaxConcurrentListeners)
            {
                var AwaiterTask = Task.Run(async () =>
                {
                    ProcessMessagesFromClient(await this.Listener.AcceptTcpClientAsync());
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
                OnHandleConnection.Invoke(ns);
            }
        }

        //private Request readRequest(Socket handler)
        //{
        //    byte[] req = ReadBytes(handler);
        //    string data = Encoding.ASCII.GetString(req);
        //    int dataStart = data.IndexOf('<');
        //    string path = data[..dataStart];
        //    data = data[dataStart..];
        //    data = data.Replace("<EOF>", "");
        //    switch (path)
        //    {
        //        case "/convert":
        //            return Request.FromData<ConvRequest>(data);
        //        case "/converters":
        //            return Request.FromData<ConvListRequest>(data);
        //    }

        //    return null;
        //}

        //private void writeResponse<T>(Socket handler, Response resp)
        //where T : Response
        //{
        //    var data = resp.ToXmlBinary<T>();
        //    WriteBytes(handler, data);
        //}

        //private ConvResponse convertMethod(ConvRequest request)
        //{
        //    switch (request.converter)
        //    {
        //        case "Time":
        //            Tuple<string, TimeFormat> ret = tConv.Convert(
        //                request.value,
        //                TimeFormatFromString(request.inputUnit),
        //                TimeFormatFromString(request.outputUnit)
        //            );
        //            return new ConvResponse { status = true, value = ret.Item1 };
        //        default:
        //            Double val = Double.Parse(request.value);
        //            foreach (IConverter<double, Unit> iconv in converters)
        //            {
        //                if (iconv.Name == request.converter)
        //                {
        //                    Tuple<double, Unit> ret2 = iconv.Convert(
        //                        val,
        //                        UnitFromString(request.inputUnit),
        //                        UnitFromString(request.outputUnit)
        //                     );

        //                    return new ConvResponse { status = true, value = ret2.Item1.ToString() };
        //                }
        //            }
        //            break;
        //    }
        //    return new ConvResponse { status = false, value = $"Converter {request.converter} not found." };
        //}

        //private ExchangeRateResponse exchangeMethod(ExchangeRateRequest _)
        //{
        //    return new ExchangeRateResponse { status = false, rates = null };
        //}

        //private ConvListResponse converterListMethod(Request _)
        //{
        //    var convs = new Dictionary<String, List<Unit>> { };

        //    foreach (IConverter<double, Unit> iconv in converters)
        //    {
        //        if (convs.ContainsKey(iconv.Name)) continue;
        //        convs.Add(iconv.Name, iconv.SupportedUnits);
        //    }
        //    // TODO: fixme
        //    convs.Add(tConv.Name, new List<Unit> { });

        //    return new ConvListResponse { status = true, converters = convs };
        //}

        //public void Listen()
        //{
        //    Console.WriteLine($"Listening on {addr}:{port}...");
        //    while (true)
        //    {
        //        try
        //        {
        //            socket.Listen(10);
        //            Socket handler = socket.Accept();

        //            Request req = readRequest(handler);

        //            Console.WriteLine($"[{DateTime.Now}] {req.method}");
        //            switch (req.method)
        //            {
        //                case Method.Convert:
        //                    writeResponse<ConvResponse>(handler, convertMethod((ConvRequest)req));
        //                    break;
        //                case Method.ConverterList:
        //                    writeResponse<ConvListResponse>(handler, converterListMethod(req));
        //                    break;
        //                case Method.ExchangeRate:
        //                    writeResponse<ExchangeRateResponse>(handler, exchangeMethod((ExchangeRateRequest)req));
        //                    break;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.ToString());
        //        }
        //    }
        //}
    }
}
