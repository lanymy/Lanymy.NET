using System;

namespace Lanymy.Common.Instruments
{

    public class LanymyUdpClient : BaseUdpClient<LanymySessionToken, LanymyFixedHeaderPackageFilter, LanymyUpPackageDataModel, LanymyDownPackageDataModel>
    {

        public LanymyUdpClient(LanymyFixedHeaderPackageFilter fixedHeaderPackageFilter, int port, int sendDataIntervalMilliseconds = 500) : base(fixedHeaderPackageFilter, port, sendDataIntervalMilliseconds)
        {

        }

        protected override void OnReceivePackage(LanymyUpPackageDataModel package)
        {

        }

        protected override void OnStartEvent()
        {
            throw new NotImplementedException();
        }

        protected override void OnCloseEvent()
        {
            throw new NotImplementedException();
        }


    }

}
