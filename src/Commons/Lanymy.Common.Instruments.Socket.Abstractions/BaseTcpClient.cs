using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{


    public abstract class BaseTcpClient<TPackage, TSendPackage, TSessionToken, TFixedHeaderPackageFilter> : ITcpClient
        where TPackage : class
        where TSendPackage : class
        where TSessionToken : ISessionToken
        where TFixedHeaderPackageFilter : IFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken>
    {


        public System.Net.Sockets.Socket CurrentSocket { get; private set; }

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


        public int ReceiveBufferSize { get; }
        public int SendBufferSize { get; }
        public bool IsRunning => _IsRunning;
        public bool IsDisposed => _IsDisposed;
        public string ServerIP { get; }
        public int Port { get; }

        #region 内部变量

        protected readonly TFixedHeaderPackageFilter _CurrentFixedHeaderPackageFilter;
        protected readonly int _SendDataIntervalMilliseconds;

        private volatile bool _IsFirstStart = true;
        protected volatile bool _IsRunning = false;
        protected volatile bool _IsDisposed = false;

        protected volatile int _CurrentReadCount = 0;

        protected NetworkStream _CurrentNetworkStream;
        protected readonly BufferModel _CurrentBuffer;
        protected readonly CacheModel _CurrentCache;

        protected readonly object _CloseLocker = new Object();
        protected readonly object _ErrorLocker = new Object();

        protected WorkTaskQueue<byte[]> _CurrentSendWorkTaskQueue;


        #endregion


        protected BaseTcpClient(TFixedHeaderPackageFilter fixedHeaderPackageFilter, string serverIP, int port, int sendDataIntervalMilliseconds = 500, int receiveBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendBufferSize = BufferSizeKeys.BUFFER_SIZE_8K)
        {

            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
            ServerIP = serverIP;
            Port = port;

            _SendDataIntervalMilliseconds = sendDataIntervalMilliseconds;
            _CurrentFixedHeaderPackageFilter = fixedHeaderPackageFilter;
            _CurrentBuffer = new BufferModel(ReceiveBufferSize);
            _CurrentCache = new CacheModel(ReceiveBufferSize);

            CurrentSocket = CreateSocket();
            _CurrentSendWorkTaskQueue = CreateSendWorkTaskQueue();


        }


        #region 通知事件


        protected abstract void OnConnectionEvent();

        protected virtual void OnConnection()
        {
            try
            {
                OnConnectionEvent();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        protected abstract void OnCloseEvent();


        protected virtual void OnReceiveDataEvent(BufferModel buffer, CacheModel cache)
        {

            try
            {
                while (true)
                {

                    var packageBytes = _CurrentFixedHeaderPackageFilter.GetPackageBytes(buffer, cache);

                    if (packageBytes.IfIsNull())
                    {
                        break;
                    }

                    if (!_CurrentFixedHeaderPackageFilter.CheckPackage(packageBytes))
                    {
                        OnError(new Exception("data bytes error"));
                        break;
                    }

                    OnReceivePackage(_CurrentFixedHeaderPackageFilter.DecodePackage(packageBytes));

                }
            }
            catch (Exception e)
            {
                OnError(e);
            }


        }

        protected abstract void OnReceivePackageEvent(TPackage package);

        protected virtual void OnReceivePackage(TPackage package)
        {

            try
            {
                OnReceivePackageEvent(package);
            }
            catch (Exception e)
            {

                ReportError(e);

            }

        }

        protected abstract void OnErrorEvent(Exception ex);

        protected virtual System.Net.Sockets.Socket CreateSocket()
        {
            return new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
            };
        }

        protected virtual WorkTaskQueue<byte[]> CreateSendWorkTaskQueue()
        {
            return new WorkTaskQueue<byte[]>(OnSendWorkTaskQueueAsync, null);
        }

        protected virtual void ReportError(Exception ex)
        {

            try
            {
                lock (_ErrorLocker)
                {
                    OnErrorEvent(ex);
                }
            }
            catch
            {

            }
        }

        protected virtual void OnError(Exception ex)
        {
            ReportError(ex);

            var closeException = TryCloseSynchronously();
            if (closeException != null)
            {
                OnCloseError(new InvalidOperationException("TcpClient close after error failed.", closeException));
            }
        }

        protected virtual void OnCloseError(Exception ex)
        {
            ReportError(ex);
        }

        protected virtual async Task StopAndDisposeSendQueueAsync(WorkTaskQueue<byte[]> currentSendWorkTaskQueue, string stopErrorMessage, string disposeErrorMessage)
        {
            if (currentSendWorkTaskQueue.IfIsNull())
            {
                return;
            }

            try
            {
                await currentSendWorkTaskQueue.StopAsync();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException(stopErrorMessage, ex));
            }

            try
            {
                currentSendWorkTaskQueue.Dispose();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException(disposeErrorMessage, ex));
            }
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

        #endregion

        public void Start()
        {
            if (_IsDisposed)
            {
                return;
            }

            if (_IsRunning)
            {
                return;
            }

            if (!_IsFirstStart)
            {
                OnError(new NotSupportedException("Not Supported ReStart! ReNew One!"));
                return;
            }

            var currentSocket = CurrentSocket;
            if (currentSocket.IfIsNull())
            {
                currentSocket = CreateSocket();
                CurrentSocket = currentSocket;
            }

            var currentSendWorkTaskQueue = _CurrentSendWorkTaskQueue;
            if (currentSendWorkTaskQueue.IfIsNull())
            {
                currentSendWorkTaskQueue = CreateSendWorkTaskQueue();
                _CurrentSendWorkTaskQueue = currentSendWorkTaskQueue;
            }

            _IsRunning = true;
            var sendWorkTaskQueueStarted = false;

            try
            {

                _CurrentBuffer.Clear();
                _CurrentCache.Clear();

                currentSocket.Connect(new IPEndPoint(IPAddress.Parse(ServerIP), Port));
                var currentNetworkStream = new NetworkStream(currentSocket);
                _CurrentNetworkStream = currentNetworkStream;

                WaitSynchronously(() => currentSendWorkTaskQueue.StartAsync());
                sendWorkTaskQueueStarted = true;

                OnConnection();

                if (CanContinueReceive(currentNetworkStream))
                {
                    currentNetworkStream.BeginRead(_CurrentBuffer.BufferData, _CurrentBuffer.Position, _CurrentBuffer.BufferSize, OnReceive, null);
                }

            }
            catch (Exception exception)
            {
                ResetStartState(currentSocket, currentSendWorkTaskQueue, sendWorkTaskQueueStarted);
                ReportError(exception);
            }


        }

        private void OnReceive(IAsyncResult ar)
        {
            NetworkStream currentNetworkStream = null;

            try
            {
                if (!_IsRunning)
                {
                    return;
                }

                currentNetworkStream = _CurrentNetworkStream;
                if (!CanContinueReceive(currentNetworkStream))
                {
                    return;
                }

                _CurrentReadCount = currentNetworkStream.EndRead(ar);

                if (_CurrentReadCount <= 0)
                {
                    Close();
                    return;
                }

                if (_CurrentReadCount > 0)
                {

                    _CurrentBuffer.Position = _CurrentReadCount;

                    OnReceiveDataEvent(_CurrentBuffer, _CurrentCache);


                }

                if (CanContinueReceive(currentNetworkStream))
                {
                    currentNetworkStream.BeginRead(_CurrentBuffer.BufferData, _CurrentBuffer.Position, _CurrentBuffer.BufferSize - _CurrentBuffer.Position, OnReceive, null);
                }

            }
            catch (Exception exception) when (CanIgnoreReceiveException(exception, currentNetworkStream))
            {
                return;
            }
            catch (Exception exception)
            {
                OnError(exception);
            }
        }

        protected virtual bool CanContinueReceive(NetworkStream currentNetworkStream)
        {
            return TcpReceiveGuardHelper.CanContinueReceive(_IsRunning, currentNetworkStream, _CurrentNetworkStream);
        }

        protected virtual bool CanIgnoreReceiveException(Exception exception, NetworkStream currentNetworkStream)
        {
            return TcpReceiveGuardHelper.CanIgnoreReceiveException(exception, CanContinueReceive(currentNetworkStream));
        }


        protected virtual async Task OnSendWorkTaskQueueAsync(byte[] sendDataBytes)
        {

            try
            {
                var currentNetworkStream = _CurrentNetworkStream;

                //if (_IsRunning && !sendDataBytes.IfIsNullOrEmpty() && IsConnected && !_CurrentNetworkStream.IfIsNull())
                if (_IsRunning && !sendDataBytes.IfIsNullOrEmpty() && !currentNetworkStream.IfIsNull())
                {

                    await currentNetworkStream.WriteAsync(sendDataBytes, 0, sendDataBytes.Length);
                    await currentNetworkStream.FlushAsync();
                    //CurrentSessionToken.LastSendDateTime = DateTime.Now;

                    await Task.Delay(_SendDataIntervalMilliseconds);

                }

            }
            catch (Exception exception)
            {
                OnError(exception);
            }

        }


        public virtual async Task SendAsync(byte[] sendDataBytes)
        {
            if (_IsDisposed || !_IsRunning || sendDataBytes.IfIsNullOrEmpty())
            {
                return;
            }

            try
            {
                await _CurrentSendWorkTaskQueue.AddToQueueAsync(sendDataBytes);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }


        public void Send(byte[] sendDataBytes)
        {
            var sendException = TryWaitSynchronously(() => SendAsync(sendDataBytes));
            if (sendException != null)
            {
                OnError(sendException);
            }
        }

        public void Send(TSendPackage sendPackage)
        {
            try
            {
                Send(_CurrentFixedHeaderPackageFilter.EncodePackage(sendPackage));
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        protected virtual void ResetStartState(System.Net.Sockets.Socket currentSocket, WorkTaskQueue<byte[]> currentSendWorkTaskQueue, bool sendWorkTaskQueueStarted)
        {
            _IsRunning = false;

            if (!currentSendWorkTaskQueue.IfIsNull())
            {
                try
                {
                    if (sendWorkTaskQueueStarted)
                    {
                        var stopException = TryWaitSynchronously(() => currentSendWorkTaskQueue.StopAsync());
                        if (stopException != null)
                        {
                            throw stopException;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ReportError(new InvalidOperationException("TcpClient reset start send queue failed.", ex));
                }

                try
                {
                    currentSendWorkTaskQueue.Dispose();
                }
                catch (Exception ex)
                {
                    ReportError(new InvalidOperationException("TcpClient dispose failed start send queue failed.", ex));
                }
            }

            if (!ReferenceEquals(_CurrentSendWorkTaskQueue, currentSendWorkTaskQueue))
            {
                currentSendWorkTaskQueue = null;
            }

            if (ReferenceEquals(_CurrentSendWorkTaskQueue, currentSendWorkTaskQueue))
            {
                _CurrentSendWorkTaskQueue = CreateSendWorkTaskQueue();
            }

            var currentNetworkStream = _CurrentNetworkStream;
            _CurrentNetworkStream = null;

            if (!currentNetworkStream.IfIsNull())
            {
                try
                {
                    currentNetworkStream.Dispose();
                }
                catch (Exception ex)
                {
                    ReportError(new InvalidOperationException("TcpClient reset start network stream failed.", ex));
                }
            }

            if (!currentSocket.IfIsNull())
            {
                try
                {
                    currentSocket.Dispose();
                }
                catch (Exception ex)
                {
                    ReportError(new InvalidOperationException("TcpClient reset start socket failed.", ex));
                }
            }

            if (ReferenceEquals(CurrentSocket, currentSocket))
            {
                CurrentSocket = CreateSocket();
            }

            _CurrentBuffer.Clear();
            _CurrentCache.Clear();
        }



        protected virtual async Task OnCloseAsync()
        {

            if (!_IsRunning)
            {
                return;
            }

            WorkTaskQueue<byte[]> currentSendWorkTaskQueue = null;
            NetworkStream currentNetworkStream = null;
            System.Net.Sockets.Socket currentSocket = null;

            lock (_CloseLocker)
            {

                if (IsRunning)
                {

                    _IsFirstStart = false;
                    _IsRunning = false;

                    currentSendWorkTaskQueue = _CurrentSendWorkTaskQueue;
                    currentNetworkStream = _CurrentNetworkStream;
                    currentSocket = CurrentSocket;

                }
                else
                {
                    return;
                }
            }

            try
            {
                await StopAndDisposeSendQueueAsync(currentSendWorkTaskQueue, "TcpClient close send queue failed.", "TcpClient dispose send queue failed.");
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpClient close unexpected send queue cleanup failed.", ex));
            }

            lock (_CloseLocker)
            {
                if (ReferenceEquals(_CurrentSendWorkTaskQueue, currentSendWorkTaskQueue))
                {
                    _CurrentSendWorkTaskQueue = null;
                }

                if (ReferenceEquals(_CurrentNetworkStream, currentNetworkStream))
                {
                    _CurrentNetworkStream = null;
                }

                if (ReferenceEquals(CurrentSocket, currentSocket))
                {
                    CurrentSocket = null;
                }
            }

            try
            {
                if (!currentNetworkStream.IfIsNull())
                {
                    currentNetworkStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpClient close network stream failed.", ex));
            }

            try
            {
                if (!currentSocket.IfIsNull())
                {
                    currentSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpClient shutdown socket failed.", ex));
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
                OnCloseError(new InvalidOperationException("TcpClient dispose socket failed.", ex));
            }

            try
            {

                _CurrentBuffer.Clear();
                _CurrentCache.Clear();

                OnCloseEvent();

            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpClient close finalization failed.", ex));
            }

        }

        public void Close()
        {
            var closeException = TryCloseSynchronously();
            if (closeException != null)
            {
                OnCloseError(new InvalidOperationException("TcpClient sync close failed.", closeException));
            }

        }

        public virtual async Task CloseAsync()
        {
            await OnCloseAsync();
        }



        public void Dispose()
        {
            WorkTaskQueue<byte[]> currentSendWorkTaskQueue = null;
            NetworkStream currentNetworkStream = null;
            System.Net.Sockets.Socket currentSocket = null;
            var wasRunning = false;
            Exception closeException = null;

            lock (_CloseLocker)
            {
                if (_IsDisposed)
                {
                    return;
                }

                wasRunning = _IsRunning;
                _IsDisposed = true;
                _IsFirstStart = false;

                if (!wasRunning)
                {
                    currentSendWorkTaskQueue = _CurrentSendWorkTaskQueue;
                    currentNetworkStream = _CurrentNetworkStream;
                    currentSocket = CurrentSocket;
                }
            }

            try
            {
                closeException = TryCloseSynchronously();
                if (closeException != null)
                {
                    OnCloseError(new InvalidOperationException("TcpClient dispose close failed.", closeException));
                }
            }
            catch (Exception ex)
            {
                closeException = ex;
                OnCloseError(new InvalidOperationException("TcpClient dispose close failed.", ex));
            }

            if (wasRunning)
            {
                if (closeException == null || _IsRunning)
                {
                    return;
                }

                lock (_CloseLocker)
                {
                    currentSendWorkTaskQueue ??= _CurrentSendWorkTaskQueue;
                    currentNetworkStream ??= _CurrentNetworkStream;
                    currentSocket ??= CurrentSocket;
                }
            }

            lock (_CloseLocker)
            {
                if (ReferenceEquals(_CurrentSendWorkTaskQueue, currentSendWorkTaskQueue))
                {
                    _CurrentSendWorkTaskQueue = null;
                }

                if (ReferenceEquals(_CurrentNetworkStream, currentNetworkStream))
                {
                    _CurrentNetworkStream = null;
                }

                if (ReferenceEquals(CurrentSocket, currentSocket))
                {
                    CurrentSocket = null;
                }

                _IsRunning = false;
            }

            try
            {
                currentSendWorkTaskQueue?.Dispose();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpClient dispose send queue failed.", ex));
            }

            try
            {
                currentNetworkStream?.Dispose();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpClient dispose network stream failed.", ex));
            }

            try
            {
                currentSocket?.Dispose();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpClient dispose socket failed.", ex));
            }

            try
            {
                _CurrentBuffer.Clear();
                _CurrentCache.Clear();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpClient dispose finalization failed.", ex));
            }

        }

    }
}
