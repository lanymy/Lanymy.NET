using System;
using System.Buffers;
using System.Collections.Concurrent;

namespace Lanymy.Common.Instruments.Common
{


    public abstract class BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        public readonly TChannelFixedHeaderPackageFilter CurrentFixedHeaderPackageFilter = new();

        public readonly ChannelOptions CurrentChannelOptions;

        /// <summary>
        /// 字节数组对象池
        /// </summary>
        public readonly ArrayPool<byte> CurrentDataBytesArrayPool = ArrayPool<byte>.Create();


        protected BaseChannelContext(ChannelOptions channelOptions)
        {
            CurrentChannelOptions = channelOptions;
        }

    }

}
