using System;
using System.Collections.Concurrent;
using DotNetty.Transport.Channels;
using Lanymy.Common.Instruments.Common;


namespace Lanymy.Common.Instruments.Server
{

    public abstract class BaseServerChannelHandler<TReceivePackage, TSendPackage, TServerChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelContext> : BaseChannelHandler<TReceivePackage, TSendPackage, TServerChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelContext>
        where TServerChannelContext : BaseServerChannelContext<TReceivePackage, TSendPackage, TServerChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TServerChannelOptions : ServerChannelOptions
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {



        protected readonly ConcurrentDictionary<Guid, IChannelClientHandler<TChannelSession>> _CurrentChannelDictionary;

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
            _CurrentChannelDictionary.TryRemove(_CurrentChannelSession.SessionID, out _);
        }


    }

}
