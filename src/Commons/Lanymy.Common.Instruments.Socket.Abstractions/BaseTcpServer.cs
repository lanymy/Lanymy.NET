using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers;
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

        protected readonly object _CloseLocker = new Object();

        protected volatile bool _IsRunning = false;

        protected readonly int _SendDataIntervalMilliseconds;

        protected readonly TFixedHeaderPackageFilter _CurrentFixedHeaderPackageFilter;

        protected ConcurrentDictionary<Guid, ITcpServerClient> _TcpServerClientDic = new ConcurrentDictionary<Guid, ITcpServerClient>();

        protected readonly int _CurrentIntervalHeartTotalMilliseconds;
        //protected readonly int _CurrentHeartTimeOutMilliseconds;
        protected readonly uint _CurrentHeartTimeOutMilliseconds;

        protected readonly int _CurrentBacklog;

        //private Task _HeartTask;

        #endregion



        protected BaseTcpServer(TFixedHeaderPackageFilter fixedHeaderPackageFilter, int port, int receiveBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendDataIntervalMilliseconds = 500, int intervalHeartTotalMilliseconds = 3 * 1000, int heartTimeOutCount = 3, int backlog = 100)
        {

            _CurrentFixedHeaderPackageFilter = fixedHeaderPackageFilter;
            Port = port;
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
            _SendDataIntervalMilliseconds = sendDataIntervalMilliseconds;
            _CurrentIntervalHeartTotalMilliseconds = intervalHeartTotalMilliseconds;
            _CurrentHeartTimeOutMilliseconds = (uint)(_CurrentIntervalHeartTotalMilliseconds * heartTimeOutCount);
            _CurrentBacklog = backlog;

        }


        #region 通知事件


        protected abstract void OnAcceptEvent(ITcpServerClient client);

        protected virtual bool TryInitializeAcceptedClient(ITcpServerClient client)
        {
            try
            {

                var ep = client.RemoteEndPoint as IPEndPoint;
                var session = CreateSessionToken(ep?.Address.ToString(), ep?.Port ?? 0);
                client.CurrentSessionToken = session;

                //_TcpServerClientDic.AddOrUpdate(session.SessionID, client, (k, c) => client);
                _TcpServerClientDic[session.SessionID] = client;

                return true;
            }
            catch
            {
                return false;
            }

        }

        protected virtual void OnAccept(ITcpServerClient client)
        {

            try
            {
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

        protected virtual void HandleServerManagedClientError(ITcpServerClient client, Exception ex)
        {
            OnServerClientErrorEvent(client, ex);
            CloseTcpServerClient(client);
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

        protected virtual void OnServerCloseError(Exception ex)
        {
            try
            {
                OnServerErrorEvent(ex);
            }
            catch
            {

            }
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
            CurrentSocket.Listen(_CurrentBacklog);
            _IsRunning = true;

            _ = Task.Factory.StartNew(
                () => BeginAcceptAsync(),
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).Unwrap();

            //if (_HeartTask.IfIsNull())
            //{
            //    _HeartTask = new Task(OnHeartTask, TaskCreationOptions.LongRunning);
            //    _HeartTask.Start();
            //}


        }

        private async Task BeginAcceptAsync()
        {
            try
            {
                while (_IsRunning)
                {
                    var currentSocket = CurrentSocket;
                    if (!_IsRunning || currentSocket.IfIsNull())
                    {
                        return;
                    }

                    var socket = currentSocket.Accept();
                    if (!_IsRunning)
                    {
                        socket.Dispose();
                        return;
                    }

                    var tcpServerClient = CreateTcpServerClient(socket);
                    tcpServerClient.StartReceiveEvent += OnServerClientStartReceiveEvent;
                    tcpServerClient.ServerClientErrorEvent += OnServerClientErrorEvent;
                    tcpServerClient.ReceiveDataEvent += OnServerClientReceiveDataEvent;
                    tcpServerClient.CloseEvent += OnServerClientCloseEvent;
                    tcpServerClient.HeartEvent += OnServerClientHeartEvent;

                    if (!TryInitializeAcceptedClient(tcpServerClient))
                    {
                        socket.Dispose();
                        continue;
                    }

                    await tcpServerClient.StartReceiveAsync();
                    OnAccept(tcpServerClient);
                }
            }
            catch (Exception exception) when (CanIgnoreAcceptException(exception))
            {
            }
            catch (Exception exception)
            {
                OnServerError(exception);
            }
        }

        protected virtual bool CanIgnoreAcceptException(Exception ex)
        {
            if (_IsRunning)
            {
                return false;
            }

            if (ex is ObjectDisposedException)
            {
                return true;
            }

            if (ex is SocketException socketException)
            {
                return socketException.SocketErrorCode == SocketError.Interrupted
                    || socketException.SocketErrorCode == SocketError.OperationAborted
                    || socketException.SocketErrorCode == SocketError.NotSocket;
            }

            return false;
        }


        protected abstract void OnServerClientHeartCallBackEvent(ITcpServerClient tcpServerClient);

        protected virtual void OnServerClientHeartEvent(ITcpServerClient tcpServerClient)
        {

            var sessionToken = tcpServerClient.CurrentSessionToken;

            //sessionToken.IntervalHeartTotalMilliseconds = (int)((DateTime.Now - sessionToken.LastReceiveDateTime).TotalMilliseconds);
            //sessionToken.IntervalHeartTotalMillisecondsFromInstantiation = DateTimeHelper.GetTotalMillisecondsFromInstantiation(DateTime.Now) - sessionToken.LastReceiveDateTimeTotalMillisecondsFromInstantiation;


            //if (sessionToken.IntervalHeartTotalMilliseconds > _CurrentHeartTimeOutMilliseconds)//心跳超时断开连接
            if ((DateTimeHelper.GetTotalMillisecondsFromInstantiation(DateTime.Now) - sessionToken.LastReceiveDateTimeTotalMillisecondsFromInstantiation) > _CurrentHeartTimeOutMilliseconds)//心跳超时断开连接
            {
                HandleServerManagedClientError(tcpServerClient, new Exception("心跳超时断开连接"));
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
        /// 数据包循环处理
        /// </summary>
        /// <param name="tcpServerClient"></param>
        /// <param name="buffer"></param>
        /// <param name="cache"></param>
        protected virtual void OnServerClientReceiveDataLoopEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache)
        {
            while (true)
            {
                var packageBytes = _CurrentFixedHeaderPackageFilter.GetPackageBytes(buffer, cache);

                if (packageBytes.IfIsNull())
                {
                    return;
                }

                if (!_CurrentFixedHeaderPackageFilter.CheckPackage(packageBytes))
                {
                    HandleServerManagedClientError(tcpServerClient, new Exception("data bytes error"));
                    return;
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
            TaskHelper.SyncWait(CloseAsync());
        }

        protected virtual async Task OnServerCloseAsync()
        {

            if (!_IsRunning)
            {
                return;
            }

            System.Net.Sockets.Socket currentSocket = null;
            List<ITcpServerClient> tcpServerClients = null;

            lock (_CloseLocker)
            {

                if (_IsRunning)
                {
                    _IsRunning = false;

                    currentSocket = CurrentSocket;
                    CurrentSocket = null;

                    tcpServerClients = _TcpServerClientDic.Values.ToList();
                }
                else
                {
                    return;
                }
            }

            try
            {

                if (!currentSocket.IfIsNull())
                {
                    currentSocket.Dispose();
                }

            }
            catch (Exception ex)
            {
                OnServerCloseError(new InvalidOperationException("TcpServer dispose listen socket failed.", ex));
            }


            try
            {

                if (!tcpServerClients.IfIsNullOrEmpty())
                {
                    foreach (var tcpServerClient in tcpServerClients)
                    {

                        try
                        {
                            await tcpServerClient.CloseAsync();
                        }
                        catch (Exception ex)
                        {
                            OnServerClientErrorEvent(tcpServerClient, new InvalidOperationException("TcpServer close child client failed.", ex));
                        }

                    }
                }

                OnServerCloseEvent();

                _TcpServerClientDic.Clear();

            }
            catch (Exception ex)
            {
                OnServerCloseError(new InvalidOperationException("TcpServer close finalization failed.", ex));
            }

        }



        public void Close()
        {

            OnServerClose();

        }

        public virtual async Task CloseAsync()
        {
            await OnServerCloseAsync();
        }

        public void Dispose()
        {

            Close();

        }



    }

}
