using System;
using System.Buffers;
using System.Collections.Concurrent;

namespace Lanymy.Common.Instruments.Common
{


    public abstract class BaseChannelContext<TReceivePackage, TSendPackage, TChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelOptions : BaseChannelOptions
        where TChannelSession : BaseChannelSession
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        public readonly TChannelFixedHeaderPackageFilter CurrentFixedHeaderPackageFilter = new();

        public readonly TChannelOptions CurrentChannelOptions;

        /// <summary>
        /// 字节数组对象池
        /// </summary>
        public readonly ArrayPool<byte> CurrentDataBytesArrayPool = ArrayPool<byte>.Create();


        protected BaseChannelContext(TChannelOptions channelOptions)
        {
            CurrentChannelOptions = channelOptions;
        }

    }

}
