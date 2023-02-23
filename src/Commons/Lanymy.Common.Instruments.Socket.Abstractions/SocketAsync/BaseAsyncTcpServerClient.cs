//using System.Collections.Generic;
//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading.Tasks;
//using Lanymy.Common.ConstKeys;
//using Lanymy.Common.ExtensionFunctions;
//using Lanymy.Common.Instruments.Common;
//using System.Threading;

//namespace Lanymy.Common.Instruments.SocketAsync
//{


//    public abstract class BaseAsyncTcpServerClient : IAsyncTcpServerClient
//    {

//        public System.Net.Sockets.Socket CurrentSocket { get; }

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

//        public EndPoint LocalEndPoint
//        {
//            get
//            {
//                if (CurrentSocket.IfIsNull())
//                {
//                    return null;
//                }
//                return CurrentSocket.LocalEndPoint;
//            }
//        }
//        public EndPoint RemoteEndPoint
//        {
//            get
//            {
//                if (CurrentSocket == null)
//                {
//                    return null;
//                }
//                return CurrentSocket.RemoteEndPoint;
//            }
//        }

//        public ISessionToken CurrentSessionToken { get; set; }

//        public int ReceiveBufferSize { get; }
//        public int SendBufferSize { get; }

//        public bool IsRunning => _IsRunning;


//        #region 通知事件


//        public event TcpServerClientErrorEventAsync ServerClientErrorEventAsync;

//        public event TcpReceiveDataEventAsync ReceiveDataEventAsync;

//        public event TcpStartReceiveEventAsync StartReceiveEventAsync;

//        public event TcpCloseEventAsync CloseEventAsync;

//        public event GetHeartBytesEvent GetHeartBytesEvent;


//        #endregion


//        #region 内部变量

//        protected readonly int _SendDataIntervalMilliseconds;
//        protected readonly int _HeartIntervalMilliseconds;


//        protected volatile bool _IsRunning = false;

//        protected volatile int _CurrentReadCount = 0;

//        protected NetworkStream _CurrentNetworkStream;
//        protected readonly BufferModel _CurrentBuffer;
//        protected readonly CacheModel _CurrentCache;

//        protected readonly SimpleWorkTask _CurrentReceiveSimpleWorkTask;
//        protected readonly WorkTaskQueue<byte[]> _CurrentSendWorkTaskQueue;
//        protected readonly TimerWorkTask _CurrentHeartTimerWorkTask;



//        #endregion


//        protected BaseAsyncTcpServerClient(System.Net.Sockets.Socket socket, int receiveBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendBufferSize = BufferSizeKeys.BUFFER_SIZE_8K, int sendDataIntervalMilliseconds = 500, int heartIntervalMilliseconds = 3 * 1000)
//        {

//            _SendDataIntervalMilliseconds = sendDataIntervalMilliseconds;
//            _HeartIntervalMilliseconds = heartIntervalMilliseconds;
//            ReceiveBufferSize = receiveBufferSize;
//            SendBufferSize = sendBufferSize;

//            _CurrentBuffer = new BufferModel(ReceiveBufferSize);
//            _CurrentCache = new CacheModel(ReceiveBufferSize);

//            CurrentSocket = socket;
//            CurrentSocket.SendBufferSize = SendBufferSize;
//            CurrentSocket.ReceiveBufferSize = ReceiveBufferSize;


//            _CurrentHeartTimerWorkTask = new TimerWorkTask(OnHeartTimerWorkTask, _HeartIntervalMilliseconds);
//            _CurrentSendWorkTaskQueue = new WorkTaskQueue<byte[]>(OnSendWorkTaskQueue, null);
//            _CurrentReceiveSimpleWorkTask = new SimpleWorkTask(OnReceiveSimpleWorkTask, 0);

//        }


//        #region 通知事件

//        protected abstract Task OnStartReceiveEventAsync();

//        protected virtual async Task OnStartReceiveAsync()
//        {
//            await OnStartReceiveEventAsync();
//            if (!StartReceiveEventAsync.IfIsNull())
//            {
//                await StartReceiveEventAsync(this);
//            }
//        }


//        protected abstract Task OnCloseEventAsync();



//        protected abstract Task OnServerClientErrorEventAsync(Exception ex);

//        protected virtual async Task OnServerClientErrorAsync(Exception ex)
//        {

//            await OnServerClientErrorEventAsync(ex);

//            if (!ServerClientErrorEventAsync.IfIsNull())
//            {
//                await ServerClientErrorEventAsync(this, ex);
//            }

//            await CloseAsync();

//        }

//        protected abstract Task OnReceiveDataEventAsync(BufferModel buffer, CacheModel cache);

//        protected virtual async Task OnReceiveDataAsync(BufferModel buffer, CacheModel cache)
//        {

//            await OnReceiveDataEventAsync(buffer, cache);

//            if (!ReceiveDataEventAsync.IfIsNull())
//            {
//                try
//                {
//                    await ReceiveDataEventAsync(this, buffer, cache);
//                }
//                catch (Exception exception)
//                {
//                    await OnServerClientErrorAsync(exception);
//                }
//            }
//        }

