using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Client
{
    /// <summary>
    /// 客户端专用的通道初始化器基类。
    /// </summary>
    public abstract class BaseClientChannelInitializer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions, TClientChannelContext, TClientChannelHandler> : BaseChannelInitializer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions, TClientChannelContext, TClientChannelHandler>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TClientChannelOptions : ClientChannelOptions
        where TClientChannelContext : BaseClientChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions>
        where TClientChannelHandler : BaseClientChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions, TClientChannelContext>
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {

        /// <summary>
        /// 初始化客户端通道初始化器。
        /// </summary>
        protected BaseClientChannelInitializer(TClientChannelContext serverChannelContext) : base(serverChannelContext)
        {
        }
    }
}
