
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Server
{
    /// <summary>
    /// 服务端专用的通道初始化器基类。
    /// </summary>
    public abstract class BaseServerChannelInitializer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext, TServerChannelHandler> : BaseChannelInitializer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext, TServerChannelHandler>
        where TServerChannelContext : BaseServerChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TServerChannelOptions : ServerChannelOptions
        where TServerChannelHandler : BaseServerChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext>
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {
        /// <summary>
        /// 初始化服务端通道初始化器。
        /// </summary>
        protected BaseServerChannelInitializer(TServerChannelContext serverChannelContext) : base(serverChannelContext)
        {
        }
    }
}
