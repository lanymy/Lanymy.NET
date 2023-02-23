//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Lanymy.Common.ConstKeys;
//using Lanymy.Common.ExtensionFunctions;
//using Lanymy.Common.Instruments.Common;

//namespace Lanymy.Common.Instruments.SocketAsync
//{


//    public abstract class BaseTcpServer<TTcpServerClient, TSessionToken, TFixedHeaderPackageFilter, TPackage, TSendPackage> : IAsyncTcpServer
//        where TTcpServerClient : BaseAsyncTcpServerClient
//        where TSessionToken : ISessionToken
//        where TFixedHeaderPackageFilter : IFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken>
//        where TPackage : class
//        where TSendPackage : class, ISendPackageSendNum
//    {



//        public System.Net.Sockets.Socket CurrentSocket { get; protected set; }

//        public bool IsConnected
//        {
//            get
//            {
//                if (CurrentSocket.IfIsNull())
//                {
//                    return false;
//                }
//                return CurrentSocket.Connected;
//            }
//        }

//        public bool IsAccept => _IsRunning;

//        public int ReceiveBufferSize { get; }
//        public int SendBufferSize { get; }
//        public bool IsRunning => _IsRunning;
//        public int Port { get; }



//        #region 内部变量


//        protected volatile bool _IsRunning = false;

//        protected readonly int _SendDataIntervalMilliseconds;

//        protected readonly TFixedHeaderPackageFilter _CurrentFixedHeaderPackageFilter;

//        protected ConcurrentDictionary<Guid, IAsyncTcpServerClient> _TcpServerClientDic = new ConcurrentDictionary<Guid, IAsyncTcpServerClient>();

//        protected readonly int _CurrentIntervalHeartTotalMilliseconds;
//        protected readonly int _CurrentHeartTimeOutMilliseconds;

//        #endregion


//        protected BaseTcpServer(TFixedHeaderPackageFilter fixedHeaderPackageFilter, int port, int receiveBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendDataIntervalMilliseconds = 500, int intervalHeartTotalMilliseconds = 3 * 1000, int heartTimeOutCount = 3)
//        {

//            _CurrentFixedHeaderPackageFilter = fixedHeaderPackageFilter;
//            Port = port;
//            ReceiveBufferSize = receiveBufferSize;
//            SendBufferSize = sendBufferSize;
//            _SendDataIntervalMilliseconds = sendDataIntervalMilliseconds;
//            _CurrentIntervalHeartTotalMilliseconds = intervalHeartTotalMilliseconds;
//            _CurrentHeartTimeOutMilliseconds = _CurrentIntervalHeartTotalMilliseconds * heartTimeOutCount;

//        }


//        #region 通知事件


//        protected abstract Task OnAcceptEventAsync(IAsyncTcpServerClient client);

//        protected virtual async Task OnAcceptAsync(IAsyncTcpServerClient client)
//        {

//            var ep = client.RemoteEndPoint as IPEndPoint;
//            var session = CreateSessionToken(ep?.Address.ToString(), ep?.Port ?? 0);
//            client.CurrentSessionToken = session;

//            //_TcpServerClientDic.AddOrUpdate(session.SessionID, client, (k, c) => client);
//            _TcpServerClientDic[session.SessionID] = client;

//            await OnAcceptEventAsync(client);

//        }


//        protected abstract void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex);

//        protected virtual void OnServerClientErrorEvent(ITcpServerClient client, Exception ex)
//        {
//            OnServerClientErrorCallBackEvent(client, ex);
//        }

//        protected abstract void OnServerErrorEvent(Exception ex);

//        protected virtual void OnServerError(Exception ex)
//        {
//            OnServerErrorEvent(ex);
//            CloseAsync().Wait();
//        }

//        protected void CloseTcpServerClient(ITcpServerClient client)
//        {

//            CloseTcpServerClient(client.CurrentSessionToken);

//        }


//        protected virtual void CloseTcpServerClient(ISessionToken sessionToken)
//        {

//            if (_TcpServerClientDic.TryRemove(sessionToken.SessionID, out var client))
//            {
//                client.Close();
//            }

//        }


//        #endregion


//        protected abstract TSessionToken CreateSessionToken(string ip, int port);
//        protected abstract bool CanSendData(ISessionToken sessionToken);
//        protected abstract TTcpServerClient CreateTcpServerClient(System.Net.Sockets.Socket client);



//        public async Task StartAsync()
//        {

//            CurrentSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
//            {
//                NoDelay = true,
//            };

//            var ipEndPoint = new IPEndPoint(IPAddress.Any, Port);
//            CurrentSocket.Bind(ipEndPoint);
//            CurrentSocket.Listen(1000);
//            _IsRunning = true;

//            ThreadPool.QueueUserWorkItem(BeginAccept);

//            await Task.CompletedTask;

//        }

//        private async void BeginAccept(object obj)
//        {
//            await BeginAcceptAsync();
//        }

//        private async Task BeginAcceptAsync()
//        {
//            try
//            {
//                while (_IsRunning)
//                {
//                    var socket = await CurrentSocket.AcceptAsync();
//                    var tcpServerClient = CreateTcpServerClient(socket);
//                    tcpServerClient.StartReceiveEventAsync += OnServerClientStartReceiveEventAsync;
//                    tcpServerClient.ServerClientErrorEventAsync += OnServerClientErrorEventAsync;
//                    tcpServerClient.ReceiveDataEventAsync += OnServerClientReceiveDataEventAsync;
//                    tcpServerClient.CloseEventAsync += OnServerClientCloseEventAsync;
//                    await tcpServerClient.StartReceiveAsync();
//                    await OnAcceptAsync(tcpServerClient);
//                }
//            }
//            catch (Exception exception)
//            {
//                OnServerError(exception);
//            }
//        }





//        private Task OnServerClientCloseEventAsync(IAsyncTcpServerClient tcpserverclient)
//        {
//            throw new NotImplementedException();
//        }

//        private Task OnServerClientReceiveDataEventAsync(IAsyncTcpServerClient tcpserverclient, BufferModel buffer, CacheModel cache)
//        {
//            throw new NotImplementedException();
//        }

//        private Task OnServerClientErrorEventAsync(IAsyncTcpServerClient client, Exception ex)
//        {
//            throw new NotImplementedException();
//        }

//        private async Task OnServerClientStartReceiveEventAsync(IAsyncTcpServerClient tcpserverclient)
//        {
//            throw new NotImplementedException();
//        }


//        public async Task CloseAsync()
//        {
//            throw new NotImplementedException();
//        }
//        public async ValueTask DisposeAsync()
//        {
//            throw new NotImplementedException();
//        }



//    }



//}
