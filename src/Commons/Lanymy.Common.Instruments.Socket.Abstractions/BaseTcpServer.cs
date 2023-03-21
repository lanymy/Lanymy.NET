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


        #region �ڲ�����

        protected readonly object _CloseLocker = new Object();

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

        #region ֪ͨ�¼�


        protected abstract void OnAcceptEvent(ITcpServerClient client);

        protected virtual void OnAccept(ITcpServerClient client)
        {

            try
            {
                var ep = client.RemoteEndPoint as IPEndPoint;
                var session = CreateSessionToken(ep?.Address.ToString(), ep?.Port ?? 0);
                client.CurrentSessionToken = session;

                //_TcpServerClientDic.AddOrUpdate(session.SessionID, client, (k, c) => client);
                _TcpServerClientDic[session.SessionID] = client;

                OnAcceptEvent(client);
            }
            catch
            {

            }

        }


        protected abstract void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex);

        protected virtual void OnServerClientErrorEvent(ITcpServerClient client, Exception ex)
        {
            OnServerClientErrorCallBackEvent(client, ex);
        }

        protected abstract void OnServerErrorEvent(Exception ex);

        protected virtual void OnServerError(Exception ex)
        {

            try
            {
                OnServerErrorEvent(ex);
            }
            catch
            {

            }

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

            if (tcpServerClient.CurrentSessionToken.IntervalHeartTotalMilliseconds > _CurrentHeartTimeOutMilliseconds)//�ر�������ʱ����
            {
                OnServerClientErrorEvent(tcpServerClient, new Exception("������ʱ�Ͽ�����"));
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

            //while (true)
            //{

            //    var packageBytes = _CurrentFixedHeaderPackageFilter.GetPackageBytes(buffer, cache);

            //    if (packageBytes.IfIsNull())
            //    {
            //        break;
            //    }

            //    if (!_CurrentFixedHeaderPackageFilter.CheckPackage(packageBytes))
            //    {
            //        OnServerClientErrorEvent(tcpServerClient, new Exception("data bytes error"));
            //        break;
            //    }


            //    OnServerReceivePackage(_CurrentFixedHeaderPackageFilter.DecodePackage(packageBytes), tcpServerClient.CurrentSessionToken);

            //}

            OnServerClientReceiveDataLoopEvent(tcpServerClient, buffer, cache);

        }

        /// <summary>
        /// ���ݰ�ѭ������
        /// </summary>
        /// <param name="tcpServerClient"></param>
        /// <param name="buffer"></param>
        /// <param name="cache"></param>
        protected virtual void OnServerClientReceiveDataLoopEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache)
        {

            var packageBytes = _CurrentFixedHeaderPackageFilter.GetPackageBytes(buffer, cache);

            if (packageBytes.IfIsNull())
            {
                return;
            }

            if (!_CurrentFixedHeaderPackageFilter.CheckPackage(packageBytes))
            {
                OnServerClientErrorEvent(tcpServerClient, new Exception("data bytes error"));
                return;
            }

            OnServerReceivePackage(_CurrentFixedHeaderPackageFilter.DecodePackage(packageBytes), tcpServerClient.CurrentSessionToken);

            OnServerClientReceiveDataLoopEvent(tcpServerClient, buffer, cache);

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

        //            if (client.CurrentSessionToken.IntervalHeartTotalMilliseconds >= _CurrentHeartTimeOutMilliseconds)//�ر�������ʱ����
        //            {
        //                OnServerClientErrorEvent(client, new Exception("������ʱ�Ͽ�����"));
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

            try
            {
                if (CanSendData(client.CurrentSessionToken))
                {
                    client.Send(dataBytes);
                }
            }
            catch
            {

            }

        }


        public void SendDataBytes(Guid sessionID, byte[] dataBytes)
        {

            try
            {
                var client = GetTcpServerClient(sessionID);

                if (!client.IfIsNull())
                {
                    SendDataBytes(client, dataBytes);
                }
            }
            catch
            {

            }

        }


        public void SendPackage(Guid sessionID, TSendPackage sendPackage)
        {

            try
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
            catch
            {

            }

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
            }

            lock (_CloseLocker)
            {

                if (_IsRunning)
                {
                    _IsRunning = false;



                    try
                    {

                        if (!CurrentSocket.IfIsNull())
                        {
                            CurrentSocket.Dispose();
                            CurrentSocket = null;
                        }

                    }
                    catch
                    {

                    }


                    try
                    {

                        var enumerator = _TcpServerClientDic.GetEnumerator();

                        while (enumerator.MoveNext())
                        {

                            try
                            {
                                enumerator.Current.Value.Close();
                            }
                            catch
                            {

                            }

                        }

                        OnServerCloseEvent();

                        _TcpServerClientDic.Clear();

                    }
                    catch
                    {

                    }

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
