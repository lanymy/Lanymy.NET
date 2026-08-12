using System;
using System.Net;
using System.Threading;
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
        protected Task _CurrentReconnectTask;
        protected CancellationTokenSource _CurrentReconnectCancellationTokenSource;
        protected long _CurrentReconnectGeneration;

        protected BaseNettySocketClient(TClientChannelContext serverChannelContext) : base(serverChannelContext)
        {

            var tcpServerIP = _CurrentChannelOptions.ServerIP;
            var tcpServerPort = _CurrentChannelOptions.Port;
            _CurrentTcpServerIPEndPoint = new IPEndPoint(IPAddress.Parse(tcpServerIP), tcpServerPort);

            _CurrentChannelContext.CurrentConnectToServerAction = EnsureReconnectLoopStarted;

        }


        protected override async Task OnStartAsync()
        {
            var currentBossGroup = default(IEventLoopGroup);
            var currentBootstrap = default(Bootstrap);
            var currentReconnectCancellationTokenSource = default(CancellationTokenSource);

            try
            {
                var reconnectGeneration = BeginReconnectGeneration();

                currentBossGroup = CreateBossGroup();
                _CurrentBossGroup = currentBossGroup;

                currentBootstrap = CreateBootstrap(currentBossGroup);
                _CurrentBootstrap = currentBootstrap;

                currentReconnectCancellationTokenSource = CreateReconnectCancellationTokenSource();
                _CurrentReconnectCancellationTokenSource = currentReconnectCancellationTokenSource;
                EnsureReconnectLoopStarted(reconnectGeneration);

                await Task.CompletedTask;

            }
            catch (Exception ex)
            {
                await RollbackStartStateAsync(currentBossGroup, currentReconnectCancellationTokenSource, currentBootstrap, _CurrentReconnectTask, _CurrentChannelHost);
                throw new InvalidOperationException("NettySocketClient start failed.", ex);
            }


        }

        protected virtual IEventLoopGroup CreateBossGroup()
        {
            return new MultithreadEventLoopGroup(1);
        }

        protected virtual Bootstrap CreateBootstrap(IEventLoopGroup currentBossGroup)
        {
            return new Bootstrap()
                .Group(currentBossGroup)
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
        }

        protected virtual CancellationTokenSource CreateReconnectCancellationTokenSource()
        {
            return new CancellationTokenSource();
        }

        protected virtual async Task RollbackStartStateAsync(IEventLoopGroup currentBossGroup, CancellationTokenSource currentReconnectCancellationTokenSource, Bootstrap currentBootstrap, Task currentReconnectTask, IChannel currentChannelHost)
        {
            try
            {
                if (!currentReconnectCancellationTokenSource.IfIsNull())
                {
                    currentReconnectCancellationTokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketClient reset start cancel reconnect failed.", ex));
            }

            try
            {
                await CloseChannelAsync(currentChannelHost);
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketClient reset start close channel failed.", ex));
            }

            try
            {
                await ShutdownBossGroupAsync(currentBossGroup);
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketClient reset start shutdown boss group failed.", ex));
            }

            try
            {
                await AwaitReconnectTaskAsync(currentReconnectTask);
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketClient reset start await reconnect task failed.", ex));
            }

            try
            {
                DisposeReconnectCancellationTokenSource(currentReconnectCancellationTokenSource);
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketClient reset start dispose reconnect token source failed.", ex));
            }

            try
            {
                ResetStartState(currentBossGroup, currentReconnectCancellationTokenSource, currentBootstrap, currentReconnectTask, currentChannelHost);
            }
            catch (Exception ex)
            {
                OnStartError(new InvalidOperationException("NettySocketClient reset start finalization failed.", ex));
            }
        }


        protected virtual long BeginReconnectGeneration()
        {
            var reconnectGeneration = Interlocked.Increment(ref _CurrentReconnectGeneration);
            _CurrentChannelContext.CurrentReconnectGeneration = reconnectGeneration;
            return reconnectGeneration;
        }

        protected virtual bool IsReconnectGenerationActive(long reconnectGeneration)
        {
            return reconnectGeneration == Volatile.Read(ref _CurrentReconnectGeneration);
        }

        protected virtual bool CanContinueReconnect(long reconnectGeneration, CancellationToken cancellationToken)
        {
            return IsRunning
                   && IsReconnectGenerationActive(reconnectGeneration)
                   && !cancellationToken.IsCancellationRequested;
        }

        protected virtual Task<IChannel> ConnectChannelAsync()
        {
            return _CurrentBootstrap.ConnectAsync(_CurrentTcpServerIPEndPoint);
        }

        protected virtual Exception CreateNullChannelConnectException()
        {
            return new InvalidOperationException("NettySocketClient connect returned null channel.");
        }

        protected virtual async Task CloseChannelAsync(IChannel channel)
        {
            if (!channel.IfIsNull())
            {
                await channel.CloseAsync();
            }
        }

        protected virtual Task ShutdownBossGroupAsync(IEventLoopGroup bossGroup)
        {
            if (bossGroup.IfIsNull())
            {
                return Task.CompletedTask;
            }

            return bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }

        protected virtual Task AwaitReconnectTaskAsync(Task reconnectTask)
        {
            return reconnectTask ?? Task.CompletedTask;
        }

        protected virtual void DisposeReconnectCancellationTokenSource(CancellationTokenSource reconnectCancellationTokenSource)
        {
            reconnectCancellationTokenSource?.Dispose();
        }

        protected virtual void ResetStartState(IEventLoopGroup currentBossGroup, CancellationTokenSource currentReconnectCancellationTokenSource, Bootstrap currentBootstrap, Task currentReconnectTask, IChannel currentChannelHost)
        {
            lock (_Locker)
            {
                IsRunning = false;

                if (ReferenceEquals(_CurrentBossGroup, currentBossGroup))
                {
                    _CurrentBossGroup = null;
                }

                if (ReferenceEquals(_CurrentReconnectCancellationTokenSource, currentReconnectCancellationTokenSource))
                {
                    _CurrentReconnectCancellationTokenSource = null;
                }

                if (ReferenceEquals(_CurrentBootstrap, currentBootstrap))
                {
                    _CurrentBootstrap = null;
                }

                if (ReferenceEquals(_CurrentReconnectTask, currentReconnectTask))
                {
                    _CurrentReconnectTask = null;
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

        protected virtual void OnConnectError(Exception ex)
        {
        }

        protected virtual bool CanIgnoreConnectException(Exception ex, long reconnectGeneration, CancellationToken cancellationToken)
        {
            if (ex is OperationCanceledException)
            {
                return cancellationToken.IsCancellationRequested || !CanContinueReconnect(reconnectGeneration, cancellationToken);
            }

            if (ex is ObjectDisposedException)
            {
                return !CanContinueReconnect(reconnectGeneration, cancellationToken);
            }

            return false;
        }

        protected virtual bool ShouldReportConnectException(Exception ex)
        {
            if (ex is OperationCanceledException || ex is ObjectDisposedException)
            {
                return false;
            }

            if (ex is System.Net.Sockets.SocketException || ex is TimeoutException)
            {
                return false;
            }

            return true;
        }

        protected virtual Task DelayBeforeReconnectAsync(CancellationToken cancellationToken)
        {
            return Task.Delay(_CurrentChannelOptions.IntervalHeartTotalMilliseconds, cancellationToken);
        }

        protected virtual void ResetStopState(IChannel currentChannelHost, IEventLoopGroup currentBossGroup, Task currentReconnectTask, CancellationTokenSource currentReconnectCancellationTokenSource, Bootstrap currentBootstrap)
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

                if (ReferenceEquals(_CurrentReconnectTask, currentReconnectTask))
                {
                    _CurrentReconnectTask = null;
                }

                if (ReferenceEquals(_CurrentReconnectCancellationTokenSource, currentReconnectCancellationTokenSource))
                {
                    _CurrentReconnectCancellationTokenSource = null;
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

        protected virtual bool HasActiveBoundChannel()
        {
            return !_CurrentChannelHost.IfIsNull() && _CurrentChannelHost.Active;
        }

        protected virtual bool TryBindConnectedChannel(long reconnectGeneration, CancellationToken cancellationToken, IChannel currentChannelHost)
        {
            if (currentChannelHost.IfIsNull())
            {
                return false;
            }

            lock (_Locker)
            {
                if (!CanContinueReconnect(reconnectGeneration, cancellationToken))
                {
                    return false;
                }

                if (HasActiveBoundChannel() && !ReferenceEquals(_CurrentChannelHost, currentChannelHost))
                {
                    return false;
                }

                _CurrentChannelHost = currentChannelHost;
                return true;
            }
        }

        protected virtual async Task ConnectToServerAsync(long reconnectGeneration)
        {
            var cancellationToken = _CurrentReconnectCancellationTokenSource?.Token ?? CancellationToken.None;

            while (CanContinueReconnect(reconnectGeneration, cancellationToken))
            {
                try
                {
                    if (_CurrentBootstrap.IfIsNull())
                    {
                        return;
                    }

                    if (!_CurrentChannelHost.IfIsNull() && _CurrentChannelHost.Active)
                    {
                        return;
                    }

                    var currentChannelHost = await ConnectChannelAsync();
                    if (currentChannelHost.IfIsNull())
                    {
                        throw CreateNullChannelConnectException();
                    }

                    if (!TryBindConnectedChannel(reconnectGeneration, cancellationToken, currentChannelHost))
                    {
                        await CloseChannelAsync(currentChannelHost);
                        return;
                    }
                    return;
                }
                catch (Exception ex) when (CanIgnoreConnectException(ex, reconnectGeneration, cancellationToken))
                {
                    return;
                }
                catch (Exception ex)
                {
                    if (ShouldReportConnectException(ex))
                    {
                        OnConnectError(new InvalidOperationException("NettySocketClient connect attempt failed.", ex));
                    }

                    if (!CanContinueReconnect(reconnectGeneration, cancellationToken))
                    {
                        return;
                    }

                    try
                    {
                        await DelayBeforeReconnectAsync(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            }

        }

        protected virtual void EnsureReconnectLoopStarted(long reconnectGeneration)
        {
            if (!CanContinueReconnect(reconnectGeneration, CancellationToken.None))
            {
                return;
            }

            lock (_Locker)
            {
                if (!CanContinueReconnect(reconnectGeneration, CancellationToken.None))
                {
                    return;
                }

                if (!_CurrentReconnectTask.IfIsNullOrEmpty() && !_CurrentReconnectTask.IsCompleted)
                {
                    return;
                }

                if (_CurrentReconnectCancellationTokenSource.IfIsNull())
                {
                    _CurrentReconnectCancellationTokenSource = new CancellationTokenSource();
                }

                _CurrentReconnectTask = ConnectToServerAsync(reconnectGeneration);
            }
        }


        protected override async Task OnStopAsync()
        {
            var currentChannelHost = _CurrentChannelHost;
            var currentBossGroup = _CurrentBossGroup;
            var currentReconnectTask = _CurrentReconnectTask;
            var currentReconnectCancellationTokenSource = _CurrentReconnectCancellationTokenSource;
            var currentBootstrap = _CurrentBootstrap;

            try
            {
                if (!currentReconnectCancellationTokenSource.IfIsNull())
                {
                    currentReconnectCancellationTokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketClient stop cancel reconnect failed.", ex));
            }

            try
            {
                await CloseChannelAsync(currentChannelHost);
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketClient stop close channel failed.", ex));
            }

            try
            {
                await ShutdownBossGroupAsync(currentBossGroup);
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketClient stop shutdown boss group failed.", ex));
            }

            try
            {
                await AwaitReconnectTaskAsync(currentReconnectTask);
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketClient stop await reconnect task failed.", ex));
            }

            try
            {
                DisposeReconnectCancellationTokenSource(currentReconnectCancellationTokenSource);
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketClient stop dispose reconnect token source failed.", ex));
            }

            try
            {
                ResetStopState(currentChannelHost, currentBossGroup, currentReconnectTask, currentReconnectCancellationTokenSource, currentBootstrap);
            }
            catch (Exception ex)
            {
                OnStopError(new InvalidOperationException("NettySocketClient stop finalization failed.", ex));
            }

        }




    }

}
