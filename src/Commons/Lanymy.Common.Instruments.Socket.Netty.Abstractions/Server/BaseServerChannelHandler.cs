using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DotNetty.Transport.Channels;
using Lanymy.Common.Instruments.Common;


namespace Lanymy.Common.Instruments.Server
{
    /// <summary>
    /// 服务端通道处理器基类，负责把在线 handler 注册到共享字典并在断链时安全移除。
    /// </summary>
    public abstract class BaseServerChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext> : BaseChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext>
        where TServerChannelContext : BaseServerChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TServerChannelOptions : ServerChannelOptions
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {
        /// <summary>
        /// 当前服务端上下文共享的在线连接字典。
        /// </summary>
        protected readonly ConcurrentDictionary<Guid, IChannelClientHandler<TChannelSession>> _CurrentChannelDictionary;

        /// <summary>
        /// 初始化服务端 handler。
        /// </summary>
        protected BaseServerChannelHandler(TServerChannelContext channelContext) : base(channelContext)
        {
            _CurrentChannelDictionary = channelContext.CurrentChannelDictionary;
        }

        protected override void OnChannelActive(IChannelHandlerContext context)
        {
            _CurrentChannelDictionary.AddOrUpdate
            (

                _CurrentChannelSession.SessionID,
                this,
                (_, _) => this
            );
        }


        /// <summary>
        /// 当前频道未激活状态 / channel处于非活跃状态，已断开远程主机连接
        /// </summary>
        /// <param name="context"></param>
        protected override void OnChannelInactive(IChannelHandlerContext context)
        {
            // 只在当前 handler 仍持有该 session 槽位时才移除，避免并发重连/替换时误删新 handler。
            ((ICollection<KeyValuePair<Guid, IChannelClientHandler<TChannelSession>>>)_CurrentChannelDictionary)
                .Remove(new KeyValuePair<Guid, IChannelClientHandler<TChannelSession>>(_CurrentChannelSession.SessionID, this));
        }
    }
}
