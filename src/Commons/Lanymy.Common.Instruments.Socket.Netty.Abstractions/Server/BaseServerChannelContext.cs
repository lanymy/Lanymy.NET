using System;
using System.Collections.Concurrent;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Server
{


    public abstract class BaseServerChannelContext<TReceivePackage, TSendPackage, TServerChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter> : BaseChannelContext<TReceivePackage, TSendPackage, TServerChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession
        where TServerChannelOptions : ServerChannelOptions
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        public readonly ConcurrentDictionary<Guid, IChannelClientHandler<TChannelSession>> CurrentChannelDictionary = new();


        protected BaseServerChannelContext(TServerChannelOptions channelOptions) : base(channelOptions)
        {

        }


    }

}
