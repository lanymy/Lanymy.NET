using System;
using System.Threading.Tasks;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments.Server
{



    public abstract class BaseNettySocketServer<TReceivePackage, TSendPackage, TServerChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelContext, TServerChannelHandler, TServerChannelInitializer> : BaseSocketHost<TReceivePackage, TSendPackage, TServerChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelContext, TServerChannelHandler, TServerChannelInitializer>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
        where TServerChannelOptions : ServerChannelOptions
        where TServerChannelInitializer : BaseServerChannelInitializer<TReceivePackage, TSendPackage, TServerChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelContext, TServerChannelHandler>
        where TServerChannelContext : BaseServerChannelContext<TReceivePackage, TSendPackage, TServerChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TServerChannelHandler : BaseServerChannelHandler<TReceivePackage, TSendPackage, TServerChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelContext>
    {

        protected IEventLoopGroup _CurrentWorkerGroup;

        protected BaseNettySocketServer(TServerChannelContext serverChannelContext) : base(serverChannelContext)
        {
        }

        protected override async Task OnStartAsync()
        {

            try
            {

                _CurrentBossGroup = new MultithreadEventLoopGroup(1);
                _CurrentWorkerGroup = new MultithreadEventLoopGroup();

                var bootstrap = new ServerBootstrap();
                bootstrap.Group(_CurrentBossGroup, _CurrentWorkerGroup);

                bootstrap.Channel<TcpServerSocketChannel>();
                bootstrap

                    .Option(ChannelOption.SoBacklog, _CurrentChannelOptions.Backlog)

                    .ChildOption(ChannelOption.SoKeepalive, false)
                    .ChildOption(ChannelOption.TcpNodelay, true)
                    .ChildOption(ChannelOption.ConnectTimeout, TimeSpan.FromMilliseconds(30 * 1000))
                    .ChildOption(ChannelOption.SoSndbuf, _CurrentChannelOptions.SendBufferSize)
                    .ChildOption(ChannelOption.SoRcvbuf, _CurrentChannelOptions.ReceiveBufferSize)


#if DEBUG

                    .Handler(new LoggingHandler("SRV-LSTN"))

#endif

                    .ChildHandler(Activator.CreateInstance(typeof(TServerChannelInitializer), _CurrentChannelContext) as IChannelHandler);

                _CurrentChannelHost = await bootstrap.BindAsync(_CurrentChannelOptions.Port);


                //await _CurrentServerChannel.CloseAsync();

            }
            catch
            {
                _CurrentChannelHost = null;
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
                await Task.WhenAll
                (
                    _CurrentBossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    _CurrentWorkerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1))
                );
            }

            try
            {
                _CurrentChannelHost = null;
                _CurrentBossGroup = null;
                _CurrentWorkerGroup = null;
            }
            catch
            {
                // ignored
            }

        }




    }

}
