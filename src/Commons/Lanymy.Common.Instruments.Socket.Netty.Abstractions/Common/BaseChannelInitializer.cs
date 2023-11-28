using System;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Lanymy.Common.Instruments.Server;

namespace Lanymy.Common.Instruments.Common
{


    public abstract class BaseChannelInitializer<TChannelContext, TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelHandler> : ChannelInitializer<ISocketChannel>
        where TChannelContext : BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TChannelHandler : BaseChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelContext>
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        protected readonly TChannelContext _CurrentServerChannelContext;
        protected readonly ChannelOptions _CurrentChannelOptions;

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

            channel.Pipeline

#if DEBUG
                .AddLast(new LoggingHandler("SRV-CONN"))
#endif

                //.AddLast(new IdleStateHandler(10, 3, 0))
                .AddLast(new IdleStateHandler(_CurrentReaderIdleTime, _CurrentWriterIdleTime, _CurrentAllIdleTime))

                //.AddLast(new LengthFieldBasedFrameDecoder(ushort.MaxValue, 6, 1, 2, 0))
                .AddLast(new LengthFieldBasedFrameDecoder(ushort.MaxValue, _CurrentChannelOptions.LengthFieldOffset, _CurrentChannelOptions.LengthFieldLength, _CurrentChannelOptions.LengthAdjustment, _CurrentChannelOptions.InitialBytesToStrip));

            //var channelHandler = Activator.CreateInstance(_CurrentChannelClientHandlerType, _CurrentServerChannelContext) as IChannelHandler;
            var channelHandler = GetChannelHandler();

            if (_CurrentChannelOptions.IsUseSingleThreadEventLoop)
            {
                channel.Pipeline.AddLast(new SingleThreadEventLoop(), channelHandler);
            }
            else
            {
                channel.Pipeline.AddLast(channelHandler);
            }



        }


        protected abstract TChannelHandler GetChannelHandler();


    }

}
