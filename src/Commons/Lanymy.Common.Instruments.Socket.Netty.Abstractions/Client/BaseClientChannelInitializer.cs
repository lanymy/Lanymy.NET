using Lanymy.Common.Instruments.Common;
using Lanymy.Common.Instruments.Server;
using System;

namespace Lanymy.Common.Instruments.Client
{


    public abstract class BaseClientChannelInitializer<TClientChannelContext, TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelHandler> : BaseChannelInitializer<TClientChannelContext, TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelHandler>
        where TClientChannelContext : BaseClientChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TClientChannelHandler : BaseClientChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelContext>
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {

        //protected readonly Action _OnConnectToServerAction;

        //protected BaseClientChannelInitializer(TClientChannelContext serverChannelContext, Action connectToServerAction) : base(serverChannelContext)
        protected BaseClientChannelInitializer(TClientChannelContext serverChannelContext) : base(serverChannelContext)
        {
            //_OnConnectToServerAction = connectToServerAction;
        }

        protected override TClientChannelHandler GetChannelHandler()
        {
            //return Activator.CreateInstance(_CurrentChannelClientHandlerType, _CurrentServerChannelContext, _OnConnectToServerAction) as TClientChannelHandler;
            return Activator.CreateInstance(_CurrentChannelClientHandlerType, _CurrentServerChannelContext) as TClientChannelHandler;
        }

    }

}
