using System;

namespace Lanymy.Common.Instruments
{

    public class LanymyTcpServer : BaseTcpServer<LanymyTcpServerClient, LanymySessionToken, LanymyFixedHeaderPackageFilter, LanymyUpPackageDataModel, LanymyDownPackageDataModel>
    {

        public LanymyTcpServer(LanymyFixedHeaderPackageFilter fixedHeaderPackageFilter, int port, int receiveBufferSize = 512, int sendBufferSize = 512, int sendDataIntervalMilliseconds = 300, int intervalHeartTotalMilliseconds = 3 * 1000) : base(fixedHeaderPackageFilter, port, receiveBufferSize, sendBufferSize, sendDataIntervalMilliseconds, intervalHeartTotalMilliseconds)
        {

        }

        protected override LanymyTcpServerClient CreateTcpServerClient(System.Net.Sockets.Socket client)
        {
            return new LanymyTcpServerClient(client, ReceiveBufferSize, SendBufferSize, _SendDataIntervalMilliseconds);
        }

        protected override void OnServerClientCloseCallBackEvent(ITcpServerClient tcpServerClient)
        {
            throw new NotImplementedException();
        }

        protected override void OnServerClientStartReceiveCallBackEvent(ITcpServerClient tcpServerClient)
        {
            throw new NotImplementedException();
        }

        protected override void OnServerClientReceiveDataCallBackEvent(ITcpServerClient tcpServerClient, BufferModel buffer, CacheModel cache)
        {
            throw new NotImplementedException();
        }

        protected override void OnServerReceivePackageEvent(LanymyUpPackageDataModel package, ISessionToken sessionToken)
        {

            if (package.FrameType == FrameTypeEnum.Answer)
            {
                if (package.CommandType == CommandTypeEnum.Heart)
                {

                    var session = sessionToken as LanymySessionToken;

                    if (!session.IsLogin)
                    {

                        //登陆标记TCP主机号
                        session.SessionTokenID = package.CMO;
                        session.IsLogin = true;

                        ////上报登陆设备信息
                        //_CurrentEventAggregator.GetEvent<EquipmentCommandBusEvent>().Publish(new EquipmentCommandEtoModel
                        //{
                        //    EquipmentCommandType = EquipmentCommandTypeEnum.IsLogin,
                        //    CurrentTZPackageDataModel = package,
                        //});

                    }

                    ////上报电量
                    //_CurrentEventAggregator.GetEvent<EquipmentCommandBusEvent>().Publish(new EquipmentCommandEtoModel
                    //{
                    //    EquipmentCommandType = EquipmentCommandTypeEnum.Heart,
                    //    CurrentTZPackageDataModel = package,
                    //});

                }
                else if (package.CommandType == CommandTypeEnum.Control)
                {

                    //ShowLogMessage(package.CMO, "指令应答", package.SourceHex, false, true);

                }
            }
        }


        protected override LanymySessionToken CreateSessionToken(string ip, int port)
        {
            return new LanymySessionToken(ip, port);
        }

        protected override bool CanSendData(ISessionToken sessionToken)
        {
            return (sessionToken as LanymySessionToken).IsLogin;
        }

        protected override void OnServerCloseEvent()
        {
            throw new NotImplementedException();
        }

        protected override void OnAcceptEvent(ITcpServerClient client)
        {
            throw new NotImplementedException();
        }

        protected override void OnAccept(ITcpServerClient client)
        {

            base.OnAccept(client);

            client.Send(_CurrentFixedHeaderPackageFilter.GetHeartBytes(client.CurrentSessionToken));

        }

        protected override void OnServerClientErrorCallBackEvent(ITcpServerClient client, Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override void OnServerClientErrorEvent(ITcpServerClient client, Exception ex)
        {

            base.OnServerClientErrorEvent(client, ex);

            var session = client.CurrentSessionToken as LanymySessionToken;
            if (session.IsLogin)
            {
                session.IsLogin = false;
                //_CurrentEventAggregator.GetEvent<EquipmentCommandBusEvent>().Publish(new EquipmentCommandEtoModel
                //{
                //    EquipmentCommandType = EquipmentCommandTypeEnum.IsLogout,
                //    CurrentTZPackageDataModel = new TzUpPackageDataModel(session.SessionTokenID),
                //});
            }
        }

        protected override void OnServerErrorEvent(Exception ex)
        {
            throw new NotImplementedException();
        }
    }

}
