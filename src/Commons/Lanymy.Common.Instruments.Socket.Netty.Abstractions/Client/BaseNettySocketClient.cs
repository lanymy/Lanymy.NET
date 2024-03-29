﻿using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Client
{



    public abstract class BaseNettySocketClient<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions, TClientChannelContext, TClientChannelHandler, TClientChannelInitializer> : BaseSocketHost<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions, TClientChannelContext, TClientChannelHandler, TClientChannelInitializer>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
        where TClientChannelOptions : ClientChannelOptions
        where TClientChannelContext : BaseClientChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions>
        where TClientChannelHandler : BaseClientChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions, TClientChannelContext>
        where TClientChannelInitializer : BaseClientChannelInitializer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TClientChannelOptions, TClientChannelContext, TClientChannelHandler>
    {


        protected readonly IPEndPoint _CurrentTcpServerIPEndPoint;
        protected Bootstrap _CurrentBootstrap;

        protected BaseNettySocketClient(TClientChannelContext serverChannelContext) : base(serverChannelContext)
        {

            var tcpServerIP = _CurrentChannelOptions.ServerIP;
            var tcpServerPort = _CurrentChannelOptions.Port;
            _CurrentTcpServerIPEndPoint = new IPEndPoint(IPAddress.Parse(tcpServerIP), tcpServerPort);

            _CurrentChannelContext.CurrentConnectToServerAction = new WeakReference<Action>(async () =>
            {
                await ConnectToServerAsync();
            });

        }


        protected override async Task OnStartAsync()
        {

            try
            {

                _CurrentBossGroup = new MultithreadEventLoopGroup(1);

                _CurrentBootstrap = new Bootstrap()
                    .Group(_CurrentBossGroup)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.SoKeepalive, false)
                    .Option(ChannelOption.TcpNodelay, true)
                    .Option(ChannelOption.ConnectTimeout, TimeSpan.FromMilliseconds(30 * 1000))
                    .Option(ChannelOption.SoSndbuf, _CurrentChannelOptions.SendBufferSize)
                    .Option(ChannelOption.SoRcvbuf, _CurrentChannelOptions.ReceiveBufferSize)


#if DEBUG

                    .Handler(new LoggingHandler("Client-LSTN"))

#endif

                    .Handler(Activator.CreateInstance(typeof(TClientChannelInitializer), _CurrentChannelContext) as IChannelHandler);



                Task.Run(ConnectToServerAsync);

                await Task.CompletedTask;

            }
            catch
            {
                _CurrentChannelHost = null;
            }


        }


        //private void OnConnectToServer()
        //{
        //    ConnectToServerAsync().Wait();
        //}


        protected virtual async Task ConnectToServerAsync()
        {
            try
            {
                if (!_CurrentBootstrap.IfIsNull())
                {
                    _CurrentChannelHost = await _CurrentBootstrap.ConnectAsync(_CurrentTcpServerIPEndPoint);
                }

            }
            catch (Exception e)
            {
                //连不上服务器 继续 重连
                await Task.Delay(_CurrentChannelOptions.IntervalHeartTotalMilliseconds);
                await ConnectToServerAsync();
            }

        }


        protected override async Task OnStopAsync()
        {

            try
            {
                if (!_CurrentChannelHost.IfIsNull())
                {
                    await _CurrentChannelHost.CloseAsync();
                }
            }
            finally
            {
                await _CurrentBossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }

            try
            {
                _CurrentBootstrap = null;
                _CurrentChannelHost = null;
                _CurrentBossGroup = null;
            }
            catch
            {
                // ignored
            }

        }




    }

}
