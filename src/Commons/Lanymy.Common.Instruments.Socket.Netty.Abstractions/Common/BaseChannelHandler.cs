using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        private const int CloseRequestStateNone = 0;
        private const int CloseRequestStateDelayedScheduled = 1;
        private const int CloseRequestStateRequested = 2;

        protected IChannelHandlerContext _CurrentChannelHandlerContext;

        protected int _CurrentCloseRequestState = CloseRequestStateNone;

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
            SafeHandleUserEventTriggered(context, evt);

        }

        protected virtual void SafeHandleUserEventTriggered(IChannelHandlerContext context, object evt)
        {
            try
            {
                OnUserEventTriggered(context, evt);
            }
            catch (Exception ex) when (CanIgnoreHandlerCallbackException(ex, context))
            {
            }
            catch (Exception ex)
            {
                OnHandlerError(new InvalidOperationException("NettyChannelHandler user event callback failed.", ex));
            }
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

            SafeHandleChannelActive(context);

        }

        protected virtual void SafeHandleChannelActive(IChannelHandlerContext context)
        {
            try
            {
                OnChannelActive(context);
            }
            catch (Exception ex) when (CanIgnoreHandlerCallbackException(ex, context))
            {
                ResetCurrentChannelState();
            }
            catch (Exception ex)
            {
                ResetCurrentChannelState();
                OnHandlerError(new InvalidOperationException("NettyChannelHandler channel active callback failed.", ex));
                OnContextClose(context);
            }
        }


        protected abstract void OnChannelActive(IChannelHandlerContext context);



        /// <summary>
        /// 当前频道未激活状态 / channel处于非活跃状态，没有连接到远程主机
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {

            base.ChannelInactive(context);

            ResetCurrentChannelState();

            SafeHandleChannelInactive(context);

        }

        protected virtual void SafeHandleChannelInactive(IChannelHandlerContext context)
        {
            try
            {
                OnChannelInactive(context);
            }
            catch (Exception ex) when (CanIgnoreHandlerCallbackException(ex, context))
            {
            }
            catch (Exception ex)
            {
                OnHandlerError(new InvalidOperationException("NettyChannelHandler channel inactive callback failed.", ex));
            }
        }


        /// <summary>
        /// 当前频道未激活状态 / channel处于非活跃状态，已断开远程主机连接
        /// </summary>
        /// <param name="context"></param>
        protected abstract void OnChannelInactive(IChannelHandlerContext context);

        protected virtual void ResetCurrentChannelState()
        {
            _CurrentChannelHandlerContext = null;
            Volatile.Write(ref _CurrentCloseRequestState, CloseRequestStateNone);
            _CurrentChannelSession.RemoteIpEndPoint = null;

            if (_CurrentChannelSession.IsLogin)
            {
                _CurrentChannelSession.IsLogin = false;
            }
        }



        /// <summary>
        /// socket接收消息方法具体的实现
        /// </summary>
        /// <param name="context">当前频道的句柄，可使用发送和接收方法</param>
        /// <param name="message">接收到的客户端发送的内容</param>
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            SafeHandleChannelRead(context, message);

        }

        protected virtual void SafeHandleChannelRead(IChannelHandlerContext context, object message)
        {
            try
            {
                OnChannelRead(context, message);
            }
            catch (Exception ex) when (CanIgnoreHandlerCallbackException(ex, context))
            {
            }
            catch (Exception ex)
            {
                OnHandlerError(new InvalidOperationException("NettyChannelHandler read callback failed.", ex));
            }
        }


        protected virtual void OnChannelRead(IChannelHandlerContext context, object message)
        {
            try
            {
                if (message is not IByteBuffer buffer)
                {
                    return;
                }

                var packageDataBytesLength = buffer.ReadableBytes;

                if (packageDataBytesLength > 0)
                {

                    var packageDataBytes = _CurrenChannelContext.CurrentDataBytesArrayPool.Rent(packageDataBytesLength);

                    try
                    {
                        buffer.GetBytes(buffer.ReaderIndex, packageDataBytes, 0, packageDataBytesLength);

                        //OnChannelReadBytes(context, packageDataBytesLength, packageDataBytes);
                        //OnChannelReadBytes(context, packageDataBytes.AsSpan(0, packageDataBytesLength));
                        OnChannelReadBytes(context, new ReadOnlySpan<byte>(packageDataBytes, 0, packageDataBytesLength));
                    }
                    finally
                    {
                        _CurrenChannelContext.CurrentDataBytesArrayPool.Return(packageDataBytes);
                    }

                }
            }
            finally
            {
                ReferenceCountUtil.Release(message);
            }

        }


        //protected abstract void OnChannelReadBytes(IChannelHandlerContext context, int packageDataBytesLength, byte[] packageDataBytes);
        //protected abstract void OnChannelReadBytes(IChannelHandlerContext context, Span<byte> packageDataBytes);
        protected abstract void OnChannelReadBytes(IChannelHandlerContext context, ReadOnlySpan<byte> packageDataBytes);


        /// <summary>
        /// 该次会话读取完成后回调函数
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            SafeHandleChannelReadComplete(context);
        }

        protected virtual void SafeHandleChannelReadComplete(IChannelHandlerContext context)
        {
            try
            {
                ExecuteChannelReadComplete(context);
            }
            catch (Exception ex) when (CanIgnoreReadCompleteException(ex, context))
            {
            }
            catch (Exception ex)
            {
                OnHandlerError(new InvalidOperationException("NettyChannelHandler flush read complete failed.", ex));
            }
        }

        protected virtual void ExecuteChannelReadComplete(IChannelHandlerContext context)
        {
            if (context.IfIsNull())
            {
                return;
            }

            context.Flush();//将WriteAsync写入的数据流缓存发送出去
        }

        protected virtual bool CanIgnoreReadCompleteException(Exception exception, IChannelHandlerContext context)
        {
            if (exception is ObjectDisposedException)
            {
                return true;
            }

            if (IsClosedOrUnregisteredChannelInvalidOperation(exception, context))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 异常捕获
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            SafeHandleException(context, exception);
        }

        protected virtual void SafeHandleException(IChannelHandlerContext context, Exception exception)
        {
            try
            {
                OnException(context, exception);
            }
            catch (Exception ex) when (CanIgnoreHandlerCallbackException(ex, context))
            {
            }
            catch (Exception ex)
            {
                OnHandlerError(new InvalidOperationException("NettyChannelHandler exception callback failed.", ex));
            }
        }



        protected virtual void OnException(IChannelHandlerContext context, Exception exception)
        {

            OnContextClose(context);

        }

        protected virtual bool CanIgnoreHandlerCallbackException(Exception exception, IChannelHandlerContext context)
        {
            if (exception is ObjectDisposedException)
            {
                return true;
            }

            if (IsClosedOrUnregisteredChannelInvalidOperation(exception, context))
            {
                return true;
            }

            return false;
        }

        protected virtual void OnHandlerError(Exception exception)
        {
        }


        public void SendBytes(byte[] bytes)
        {

            SendBytes(_CurrentChannelHandlerContext, bytes);

        }


        protected virtual void SendBytes(IChannelHandlerContext context, byte[] bytes)
        {

            if (CanScheduleSend(context, bytes))
            {
                _ = SafeScheduleSendBytesAsync(context, bytes);
            }

        }

        protected virtual bool CanScheduleSend(IChannelHandlerContext context, byte[] bytes)
        {
            return !context.IfIsNull() && !bytes.IfIsNullOrEmpty();
        }

        protected virtual Task ScheduleSendBytesAsync(IChannelHandlerContext context, byte[] bytes)
        {
            return context.Executor.ScheduleAsync(async () => await WriteBytesAsync(context, bytes), _CurrentSendDataIntervalMilliseconds);
        }

        protected virtual async Task SafeScheduleSendBytesAsync(IChannelHandlerContext context, byte[] bytes)
        {
            try
            {
                await ScheduleSendBytesAsync(context, bytes);
            }
            catch (Exception ex) when (CanIgnoreSendException(ex, context))
            {
            }
            catch (Exception ex)
            {
                OnHandlerError(new InvalidOperationException("NettyChannelHandler schedule send failed.", ex));
            }
        }

        protected virtual async Task WriteBytesAsync(IChannelHandlerContext context, byte[] bytes)
        {
            try
            {
                await ExecuteWriteAndFlushAsync(context, bytes);
            }
            catch (Exception ex) when (CanIgnoreSendException(ex, context))
            {
                return;
            }
            catch (Exception ex)
            {
                OnHandlerError(new InvalidOperationException("NettyChannelHandler write send failed.", ex));
                return;
            }
        }

        protected virtual Task ExecuteWriteAndFlushAsync(IChannelHandlerContext context, byte[] bytes)
        {
            var messageBytes = Unpooled.CopiedBuffer(bytes);
            return context.WriteAndFlushAsync(messageBytes);
        }

        protected virtual bool CanIgnoreSendException(Exception exception, IChannelHandlerContext context)
        {
            if (exception is OperationCanceledException)
            {
                return true;
            }

            if (exception is ObjectDisposedException)
            {
                return true;
            }

            if (IsClosedOrUnregisteredChannelInvalidOperation(exception, context))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Context 关闭 事件; 通信管道最后关闭事件 需要 监听  OnChannelInactive
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnContextClose(IChannelHandlerContext context)
        {
            if (!CanCloseContext(context))
            {
                return;
            }

            if (!TryBeginCloseRequest())
            {
                return;
            }

            _ = SafeCloseContextAsync(context);
        }

        protected virtual bool CanCloseContext(IChannelHandlerContext context)
        {
            return !context.IfIsNull();
        }

        protected virtual Task ExecuteCloseContextAsync(IChannelHandlerContext context)
        {
            return context.CloseAsync();
        }

        protected virtual async Task SafeCloseContextAsync(IChannelHandlerContext context)
        {
            try
            {
                await ExecuteCloseContextAsync(context);
            }
            catch (Exception ex) when (CanIgnoreContextCloseException(ex, context))
            {
                ReleaseCloseRequestAfterFailure();
            }
            catch (Exception ex)
            {
                ReleaseCloseRequestAfterFailure();
                OnHandlerError(new InvalidOperationException("NettyChannelHandler close context failed.", ex));
            }

        }


        /// <summary>
        /// 延迟执行连接关闭
        /// </summary>
        /// <param name="context"></param>
        /// <param name="delayMilliseconds">延迟执行的毫秒数</param>
        protected virtual void OnContextClose(IChannelHandlerContext context, int delayMilliseconds)
        {
            if (!CanScheduleContextClose(context))
            {
                return;
            }

            if (!TryBeginDelayedCloseRequest())
            {
                return;
            }

            _ = SafeScheduleCloseContextAsync(context, delayMilliseconds);

        }

        protected virtual bool CanScheduleContextClose(IChannelHandlerContext context)
        {
            return !context.IfIsNull();
        }

        protected virtual Task ScheduleCloseContextAsync(IChannelHandlerContext context, int delayMilliseconds)
        {
            return context.Executor.ScheduleAsync(() =>
            {
                OnContextClose(context);

            }, TimeSpan.FromMilliseconds(delayMilliseconds));
        }

        protected virtual async Task SafeScheduleCloseContextAsync(IChannelHandlerContext context, int delayMilliseconds)
        {
            try
            {
                await ScheduleCloseContextAsync(context, delayMilliseconds);
            }
            catch (Exception ex) when (CanIgnoreContextCloseException(ex, context))
            {
                ReleaseDelayedCloseRequestAfterFailure();
            }
            catch (Exception ex)
            {
                ReleaseDelayedCloseRequestAfterFailure();
                OnHandlerError(new InvalidOperationException("NettyChannelHandler schedule close context failed.", ex));
            }
        }

        protected virtual bool CanIgnoreContextCloseException(Exception exception, IChannelHandlerContext context)
        {
            if (exception is OperationCanceledException)
            {
                return true;
            }

            if (exception is ObjectDisposedException)
            {
                return true;
            }

            if (IsClosedOrUnregisteredChannelInvalidOperation(exception, context))
            {
                return true;
            }

            return false;
        }

        protected virtual bool IsClosedOrUnregisteredChannelInvalidOperation(Exception exception, IChannelHandlerContext context)
        {
            if (exception is not InvalidOperationException || context.IfIsNull())
            {
                return false;
            }

            var channel = context.Channel;

            return channel.IfIsNull()
                   || !channel.Open
                   || IsUnregisteredChannelOperationNoise(exception, channel)
                   || IsRemovedHandlerContextOperationNoise(exception, context);
        }

        protected virtual bool IsUnregisteredChannelOperationNoise(Exception exception, IChannel channel)
        {
            return channel != null
                   && !channel.Registered
                   && exception.Message?.IndexOf("not registered to an event loop", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected virtual bool IsRemovedHandlerContextOperationNoise(Exception exception, IChannelHandlerContext context)
        {
            return context != null
                   && context.Removed
                   && exception.Message?.IndexOf("handler not added to pipeline yet", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected virtual bool TryBeginCloseRequest()
        {
            while (true)
            {
                var currentState = Volatile.Read(ref _CurrentCloseRequestState);
                if (currentState == CloseRequestStateRequested)
                {
                    return false;
                }

                if (Interlocked.CompareExchange(ref _CurrentCloseRequestState, CloseRequestStateRequested, currentState) == currentState)
                {
                    return true;
                }
            }
        }

        protected virtual bool TryBeginDelayedCloseRequest()
        {
            return Interlocked.CompareExchange(ref _CurrentCloseRequestState, CloseRequestStateDelayedScheduled, CloseRequestStateNone) == CloseRequestStateNone;
        }

        protected virtual void ReleaseDelayedCloseRequestAfterFailure()
        {
            Interlocked.CompareExchange(ref _CurrentCloseRequestState, CloseRequestStateNone, CloseRequestStateDelayedScheduled);
        }

        protected virtual void ReleaseCloseRequestAfterFailure()
        {
            Interlocked.CompareExchange(ref _CurrentCloseRequestState, CloseRequestStateNone, CloseRequestStateRequested);
        }



    }

}
