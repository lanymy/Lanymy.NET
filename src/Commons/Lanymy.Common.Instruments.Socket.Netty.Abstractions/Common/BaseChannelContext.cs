using System;
using System.Buffers;
using System.Collections.Concurrent;

namespace Lanymy.Common.Instruments.Common
{
    /// <summary>
    /// 聚合 Netty 通道处理链共用的过滤器、配置和缓冲池。
    /// </summary>
    public abstract class BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelOptions>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelOptions : BaseChannelOptions
        where TChannelSession : BaseChannelSession
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {
        /// <summary>
        /// 当前协议使用的固定包头过滤器。
        /// </summary>
        public readonly TChannelFixedHeaderPackageFilter CurrentFixedHeaderPackageFilter = new();

        /// <summary>
        /// 当前通道配置。
        /// </summary>
        public readonly TChannelOptions CurrentChannelOptions;

        /// <summary>
        /// 字节数组对象池
        /// </summary>
        public readonly ArrayPool<byte> CurrentDataBytesArrayPool = ArrayPool<byte>.Create();

        /// <summary>
        /// 初始化通道上下文。
        /// </summary>
        /// <param name="channelOptions">通道配置。</param>
        protected BaseChannelContext(TChannelOptions channelOptions)
        {
            CurrentChannelOptions = channelOptions;
        }
    }
}
