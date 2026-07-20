using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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

            _ReceiveWorkTaskQueue = new WorkTaskQueue<UdpSourceDataModel>(OnReceiveWorkTaskQueue, null);
            _SendWorkTaskQueue = new WorkTaskQueue<SendUdpDataModel>(OnSendWorkTaskQueueAsync, null);

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
                TaskHelper.SyncWait(receiveWorkTaskQueue.StartAsync());
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

                TaskHelper.SyncWait(sendWorkTaskQueue.StartAsync());
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
            catch
            {
            }

            try
            {
                if (sendWorkTaskQueueStarted && !sendWorkTaskQueue.IfIsNull())
                {
                    TaskHelper.SyncWait(sendWorkTaskQueue.StopAsync());
                }
            }
            catch
            {
            }

            try
            {
                if (receiveWorkTaskQueueStarted && !receiveWorkTaskQueue.IfIsNull())
                {
                    TaskHelper.SyncWait(receiveWorkTaskQueue.StopAsync());
                }
            }
            catch
            {
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
            catch
            {
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

            TaskHelper.SyncWait(CloseAsync());

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
                if (!receiveWorkTaskQueue.IfIsNull())
                {
                    await receiveWorkTaskQueue.StopAsync();
                }
            }
            catch
            {

            }

            try
            {
                if (!sendWorkTaskQueue.IfIsNull())
                {
                    await sendWorkTaskQueue.StopAsync();
                }
            }
            catch
            {

            }

            lock (_CloseLocker)
            {
                if (ReferenceEquals(_CurrentUdpClient, currentUdpClient))
                {
                    _CurrentUdpClient = null;
                }
            }


            try
            {

                if (!currentUdpClient.IfIsNull())
                {
                    currentUdpClient.Close();
                    currentUdpClient.Dispose();
                }

            }
            catch
            {

            }


            try
            {
                OnCloseEvent();
            }
            catch (Exception ex)
            {
                ReportError(null, ex);
            }
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

            Close();

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
                receiveWorkTaskQueue?.Dispose();
            }
            catch
            {
            }

            try
            {
                sendWorkTaskQueue?.Dispose();
            }
            catch
            {
            }

            try
            {
                currentUdpClient?.Close();
                currentUdpClient?.Dispose();
            }
            catch
            {
            }
        }

    }

}
