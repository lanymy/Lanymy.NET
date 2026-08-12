using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{

    public abstract class BaseUdpClient<TSessionToken, TFixedHeaderPackageFilter, TPackage, TSendPackage> : IUdpClient
        where TSessionToken : ISessionToken
        where TFixedHeaderPackageFilter : IFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken>
        where TPackage : class, IUdpPackage
        where TSendPackage : class, IUdpPackage
    {

        public int Port { get; }
        public bool IsAccept => _IsRunning;
        public bool IsDisposed => _IsDisposed;

        #region 内部变量

        protected UdpClient _CurrentUdpClient;

        protected readonly TFixedHeaderPackageFilter _CurrentFixedHeaderPackageFilter;

        protected readonly int _SendDataIntervalMilliseconds;

        protected volatile bool _IsRunning = false;
        protected volatile bool _IsDisposed = false;

        protected readonly object _CloseLocker = new Object();

        protected WorkTaskQueue<UdpSourceDataModel> _ReceiveWorkTaskQueue;
        protected WorkTaskQueue<SendUdpDataModel> _SendWorkTaskQueue;


        #endregion


        protected BaseUdpClient(TFixedHeaderPackageFilter fixedHeaderPackageFilter, int port, int sendDataIntervalMilliseconds = 500)
        {

            Port = port;
            _SendDataIntervalMilliseconds = sendDataIntervalMilliseconds;
            _CurrentFixedHeaderPackageFilter = fixedHeaderPackageFilter;

            _ReceiveWorkTaskQueue = CreateReceiveWorkTaskQueue();
            _SendWorkTaskQueue = CreateSendWorkTaskQueue();

        }

        protected virtual WorkTaskQueue<UdpSourceDataModel> CreateReceiveWorkTaskQueue()
        {
            return new WorkTaskQueue<UdpSourceDataModel>(OnReceiveWorkTaskQueue, null);
        }

        protected virtual WorkTaskQueue<SendUdpDataModel> CreateSendWorkTaskQueue()
        {
            return new WorkTaskQueue<SendUdpDataModel>(OnSendWorkTaskQueueAsync, null);
        }




        #region 通知事件


        private void OnReceiveWorkTaskQueue(UdpSourceDataModel udpSourceDataModel)
        {

            try
            {
                OnReceiveDataEvent(udpSourceDataModel.RemoteIPEndPoint, udpSourceDataModel.SourceDataBytes);
            }
            catch (Exception ex)
            {
                ReportError(udpSourceDataModel.RemoteIPEndPoint, ex);
            }

        }


        protected virtual void OnReceiveDataEvent(IPEndPoint remoteIPEndPoint, byte[] packageBytes)
        {

            //System.Diagnostics.Debug.WriteLine(string.Format("[ {0:HH:mm:ss:fff} ] - {1}", DateTime.Now, remoteIPEndPoint));

            if (!_CurrentFixedHeaderPackageFilter.CheckPackage(packageBytes))
            {
                ReportError(remoteIPEndPoint, new Exception("data bytes error"));
                return;
            }

            var package = _CurrentFixedHeaderPackageFilter.DecodePackage(packageBytes);
            package.RemoteIpEndPoint = remoteIPEndPoint;

            DispatchReceivePackage(package);

        }


        protected abstract void OnReceivePackage(TPackage package);

        protected virtual void DispatchReceivePackage(TPackage package)
        {

            try
            {
                OnReceivePackage(package);
            }
            catch (Exception ex)
            {
                ReportError(package?.RemoteIpEndPoint, ex);
            }

        }


        protected virtual void OnErrorEvent(IPEndPoint remoteIPEndPoint, Exception ex)
        {

        }

        protected virtual void ReportError(IPEndPoint remoteIPEndPoint, Exception ex)
        {

            try
            {
                OnErrorEvent(remoteIPEndPoint, ex);
            }
            catch
            {

            }

        }

        protected virtual void OnCloseError(Exception ex)
        {
            ReportError(null, ex);
        }

        protected virtual void WaitSynchronously(Func<Task> taskFactory)
        {
            TaskHelper.SyncWait(taskFactory);
        }

        protected virtual Exception TryWaitSynchronously(Func<Task> taskFactory)
        {
            return TaskHelper.TrySyncWait(taskFactory);
        }

        protected virtual Exception TryCloseSynchronously()
        {
            return TryWaitSynchronously(CloseAsync);
        }

        protected async Task OnSendWorkTaskQueueAsync(SendUdpDataModel sendUdpDataModel)
        {

            try
            {
                var currentUdpClient = _CurrentUdpClient;

                if (_IsRunning && !sendUdpDataModel.PackageBytes.IfIsNullOrEmpty() && !currentUdpClient.IfIsNull())
                {

                    await currentUdpClient.SendAsync(sendUdpDataModel.PackageBytes, sendUdpDataModel.PackageBytes.Length, sendUdpDataModel.RemoteIpEndPoint);

                    await Task.Delay(_SendDataIntervalMilliseconds);

                }

            }
            catch (Exception ex)
            {
                ReportError(sendUdpDataModel.RemoteIpEndPoint, ex);
            }

        }


        #endregion


        public void Start()
        {
            if (!TryBeginStart(out var receiveWorkTaskQueue, out var sendWorkTaskQueue))
            {
                return;
            }

            UdpClient currentUdpClient = null;
            var receiveWorkTaskQueueStarted = false;
            var sendWorkTaskQueueStarted = false;

            try
            {
                WaitSynchronously(() => receiveWorkTaskQueue.StartAsync());
                receiveWorkTaskQueueStarted = true;

                if (!CanContinueStart(receiveWorkTaskQueue, sendWorkTaskQueue, null))
                {
                    return;
                }

                currentUdpClient = new UdpClient(Port);
                currentUdpClient.EnableBroadcast = true;
                if (!TryBindCurrentUdpClient(receiveWorkTaskQueue, sendWorkTaskQueue, currentUdpClient))
                {
                    try
                    {
                        currentUdpClient.Close();
                        currentUdpClient.Dispose();
                    }
                    catch
                    {
                    }
                    return;
                }

                currentUdpClient.BeginReceive(ReciveCallBack, null);

                if (!CanContinueStart(receiveWorkTaskQueue, sendWorkTaskQueue, currentUdpClient))
                {
                    return;
                }

                WaitSynchronously(() => sendWorkTaskQueue.StartAsync());
                sendWorkTaskQueueStarted = true;

                if (!CanContinueStart(receiveWorkTaskQueue, sendWorkTaskQueue, currentUdpClient))
                {
                    return;
                }

                OnStart();
            }
            catch
            {
                ResetStartState(receiveWorkTaskQueue, sendWorkTaskQueue, currentUdpClient, receiveWorkTaskQueueStarted, sendWorkTaskQueueStarted);
                throw;
            }

        }

        protected virtual bool TryBeginStart(out WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue, out WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue)
        {
            receiveWorkTaskQueue = null;
            sendWorkTaskQueue = null;

            lock (_CloseLocker)
            {
                if (_IsDisposed || _IsRunning)
                {
                    return false;
                }

                receiveWorkTaskQueue = _ReceiveWorkTaskQueue;
                sendWorkTaskQueue = _SendWorkTaskQueue;

                if (receiveWorkTaskQueue.IfIsNull() || sendWorkTaskQueue.IfIsNull())
                {
                    return false;
                }

                _IsRunning = true;
                return true;
            }
        }

        protected virtual bool CanContinueStart(WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue, WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue, UdpClient currentUdpClient)
        {
            lock (_CloseLocker)
            {
                if (_IsDisposed || !_IsRunning)
                {
                    return false;
                }

                if (!ReferenceEquals(_ReceiveWorkTaskQueue, receiveWorkTaskQueue) || !ReferenceEquals(_SendWorkTaskQueue, sendWorkTaskQueue))
                {
                    return false;
                }

                if (currentUdpClient != null && !ReferenceEquals(_CurrentUdpClient, currentUdpClient))
                {
                    return false;
                }

                return true;
            }
        }

        protected virtual bool TryBindCurrentUdpClient(WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue, WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue, UdpClient currentUdpClient)
        {
            lock (_CloseLocker)
            {
                if (_IsDisposed || !_IsRunning)
                {
                    return false;
                }

                if (!ReferenceEquals(_ReceiveWorkTaskQueue, receiveWorkTaskQueue) || !ReferenceEquals(_SendWorkTaskQueue, sendWorkTaskQueue))
                {
                    return false;
                }

                _CurrentUdpClient = currentUdpClient;
                return true;
            }
        }

        protected virtual void ResetStartState(WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue, WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue, UdpClient currentUdpClient, bool receiveWorkTaskQueueStarted, bool sendWorkTaskQueueStarted)
        {
            var receiveQueueStopFailed = false;
            var sendQueueStopFailed = false;
            WorkTaskQueue<UdpSourceDataModel> failedReceiveWorkTaskQueue = null;
            WorkTaskQueue<SendUdpDataModel> failedSendWorkTaskQueue = null;

            lock (_CloseLocker)
            {
                if (ReferenceEquals(_CurrentUdpClient, currentUdpClient))
                {
                    _CurrentUdpClient = null;
                }

                _IsRunning = false;
            }

            try
            {
                currentUdpClient?.Close();
                currentUdpClient?.Dispose();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("UdpClient reset start udp client failed.", ex));
            }

            try
            {
                if (sendWorkTaskQueueStarted && !sendWorkTaskQueue.IfIsNull())
                {
                    WaitSynchronously(() => sendWorkTaskQueue.StopAsync());
                }
            }
            catch (Exception ex)
            {
                sendQueueStopFailed = true;
                OnCloseError(new InvalidOperationException("UdpClient reset start send queue failed.", ex));
            }

            try
            {
                if (receiveWorkTaskQueueStarted && !receiveWorkTaskQueue.IfIsNull())
                {
                    WaitSynchronously(() => receiveWorkTaskQueue.StopAsync());
                }
            }
            catch (Exception ex)
            {
                receiveQueueStopFailed = true;
                OnCloseError(new InvalidOperationException("UdpClient reset start receive queue failed.", ex));
            }

            lock (_CloseLocker)
            {
                if (sendQueueStopFailed && ReferenceEquals(_SendWorkTaskQueue, sendWorkTaskQueue))
                {
                    failedSendWorkTaskQueue = _SendWorkTaskQueue;
                    _SendWorkTaskQueue = CreateSendWorkTaskQueue();
                }

                if (receiveQueueStopFailed && ReferenceEquals(_ReceiveWorkTaskQueue, receiveWorkTaskQueue))
                {
                    failedReceiveWorkTaskQueue = _ReceiveWorkTaskQueue;
                    _ReceiveWorkTaskQueue = CreateReceiveWorkTaskQueue();
                }
            }

            try
            {
                DisposeSendWorkTaskQueue(failedSendWorkTaskQueue);
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("UdpClient dispose send queue after reset start failure failed.", ex));
            }

            try
            {
                DisposeReceiveWorkTaskQueue(failedReceiveWorkTaskQueue);
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("UdpClient dispose receive queue after reset start failure failed.", ex));
            }
        }

        protected abstract void OnStartEvent();

        public virtual void OnStart()
        {

            try
            {
                OnStartEvent();
            }
            catch (Exception ex)
            {
                ReportError(null, ex);
            }

        }

        private void ReciveCallBack(IAsyncResult asyncResult)
        {
            if (!TryGetReceiveContext(out var currentUdpClient, out var receiveWorkTaskQueue))
            {
                return;
            }

            IPEndPoint remoteIPEndPoint = null;

            try
            {
                byte[] bytes = currentUdpClient.EndReceive(asyncResult, ref remoteIPEndPoint);//*结束挂起的异步接收

                var addReceiveQueueTask = receiveWorkTaskQueue.AddToQueueAsync(new UdpSourceDataModel
                {
                    RemoteIPEndPoint = remoteIPEndPoint,
                    SourceDataBytes = bytes,
                });
                TaskHelper.SyncWait(addReceiveQueueTask);

                if (CanContinueReceive(currentUdpClient, receiveWorkTaskQueue))
                {
                    currentUdpClient.BeginReceive(ReciveCallBack, null);
                }
            }
            catch (ObjectDisposedException ex)
            {
                if (_IsRunning)
                {
                    ReportError(remoteIPEndPoint, ex);
                }
            }
            catch (SocketException ex)
            {
                if (_IsRunning)
                {
                    ReportError(remoteIPEndPoint, ex);
                }
            }
            catch (Exception ex)
            {
                if (_IsRunning)
                {
                    ReportError(remoteIPEndPoint, ex);
                }
            }

        }

        protected virtual bool TryGetReceiveContext(out UdpClient currentUdpClient, out WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue)
        {
            lock (_CloseLocker)
            {
                if (_IsDisposed || !_IsRunning)
                {
                    currentUdpClient = null;
                    receiveWorkTaskQueue = null;
                    return false;
                }

                currentUdpClient = _CurrentUdpClient;
                receiveWorkTaskQueue = _ReceiveWorkTaskQueue;
                return !currentUdpClient.IfIsNull() && !receiveWorkTaskQueue.IfIsNull();
            }
        }

        protected virtual bool CanContinueReceive(UdpClient currentUdpClient, WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue)
        {
            lock (_CloseLocker)
            {
                if (_IsDisposed || !_IsRunning)
                {
                    return false;
                }

                return ReferenceEquals(_CurrentUdpClient, currentUdpClient)
                    && ReferenceEquals(_ReceiveWorkTaskQueue, receiveWorkTaskQueue);
            }
        }

        public bool Send(byte[] data, IPEndPoint remoteIpEndPoint)
        {
            return Send(new SendUdpDataModel(remoteIpEndPoint, data));
        }

        public bool Send(byte[] data, string remoteIP, int remotePort)
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
            return Send(data, ipEndPoint);
        }

        /// <summary>
        /// 255.255.255.255 广播
        /// </summary>
        /// <param name="data"></param>
        /// <param name="broadcastPort"></param>
        /// <returns></returns>
        public bool SendBroadcast(byte[] data, int broadcastPort)
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
            return Send(data, ipEndPoint);
        }

        public bool Send(TSendPackage sendPackage)
        {
            if (sendPackage == null)
            {
                return false;
            }

            var packageDataBytes = _CurrentFixedHeaderPackageFilter.EncodePackage(sendPackage);
            return Send(packageDataBytes, sendPackage.RemoteIpEndPoint);
        }

        public bool Send(SendUdpDataModel sendUdpDataModel)
        {
            try
            {
                return TrySendAsync(sendUdpDataModel).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                ReportError(sendUdpDataModel?.RemoteIpEndPoint, ex);
                return false;
            }
        }

        public async Task SendAsync(SendUdpDataModel sendUdpDataModel)
        {
            await TrySendAsync(sendUdpDataModel);
        }

        protected virtual async Task<bool> TrySendAsync(SendUdpDataModel sendUdpDataModel)
        {
            if (_IsDisposed || !_IsRunning || sendUdpDataModel == null || sendUdpDataModel.RemoteIpEndPoint == null || sendUdpDataModel.PackageBytes.IfIsNullOrEmpty())
            {
                return false;
            }

            try
            {
                var sendWorkTaskQueue = _SendWorkTaskQueue;
                if (!sendWorkTaskQueue.IfIsNull())
                {
                    await sendWorkTaskQueue.AddToQueueAsync(sendUdpDataModel);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ReportError(sendUdpDataModel.RemoteIpEndPoint, ex);
                return false;
            }
        }





        public void Close()
        {
            var closeException = TryCloseSynchronously();
            if (closeException != null)
            {
                OnCloseError(new InvalidOperationException("UdpClient sync close failed.", closeException));
            }

        }

        public virtual async Task CloseAsync()
        {

            if (!_IsRunning)
            {
                return;
            }

            WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue = null;
            WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue = null;
            UdpClient currentUdpClient = null;
            var receiveQueueStopFailed = false;
            var sendQueueStopFailed = false;
            WorkTaskQueue<UdpSourceDataModel> failedReceiveWorkTaskQueue = null;
            WorkTaskQueue<SendUdpDataModel> failedSendWorkTaskQueue = null;

            lock (_CloseLocker)
            {

                if (_IsRunning)
                {

                    _IsRunning = false;

                    receiveWorkTaskQueue = _ReceiveWorkTaskQueue;
                    sendWorkTaskQueue = _SendWorkTaskQueue;
                    currentUdpClient = _CurrentUdpClient;

                }
                else
                {
                    return;
                }

            }

            try
            {
                await StopReceiveWorkTaskQueueAsync(receiveWorkTaskQueue);
            }
            catch (Exception ex)
            {
                receiveQueueStopFailed = true;
                OnCloseError(new InvalidOperationException("UdpClient close receive queue failed.", ex));
            }

            try
            {
                await StopSendWorkTaskQueueAsync(sendWorkTaskQueue);
            }
            catch (Exception ex)
            {
                sendQueueStopFailed = true;
                OnCloseError(new InvalidOperationException("UdpClient close send queue failed.", ex));
            }

            lock (_CloseLocker)
            {
                if (receiveQueueStopFailed && ReferenceEquals(_ReceiveWorkTaskQueue, receiveWorkTaskQueue))
                {
                    failedReceiveWorkTaskQueue = _ReceiveWorkTaskQueue;
                    _ReceiveWorkTaskQueue = CreateReceiveWorkTaskQueue();
                }

                if (sendQueueStopFailed && ReferenceEquals(_SendWorkTaskQueue, sendWorkTaskQueue))
                {
                    failedSendWorkTaskQueue = _SendWorkTaskQueue;
                    _SendWorkTaskQueue = CreateSendWorkTaskQueue();
                }

                if (ReferenceEquals(_CurrentUdpClient, currentUdpClient))
                {
                    _CurrentUdpClient = null;
                }
            }

            try
            {
                DisposeReceiveWorkTaskQueue(failedReceiveWorkTaskQueue);
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("UdpClient dispose receive queue after close failure failed.", ex));
            }

            try
            {
                DisposeSendWorkTaskQueue(failedSendWorkTaskQueue);
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("UdpClient dispose send queue after close failure failed.", ex));
            }

            try
            {
                DisposeCurrentUdpClient(currentUdpClient);
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("UdpClient dispose udp client failed.", ex));
            }


            try
            {
                OnCloseEvent();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("UdpClient close finalization failed.", ex));
            }
        }

        protected virtual async Task StopReceiveWorkTaskQueueAsync(WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue)
        {
            if (!receiveWorkTaskQueue.IfIsNull())
            {
                await receiveWorkTaskQueue.StopAsync();
            }
        }

        protected virtual async Task StopSendWorkTaskQueueAsync(WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue)
        {
            if (!sendWorkTaskQueue.IfIsNull())
            {
                await sendWorkTaskQueue.StopAsync();
            }
        }

        protected virtual void DisposeCurrentUdpClient(UdpClient currentUdpClient)
        {
            if (!currentUdpClient.IfIsNull())
            {
                currentUdpClient.Close();
                currentUdpClient.Dispose();
            }
        }

        protected virtual void DisposeReceiveWorkTaskQueue(WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue)
        {
            receiveWorkTaskQueue?.Dispose();
        }

        protected virtual void DisposeSendWorkTaskQueue(WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue)
        {
            sendWorkTaskQueue?.Dispose();
        }


        protected abstract void OnCloseEvent();

        public void Dispose()
        {
            if (_IsDisposed)
            {
                return;
            }

            WorkTaskQueue<UdpSourceDataModel> receiveWorkTaskQueue = null;
            WorkTaskQueue<SendUdpDataModel> sendWorkTaskQueue = null;
            UdpClient currentUdpClient = null;

            lock (_CloseLocker)
            {
                if (_IsDisposed)
                {
                    return;
                }

                _IsDisposed = true;

                receiveWorkTaskQueue = _ReceiveWorkTaskQueue;
                sendWorkTaskQueue = _SendWorkTaskQueue;
                currentUdpClient = _CurrentUdpClient;
            }

            var disposeExceptions = new List<Exception>();

            var closeException = TryCloseSynchronously();
            if (closeException != null)
            {
                var disposeCloseException = new InvalidOperationException("UdpClient dispose close failed.", closeException);
                OnCloseError(disposeCloseException);
                disposeExceptions.Add(disposeCloseException);
            }

            lock (_CloseLocker)
            {
                if (ReferenceEquals(_ReceiveWorkTaskQueue, receiveWorkTaskQueue))
                {
                    _ReceiveWorkTaskQueue = null;
                }

                if (ReferenceEquals(_SendWorkTaskQueue, sendWorkTaskQueue))
                {
                    _SendWorkTaskQueue = null;
                }

                if (ReferenceEquals(_CurrentUdpClient, currentUdpClient))
                {
                    _CurrentUdpClient = null;
                }

                _IsRunning = false;
            }

            try
            {
                DisposeReceiveWorkTaskQueue(receiveWorkTaskQueue);
            }
            catch (Exception ex)
            {
                var disposeReceiveQueueException = new InvalidOperationException("UdpClient dispose receive queue failed.", ex);
                OnCloseError(disposeReceiveQueueException);
                disposeExceptions.Add(disposeReceiveQueueException);
            }

            try
            {
                DisposeSendWorkTaskQueue(sendWorkTaskQueue);
            }
            catch (Exception ex)
            {
                var disposeSendQueueException = new InvalidOperationException("UdpClient dispose send queue failed.", ex);
                OnCloseError(disposeSendQueueException);
                disposeExceptions.Add(disposeSendQueueException);
            }

            try
            {
                DisposeCurrentUdpClient(currentUdpClient);
            }
            catch (Exception ex)
            {
                var disposeUdpClientException = new InvalidOperationException("UdpClient dispose udp client failed.", ex);
                OnCloseError(disposeUdpClientException);
                disposeExceptions.Add(disposeUdpClientException);
            }

            if (disposeExceptions.Count == 1)
            {
                ExceptionDispatchInfo.Capture(disposeExceptions[0]).Throw();
            }

            if (disposeExceptions.Count > 1)
            {
                throw new AggregateException(disposeExceptions);
            }
        }

    }

}
