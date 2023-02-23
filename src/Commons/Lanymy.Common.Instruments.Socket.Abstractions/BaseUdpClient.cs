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
        public bool IsAccept => _IsAccept;
        public bool IsDisposed => _IsDisposed;

        #region 内部变量

        protected UdpClient _CurrentUdpClient;

        protected readonly TFixedHeaderPackageFilter _CurrentFixedHeaderPackageFilter;

        protected readonly int _SendDataIntervalMilliseconds;

        protected volatile bool _IsClose = false;
        protected volatile bool _IsSend = false;
        protected volatile bool _IsAccept = false;
        protected volatile bool _IsDisposed = false;

        protected readonly object _Locker = new Object();

        protected readonly WorkTaskQueue<UdpSourceDataModel> _ReceiveWorkTaskQueue;

        protected Queue<SendUdpDataModel> _SendQueue = new Queue<SendUdpDataModel>(50);


        #endregion


        protected BaseUdpClient(TFixedHeaderPackageFilter fixedHeaderPackageFilter, int port, int sendDataIntervalMilliseconds = 500)
        {
            Port = port;
            _SendDataIntervalMilliseconds = sendDataIntervalMilliseconds;
            _CurrentFixedHeaderPackageFilter = fixedHeaderPackageFilter;
            _ReceiveWorkTaskQueue = new WorkTaskQueue<UdpSourceDataModel>(OnReceiveWorkTaskQueue, null);
        }




        #region 通知事件


        private void OnReceiveWorkTaskQueue(UdpSourceDataModel udpSourceDataModel)
        {
            OnReceiveDataEvent(udpSourceDataModel.RemoteIPEndPoint, udpSourceDataModel.SourceDataBytes);
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

        #endregion


        public void Start()
        {

            if (_IsAccept)
            {
                return;
            }

            _IsAccept = true;

            _ReceiveWorkTaskQueue.StartAsync().Wait();

            _CurrentUdpClient = new UdpClient(Port);
            _CurrentUdpClient.EnableBroadcast = true;
            _CurrentUdpClient.BeginReceive(ReciveCallBack, null);

            OnStart();

        }


        public abstract void OnStart();

        private void ReciveCallBack(IAsyncResult asyncResult)
        {



            IPEndPoint remoteIPEndPoint = null;
            byte[] bytes = _CurrentUdpClient.EndReceive(asyncResult, ref remoteIPEndPoint);//*结束挂起的异步接收

            _ReceiveWorkTaskQueue.AddToQueueAsync(new UdpSourceDataModel
            {
                RemoteIPEndPoint = remoteIPEndPoint,
                SourceDataBytes = bytes,
            }).Wait();

            _CurrentUdpClient.BeginReceive(ReciveCallBack, null);//*异步接收数据

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
            var flag = false;

            if (!sendUdpDataModel.PackageBytes.IfIsNullOrEmpty() && _IsAccept)
            {
                lock (_Locker)
                {

                    if (_SendQueue.Count >= 100)
                    {
                        flag = false;
                    }
                    else
                    {
                        _SendQueue.Enqueue(sendUdpDataModel);
                        BeginSend();
                        flag = true;
                    }
                }
            }

            return flag;
        }


        protected void BeginSend()
        {

            if (!_IsSend && _IsAccept && _SendQueue.Count > 0)
            {

                _IsSend = true;

                Task.Delay(_SendDataIntervalMilliseconds).Wait();


                var sendDataModel = _SendQueue.Dequeue();

                var ipEndPoint = sendDataModel.RemoteIpEndPoint;

                try
                {

                    _CurrentUdpClient.BeginSend(sendDataModel.PackageBytes, sendDataModel.PackageBytes.Length, ipEndPoint, OnSend, ipEndPoint);

                }
                catch (Exception exception)
                {
                    OnErrorEvent(ipEndPoint, exception);
                }

            }
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                _CurrentUdpClient.EndSend(ar);
                _IsSend = false;
                BeginSend();
            }
            catch (Exception exception)
            {
                OnErrorEvent(ar.AsyncState as IPEndPoint, exception);
            }
        }


        public void Close()
        {

            _IsClose = true;

            if (_IsAccept)
            {
                _IsAccept = false;


                try
                {
                    _CurrentUdpClient.Close();
                    _ReceiveWorkTaskQueue.StopAsync().Wait();
                }
                catch
                {

                }

                OnClose();

            }

        }

        public abstract void OnClose();

        public void Dispose()
        {
            Close();
        }

    }

}
