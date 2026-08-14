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
    /// <summary>
    /// 提供 Netty TCP 服务端的启动、绑定、停止和资源收口逻辑。
    /// </summary>
    public abstract class BaseNettySocketServer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext, TServerChannelHandler, TServerChannelInitializer> : BaseSocketHost<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext, TServerChannelHandler, TServerChannelInitializer>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
        where TServerChannelOptions : ServerChannelOptions
        where TServerChannelInitializer : BaseServerChannelInitializer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext, TServerChannelHandler>
        where TServerChannelContext : BaseServerChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions>
        where TServerChannelHandler : BaseServerChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TServerChannelOptions, TServerChannelContext>
    {
        /// <summary>
        /// Worker 事件循环组。
        /// </summary>
        protected IEventLoopGroup _CurrentWorkerGroup;

        /// <summary>
        /// 当前服务端 Bootstrap。
        /// </summary>
        protected ServerBootstrap _CurrentBootstrap;

        /// <summary>
        /// 初始化 Netty 服务端。
        /// </summary>
        protected BaseNettySocketServer(TServerChannelContext serverChannelContext) : base(serverChannelContext)
        {
        }

        protected override async Task OnStartAsync()
        {
            var currentBossGroup = default(IEventLoopGroup);
            var currentWorkerGroup = default(IEventLoopGroup);
            var currentBootstrap = default(ServerBootstrap);
            var currentChannelHost = default(IChannel);

            try
            {
                currentBossGroup = CreateBossGroup();
                _CurrentBossGroup = currentBossGroup;

                currentWorkerGroup = CreateWorkerGroup();
                _CurrentWorkerGroup = currentWorkerGroup;

                currentBootstrap = CreateBootstrap(currentBossGroup, currentWorkerGroup);
                _CurrentBootstrap = currentBootstrap;

                currentChannelHost = await BindServerAsync(currentBootstrap);
                if (currentChannelHost == null)
                {
                    throw CreateNullBindException();
                }

                if (!IsBoundChannelReady(currentChannelHost))
                {
                    // bind 成功但 channel 未活跃时，视为失败并按启动回滚流程收口。
                    await CleanupRejectedBoundChannelAsync(currentChannelHost);
                    currentChannelHost = null;
                    throw CreateInactiveBindException();
                }

                _CurrentChannelHost = currentChannelHost;
            }
            catch (Exception ex)
            {
                await RollbackStartStateAsync(currentBossGroup, currentWorkerGroup, currentBootstrap, currentChannelHost);
                throw new InvalidOperationException("NettySocketServer start failed.", ex);
            }
        }

        protected virtual IEventLoopGroup CreateBossGroup()
        {
            return new MultithreadEventLoopGroup(1);
        }

        protected virtual IEventLoopGroup CreateWorkerGroup()
        {
            return new MultithreadEventLoopGroup();
        }

        protected virtual ServerBootstrap CreateBootstrap(IEventLoopGroup currentBossGroup, IEventLoopGroup currentWorkerGroup)
        {
            var currentChannelInitializer = EnsureChannelInitializerCreated(CreateChannelInitializer());

            var bootstrap = new ServerBootstrap();
            bootstrap.Group(currentBossGroup, currentWorkerGroup);

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

                .ChildHandler(currentChannelInitializer);

            return bootstrap;
        }

        protected virtual IChannelHandler CreateChannelInitializer()
        {
            return Activator.CreateInstance(typeof(TServerChannelInitializer), _CurrentChannelContext) as IChannelHandler;
        }

        protected virtual IChannelHandler EnsureChannelInitializerCreated(IChannelHandler channelInitializer)
        {
            if (channelInitializer == null)
            {
                throw new InvalidOperationException("NettySocketServer create channel initializer returned null.");
            }

            return channelInitializer;
        }

        protected virtual Task<IChannel> BindServerAsync(ServerBootstrap bootstrap)
        {
            return bootstrap.BindAsync(_CurrentChannelOptions.Port);
        }

        protected virtual Exception CreateNullBindException()
        {
            return new InvalidOperationException("NettySocketServer bind returned null channel.");
        }

        protected virtual Exception CreateInactiveBindException()
        {
            return new InvalidOperationException("NettySocketServer bind returned inactive channel.");
        }

        protected virtual bool IsBoundChannelReady(IChannel currentChannelHost)
        {
            return !currentChannelHost.IfIsNull() && currentChannelHost.Active;
        }

        protected virtual async Task CleanupRejectedBoundChannelAsync(IChannel currentChannelHost)
        {
            try
            {
                await CloseChannelAsync(currentChannelHost);
            }
            catch (Exception ex) when (CanIgnoreRejectedBoundChannelCloseException(ex, currentChannelHost))
            {
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketServer close rejected bound channel failed.", ex));
            }
        }

        protected virtual bool CanIgnoreRejectedBoundChannelCloseException(Exception exception, IChannel currentChannelHost)
        {
            return currentChannelHost.IfIsNull()
                   || CanIgnoreDirectChannelCloseException(exception, currentChannelHost);
        }

        protected virtual bool CanIgnoreDirectChannelCloseException(Exception exception, IChannel currentChannelHost)
        {
            if (exception is OperationCanceledException || exception is ObjectDisposedException)
            {
                return true;
            }

            return exception is InvalidOperationException invalidOperationException
                   && currentChannelHost != null
                   && (!currentChannelHost.Open || IsUnregisteredChannelCloseNoise(invalidOperationException, currentChannelHost));
        }

        protected virtual bool IsUnregisteredChannelCloseNoise(InvalidOperationException exception, IChannel currentChannelHost)
        {
            return currentChannelHost != null
                   && !currentChannelHost.Registered
                   && exception.Message?.IndexOf("not registered to an event loop", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected virtual Task CloseChannelAsync(IChannel currentChannelHost)
        {
            if (currentChannelHost.IfIsNull())
            {
                return Task.CompletedTask;
            }

            return currentChannelHost.CloseAsync();
        }

        protected virtual Task ShutdownGroupAsync(IEventLoopGroup currentEventLoopGroup)
        {
            if (currentEventLoopGroup.IfIsNull())
            {
                return Task.CompletedTask;
            }

            return currentEventLoopGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }

        protected virtual async Task RollbackStartStateAsync(IEventLoopGroup currentBossGroup, IEventLoopGroup currentWorkerGroup, ServerBootstrap currentBootstrap, IChannel currentChannelHost)
        {
            try
            {
                await CloseChannelAsync(currentChannelHost);
            }
            catch (Exception ex) when (CanIgnoreDirectChannelCloseException(ex, currentChannelHost))
            {
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketServer reset start close channel failed.", ex));
            }

            try
            {
                await ShutdownGroupAsync(currentBossGroup);
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketServer reset start shutdown boss group failed.", ex));
            }

            try
            {
                await ShutdownGroupAsync(currentWorkerGroup);
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketServer reset start shutdown worker group failed.", ex));
            }

            try
            {
                ResetStartState(currentBossGroup, currentWorkerGroup, currentBootstrap, currentChannelHost);
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketServer reset start finalization failed.", ex));
            }
        }

        protected virtual void ResetStartState(IEventLoopGroup currentBossGroup, IEventLoopGroup currentWorkerGroup, ServerBootstrap currentBootstrap, IChannel currentChannelHost)
        {
            lock (_Locker)
            {
                IsRunning = false;

                if (ReferenceEquals(_CurrentBossGroup, currentBossGroup))
                {
                    _CurrentBossGroup = null;
                }

                if (ReferenceEquals(_CurrentWorkerGroup, currentWorkerGroup))
                {
                    _CurrentWorkerGroup = null;
                }

                if (ReferenceEquals(_CurrentBootstrap, currentBootstrap))
                {
                    _CurrentBootstrap = null;
                }

                if (ReferenceEquals(_CurrentChannelHost, currentChannelHost))
                {
                    _CurrentChannelHost = null;
                }
            }
        }

        protected virtual void OnStartError(Exception ex)
        {
        }

        protected virtual void ResetStopState(IChannel currentChannelHost, IEventLoopGroup currentBossGroup, IEventLoopGroup currentWorkerGroup, ServerBootstrap currentBootstrap)
        {
            lock (_Locker)
            {
                if (ReferenceEquals(_CurrentChannelHost, currentChannelHost))
                {
                    _CurrentChannelHost = null;
                }

                if (ReferenceEquals(_CurrentBossGroup, currentBossGroup))
                {
                    _CurrentBossGroup = null;
                }

                if (ReferenceEquals(_CurrentWorkerGroup, currentWorkerGroup))
                {
                    _CurrentWorkerGroup = null;
                }

                if (ReferenceEquals(_CurrentBootstrap, currentBootstrap))
                {
                    _CurrentBootstrap = null;
                }
            }
        }

        protected virtual void OnStopError(Exception ex)
        {
        }

        protected virtual void CleanupTrackedChannelHandlers()
        {
            // 服务端停止后把在线 handler 视图整体清空，避免残留旧会话引用。
            _CurrentChannelContext.CurrentChannelDictionary.Clear();
        }


        protected override async Task OnStopAsync()
        {
            var currentChannelHost = _CurrentChannelHost;
            var currentBossGroup = _CurrentBossGroup;
            var currentWorkerGroup = _CurrentWorkerGroup;
            var currentBootstrap = _CurrentBootstrap;

            try
            {
                await CloseChannelAsync(currentChannelHost);
            }
            catch (Exception ex) when (CanIgnoreDirectChannelCloseException(ex, currentChannelHost))
            {
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketServer stop close channel failed.", ex));
            }

            try
            {
                await ShutdownGroupAsync(currentBossGroup);
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketServer stop shutdown boss group failed.", ex));
            }

            try
            {
                await ShutdownGroupAsync(currentWorkerGroup);
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketServer stop shutdown worker group failed.", ex));
            }

            try
            {
                ResetStopState(currentChannelHost, currentBossGroup, currentWorkerGroup, currentBootstrap);
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketServer stop finalization failed.", ex));
            }

            try
            {
                CleanupTrackedChannelHandlers();
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketServer stop clear tracked handlers failed.", ex));
            }
        }




    }

}
