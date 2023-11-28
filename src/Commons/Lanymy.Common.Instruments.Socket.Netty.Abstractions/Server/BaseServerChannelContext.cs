using System;
using System.Buffers;
using System.Collections.Concurrent;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Server
{


    public abstract class BaseServerChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter> : BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        public readonly ConcurrentDictionary<Guid, IChannelClientHandler<TChannelSession>> CurrentChannelDictionary = new();


        protected BaseServerChannelContext(ChannelOptions channelOptions) : base(channelOptions)
        {

        }


    }

}
