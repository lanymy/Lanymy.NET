using System;

namespace Lanymy.Common.Instruments.Common
{



    public abstract class BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession
    {


        public virtual byte[] GetHeartBytes(TChannelSession channelSession)
        {

            return channelSession.CacheHeartBytes;

        }


        //public virtual byte[] GetConnectBytes(TChannelSession channelSession)
        //{
        //    return channelSession.CacheConnectBytes;
        //}


        //public virtual byte[] GetConnectLogoutBytes(TChannelSession channelSession)
        //{
        //    return channelSession.CacheConnectLogoutBytes;
        //}


        //public abstract bool CheckPackage(int packageDataBytesLength, byte[] packageBytes);
        //public abstract bool CheckPackage(Span<byte> packageBytes);
        public abstract bool CheckPackage(ReadOnlySpan<byte> packageBytes);


        public abstract byte[] EncodePackage(TSendPackage sendPackage);


        public abstract TReceivePackage DecodePackage(int packageDataBytesLength, byte[] packageBytes);



    }

}
