using System;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Lanymy.Common.Instruments.Common
{


    public abstract class BaseChannelInitializer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelOptions, TChannelContext, TChannelHandler> : ChannelInitializer<ISocketChannel>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelOptions : BaseChannelOptions
        where TChannelSession : BaseChannelSession, new()
        where TChannelHandler : BaseChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelOptions, TChannelContext>
        where TChannelContext : BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelOptions>
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        protected readonly TChannelContext _CurrentServerChannelContext;
        protected readonly BaseChannelOptions _CurrentChannelOptions;

        protected readonly TimeSpan _CurrentReaderIdleTime;
        protected readonly TimeSpan _CurrentWriterIdleTime;
        protected readonly TimeSpan _CurrentAllIdleTime;

        protected readonly Type _CurrentChannelClientHandlerType;


        protected BaseChannelInitializer
        (
            //ChannelOptionDto channelOptionDto
            TChannelContext serverChannelContext
        )
        {

            _CurrentChannelClientHandlerType = typeof(TChannelHandler);

            //_CurrentServerChannelContext = Activator.CreateInstance(typeof(TServerChannelContext), channelOptionDto) as TServerChannelContext;
            _CurrentServerChannelContext = serverChannelContext;
            _CurrentChannelOptions = _CurrentServerChannelContext.CurrentChannelOptions;

            _CurrentAllIdleTime = TimeSpan.FromMilliseconds(0);
            _CurrentReaderIdleTime = TimeSpan.FromMilliseconds(_CurrentChannelOptions.HeartTimeOutCount * _CurrentChannelOptions.IntervalHeartTotalMilliseconds + 1000);
            _CurrentWriterIdleTime = TimeSpan.FromMilliseconds(_CurrentChannelOptions.IntervalHeartTotalMilliseconds);

        }


        protected override void InitChannel(ISocketChannel channel)
        {
            try
            {
                InitializeChannel(channel);
            }
            catch (Exception ex)
            {
                HandleInitChannelException(channel, ex);
            }
        }

        protected virtual void InitializeChannel(ISocketChannel channel)
        {
            var pipeline = GetChannelPipeline(channel);

            ConfigureBasePipeline(pipeline);

            var channelHandler = EnsureChannelHandlerCreated(GetChannelHandler());

            AddTerminalChannelHandlers(pipeline, channelHandler);
        }

        protected virtual IChannelPipeline GetChannelPipeline(ISocketChannel channel)
        {
            if (channel == null)
            {
                throw new InvalidOperationException("NettyChannelInitializer channel is null.");
            }

            return channel.Pipeline;
        }

        protected virtual void ConfigureBasePipeline(IChannelPipeline pipeline)
        {
            pipeline

#if DEBUG
                .AddLast(new LoggingHandler("SRV-CONN"))
#endif

                //.AddLast(new IdleStateHandler(10, 3, 0))
                .AddLast(new IdleStateHandler(_CurrentReaderIdleTime, _CurrentWriterIdleTime, _CurrentAllIdleTime))

                //.AddLast(new LengthFieldBasedFrameDecoder(ushort.MaxValue, 6, 1, 2, 0))
                .AddLast(new LengthFieldBasedFrameDecoder(ushort.MaxValue, _CurrentChannelOptions.LengthFieldOffset, _CurrentChannelOptions.LengthFieldLength, _CurrentChannelOptions.LengthAdjustment, _CurrentChannelOptions.InitialBytesToStrip));
        }

        protected virtual TChannelHandler EnsureChannelHandlerCreated(TChannelHandler channelHandler)
        {
            if (channelHandler == null)
            {
                throw new InvalidOperationException("NettyChannelInitializer create channel handler returned null.");
            }

            return channelHandler;
        }

        protected virtual void AddTerminalChannelHandlers(IChannelPipeline pipeline, TChannelHandler channelHandler)
        {
            if (_CurrentChannelOptions.IsUseSingleThreadEventLoop)
            {
                pipeline.AddLast(new SingleThreadEventLoop(), channelHandler);
            }
            else
            {
                pipeline.AddLast(channelHandler);
            }
        }

        protected virtual void HandleInitChannelException(ISocketChannel channel, Exception exception)
        {
            var wrappedException = new InvalidOperationException("NettyChannelInitializer init channel failed.", exception);
            TryCloseChannelAfterInitFailure(channel);
            throw wrappedException;
        }

        protected virtual void TryCloseChannelAfterInitFailure(ISocketChannel channel)
        {
            if (!CanCloseChannelAfterInitFailure(channel))
            {
                return;
            }

            _ = SafeCloseChannelAfterInitFailureAsync(channel);
        }

        protected virtual bool CanCloseChannelAfterInitFailure(ISocketChannel channel)
        {
            return channel != null;
        }

        protected virtual Task ExecuteCloseChannelAfterInitFailureAsync(ISocketChannel channel)
        {
            return channel.CloseAsync();
        }

        protected virtual async Task SafeCloseChannelAfterInitFailureAsync(ISocketChannel channel)
        {
            try
            {
                await ExecuteCloseChannelAfterInitFailureAsync(channel);
            }
            catch (Exception ex) when (CanIgnoreInitChannelCloseException(ex, channel))
            {
            }
            catch (Exception ex)
            {
                OnInitChannelError(new InvalidOperationException("NettyChannelInitializer close channel after init failed.", ex));
            }
        }

        protected virtual bool CanIgnoreInitChannelCloseException(Exception exception, ISocketChannel channel)
        {
            if (exception is OperationCanceledException)
            {
                return true;
            }

            if (exception is ObjectDisposedException)
            {
                return true;
            }

            if (exception is InvalidOperationException && (channel == null || !channel.Open || IsUnregisteredChannelCloseNoise(exception, channel)))
            {
                return true;
            }

            return false;
        }

        protected virtual bool IsUnregisteredChannelCloseNoise(Exception exception, ISocketChannel channel)
        {
            return channel != null
                   && !channel.Registered
                   && exception.Message?.IndexOf("not registered to an event loop", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected virtual void OnInitChannelError(Exception exception)
        {
        }


        //protected abstract TChannelHandler GetChannelHandler();

        protected virtual TChannelHandler GetChannelHandler()
        {
            //return Activator.CreateInstance(_CurrentChannelClientHandlerType, _CurrentServerChannelContext, _OnConnectToServerAction) as TClientChannelHandler;
            return Activator.CreateInstance(_CurrentChannelClientHandlerType, _CurrentServerChannelContext) as TChannelHandler;
        }


    }

}
