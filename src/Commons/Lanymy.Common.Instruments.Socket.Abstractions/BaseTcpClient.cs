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
        protected volatile bool _IsRunning = false;

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

            CurrentSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
            };

            _CurrentSendWorkTaskQueue = new WorkTaskQueue<byte[]>(OnSendWorkTaskQueue, null);


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
                OnError(ex);
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

                OnError(e);

            }

        }

        protected abstract void OnErrorEvent(Exception ex);

        protected virtual void OnError(Exception ex)
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
                OnError(new NotSupportedException("Not Supported ReStart! ReNew One!"));
                return;
            }


            _IsRunning = true;

            try
            {

                _CurrentBuffer.Clear();
                _CurrentCache.Clear();

                CurrentSocket.Connect(new IPEndPoint(IPAddress.Parse(ServerIP), Port));
                _CurrentNetworkStream = new NetworkStream(CurrentSocket);

                OnConnection();

                _CurrentSendWorkTaskQueue.StartAsync().Wait();

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


        private async void OnSendWorkTaskQueue(byte[] sendDataBytes)
        {
            try
            {
                await OnSendWorkTaskQueueAsync(sendDataBytes);
            }
            catch
            {

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
                    //CurrentSessionToken.LastSendDateTime = DateTime.Now;

                    await Task.Delay(_SendDataIntervalMilliseconds);

                }

            }
            catch (Exception exception)
            {
                OnError(exception);
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


        public void Send(byte[] sendDataBytes)
        {

            try
            {
                SendAsync(sendDataBytes).Wait();
            }
            catch
            {

            }

        }

        public void Send(TSendPackage sendPackage)
        {
            Send(_CurrentFixedHeaderPackageFilter.EncodePackage(sendPackage));
        }



        protected virtual void OnClose()
        {

            if (!_IsRunning)
            {
                return;
            }

            lock (_CloseLocker)
            {

                if (IsRunning)
                {

                    _IsFirstStart = false;
                    _IsRunning = false;

                    try
                    {

                        if (!_CurrentSendWorkTaskQueue.IfIsNull())
                        {
                            _CurrentSendWorkTaskQueue.StopAsync().Wait();
                            _CurrentSendWorkTaskQueue.Dispose();
                            _CurrentSendWorkTaskQueue = null;
                        }

                    }
                    catch
                    {

                    }


                    try
                    {
                        if (!_CurrentNetworkStream.IfIsNull())
                        {
                            _CurrentNetworkStream.Dispose();
                            _CurrentNetworkStream = null;
                        }
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

                        OnCloseEvent();

                    }
                    catch
                    {

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
