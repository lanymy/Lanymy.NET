using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
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
        protected volatile bool _IsSend = false;

        protected volatile int _CurrentReadCount = 0;

        protected NetworkStream _CurrentNetworkStream;
        protected readonly BufferModel _CurrentBuffer;
        protected readonly CacheModel _CurrentCache;

        protected readonly TimerWorkTask _CurrentHeartTimerWorkTask;

        protected readonly object _Locker = new Object();

        protected Queue<byte[]> _SendQueue = new Queue<byte[]>(SEND_MAX_COUNT);

        private const int SEND_MAX_COUNT = 100;


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

        }


        #region 通知事件

        protected abstract void OnStartReceiveEvent();

        protected virtual void OnStartReceive()
        {
            OnStartReceiveEvent();
            if (!StartReceiveEvent.IfIsNull())
            {
                StartReceiveEvent(this);
            }
        }


        protected abstract void OnCloseEvent();



        protected abstract void OnServerClientErrorEvent(Exception ex);

        protected virtual void OnServerClientError(Exception ex)
        {

            OnServerClientErrorEvent(ex);

            if (!ServerClientErrorEvent.IfIsNull())
            {
                ServerClientErrorEvent(this, ex);
            }

            Close();

        }

        protected abstract void OnReceiveDataEvent(BufferModel buffer, CacheModel cache);

        protected virtual void OnReceiveData(BufferModel buffer, CacheModel cache)
        {

            OnReceiveDataEvent(buffer, cache);

            if (!ReceiveDataEvent.IfIsNull())
            {
                try
                {
                    ReceiveDataEvent(this, buffer, cache);
                }
                catch (Exception exception)
                {
                    OnServerClientError(exception);
                }
            }
        }

        #endregion

        private TimerWorkTaskDataResult OnHeartTimerWorkTask()
        {

            if (!HeartEvent.IfIsNull())
            {
                HeartEvent(this);
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

            _CurrentHeartTimerWorkTask.StartAsync().Wait();

        }


        private void BeginReceive()
        {
            try
            {
                _CurrentBuffer.Clear();
                _CurrentCache.Clear();
                _CurrentNetworkStream = new NetworkStream(CurrentSocket);
                _CurrentNetworkStream.BeginRead(_CurrentBuffer.BufferData, _CurrentBuffer.Position, _CurrentBuffer.BufferSize, OnReceive, null);
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

                _CurrentReadCount = _CurrentNetworkStream.EndRead(ar);

                if (_CurrentReadCount > 0)
                {

                    CurrentSessionToken.LastReceiveDateTime = DateTime.Now;

                    _CurrentBuffer.Position = _CurrentReadCount;

                    OnReceiveData(_CurrentBuffer, _CurrentCache);

                }

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



        public void Send(byte[] data)
        {

            if (!data.IfIsNullOrEmpty() && IsConnected)
            {
                lock (_Locker)
                {
                    if (_SendQueue.Count < SEND_MAX_COUNT)
                    {
                        _SendQueue.Enqueue(data);
                        BeginSend();
                        CurrentSessionToken.LastSendDateTime = DateTime.Now;
                    }
                }
            }

        }

        private void BeginSend()
        {

            if (!_IsSend && IsConnected && _SendQueue.Count > 0)
            {

                _IsSend = true;

                Task.Delay(_SendDataIntervalMilliseconds).Wait();


                var sendDataBytes = _SendQueue.Dequeue();

                try
                {

                    _CurrentNetworkStream.BeginWrite(sendDataBytes, 0, sendDataBytes.Length, OnSend, null);

                }
                catch (Exception exception)
                {
                    OnServerClientError(exception);
                }
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                _CurrentNetworkStream.EndWrite(ar);
                _IsSend = false;
                BeginSend();
            }
            catch (Exception exception)
            {
                OnServerClientError(exception);
            }
        }


        protected virtual void OnClose()
        {

            if (!_IsRunning)
            {
                return;
            }


            _IsRunning = false;


            if (IsConnected || !_CurrentNetworkStream.IfIsNull())
            {


                try
                {
                    _CurrentHeartTimerWorkTask.StopAsync().Wait();
                    _CurrentHeartTimerWorkTask.Dispose();
                }
                catch
                {

                }

                try
                {
                    _CurrentNetworkStream.Dispose();
                    _CurrentNetworkStream = null;
                }
                catch
                {

                }

                try
                {
                    CurrentSocket.Shutdown(SocketShutdown.Both);
                }
                catch
                {

                }

                try
                {
                    CurrentSocket.Dispose();
                }
                catch
                {

                }

                try
                {

                    _CurrentBuffer.Clear();
                    _CurrentCache.Clear();

                    _IsSend = false;
                    if (!_SendQueue.IfIsNull())
                    {
                        _SendQueue.Clear();
                    }
                    _SendQueue = null;


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
                catch
                {

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
