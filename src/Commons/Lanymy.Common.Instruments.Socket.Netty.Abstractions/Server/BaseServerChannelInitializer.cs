using System;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Server
{


    public abstract class BaseServerChannelInitializer<TServerChannelContext, TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelHandler> : BaseChannelInitializer<TServerChannelContext, TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelHandler>
        where TServerChannelContext : BaseServerChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TServerChannelHandler : BaseServerChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelContext>
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        protected BaseServerChannelInitializer(TServerChannelContext serverChannelContext) : base(serverChannelContext)
        {


        }


        protected override TServerChannelHandler GetChannelHandler()
        {
            return Activator.CreateInstance(_CurrentChannelClientHandlerType, _CurrentServerChannelContext) as TServerChannelHandler;
        }

    }

}
