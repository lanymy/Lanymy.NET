using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DotNetty.Transport.Channels;
using Lanymy.Common.Instruments.Common;


namespace Lanymy.Common.Instruments.Server
{

    public abstract class BaseServerChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext> : BaseChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext>
        where TServerChannelContext : BaseServerChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions>
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
            // Only clear the session slot when this handler still owns it.
            ((ICollection<KeyValuePair<Guid, IChannelClientHandler<TChannelSession>>>)_CurrentChannelDictionary)
                .Remove(new KeyValuePair<Guid, IChannelClientHandler<TChannelSession>>(_CurrentChannelSession.SessionID, this));
        }


    }

}
