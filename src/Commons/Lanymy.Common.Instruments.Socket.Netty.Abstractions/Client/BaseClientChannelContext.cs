using System;
using System.Buffers;
using System.Collections.Concurrent;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Client
{


    public abstract class BaseClientChannelContext<TReceivePackage, TSendPackage, TChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter> : BaseChannelContext<TReceivePackage, TSendPackage, TChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelOptions : ClientChannelOptions
        where TChannelSession : BaseChannelSession
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        public WeakReference<Action> CurrentConnectToServerAction;


        protected BaseClientChannelContext(TChannelOptions channelOptions) : base(channelOptions)
        {

        }


    }

}
