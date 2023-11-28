using System;
using System.Net;

namespace Lanymy.Common.Instruments.Common
{


    public abstract class BaseChannelSession
    {


        public Guid SessionID { get; } = Guid.NewGuid();

        public IPEndPoint RemoteIpEndPoint { get; set; }

        public bool IsLogin { get; set; }

        public abstract byte[] CacheHeartBytes { get; }

        //public abstract byte[] CacheConnectBytes { get; }
        //public abstract byte[] CacheConnectLogoutBytes { get; }


    }

}
