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
            _SendWorkTaskQueue = new WorkTaskQueue<SendUdpDataModel>(OnSendWorkTaskQueue, null);

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
                OnErrorEvent(udpSourceDataModel.RemoteIPEndPoint, ex);
            }

        }


        protected virtual void OnReceiveDataEvent(IPEndPoint remoteIPEndPoint, byte[] packageBytes)
        {

            //System.Diagnostics.Debug.WriteLine(string.Format("[ {0:HH:mm:ss:fff} ] - {1}", DateTime.Now, remoteIPEndPoint));

            if (!_CurrentFixedHeaderPackageFilter.CheckPackage(packageBytes))
            {
                OnErrorEvent(remoteIPEndPoint, new Exception("data bytes error"));
                return;
            }

            var package = _CurrentFixedHeaderPackageFilter.DecodePackage(packageBytes);
            package.RemoteIpEndPoint = remoteIPEndPoint;

            OnReceivePackage(package);

        }


        protected abstract void OnReceivePackage(TPackage package);


        protected virtual void OnErrorEvent(IPEndPoint remoteIPEndPoint, Exception ex)
        {

        }

        protected virtual void OnSendWorkTaskQueue(SendUdpDataModel sendUdpDataModel)
        {

            try
            {
                OnSendWorkTaskQueueAsync(sendUdpDataModel).Wait();
            }
            catch (Exception ex)
            {
                OnErrorEvent(sendUdpDataModel.RemoteIpEndPoint, ex);
            }

        }

        protected async Task OnSendWorkTaskQueueAsync(SendUdpDataModel sendUdpDataModel)
        {

            try
            {

                if (_IsRunning && !sendUdpDataModel.PackageBytes.IfIsNullOrEmpty())
                {

                    await _CurrentUdpClient.SendAsync(sendUdpDataModel.PackageBytes, sendUdpDataModel.PackageBytes.Length, sendUdpDataModel.RemoteIpEndPoint);

                    await Task.Delay(_SendDataIntervalMilliseconds);

                }

            }
            catch (Exception ex)
            {
                OnErrorEvent(sendUdpDataModel.RemoteIpEndPoint, ex);
            }

        }


        #endregion


        public void Start()
        {

            if (_IsRunning)
            {
                return;
            }

            _IsRunning = true;

            _ReceiveWorkTaskQueue.StartAsync().Wait();

            _CurrentUdpClient = new UdpClient(Port);
            _CurrentUdpClient.EnableBroadcast = true;
            _CurrentUdpClient.BeginReceive(ReciveCallBack, null);

            _SendWorkTaskQueue.StartAsync().Wait();

            OnStart();

        }


        protected abstract void OnStartEvent();

        public virtual void OnStart()
        {

            try
            {
                OnStartEvent();
            }
            catch
            {

            }

        }

        private void ReciveCallBack(IAsyncResult asyncResult)
        {

            if (_IsRunning)
            {

                IPEndPoint remoteIPEndPoint = null;
                byte[] bytes = _CurrentUdpClient.EndReceive(asyncResult, ref remoteIPEndPoint);//*结束挂起的异步接收

                _ReceiveWorkTaskQueue.AddToQueueAsync(new UdpSourceDataModel
                {
                    RemoteIPEndPoint = remoteIPEndPoint,
                    SourceDataBytes = bytes,
                }).Wait();

                _CurrentUdpClient.BeginReceive(ReciveCallBack, null);

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
            var packageDataBytes = _CurrentFixedHeaderPackageFilter.EncodePackage(sendPackage);
            return Send(packageDataBytes, sendPackage.RemoteIpEndPoint);
        }

        public bool Send(SendUdpDataModel sendUdpDataModel)
        {

            try
            {
                SendAsync(sendUdpDataModel).Wait();
            }
            catch
            {

            }

            return true;

        }

        protected async Task SendAsync(SendUdpDataModel sendUdpDataModel)
        {

            try
            {

                if (_IsRunning)
                {
                    await _SendWorkTaskQueue.AddToQueueAsync(sendUdpDataModel);
                }

            }
            catch (Exception ex)
            {

                OnErrorEvent(sendUdpDataModel.RemoteIpEndPoint, ex);

            }

        }





        public void Close()
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
                        if (_ReceiveWorkTaskQueue.IfIsNull())
                        {
                            _ReceiveWorkTaskQueue.StopAsync().Wait();
                            _ReceiveWorkTaskQueue.Dispose();
                            _ReceiveWorkTaskQueue = null; ;
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        if (_SendWorkTaskQueue.IfIsNull())
                        {
                            _SendWorkTaskQueue.StopAsync().Wait();
                            _SendWorkTaskQueue.Dispose();
                            _SendWorkTaskQueue = null; ;
                        }
                    }
                    catch
                    {

                    }


                    try
                    {

                        _CurrentUdpClient.Close();
                        _CurrentUdpClient.Dispose();

                    }
                    catch
                    {

                    }


                    try
                    {
                        OnCloseEvent();
                    }
                    catch
                    {

                    }


                }

            }
        }


        protected abstract void OnCloseEvent();

        public void Dispose()
        {
            Close();
        }

    }

}
