using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Client
{


    public abstract class BaseClientChannelInitializer<TReceivePackage, TSendPackage, TClientChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelContext, TClientChannelHandler> : BaseChannelInitializer<TReceivePackage, TSendPackage, TClientChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelContext, TClientChannelHandler>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TClientChannelOptions : ClientChannelOptions
        where TClientChannelContext : BaseClientChannelContext<TReceivePackage, TSendPackage, TClientChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TClientChannelHandler : BaseClientChannelHandler<TReceivePackage, TSendPackage, TClientChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelContext>
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {

        //protected readonly Action _OnConnectToServerAction;

        //protected BaseClientChannelInitializer(TClientChannelContext serverChannelContext, Action connectToServerAction) : base(serverChannelContext)
        protected BaseClientChannelInitializer(TClientChannelContext serverChannelContext) : base(serverChannelContext)
        {
            //_OnConnectToServerAction = connectToServerAction;
        }

        //protected override TClientChannelHandler GetChannelHandler()
        //{
        //    //return Activator.CreateInstance(_CurrentChannelClientHandlerType, _CurrentServerChannelContext, _OnConnectToServerAction) as TClientChannelHandler;
        //    return Activator.CreateInstance(_CurrentChannelClientHandlerType, _CurrentServerChannelContext) as TClientChannelHandler;
        //}

    }

}
