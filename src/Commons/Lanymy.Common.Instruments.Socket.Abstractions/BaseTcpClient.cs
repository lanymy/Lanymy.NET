using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{


    public abstract class BaseTcpClient<TPackage, TSendPackage, TSessionToken, TFixedHeaderPackageFilter> : ITcpClient
        where TPackage : class
        where TSendPackage : class
        where TSessionToken : ISessionToken
        where TFixedHeaderPackageFilter : IFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken>
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


        public int ReceiveBufferSize { get; }
        public int SendBufferSize { get; }
        public bool IsRunning => _IsRunning;
        public string ServerIP { get; }
        public int Port { get; }

        #region 内部变量

        protected readonly TFixedHeaderPackageFilter _CurrentFixedHeaderPackageFilter;
        protected readonly int _SendDataIntervalMilliseconds;

        private volatile bool _IsFirstStart = true;
        protected volatile bool _IsSend = false;
        protected volatile bool _IsRunning = false;

        protected volatile int _CurrentReadCount = 0;

        protected NetworkStream _CurrentNetworkStream;
        protected readonly BufferModel _CurrentBuffer;
        protected readonly CacheModel _CurrentCache;

        protected readonly object _Locker = new Object();

        protected Queue<byte[]> _SendQueue = new Queue<byte[]>(SEND_MAX_COUNT);

        private const int SEND_MAX_COUNT = 100;

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

            CurrentSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
            };

        }


        #region 通知事件


        protected abstract void OnConnectionEvent();

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

        protected abstract void OnReceivePackage(TPackage package);

        protected abstract void OnErrorEvent(Exception ex);

        protected virtual void OnError(Exception ex)
        {

            OnErrorEvent(ex);
            Close();
        }

        #endregion

        public void Start()
        {

            if (_IsRunning)
            {
                return;
            }

            if (!_IsFirstStart)
            {
                OnError(new NotSupportedException("Not Supported ReStart!"));
                return;
            }


            _IsRunning = true;

            try
            {

                _CurrentBuffer.Clear();
                _CurrentCache.Clear();
                _IsSend = false;

                CurrentSocket.Connect(new IPEndPoint(IPAddress.Parse(ServerIP), Port));
                _CurrentNetworkStream = new NetworkStream(CurrentSocket);

                OnConnectionEvent();

                _CurrentNetworkStream.BeginRead(_CurrentBuffer.BufferData, _CurrentBuffer.Position, _CurrentBuffer.BufferSize, OnReceive, null);

            }
            catch (Exception exception)
            {
                OnError(exception);
            }


        }

        private void OnReceive(IAsyncResult ar)
        {

            try
            {

                _CurrentReadCount = _CurrentNetworkStream.EndRead(ar);

                if (_CurrentReadCount > 0)
                {

                    _CurrentBuffer.Position = _CurrentReadCount;

                    OnReceiveDataEvent(_CurrentBuffer, _CurrentCache);


                }

                if (_IsRunning)
                {
                    _CurrentNetworkStream.BeginRead(_CurrentBuffer.BufferData, _CurrentBuffer.Position, _CurrentBuffer.BufferSize - _CurrentBuffer.Position, OnReceive, null);
                }

            }
            catch (Exception exception)
            {
                OnError(exception);
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
                    }
                }

            }
        }

        public void Send(TSendPackage sendPackage)
        {
            Send(_CurrentFixedHeaderPackageFilter.EncodePackage(sendPackage));
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
                    OnError(exception);
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
                OnError(exception);
            }
        }


        protected virtual void OnClose()
        {

            if (!_IsRunning)
            {
                return;
            }

            _IsFirstStart = false;
            _IsRunning = false;

            if (IsConnected || !_CurrentNetworkStream.IfIsNull())
            {

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
