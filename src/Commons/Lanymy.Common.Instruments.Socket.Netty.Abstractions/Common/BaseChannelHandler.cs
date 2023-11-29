using System;
using System.Net;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments.Common
{

    public abstract class BaseChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelOptions, TChannelContext> : ChannelHandlerAdapter, IChannelClientHandler<TChannelSession>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelOptions : BaseChannelOptions
        where TChannelSession : BaseChannelSession, new()
        where TChannelContext : BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelOptions>
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {

        protected IChannelHandlerContext _CurrentChannelHandlerContext;

        protected TChannelSession _CurrentChannelSession = new();

        public TChannelSession CurrentChannelSession => _CurrentChannelSession;

        protected readonly TimeSpan _CurrentSendDataIntervalMilliseconds;

        protected readonly TChannelContext _CurrenChannelContext;

        protected readonly TChannelFixedHeaderPackageFilter _CurrentFixedHeaderPackageFilter;


        protected BaseChannelHandler(TChannelContext channelContext)
        {

            _CurrenChannelContext = channelContext;

            _CurrentFixedHeaderPackageFilter = _CurrenChannelContext.CurrentFixedHeaderPackageFilter;

            _CurrentSendDataIntervalMilliseconds = TimeSpan.FromMilliseconds(_CurrenChannelContext.CurrentChannelOptions.SendDataIntervalMilliseconds);

        }

        /// <summary>
        /// 原始基类事件触发器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="evt"></param>
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {

            OnUserEventTriggered(context, evt);

        }

        /// <summary>
        /// 重载 事件 触发器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="evt"></param>
        protected virtual void OnUserEventTriggered(IChannelHandlerContext context, object evt)
        {

            if (evt is IdleStateEvent idleStateEvent)
            {
                OnIdleEventTrigger(context, idleStateEvent);
            }

        }

        /// <summary>
        /// 超时 监测 触发器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="idleStateEvent"></param>
        protected virtual void OnIdleEventTrigger(IChannelHandlerContext context, IdleStateEvent idleStateEvent)
        {

            if (idleStateEvent.State == IdleState.WriterIdle)
            {

                OnWriterTimeOutEventTrigger(context);

            }
            else if (idleStateEvent.State == IdleState.ReaderIdle)
            {

                OnReaderTimeOutEventTrigger(context);

            }

        }


        /// <summary>
        /// 写入 超时 触发器
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnWriterTimeOutEventTrigger(IChannelHandlerContext context)
        {
            //3秒没写数据 就发个心跳包
            OnHeart(context);
        }


        /// <summary>
        /// 读取 超时 触发器
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnReaderTimeOutEventTrigger(IChannelHandlerContext context)
        {
            //读数据超时 认为远程连接已挂 主动断开连接
            OnContextClose(context);
        }


        /// <summary>
        /// 心跳事件
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnHeart(IChannelHandlerContext context)
        {
            SendBytes(context, _CurrentFixedHeaderPackageFilter.GetHeartBytes(_CurrentChannelSession));
        }


        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelActive(IChannelHandlerContext context)
        {


            base.ChannelActive(context);

            _CurrentChannelHandlerContext = context;
            _CurrentChannelSession.RemoteIpEndPoint = context.Channel.RemoteAddress as IPEndPoint;

            //_CurrentChannelDictionary.AddOrUpdate
            //(

            //    _CurrentChannelSession.SessionID,
            //    this,
            //    (_, _) => this
            //);

            OnChannelActive(context);

        }


        protected abstract void OnChannelActive(IChannelHandlerContext context);



        /// <summary>
        /// 当前频道未激活状态 / channel处于非活跃状态，没有连接到远程主机
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {

            base.ChannelInactive(context);

            _CurrentChannelHandlerContext = null;

            //_CurrentChannelDictionary.TryRemove(_CurrentChannelSession.SessionID, out _);

            if (_CurrentChannelSession.IsLogin)
            {

                _CurrentChannelSession.IsLogin = false;

            }

            OnChannelInactive(context);

        }


        /// <summary>
        /// 当前频道未激活状态 / channel处于非活跃状态，已断开远程主机连接
        /// </summary>
        /// <param name="context"></param>
        protected abstract void OnChannelInactive(IChannelHandlerContext context);



        /// <summary>
        /// socket接收消息方法具体的实现
        /// </summary>
        /// <param name="context">当前频道的句柄，可使用发送和接收方法</param>
        /// <param name="message">接收到的客户端发送的内容</param>
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {

            OnChannelRead(context, message);

        }


        protected virtual void OnChannelRead(IChannelHandlerContext context, object message)
        {

            if (message is not IByteBuffer buffer) return;

            var packageDataBytesLength = buffer.ReadableBytes;

            if (packageDataBytesLength > 0)
            {

                var packageDataBytes = _CurrenChannelContext.CurrentDataBytesArrayPool.Rent(packageDataBytesLength);

                buffer.GetBytes(buffer.ReaderIndex, packageDataBytes, 0, packageDataBytesLength);

                //OnChannelReadBytes(context, packageDataBytesLength, packageDataBytes);
                OnChannelReadBytes(context, packageDataBytes.AsSpan(0, packageDataBytesLength));

                _CurrenChannelContext.CurrentDataBytesArrayPool.Return(packageDataBytes);

            }


            ReferenceCountUtil.Release(message);

        }


        //protected abstract void OnChannelReadBytes(IChannelHandlerContext context, int packageDataBytesLength, byte[] packageDataBytes);
        protected abstract void OnChannelReadBytes(IChannelHandlerContext context, Span<byte> packageDataBytes);


        /// <summary>
        /// 该次会话读取完成后回调函数
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();//将WriteAsync写入的数据流缓存发送出去

        /// <summary>
        /// 异常捕获
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            OnException(context, exception);
        }



        protected virtual void OnException(IChannelHandlerContext context, Exception exception)
        {

            OnContextClose(context);

        }


        public void SendBytes(byte[] bytes)
        {

            SendBytes(_CurrentChannelHandlerContext, bytes);

        }


        protected virtual void SendBytes(IChannelHandlerContext context, byte[] bytes)
        {

            if (!context.IfIsNull() && !bytes.IfIsNullOrEmpty())
            {
                context.Executor.ScheduleAsync(() =>
                {
                    var messageBytes = Unpooled.CopiedBuffer(bytes);
                    context.WriteAndFlushAsync(messageBytes);

                }, _CurrentSendDataIntervalMilliseconds);
            }

        }

        /// <summary>
        /// Context 关闭 事件; 通信管道最后关闭事件 需要 监听  OnChannelInactive
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnContextClose(IChannelHandlerContext context)
        {

            context.CloseAsync();

        }


        /// <summary>
        /// 延迟执行连接关闭
        /// </summary>
        /// <param name="context"></param>
        /// <param name="delayMilliseconds">延迟执行的毫秒数</param>
        protected virtual void OnContextClose(IChannelHandlerContext context, int delayMilliseconds)
        {


            context.Executor.ScheduleAsync(() =>
            {
                context.CloseAsync();

            }, TimeSpan.FromMilliseconds(delayMilliseconds));

        }



    }

}