//        #endregion



//        private TimerWorkTaskDataResult OnHeartTimerWorkTask()
//        {

//            OnHeartTimerWorkTaskAsync().Wait();

//            return null;
//        }

//        private async Task OnHeartTimerWorkTaskAsync()
//        {
//            if (!GetHeartBytesEvent.IfIsNull())
//            {
//                await SendAsync(GetHeartBytesEvent(CurrentSessionToken));
//            }
//        }


//        private async void OnSendWorkTaskQueue(byte[] bytes)
//        {
//            await OnSendWorkTaskQueueAsync(bytes);
//        }

//        private async Task OnSendWorkTaskQueueAsync(byte[] bytes)
//        {

//            if (!bytes.IfIsNullOrEmpty() && IsConnected)
//            {

//                await Task.Delay(_SendDataIntervalMilliseconds);

//                try
//                {

//                    await _CurrentNetworkStream.WriteAsync(bytes, 0, bytes.Length);
//                    CurrentSessionToken.LastSendDateTime = DateTime.Now;

//                }
//                catch (Exception exception)
//                {
//                    await OnServerClientErrorAsync(exception);
//                }

//            }

//        }


//        private async void OnReceiveSimpleWorkTask(CancellationToken token)
//        {
//            await OnReceiveSimpleWorkTaskAsync(token);
//        }

//        private async Task OnReceiveSimpleWorkTaskAsync(CancellationToken token)
//        {

//            try
//            {

//                _CurrentReadCount = await _CurrentNetworkStream.ReadAsync(_CurrentBuffer.BufferData, _CurrentBuffer.Position, _CurrentBuffer.BufferSize, token);

//                if (_CurrentReadCount > 0)
//                {

//                    CurrentSessionToken.LastReceiveDateTime = DateTime.Now;

//                    _CurrentBuffer.Position = _CurrentReadCount;

//                    await OnReceiveDataAsync(_CurrentBuffer, _CurrentCache);

//                }

//            }
//            catch (Exception exception)
//            {
//                await OnServerClientErrorAsync(exception);
//            }

//        }


//        internal async Task StartReceiveAsync()
//        {

//            if (_IsRunning)
//            {
//                return;
//            }

//            _IsRunning = true;

//            await BeginReceiveAsync();

//        }


//        private async Task BeginReceiveAsync()
//        {

//            try
//            {

//                _CurrentBuffer.Clear();
//                _CurrentCache.Clear();
//                _CurrentNetworkStream = new NetworkStream(CurrentSocket);

//                await _CurrentReceiveSimpleWorkTask.StartAsync();
//                await _CurrentSendWorkTaskQueue.StartAsync();
//                await _CurrentHeartTimerWorkTask.StartAsync();

//                //_CurrentNetworkStream.BeginRead(_CurrentBuffer.BufferData, _CurrentBuffer.Position, _CurrentBuffer.BufferSize, OnReceive, null);

//            }
//            catch (Exception exception)
//            {
//                await OnServerClientErrorAsync(exception);
//            }

//        }


//        public async Task SendAsync(byte[] data)
//        {
//            await _CurrentSendWorkTaskQueue.AddToQueueAsync(data);
//        }


//        protected virtual async Task OnCloseAsync()
//        {

//            if (!_IsRunning)
//            {
//                return;
//            }


//            _IsRunning = false;


//            if (IsConnected || !_CurrentNetworkStream.IfIsNull())
//            {

//                try
//                {
//                    await _CurrentReceiveSimpleWorkTask.StopAsync();
//                    await _CurrentSendWorkTaskQueue.StopAsync();
//                    await _CurrentHeartTimerWorkTask.StopAsync();

//                    _CurrentReceiveSimpleWorkTask.Dispose();
//                    _CurrentSendWorkTaskQueue.Dispose();
//                    _CurrentHeartTimerWorkTask.Dispose();

//                }
//                catch
//                {

//                }

//                try
//                {
//                    await _CurrentNetworkStream.DisposeAsync();
//                    _CurrentNetworkStream = null;
//                }
//                catch
//                {

//                }

//                try
//                {
//                    CurrentSocket.Shutdown(SocketShutdown.Both);
//                }
//                catch
//                {

//                }

//                try
//                {
//                    CurrentSocket.Dispose();
//                }
//                catch
//                {

//                }

//                try
//                {

//                    _CurrentBuffer.Clear();
//                    _CurrentCache.Clear();

//                    await OnCloseEventAsync();

//                    if (!CloseEventAsync.IfIsNull())
//                    {
//                        CloseEventAsync(this);
//                    }


//                    ServerClientErrorEventAsync = null;
//                    ReceiveDataEventAsync = null;
//                    StartReceiveEventAsync = null;
//                    CloseEventAsync = null;
//                    GetHeartBytesEvent = null;

//                    //CurrentSessionToken = null;

//                }
//                catch
//                {

//                }

//            }

//        }


//        public async Task CloseAsync()
//        {
//            await OnCloseAsync();
//        }



//        public async ValueTask DisposeAsync()
//        {
//            await CloseAsync();
//        }


//    }

//}
