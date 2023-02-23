using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{



    public abstract class BaseTcpServer<TTcpServerClient, TSessionToken, TFixedHeaderPackageFilter, TPackage, TSendPackage> : ITcpServer
        where TTcpServerClient : BaseTcpServerClient
        where TSessionToken : ISessionToken
        where TFixedHeaderPackageFilter : IFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken>
        where TPackage : class
        where TSendPackage : class, ISendPackageSendNum
    {


        public System.Net.Sockets.Socket CurrentSocket { get; protected set; }

        public bool IsConnected
        {
            get
            {
                if (CurrentSocket.IfIsNull())
                {
                    return false;
                }
                return CurrentSocket.Connected;
            }
        }

        //public bool IsDisposed => _IsDisposed;
        public bool IsAccept => _IsRunning;

        public int ReceiveBufferSize { get; }
        public int SendBufferSize { get; }
        public bool IsRunning => _IsRunning;
        public int Port { get; }


        #region 内部变量


        protected volatile bool _IsRunning = false;

        protected readonly int _SendDataIntervalMilliseconds;

        protected readonly TFixedHeaderPackageFilter _CurrentFixedHeaderPackageFilter;

        protected ConcurrentDictionary<Guid, ITcpServerClient> _TcpServerClientDic = new ConcurrentDictionary<Guid, ITcpServerClient>();

        protected readonly int _CurrentIntervalHeartTotalMilliseconds;
        protected readonly int _CurrentHeartTimeOutMilliseconds;

        //private Task _HeartTask;

        #endregion


        protected BaseTcpServer(TFixedHeaderPackageFilter fixedHeaderPackageFilter, int port, int receiveBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendDataIntervalMilliseconds = 500, int intervalHeartTotalMilliseconds = 3 * 1000, int heartTimeOutCount = 3)
        {

            _CurrentFixedHeaderPackageFilter = fixedHeaderPackageFilter;
            Port = port;
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
            _SendDataIntervalMilliseconds = sendDataIntervalMilliseconds;
            _CurrentIntervalHeartTotalMilliseconds = intervalHeartTotalMilliseconds;
            _CurrentHeartTimeOutMilliseconds = _CurrentIntervalHeartTotalMilliseconds * heartTimeOutCount;

        }

        #region 通知事件

        protected abstract void OnAcceptEvent(ITcpServerClient client);

        protected virtual void OnAccept(ITcpServerClient client)
        {

            var ep = client.RemoteEndPoint as IPEndPoint;
            var session = CreateSessionToken(ep?.Address.ToString(), ep?.Port ?? 0);
            client.CurrentSessionToken = session;

            //_TcpServerClientDic.AddOrUpdate(session.SessionID, client, (k, c) => client);
            _TcpServerClientDic[session.SessionID] = client;

            OnAcceptEvent(client);

        }

        protected abstract void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex);

        protected virtual void OnServerClientErrorEvent(ITcpServerClient client, Exception ex)
        {
            OnServerClientErrorCallBackEvent(client, ex);
        }

        protected abstract void OnServerErrorEvent(Exception ex);

        protected virtual void OnServerError(Exception ex)
        {
            OnServerErrorEvent(ex);
            Close();
        }

        protected void CloseTcpServerClient(ITcpServerClient client)
        {

            CloseTcpServerClient(client.CurrentSessionToken);

        }


        protected virtual void CloseTcpServerClient(ISessionToken sessionToken)
        {

            if (_TcpServerClientDic.TryRemove(sessionToken.SessionID, out var client))
            {
                client.Close();
            }

        }


        #endregion


        protected abstract TTcpServerClient CreateTcpServerClient(System.Net.Sockets.Socket client);

        public void Start()
        {

            CurrentSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
            };

            var ipEndPoint = new IPEndPoint(IPAddress.Any, Port);
            CurrentSocket.Bind(ipEndPoint);
            CurrentSocket.Listen(1000);
            _IsRunning = true;

            ThreadPool.QueueUserWorkItem(BeginAccept);

            //if (_HeartTask.IfIsNull())
            //{
            //    _HeartTask = new Task(OnHeartTask, TaskCreationOptions.LongRunning);
            //    _HeartTask.Start();
            //}


        }

        private void BeginAccept(object obj)
        {
            try
            {
                while (_IsRunning)
                {
                    var socket = CurrentSocket.Accept();
                    var tcpServerClient = CreateTcpServerClient(socket);
                    tcpServerClient.StartReceiveEvent += OnServerClientStartReceiveEvent;
                    tcpServerClient.ServerClientErrorEvent += OnServerClientErrorEvent;
                    tcpServerClient.ReceiveDataEvent += OnServerClientReceiveDataEvent;
                    tcpServerClient.CloseEvent += OnServerClientCloseEvent;
                    tcpServerClient.HeartEvent += OnServerClientHeartEvent;
                    tcpServerClient.StartReceive();
                    OnAccept(tcpServerClient);
                }
            }
            catch (Exception exception)
            {
                OnServerError(exception);
            }
        }


        protected abstract void OnServerClientHeartCallBackEvent(ITcpServerClient tcpServerClient);

        protected virtual void OnServerClientHeartEvent(ITcpServerClient tcpServerClient)
        {

            var sessionToken = tcpServerClient.CurrentSessionToken;

            tcpServerClient.CurrentSessionToken.IntervalHeartTotalMilliseconds = (int)((DateTime.Now - tcpServerClient.CurrentSessionToken.LastReceiveDateTime).TotalMilliseconds);

            if (tcpServerClient.CurrentSessionToken.IntervalHeartTotalMilliseconds > _CurrentHeartTimeOutMilliseconds)//关闭心跳超时链接
            {
                OnServerClientErrorEvent(tcpServerClient, new Exception("心跳超时断开链接"));
            }
            //else if (tcpServerClient.CurrentSessionToken.IntervalHeartTotalMilliseconds >= _CurrentIntervalHeartTotalMilliseconds)
            else
            {

                //if (CanSendData(sessionToken))
                //{
                //    SendDataBytes(tcpServerClient, _CurrentFixedHeaderPackageFilter.GetHeartBytes(sessionToken));
                //}

                SendDataBytes(tcpServerClient, _CurrentFixedHeaderPackageFilter.GetHeartBytes(sessionToken));

            }

            OnServerClientHeartCallBackEvent(tcpServerClient);

        }


        protected abstract void OnServerClientCloseCallBackEvent(ITcpServerClient tcpServerClient);
        protected virtual void OnServerClientCloseEvent(ITcpServerClient tcpServerClient)
        {
            OnServerClientCloseCallBackEvent(tcpServerClient);
            CloseTcpServerClient(tcpServerClient);
        }

        protected abstract void OnServerClientStartReceiveCallBackEvent(ITcpServerClient tcpServerClient);
        protected virtual void OnServerClientStartReceiveEvent(ITcpServerClient tcpServerClient)
        {
            OnServerClientStartReceiveCallBackEvent(tcpServerClient);
        }

        protected abstract void OnServerClientReceiveDataCallBackEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache);
        protected virtual void OnServerClientReceiveDataEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache)
        {

            OnServerClientReceiveDataCallBackEvent(tcpServerClient, buffer, cache);

            while (true)
            {

                var packageBytes = _CurrentFixedHeaderPackageFilter.GetPackageBytes(buffer, cache);

                if (packageBytes.IfIsNull())
                {
                    break;
                }

                if (!_CurrentFixedHeaderPackageFilter.CheckPackage(packageBytes))
                {
                    OnServerClientErrorEvent(tcpServerClient, new Exception("data bytes error"));
                    break;
                }


                OnServerReceivePackage(_CurrentFixedHeaderPackageFilter.DecodePackage(packageBytes), tcpServerClient.CurrentSessionToken);

            }

        }


        protected abstract void OnServerReceivePackageEvent(TPackage package, ISessionToken sessionToken);

        protected virtual void OnServerReceivePackage(TPackage package, ISessionToken sessionToken)
        {

            OnServerReceivePackageEvent(package, sessionToken);

        }


        protected abstract TSessionToken CreateSessionToken(string ip, int port);
        protected abstract bool CanSendData(ISessionToken sessionToken);

        //private async void OnHeartTask()
        //{

        //    while (true)
        //    {

        //        await Task.Delay(_CurrentIntervalHeartTotalMilliseconds);

        //        Parallel.ForEach(_TcpServerClientDic, item =>
        //        {

        //            var client = item.Value;
        //            var sessionToken = client.CurrentSessionToken;

        //            client.CurrentSessionToken.IntervalHeartTotalMilliseconds = (int)((DateTime.Now - client.CurrentSessionToken.LastReceiveDateTime).TotalMilliseconds);

        //            if (client.CurrentSessionToken.IntervalHeartTotalMilliseconds >= _CurrentHeartTimeOutMilliseconds)//关闭心跳超时链接
        //            {
        //                OnServerClientErrorEvent(client, new Exception("心跳超时断开链接"));
        //            }
        //            else if (client.CurrentSessionToken.IntervalHeartTotalMilliseconds >= _CurrentIntervalHeartTotalMilliseconds)
        //            {

        //                if (CanSendData(sessionToken))
        //                {
        //                    SendDataBytes(client, _CurrentFixedHeaderPackageFilter.GetHeartBytes(sessionToken));
        //                }

        //            }

        //        });

        //    }
        //}



        public void SendDataBytes(ITcpServerClient client, byte[] dataBytes)
        {

            if (CanSendData(client.CurrentSessionToken) && !dataBytes.IfIsNullOrEmpty())
            {
                client.Send(dataBytes);
            }

        }


        public void SendDataBytes(Guid sessionID, byte[] dataBytes)
        {

            var client = GetTcpServerClient(sessionID);

            if (!client.IfIsNull())
            {
                SendDataBytes(client, dataBytes);
            }

        }


        public void SendPackage(Guid sessionID, TSendPackage sendPackage)
        {

            var client = GetTcpServerClient(sessionID);
            if (client.IfIsNull())
            {
                return;
            }
            var session = client.CurrentSessionToken;
            sendPackage.SendNum = session.SendNum;

            var sendPackageDataBytes = _CurrentFixedHeaderPackageFilter.EncodePackage(sendPackage);

            SendDataBytes(client, sendPackageDataBytes);

        }

        protected ITcpServerClient GetTcpServerClient(Guid sessionID)
        {

            _TcpServerClientDic.TryGetValue(sessionID, out var client);

            return client;

        }



        protected abstract void OnServerCloseEvent();

        protected virtual void OnServerClose()
        {

            if (!_IsRunning)
            {
                return;
                ;
            }

            _IsRunning = false;

            if (!CurrentSocket.IfIsNull())
            {

                try
                {
                    CurrentSocket.Dispose();
                    CurrentSocket = null;
                }
                catch
                {

                }


                try
                {

                    var list = _TcpServerClientDic.Values.ToList();

                    Parallel.ForEach(list, client =>
                    {

                        client.Close();

                    });

                    list.Clear();

                    OnServerCloseEvent();

                }
                catch
                {

                }

            }

        }

        public void Close()
        {

            OnServerClose();

        }

        public void Dispose()
        {

            Close();

        }



    }

}
