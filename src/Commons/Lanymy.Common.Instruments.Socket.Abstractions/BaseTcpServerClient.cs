using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 表示服务端 accepted client 的收发和生命周期管理基类。
    /// </summary>
    public abstract class BaseTcpServerClient : ITcpServerClient
    {
        /// <summary>
        /// 当前 accepted socket。
        /// </summary>
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

        public EndPoint LocalEndPoint
        {
            get
            {
                if (CurrentSocket.IfIsNull())
                {
                    return null;
                }
                return CurrentSocket.LocalEndPoint;
            }
        }
        public EndPoint RemoteEndPoint
        {
            get
            {
                if (CurrentSocket == null)
                {
                    return null;
                }
                return CurrentSocket.RemoteEndPoint;
            }
        }

        public ISessionToken CurrentSessionToken { get; set; }

        public int ReceiveBufferSize { get; }
        public int SendBufferSize { get; }

        public bool IsRunning => _IsRunning;
        public bool IsDisposed => _IsDisposed;



        public event TcpServerClientErrorEvent ServerClientErrorEvent;

        public event TcpReceiveDataEvent ReceiveDataEvent;

        public event TcpStartReceiveEvent StartReceiveEvent;

        public event TcpCloseEvent CloseEvent;

        public event TcpHeartEvent HeartEvent;

        protected readonly int _SendDataIntervalMilliseconds;

        protected volatile bool _IsRunning = false;
        protected volatile bool _IsDisposed = false;
        //protected volatile bool _IsSend = false;

        protected volatile int _CurrentReadCount = 0;

        protected NetworkStream _CurrentNetworkStream;
        protected readonly BufferModel _CurrentBuffer;
        protected readonly CacheModel _CurrentCache;


        protected readonly object _ServerClientErrorLocker = new Object();
        protected readonly object _CloseLocker = new Object();


        protected TimerWorkTask _CurrentHeartTimerWorkTask;
        protected WorkTaskQueue<byte[]> _CurrentSendWorkTaskQueue;
        
        /// <summary>
        /// 初始化服务端 client 包装对象。
        /// </summary>
        /// <param name="socket">accepted socket。</param>
        /// <param name="receiveBufferSize">接收缓冲区大小。</param>
        /// <param name="sendBufferSize">发送缓冲区大小。</param>
        /// <param name="sendDataIntervalMilliseconds">连续发送之间的节流间隔。</param>
        /// <param name="heartIntervalMilliseconds">心跳检查间隔。</param>
        protected BaseTcpServerClient(System.Net.Sockets.Socket socket, int receiveBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendDataIntervalMilliseconds = 500, int heartIntervalMilliseconds = 3 * 1000)
        {
            _SendDataIntervalMilliseconds = sendDataIntervalMilliseconds;
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;

            _CurrentBuffer = new BufferModel(ReceiveBufferSize);
            _CurrentCache = new CacheModel(ReceiveBufferSize);

            CurrentSocket = socket;
            CurrentSocket.SendBufferSize = SendBufferSize;
            CurrentSocket.ReceiveBufferSize = ReceiveBufferSize;

            _CurrentHeartTimerWorkTask = new TimerWorkTask(OnHeartTimerWorkTask, heartIntervalMilliseconds);
            _CurrentSendWorkTaskQueue = new WorkTaskQueue<byte[]>(OnSendWorkTaskQueueAsync, null);
        }

        protected abstract void OnStartReceiveEvent();

        protected virtual void OnStartReceive()
        {

            try
            {
                OnStartReceiveEvent();
                if (!StartReceiveEvent.IfIsNull())
                {
                    StartReceiveEvent(this);
                }
            }
            catch (Exception ex)
            {
                ReportServerClientError(ex);
            }

        }


        protected abstract void OnCloseEvent();



        protected abstract void OnServerClientErrorEvent(Exception ex);

        protected virtual void ReportServerClientError(Exception ex)
        {
            try
            {
                OnServerClientErrorEvent(ex);

                lock (_ServerClientErrorLocker)
                {

                    if (!ServerClientErrorEvent.IfIsNull())
                    {
                        ServerClientErrorEvent(this, ex);
                    }

                }

            }
            catch
            {

            }
        }

        protected virtual void OnServerClientError(Exception ex)
        {
            ReportServerClientError(ex);

            Exception closeException = null;

            try
            {
                closeException = TryCloseSynchronously();
            }
            catch (Exception closeEx)
            {
                closeException = closeEx;
            }

            if (closeException != null)
            {
                OnCloseError(new InvalidOperationException("TcpServerClient close after error failed.", closeException));
            }
        }

        protected virtual void OnCloseError(Exception ex)
        {
            ReportServerClientError(ex);
        }

        protected virtual async Task StopAndDisposeHeartTimerAsync(TimerWorkTask currentHeartTimerWorkTask, string stopErrorMessage, string disposeErrorMessage)
        {
            if (currentHeartTimerWorkTask.IfIsNull())
            {
                return;
            }

            try
            {
                await currentHeartTimerWorkTask.StopAsync();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException(stopErrorMessage, ex));
            }

            try
            {
                currentHeartTimerWorkTask.Dispose();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException(disposeErrorMessage, ex));
            }
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

        protected virtual void ClearServerClientEvents()
        {
            ServerClientErrorEvent = null;
            ReceiveDataEvent = null;
            StartReceiveEvent = null;
            CloseEvent = null;
            HeartEvent = null;
        }

        protected abstract void OnReceiveDataEvent(BufferModel buffer, CacheModel cache);

        protected virtual void OnReceiveData(BufferModel buffer, CacheModel cache)
        {
            try
            {
                OnReceiveDataEvent(buffer, cache);

                if (!ReceiveDataEvent.IfIsNull())
                {
                    ReceiveDataEvent(this, buffer, cache);
                }

            }
            catch (Exception ex)
            {
                ReportServerClientError(ex);
            }

        }

        private TimerWorkTaskDataResult OnHeartTimerWorkTask()
        {
            try
            {
                if (!HeartEvent.IfIsNull())
                {
                    HeartEvent(this);
                }
            }
            catch (Exception ex)
            {
                ReportServerClientError(ex);
            }

            return null;
        }

        internal async Task StartReceiveAsync()
        {
            if (_IsDisposed)
            {
                return;
            }

            if (_IsRunning)
            {
                return;
            }

            _IsRunning = true;

            var sendWorkTaskQueueStarted = false;
            var heartTimerStarted = false;

            try
            {
                if (!TryBeginReceive())
                {
                    await ResetStartReceiveStateAsync(sendWorkTaskQueueStarted, heartTimerStarted);
                    return;
                }

                if (!CanContinueStartReceive())
                {
                    await ResetStartReceiveStateAsync(sendWorkTaskQueueStarted, heartTimerStarted);
                    return;
                }

                // 先启动发送队列与心跳任务，再触发启动事件，保证事件内部可直接收发。
                await _CurrentSendWorkTaskQueue.StartAsync();
                sendWorkTaskQueueStarted = true;
                if (!CanContinueStartReceive())
                {
                    await ResetStartReceiveStateAsync(sendWorkTaskQueueStarted, heartTimerStarted);
                    return;
                }

                await _CurrentHeartTimerWorkTask.StartAsync();
                heartTimerStarted = true;
                if (!CanContinueStartReceive())
                {
                    await ResetStartReceiveStateAsync(sendWorkTaskQueueStarted, heartTimerStarted);
                    return;
                }

                OnStartReceive();
            }
            catch (Exception ex)
            {
                OnServerClientError(ex);
                await ResetStartReceiveStateAsync(sendWorkTaskQueueStarted, heartTimerStarted);
            }

        }

        protected virtual bool CanContinueStartReceive()
        {
            return _IsRunning
                   && !_CurrentSendWorkTaskQueue.IfIsNull()
                   && !_CurrentHeartTimerWorkTask.IfIsNull();
        }

        protected virtual async Task ResetStartReceiveStateAsync(bool sendWorkTaskQueueStarted, bool heartTimerStarted)
        {
            _IsRunning = false;

            if (heartTimerStarted && !_CurrentHeartTimerWorkTask.IfIsNull())
            {
                await StopAndDisposeHeartTimerAsync(_CurrentHeartTimerWorkTask, "TcpServerClient reset start heart timer failed.", "TcpServerClient reset start dispose heart timer failed.");
                _CurrentHeartTimerWorkTask = null;
            }

            if (sendWorkTaskQueueStarted && !_CurrentSendWorkTaskQueue.IfIsNull())
            {
                await StopAndDisposeSendQueueAsync(_CurrentSendWorkTaskQueue, "TcpServerClient reset start send queue failed.", "TcpServerClient reset start dispose send queue failed.");
                _CurrentSendWorkTaskQueue = null;
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
                    ReportServerClientError(new InvalidOperationException("TcpServerClient reset start network stream failed.", ex));
                }
            }

            _CurrentBuffer.Clear();
            _CurrentCache.Clear();
        }

        private bool TryBeginReceive()
        {
            try
            {
                if (_IsRunning)
                {
                    _CurrentBuffer.Clear();
                    _CurrentCache.Clear();
                    var currentNetworkStream = new NetworkStream(CurrentSocket);
                    _CurrentNetworkStream = currentNetworkStream;
                    if (!CanContinueReceive(currentNetworkStream))
                    {
                        return false;
                    }

                    // 接收链以 BeginRead 为起点，后续每次回调按流快照继续续接。
                    currentNetworkStream.BeginRead(_CurrentBuffer.BufferData, _CurrentBuffer.Position, _CurrentBuffer.BufferSize, OnReceive, null);
                }

                return true;
            }
            catch (Exception exception)
            {
                OnServerClientError(exception);
                return false;
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


        protected virtual void OnReceive(IAsyncResult ar)
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
                    // 会话时间戳更新放在拆包前，保证即便后续业务处理失败也能反映“最近有流量进入”。
                    CurrentSessionToken.LastReceiveDateTimeTotalMillisecondsFromInstantiation = DateTimeHelper.GetTotalMillisecondsFromInstantiation(DateTime.Now);
                    CurrentSessionToken.LastReceiveDateTime = DateTime.Now;

                    _CurrentBuffer.Position = _CurrentReadCount;

                    OnReceiveData(_CurrentBuffer, _CurrentCache);

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
                OnServerClientError(exception);
            }
        }


        protected virtual async Task OnSendWorkTaskQueueAsync(byte[] sendDataBytes)
        {
            try
            {
                var currentNetworkStream = _CurrentNetworkStream;

                if (_IsRunning && !sendDataBytes.IfIsNullOrEmpty() && !currentNetworkStream.IfIsNull())
                {
                    await currentNetworkStream.WriteAsync(sendDataBytes, 0, sendDataBytes.Length);
                    await currentNetworkStream.FlushAsync();
                    CurrentSessionToken.LastSendDateTime = DateTime.Now;

                    await Task.Delay(_SendDataIntervalMilliseconds);
                }
            }
            catch (Exception exception)
            {
                OnServerClientError(exception);
            }

        }


        public virtual void Send(byte[] sendDataBytes)
        {
            Exception sendException = null;

            try
            {
                sendException = TryWaitSynchronously(() => SendAsync(sendDataBytes));
            }
            catch (Exception ex)
            {
                sendException = ex;
            }

            if (sendException != null)
            {
                OnServerClientError(sendException);
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
                OnServerClientError(ex);
            }
        }


        protected virtual async Task OnCloseAsync()
        {
            if (!_IsRunning)
            {
                return;
            }

            TimerWorkTask currentHeartTimerWorkTask = null;
            WorkTaskQueue<byte[]> currentSendWorkTaskQueue = null;
            NetworkStream currentNetworkStream = null;
            System.Net.Sockets.Socket currentSocket = null;

            lock (_CloseLocker)
            {

                if (_IsRunning)
                {

                    _IsRunning = false;

                    currentHeartTimerWorkTask = _CurrentHeartTimerWorkTask;
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
                await StopAndDisposeHeartTimerAsync(currentHeartTimerWorkTask, "TcpServerClient close heart timer failed.", "TcpServerClient dispose heart timer failed.");
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpServerClient close unexpected heart timer cleanup failed.", ex));
            }

            try
            {
                await StopAndDisposeSendQueueAsync(currentSendWorkTaskQueue, "TcpServerClient close send queue failed.", "TcpServerClient dispose send queue failed.");
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpServerClient close unexpected send queue cleanup failed.", ex));
            }

            lock (_CloseLocker)
            {
                if (ReferenceEquals(_CurrentHeartTimerWorkTask, currentHeartTimerWorkTask))
                {
                    _CurrentHeartTimerWorkTask = null;
                }

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
                    // 先断开公开引用，再进入具体释放步骤，避免关闭中的对象继续暴露旧句柄。
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
                OnCloseError(new InvalidOperationException("TcpServerClient close network stream failed.", ex));
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
                OnCloseError(new InvalidOperationException("TcpServerClient shutdown socket failed.", ex));
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
                OnCloseError(new InvalidOperationException("TcpServerClient dispose socket failed.", ex));
            }

            try
            {
                _CurrentBuffer.Clear();
                _CurrentCache.Clear();

                OnCloseEvent();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpServerClient close finalization failed.", ex));
            }
            finally
            {
                try
                {
                    if (!CloseEvent.IfIsNull())
                    {
                        CloseEvent(this);
                    }
                }
                catch (Exception ex)
                {
                    OnCloseError(new InvalidOperationException("TcpServerClient close event failed.", ex));
                }
                finally
                {
                    ClearServerClientEvents();
                }
            }
        }

        public void Close()
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
                OnCloseError(new InvalidOperationException("TcpServerClient sync close failed.", closeException));
            }

        }

        public virtual async Task CloseAsync()
        {
            await OnCloseAsync();
        }

        public void Dispose()
        {
            TimerWorkTask currentHeartTimerWorkTask = null;
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

                if (!wasRunning)
                {
                    currentHeartTimerWorkTask = _CurrentHeartTimerWorkTask;
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
                    OnCloseError(new InvalidOperationException("TcpServerClient dispose close failed.", closeException));
                }
            }
            catch (Exception ex)
            {
                closeException = ex;
                OnCloseError(new InvalidOperationException("TcpServerClient dispose close failed.", ex));
            }

            if (wasRunning)
            {
                if (closeException == null || _IsRunning)
                {
                    return;
                }

                lock (_CloseLocker)
                {
                    currentHeartTimerWorkTask ??= _CurrentHeartTimerWorkTask;
                    currentSendWorkTaskQueue ??= _CurrentSendWorkTaskQueue;
                    currentNetworkStream ??= _CurrentNetworkStream;
                    currentSocket ??= CurrentSocket;
                }
            }

            lock (_CloseLocker)
            {
                if (ReferenceEquals(_CurrentHeartTimerWorkTask, currentHeartTimerWorkTask))
                {
                    _CurrentHeartTimerWorkTask = null;
                }

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
                    // Dispose 路径同样保证关闭后不再保留 socket 引用。
                    CurrentSocket = null;
                }

                _IsRunning = false;
            }

            try
            {
                currentHeartTimerWorkTask?.Dispose();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpServerClient dispose heart timer failed.", ex));
            }

            try
            {
                currentSendWorkTaskQueue?.Dispose();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpServerClient dispose send queue failed.", ex));
            }

            try
            {
                currentNetworkStream?.Dispose();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpServerClient dispose network stream failed.", ex));
            }

            try
            {
                currentSocket?.Dispose();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpServerClient dispose socket failed.", ex));
            }

            try
            {
                _CurrentBuffer.Clear();
                _CurrentCache.Clear();
            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpServerClient dispose finalization failed.", ex));
            }
            finally
            {
                ClearServerClientEvents();
            }
        }
    }
}
