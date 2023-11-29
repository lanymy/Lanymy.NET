using System;
using System.Collections.Concurrent;
using System.Net;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Client
{

    public abstract class BaseClientChannelHandler<TReceivePackage, TSendPackage, TClientChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelContext> : BaseChannelHandler<TReceivePackage, TSendPackage, TClientChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelContext>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TClientChannelOptions : ClientChannelOptions
        where TClientChannelContext : BaseClientChannelContext<TReceivePackage, TSendPackage, TClientChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {

        protected readonly Action _OnConnectToServerAction;

        //protected BaseClientChannelHandler(TChannelContext channelContext, Action connectToServerAction) : base(channelContext)
        protected BaseClientChannelHandler(TClientChannelContext channelContext) : base(channelContext)
        {
            //_OnConnectToServerAction = connectToServerAction;
            channelContext.CurrentConnectToServerAction.TryGetTarget(out _OnConnectToServerAction);
        }



        /// <summary>
        /// 当前频道未激活状态 / channel处于非活跃状态，已断开远程主机连接
        /// </summary>
        /// <param name="context"></param>
        protected override void OnChannelInactive(IChannelHandlerContext context)
        {

            if (!_OnConnectToServerAction.IfIsNull())
            {
                _OnConnectToServerAction();
            }

        }


    }

}
