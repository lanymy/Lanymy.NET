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

        public bool IsDisposed => _IsDisposed;
        public bool IsAccept => _IsRunning;

        public int ReceiveBufferSize { get; }
        public int SendBufferSize { get; }
        public bool IsRunning => _IsRunning;
        public int Port { get; }


        #region 内部变量

        protected readonly object _CloseLocker = new Object();

        protected volatile bool _IsRunning = false;
        protected volatile bool _IsDisposed = false;

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
            catch (Exception ex)
            {
                ReportServerError(new InvalidOperationException("TcpServer initialize accepted client failed.", ex));
                return false;
            }

        }

        protected virtual void CleanupAcceptedClientInitializationFailure(ITcpServerClient tcpServerClient)
        {
            if (tcpServerClient.IfIsNull())
            {
                return;
            }

            try
            {
                if (!tcpServerClient.CurrentSessionToken.IfIsNull())
                {
                    _TcpServerClientDic.TryRemove(tcpServerClient.CurrentSessionToken.SessionID, out _);
                }
            }
            catch (Exception ex)
            {
                ReportServerError(new InvalidOperationException("TcpServer cleanup accepted client dictionary failed.", ex));
            }

            try
            {
                DetachTcpServerClientEventHandlers(tcpServerClient);
            }
            catch (Exception ex)
            {
                ReportServerError(new InvalidOperationException("TcpServer detach accepted client handlers failed.", ex));
            }

            try
            {
                tcpServerClient.Dispose();
            }
            catch (Exception ex)
            {
                ReportServerError(new InvalidOperationException("TcpServer dispose accepted client failed.", ex));
            }
        }

        protected virtual void CleanupClosedTcpServerClientFailure(ITcpServerClient tcpServerClient)
        {
            if (tcpServerClient.IfIsNull())
            {
                return;
            }

            try
            {
                tcpServerClient.Dispose();
            }
            catch (Exception ex)
            {
                OnServerCloseError(new InvalidOperationException("TcpServer dispose child client after close failure failed.", ex));
            }
        }

        protected virtual void ReportChildClientCloseFailure(ITcpServerClient tcpServerClient, Exception ex)
        {
            if (tcpServerClient.IfIsNull() || ex.IfIsNull())
            {
                return;
            }

            try
            {
                if (IsManagedTcpServerClient(tcpServerClient))
                {
                    OnServerClientErrorEvent(tcpServerClient, ex);
                }
                else
                {
                    OnServerClientErrorCallBackEvent(tcpServerClient, ex);
                }
            }
            catch (Exception reportEx)
            {
                OnServerCloseError(new InvalidOperationException("TcpServer report child close failure failed.", reportEx));
            }
        }

        protected virtual void ReportManagedClientError(ITcpServerClient tcpServerClient, Exception ex)
        {
            if (tcpServerClient.IfIsNull() || ex.IfIsNull())
            {
                return;
            }

            try
            {
                if (IsManagedTcpServerClient(tcpServerClient))
                {
                    OnServerClientErrorEvent(tcpServerClient, ex);
                }
                else
                {
                    OnServerClientErrorCallBackEvent(tcpServerClient, ex);
                }
            }
            catch (Exception reportEx)
            {
                ReportServerError(new InvalidOperationException("TcpServer report child client error failed.", reportEx));
            }
        }

        protected virtual void ReportManagedClientCallbackError(ITcpServerClient tcpServerClient, string message, Exception ex)
        {
            if (string.IsNullOrWhiteSpace(message) || ex.IfIsNull())
            {
                return;
            }

            ReportManagedClientError(tcpServerClient, new InvalidOperationException(message, ex));
        }

        protected virtual bool TryInvokeManagedClientCallback(ITcpServerClient tcpServerClient, Action callbackAction, string errorMessage)
        {
            if (!IsManagedTcpServerClient(tcpServerClient))
            {
                return false;
            }

            try
            {
                callbackAction?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                ReportManagedClientCallbackError(tcpServerClient, errorMessage, ex);
                return false;
            }
        }

        protected virtual void OnAccept(ITcpServerClient client)
        {
            try
            {
                OnAcceptEvent(client);
            }
            catch (Exception ex)
            {
                ReportManagedClientError(client, ex);
            }
        }


        protected abstract void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex);

        protected virtual void OnServerClientErrorEvent(ITcpServerClient client, Exception ex)
        {
            if (!IsManagedTcpServerClient(client))
            {
                return;
            }

            OnServerClientErrorCallBackEvent(client, ex);
        }

        protected virtual void AttachTcpServerClientEventHandlers(ITcpServerClient tcpServerClient)
        {
            tcpServerClient.StartReceiveEvent += OnServerClientStartReceiveEvent;
            tcpServerClient.ServerClientErrorEvent += OnServerClientErrorEvent;
            tcpServerClient.ReceiveDataEvent += OnServerClientReceiveDataEvent;
            tcpServerClient.CloseEvent += OnServerClientCloseEvent;
            tcpServerClient.HeartEvent += OnServerClientHeartEvent;
        }

        protected virtual void DetachTcpServerClientEventHandlers(ITcpServerClient tcpServerClient)
        {
            if (tcpServerClient.IfIsNull())
            {
                return;
            }

            tcpServerClient.StartReceiveEvent -= OnServerClientStartReceiveEvent;
            tcpServerClient.ServerClientErrorEvent -= OnServerClientErrorEvent;
            tcpServerClient.ReceiveDataEvent -= OnServerClientReceiveDataEvent;
            tcpServerClient.CloseEvent -= OnServerClientCloseEvent;
            tcpServerClient.HeartEvent -= OnServerClientHeartEvent;
        }

        protected virtual bool IsManagedTcpServerClient(ITcpServerClient tcpServerClient)
        {
            if (tcpServerClient.IfIsNull() || tcpServerClient.CurrentSessionToken.IfIsNull())
            {
                return false;
            }

            return _TcpServerClientDic.TryGetValue(tcpServerClient.CurrentSessionToken.SessionID, out var currentClient)
                   && ReferenceEquals(currentClient, tcpServerClient);
        }

        protected virtual void HandleServerManagedClientError(ITcpServerClient client, Exception ex)
        {
            ReportManagedClientError(client, ex);
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

        protected virtual void ReportServerError(Exception ex)
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
            if (sessionToken.IfIsNull())
            {
                return;
            }

            if (_TcpServerClientDic.TryRemove(sessionToken.SessionID, out var client))
            {
                try
                {
                    DetachTcpServerClientEventHandlers(client);
                }
                catch (Exception ex)
                {
                    ReportServerError(new InvalidOperationException("TcpServer detach child client handlers failed.", ex));
                }

                try
                {
                    var closeException = TaskHelper.TrySyncWait(client.CloseAsync);
                    if (closeException != null)
                    {
                        throw closeException;
                    }
                }
                catch (Exception ex)
                {
                    ReportChildClientCloseFailure(client, new InvalidOperationException("TcpServer close child client failed.", ex));
                    CleanupClosedTcpServerClientFailure(client);
                }
            }

        }


        #endregion


        protected abstract TTcpServerClient CreateTcpServerClient(System.Net.Sockets.Socket client);

        protected virtual System.Net.Sockets.Socket CreateListenSocket()
        {
            return new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
            };
        }

        protected virtual void BindAndListenSocket(System.Net.Sockets.Socket currentSocket, IPEndPoint ipEndPoint)
        {
            currentSocket.Bind(ipEndPoint);
            currentSocket.Listen(_CurrentBacklog);
        }

        protected virtual bool TryBeginStart(out System.Net.Sockets.Socket currentSocket)
        {
            currentSocket = null;

            lock (_CloseLocker)
            {
                if (_IsDisposed || _IsRunning)
                {
                    return false;
                }

                currentSocket = CreateListenSocket();
                CurrentSocket = currentSocket;
                _IsRunning = true;
                return true;
            }
        }

        protected virtual bool CanContinueStart(System.Net.Sockets.Socket currentSocket)
        {
            lock (_CloseLocker)
            {
                if (_IsDisposed)
                {
                    return false;
                }

                if (!ReferenceEquals(CurrentSocket, currentSocket))
                {
                    return false;
                }

                return _IsRunning;
            }
        }

        protected virtual void ResetStartState(System.Net.Sockets.Socket currentSocket)
        {
            lock (_CloseLocker)
            {
                if (ReferenceEquals(CurrentSocket, currentSocket))
                {
                    CurrentSocket = null;
                }

                _IsRunning = false;
            }

            if (currentSocket.IfIsNull())
            {
                return;
            }

            try
            {
                currentSocket.Dispose();
            }
            catch (Exception ex)
            {
                OnServerCloseError(new InvalidOperationException("TcpServer reset start listen socket failed.", ex));
            }
        }

        public void Start()
        {
            if (!TryBeginStart(out var currentSocket))
            {
                return;
            }

            try
            {
                var ipEndPoint = new IPEndPoint(IPAddress.Any, Port);
                BindAndListenSocket(currentSocket, ipEndPoint);
                if (!CanContinueStart(currentSocket))
                {
                    ResetStartState(currentSocket);
                    return;
                }

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
            catch (Exception ex)
            {
                var canContinueStart = CanContinueStart(currentSocket);
                ResetStartState(currentSocket);
                if (!canContinueStart && CanIgnoreStartException(ex))
                {
                    return;
                }

                throw;
            }
        }

        protected virtual bool CanIgnoreStartException(Exception ex)
        {
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

                    System.Net.Sockets.Socket socket = null;
                    try
                    {
                        socket = currentSocket.Accept();
                    }
                    catch (Exception exception) when (CanIgnoreAcceptException(exception, currentSocket))
                    {
                        return;
                    }
                    catch (Exception exception)
                    {
                        OnServerError(exception);
                        return;
                    }

                    if (!_IsRunning)
                    {
                        socket.Dispose();
                        return;
                    }

                    TTcpServerClient tcpServerClient = null;
                    var handlersAttached = false;
                    var clientInitialized = false;

                    try
                    {
                        tcpServerClient = CreateTcpServerClient(socket);
                        AttachTcpServerClientEventHandlers(tcpServerClient);
                        handlersAttached = true;

                        if (!TryInitializeAcceptedClient(tcpServerClient))
                        {
                            CleanupAcceptedClientInitializationFailure(tcpServerClient);
                            continue;
                        }

                        clientInitialized = true;

                        await tcpServerClient.StartReceiveAsync();
                        if (!tcpServerClient.IsRunning)
                        {
                            CleanupAcceptedClientInitializationFailure(tcpServerClient);
                            continue;
                        }

                        OnAccept(tcpServerClient);
                    }
                    catch (Exception exception)
                    {
                        if (clientInitialized && tcpServerClient != null)
                        {
                            HandleServerManagedClientError(tcpServerClient, new InvalidOperationException("TcpServer accept client initialization failed.", exception));
                            continue;
                        }

                        if (handlersAttached && tcpServerClient != null)
                        {
                            CleanupAcceptedClientInitializationFailure(tcpServerClient);
                        }
                        else
                        {
                            try
                            {
                                socket.Dispose();
                            }
                            catch (Exception disposeException)
                            {
                                ReportServerError(new InvalidOperationException("TcpServer dispose failed accepted socket failed.", disposeException));
                            }
                        }

                        ReportServerError(new InvalidOperationException("TcpServer create accepted client failed.", exception));
                    }
                }
            }
            catch (Exception exception)
            {
                OnServerError(exception);
            }
        }

        protected virtual bool CanIgnoreAcceptException(Exception ex, System.Net.Sockets.Socket acceptSocket)
        {
            if (_IsRunning && ReferenceEquals(CurrentSocket, acceptSocket))
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
            if (!IsManagedTcpServerClient(tcpServerClient))
            {
                return;
            }

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

            if (!IsManagedTcpServerClient(tcpServerClient))
            {
                return;
            }

            TryInvokeManagedClientCallback(tcpServerClient, () => OnServerClientHeartCallBackEvent(tcpServerClient), "TcpServer heart callback failed.");

        }


        protected abstract void OnServerClientCloseCallBackEvent(ITcpServerClient tcpServerClient);
        protected virtual void OnServerClientCloseEvent(ITcpServerClient tcpServerClient)
        {
            if (!IsManagedTcpServerClient(tcpServerClient))
            {
                return;
            }

            try
            {
                OnServerClientCloseCallBackEvent(tcpServerClient);
            }
            catch (Exception ex)
            {
                HandleServerManagedClientError(tcpServerClient, new InvalidOperationException("TcpServer close callback failed.", ex));
                return;
            }

            CloseTcpServerClient(tcpServerClient);
        }

        protected abstract void OnServerClientStartReceiveCallBackEvent(ITcpServerClient tcpServerClient);
        protected virtual void OnServerClientStartReceiveEvent(ITcpServerClient tcpServerClient)
        {
            TryInvokeManagedClientCallback(tcpServerClient, () => OnServerClientStartReceiveCallBackEvent(tcpServerClient), "TcpServer start receive callback failed.");
        }

        protected abstract void OnServerClientReceiveDataCallBackEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache);
        protected virtual void OnServerClientReceiveDataEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache)
        {
            if (!IsManagedTcpServerClient(tcpServerClient))
            {
                return;
            }

            TryInvokeManagedClientCallback(tcpServerClient, () => OnServerClientReceiveDataCallBackEvent(tcpServerClient, buffer, cache), "TcpServer receive data callback failed.");

            if (!IsManagedTcpServerClient(tcpServerClient))
            {
                return;
            }

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
            if (!IsManagedTcpServerClient(tcpServerClient))
            {
                return;
            }

            while (true)
            {
                if (!IsManagedTcpServerClient(tcpServerClient))
                {
                    return;
                }

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

                try
                {
                    OnServerReceivePackage(_CurrentFixedHeaderPackageFilter.DecodePackage(packageBytes), tcpServerClient.CurrentSessionToken);
                }
                catch (Exception ex)
                {
                    ReportManagedClientCallbackError(tcpServerClient, "TcpServer receive package callback failed.", ex);
                }
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
            if (client.IfIsNull() || !IsManagedTcpServerClient(client))
            {
                return;
            }

            bool canSendData;
            try
            {
                canSendData = CanSendData(client.CurrentSessionToken);
            }
            catch (Exception ex)
            {
                ReportManagedClientCallbackError(client, "TcpServer can send data check failed.", ex);
                return;
            }

            if (!canSendData)
            {
                return;
            }

            try
            {
                client.Send(dataBytes);
            }
            catch (Exception ex)
            {
                HandleServerManagedClientError(client, ex);
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

            try
            {
                var session = client.CurrentSessionToken;
                sendPackage.SendNum = session.SendNum;
                var sendPackageDataBytes = _CurrentFixedHeaderPackageFilter.EncodePackage(sendPackage);
                SendDataBytes(client, sendPackageDataBytes);
            }
            catch (Exception ex)
            {
                ReportManagedClientError(client, ex);
            }
        }

        protected ITcpServerClient GetTcpServerClient(Guid sessionID)
        {

            _TcpServerClientDic.TryGetValue(sessionID, out var client);

            return client;

        }



        protected abstract void OnServerCloseEvent();

        protected virtual Exception TryCloseSynchronously()
        {
            return TaskHelper.TrySyncWait(CloseAsync);
        }

        protected virtual void OnServerClose()
        {
            Exception closeException = null;

            try
            {
                closeException = TryCloseSynchronously();
            }
            catch (Exception ex)
            {
                closeException = ex;
            }

            if (closeException != null)
            {
                OnServerCloseError(new InvalidOperationException("TcpServer sync close failed.", closeException));
            }
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


            if (!tcpServerClients.IfIsNullOrEmpty())
            {
                foreach (var tcpServerClient in tcpServerClients)
                {
                    try
                    {
                        DetachTcpServerClientEventHandlers(tcpServerClient);
                        await tcpServerClient.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        ReportChildClientCloseFailure(tcpServerClient, new InvalidOperationException("TcpServer close child client failed.", ex));
                        CleanupClosedTcpServerClientFailure(tcpServerClient);
                    }
                }
            }

            try
            {
                OnServerCloseEvent();
            }
            catch (Exception ex)
            {
                OnServerCloseError(new InvalidOperationException("TcpServer close finalization failed.", ex));
            }
            finally
            {
                try
                {
                    _TcpServerClientDic.Clear();
                }
                catch (Exception ex)
                {
                    OnServerCloseError(new InvalidOperationException("TcpServer clear client dictionary failed.", ex));
                }
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

        protected virtual void CleanupDisposeFailureSocket(System.Net.Sockets.Socket currentSocket)
        {
            if (currentSocket.IfIsNull())
            {
                return;
            }

            try
            {
                currentSocket.Dispose();
            }
            catch (Exception ex)
            {
                OnServerCloseError(new InvalidOperationException("TcpServer dispose listen socket failed.", ex));
            }
        }

        protected virtual void CleanupDisposeFailureTcpServerClients(IEnumerable<ITcpServerClient> tcpServerClients)
        {
            if (tcpServerClients.IfIsNullOrEmpty())
            {
                return;
            }

            foreach (var tcpServerClient in tcpServerClients)
            {
                if (tcpServerClient.IfIsNull())
                {
                    continue;
                }

                try
                {
                    DetachTcpServerClientEventHandlers(tcpServerClient);
                }
                catch (Exception ex)
                {
                    OnServerCloseError(new InvalidOperationException("TcpServer detach child client handlers failed.", ex));
                }

                try
                {
                    tcpServerClient.Dispose();
                }
                catch (Exception ex)
                {
                    OnServerCloseError(new InvalidOperationException("TcpServer dispose child client failed.", ex));
                }
            }
        }

        protected virtual void CleanupDisposeFailureTrackedClients()
        {
            try
            {
                _TcpServerClientDic.Clear();
            }
            catch (Exception ex)
            {
                OnServerCloseError(new InvalidOperationException("TcpServer clear client dictionary failed.", ex));
            }
        }

        public void Dispose()
        {
            System.Net.Sockets.Socket currentSocket = null;
            List<ITcpServerClient> tcpServerClients = null;
            var closeExceptionReported = false;

            if (_IsDisposed)
            {
                return;
            }

            lock (_CloseLocker)
            {
                if (_IsDisposed)
                {
                    return;
                }

                _IsDisposed = true;
            }

            Exception closeException = null;

            try
            {
                closeException = TryCloseSynchronously();
            }
            catch (Exception ex)
            {
                closeException = ex;
                closeExceptionReported = true;
                OnServerCloseError(new InvalidOperationException("TcpServer dispose close failed.", ex));
            }

            if (closeException != null && !closeExceptionReported)
            {
                OnServerCloseError(new InvalidOperationException("TcpServer dispose close failed.", closeException));
            }

            lock (_CloseLocker)
            {
                _IsRunning = false;

                currentSocket = CurrentSocket;
                CurrentSocket = null;

                if (_TcpServerClientDic.Count > 0)
                {
                    tcpServerClients = _TcpServerClientDic.Values.ToList();
                }
            }

            if (closeException == null && currentSocket.IfIsNull() && tcpServerClients.IfIsNullOrEmpty())
            {
                return;
            }

            CleanupDisposeFailureSocket(currentSocket);
            CleanupDisposeFailureTcpServerClients(tcpServerClients);
            CleanupDisposeFailureTrackedClients();

        }



    }

}
