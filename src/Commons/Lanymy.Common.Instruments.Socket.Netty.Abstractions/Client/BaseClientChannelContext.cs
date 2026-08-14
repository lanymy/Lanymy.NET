using System;
using System.Buffers;
using System.Collections.Concurrent;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Client
{
    /// <summary>
    /// 承载客户端通道特有状态，例如重连 generation 和重连触发回调。
    /// </summary>
    public abstract class BaseClientChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions> : BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions>
        where TReceivePackage : class
        where TSendPackage : class
        where TClientChannelOptions : ClientChannelOptions
        where TChannelSession : BaseChannelSession
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {
        /// <summary>
        /// 请求外部宿主启动重连循环的回调。
        /// </summary>
        public Action<long> CurrentConnectToServerAction { get; set; }

        /// <summary>
        /// 当前重连世代号，用于丢弃陈旧重连请求。
        /// </summary>
        public long CurrentReconnectGeneration { get; set; }

        /// <summary>
        /// 初始化客户端通道上下文。
        /// </summary>
        protected BaseClientChannelContext(TClientChannelOptions channelOptions) : base(channelOptions)
        {
        }
    }
}
