using System;
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


    public abstract class BaseTcpServerClient : ITcpServerClient
    {

        public System.Net.Sockets.Socket CurrentSocket { get; }

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



        #region 通知事件


        public event TcpServerClientErrorEvent ServerClientErrorEvent;

        public event TcpReceiveDataEvent ReceiveDataEvent;

        public event TcpStartReceiveEvent StartReceiveEvent;

        public event TcpCloseEvent CloseEvent;

        public event TcpHeartEvent HeartEvent;


        #endregion


        #region 内部变量

        protected readonly int _SendDataIntervalMilliseconds;

        protected volatile bool _IsRunning = false;
        //protected volatile bool _IsSend = false;

        protected volatile int _CurrentReadCount = 0;

        protected NetworkStream _CurrentNetworkStream;
        protected readonly BufferModel _CurrentBuffer;
        protected readonly CacheModel _CurrentCache;


        protected readonly object _ServerClientErrorLocker = new Object();
        protected readonly object _CloseLocker = new Object();


        protected TimerWorkTask _CurrentHeartTimerWorkTask;
        protected WorkTaskQueue<byte[]> _CurrentSendWorkTaskQueue;



        //protected Queue<byte[]> _SendQueue = new Queue<byte[]>(SEND_MAX_COUNT);

        //private const int SEND_MAX_COUNT = 100;


        #endregion




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





        #region 通知事件

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
                OnServerClientError(ex);
            }

        }


        protected abstract void OnCloseEvent();



        protected abstract void OnServerClientErrorEvent(Exception ex);

        protected virtual void OnServerClientError(Exception ex)
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

            Close();

        }

        protected virtual void OnCloseError(Exception ex)
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
                OnServerClientError(ex);
            }

        }

        #endregion

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
                OnServerClientError(ex);
            }

            return null;

        }




        internal void StartReceive()
        {

            if (_IsRunning)
            {
                return;
            }

            _IsRunning = true;

            BeginReceive();

            OnStartReceive();

            TaskHelper.SyncWait(_CurrentSendWorkTaskQueue.StartAsync());
            TaskHelper.SyncWait(_CurrentHeartTimerWorkTask.StartAsync());

        }


        private void BeginReceive()
        {
            try
            {

                if (_IsRunning)
                {

                    _CurrentBuffer.Clear();
                    _CurrentCache.Clear();
                    _CurrentNetworkStream = new NetworkStream(CurrentSocket);
                    _CurrentNetworkStream.BeginRead(_CurrentBuffer.BufferData, _CurrentBuffer.Position, _CurrentBuffer.BufferSize, OnReceive, null);

                }

            }
            catch (Exception exception)
            {
                OnServerClientError(exception);
            }
        }


        protected virtual void OnReceive(IAsyncResult ar)
        {

            try
            {

                if (!_IsRunning)
                    return;

                _CurrentReadCount = _CurrentNetworkStream.EndRead(ar);


                if (_CurrentReadCount > 0)
                {

                    //if (CurrentSessionToken != null)
                    //{
                    //    CurrentSessionToken.LastReceiveDateTime = DateTime.Now;
                    //}

                    CurrentSessionToken.LastReceiveDateTimeTotalMillisecondsFromInstantiation = DateTimeHelper.GetTotalMillisecondsFromInstantiation(DateTime.Now);
                    CurrentSessionToken.LastReceiveDateTime = DateTime.Now;

                    _CurrentBuffer.Position = _CurrentReadCount;

                    OnReceiveData(_CurrentBuffer, _CurrentCache);

                }

                //if (_IsRunning && IsConnected && !_CurrentNetworkStream.IfIsNull())
                if (_IsRunning)
                {
                    _CurrentNetworkStream.BeginRead(_CurrentBuffer.BufferData, _CurrentBuffer.Position, _CurrentBuffer.BufferSize - _CurrentBuffer.Position, OnReceive, null);
                }

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

                //if (_IsRunning && !sendDataBytes.IfIsNullOrEmpty() && IsConnected && !_CurrentNetworkStream.IfIsNull())
                if (_IsRunning && !sendDataBytes.IfIsNullOrEmpty())
                {

                    await _CurrentNetworkStream.WriteAsync(sendDataBytes, 0, sendDataBytes.Length);
                    await _CurrentNetworkStream.FlushAsync();

                    //if (CurrentSessionToken != null)
                    //{
                    //    CurrentSessionToken.LastSendDateTime = DateTime.Now;
                    //}

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

            try
            {
                TaskHelper.SyncWait(SendAsync(sendDataBytes));

            }
            catch
            {

            }

        }


        protected virtual async Task SendAsync(byte[] sendDataBytes)
        {

            try
            {

                //if (!sendDataBytes.IfIsNullOrEmpty() && IsConnected)
                if (_IsRunning && !sendDataBytes.IfIsNullOrEmpty())
                {
                    await _CurrentSendWorkTaskQueue.AddToQueueAsync(sendDataBytes);
                }

            }
            catch
            {

            }

        }


        protected virtual void OnClose()
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

                        if (!_CurrentHeartTimerWorkTask.IfIsNull())
                        {
                            TaskHelper.SyncWait(_CurrentHeartTimerWorkTask.StopAsync());
                            _CurrentHeartTimerWorkTask.Dispose();
                        }

                        _CurrentHeartTimerWorkTask = null;

                    }
                    catch (Exception ex)
                    {
                        OnCloseError(new InvalidOperationException("TcpServerClient close heart timer failed.", ex));
                    }


                    try
                    {

                        if (!_CurrentSendWorkTaskQueue.IfIsNull())
                        {
                            TaskHelper.SyncWait(_CurrentSendWorkTaskQueue.StopAsync());
                            _CurrentSendWorkTaskQueue.Dispose();
                        }

                        _CurrentSendWorkTaskQueue = null;

                    }
                    catch (Exception ex)
                    {
                        OnCloseError(new InvalidOperationException("TcpServerClient close send queue failed.", ex));
                    }


                    try
                    {

                        if (!_CurrentNetworkStream.IfIsNull())
                        {
                            _CurrentNetworkStream.Dispose();
                            _CurrentNetworkStream = null;
                        }

                    }
                    catch (Exception ex)
                    {
                        OnCloseError(new InvalidOperationException("TcpServerClient close network stream failed.", ex));
                    }

                    try
                    {
                        CurrentSocket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception ex)
                    {
                        OnCloseError(new InvalidOperationException("TcpServerClient shutdown socket failed.", ex));
                    }

                    try
                    {
                        CurrentSocket.Dispose();
                    }
                    catch (Exception ex)
                    {
                        OnCloseError(new InvalidOperationException("TcpServerClient dispose socket failed.", ex));
                    }


                    try
                    {

                        _CurrentBuffer.Clear();
                        _CurrentCache.Clear();

                        //_IsSend = false;
                        //if (!_SendQueue.IfIsNull())
                        //{
                        //    _SendQueue.Clear();
                        //}
                        //_SendQueue = null;


                        OnCloseEvent();

                        if (!CloseEvent.IfIsNull())
                        {
                            CloseEvent(this);
                        }

                        ServerClientErrorEvent = null;
                        ReceiveDataEvent = null;
                        StartReceiveEvent = null;
                        CloseEvent = null;
                        HeartEvent = null;

                        //CurrentSessionToken = null;

                    }
                    catch (Exception ex)
                    {
                        OnCloseError(new InvalidOperationException("TcpServerClient close finalization failed.", ex));
                    }


                }


            }

        }


        public void Close()
        {

            OnClose();

        }


        public void Dispose()
        {

            Close();

        }


    }

}
