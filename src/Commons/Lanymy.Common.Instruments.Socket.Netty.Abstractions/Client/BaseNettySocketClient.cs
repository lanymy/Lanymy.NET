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

        protected BaseNettySocketClient(TClientChannelContext serverChannelContext) : base(serverChannelContext)
        {

            var tcpServerIP = _CurrentChannelOptions.ServerIP;
            var tcpServerPort = _CurrentChannelOptions.Port;
            _CurrentTcpServerIPEndPoint = new IPEndPoint(IPAddress.Parse(tcpServerIP), tcpServerPort);

            _CurrentChannelContext.CurrentConnectToServerAction = new WeakReference<Action>(EnsureReconnectLoopStarted);

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



                _CurrentReconnectCancellationTokenSource = new CancellationTokenSource();
                EnsureReconnectLoopStarted();

                await Task.CompletedTask;

            }
            catch
            {
                _CurrentChannelHost = null;
            }


        }


        protected virtual async Task ConnectToServerAsync()
        {
            var cancellationToken = _CurrentReconnectCancellationTokenSource?.Token ?? CancellationToken.None;

            while (IsRunning && !cancellationToken.IsCancellationRequested)
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

                    _CurrentChannelHost = await _CurrentBootstrap.ConnectAsync(_CurrentTcpServerIPEndPoint);
                    return;
                }
                catch
                {
                    if (!IsRunning || cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        await Task.Delay(_CurrentChannelOptions.IntervalHeartTotalMilliseconds, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            }

        }

        protected virtual void EnsureReconnectLoopStarted()
        {
            if (!IsRunning)
            {
                return;
            }

            lock (_Locker)
            {
                if (!IsRunning)
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

                _CurrentReconnectTask = ConnectToServerAsync();
            }
        }


        protected override async Task OnStopAsync()
        {
            if (!_CurrentReconnectCancellationTokenSource.IfIsNull())
            {
                _CurrentReconnectCancellationTokenSource.Cancel();
            }

            try
            {
                if (!_CurrentChannelHost.IfIsNull())
                {
                    await _CurrentChannelHost.CloseAsync();
                }
            }
            finally
            {
                if (!_CurrentBossGroup.IfIsNull())
                {
                    await _CurrentBossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                }
            }

            try
            {
                if (!_CurrentReconnectTask.IfIsNullOrEmpty())
                {
                    await _CurrentReconnectTask;
                }
            }
            catch (OperationCanceledException)
            {
                // ignored
            }

            try
            {
                if (!_CurrentReconnectCancellationTokenSource.IfIsNull())
                {
                    _CurrentReconnectCancellationTokenSource.Dispose();
                }

                _CurrentReconnectCancellationTokenSource = null;
                _CurrentReconnectTask = null;
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
