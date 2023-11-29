
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Server
{


    public abstract class BaseServerChannelInitializer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext, TServerChannelHandler> : BaseChannelInitializer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext, TServerChannelHandler>
        where TServerChannelContext : BaseServerChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TServerChannelOptions : ServerChannelOptions
        where TServerChannelHandler : BaseServerChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext>
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        protected BaseServerChannelInitializer(TServerChannelContext serverChannelContext) : base(serverChannelContext)
        {


        }


        //protected override TServerChannelHandler GetChannelHandler()
        //{
        //    return Activator.CreateInstance(_CurrentChannelClientHandlerType, _CurrentServerChannelContext) as TServerChannelHandler;
        //}

    }

}
