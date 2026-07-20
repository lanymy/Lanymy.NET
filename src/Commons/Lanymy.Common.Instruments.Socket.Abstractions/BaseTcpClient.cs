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

            _CurrentSendWorkTaskQueue = new WorkTaskQueue<byte[]>(OnSendWorkTaskQueueAsync, null);


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


            Close();
        }

        protected virtual void OnCloseError(Exception ex)
        {
            ReportError(ex);
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

                TaskHelper.SyncWait(_CurrentSendWorkTaskQueue.StartAsync());

                OnConnection();

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
                TaskHelper.SyncWait(SendAsync(sendDataBytes));
            }
            catch
            {

            }

        }

        public void Send(TSendPackage sendPackage)
        {
            Send(_CurrentFixedHeaderPackageFilter.EncodePackage(sendPackage));
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

                if (!currentSendWorkTaskQueue.IfIsNull())
                {
                    await currentSendWorkTaskQueue.StopAsync();
                    currentSendWorkTaskQueue.Dispose();
                }

            }
            catch (Exception ex)
            {
                OnCloseError(new InvalidOperationException("TcpClient close send queue failed.", ex));
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

            TaskHelper.SyncWait(CloseAsync());

        }

        public virtual async Task CloseAsync()
        {
            await OnCloseAsync();
        }



        public void Dispose()
        {

            Close();

        }

    }
}
