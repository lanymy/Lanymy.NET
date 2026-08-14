using System;
using System.Collections.Concurrent;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Server
{
    /// <summary>
    /// 承载服务端通道特有状态，例如当前在线 handler 字典。
    /// </summary>
    public abstract class BaseServerChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions> : BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession
        where TServerChannelOptions : ServerChannelOptions
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {
        /// <summary>
        /// 当前在线会话处理器字典，键为会话 ID。
        /// </summary>
        public readonly ConcurrentDictionary<Guid, IChannelClientHandler<TChannelSession>> CurrentChannelDictionary = new();

        /// <summary>
        /// 初始化服务端通道上下文。
        /// </summary>
        protected BaseServerChannelContext(TServerChannelOptions channelOptions) : base(channelOptions)
        {
        }
    }
}
