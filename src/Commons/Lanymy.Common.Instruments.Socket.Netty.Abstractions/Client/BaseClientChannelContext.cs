using System;
using System.Buffers;
using System.Collections.Concurrent;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Client
{


    public abstract class BaseClientChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions> : BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions>
        where TReceivePackage : class
        where TSendPackage : class
        where TClientChannelOptions : ClientChannelOptions
        where TChannelSession : BaseChannelSession
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        public WeakReference<Action> CurrentConnectToServerAction;


        protected BaseClientChannelContext(TClientChannelOptions channelOptions) : base(channelOptions)
        {

        }


    }

}
