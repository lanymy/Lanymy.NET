using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;


namespace Lanymy.Common.Instruments.Common
{
    /// <summary>
    /// 定义 Netty Socket 宿主的统一生命周期：启动、停止和异步释放。
    /// </summary>
    public abstract class BaseSocketHost<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelOptions, TChannelContext, TChannelHandler, TChannelInitializer> : IAsyncDisposable
        where TChannelInitializer : BaseChannelInitializer<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelOptions, TChannelContext, TChannelHandler>
        where TChannelContext : BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelOptions>
        where TChannelHandler : BaseChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelOptions, TChannelContext>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelOptions : BaseChannelOptions
        where TChannelSession : BaseChannelSession, new()
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {
        /// <summary>
        /// 保护运行态切换和引用清理的共享锁。
        /// </summary>
        protected readonly object _Locker = new object();

        /// <summary>
        /// 串行化 Start/Stop/DisposeAsync，避免生命周期并发重入。
        /// </summary>
        protected readonly SemaphoreSlim _LifecycleSemaphore = new(1, 1);

        private bool _IsRunning = false;
        private bool _IsDisposed = false;
        /// <summary>
        /// 指示宿主当前是否处于运行态。
        /// </summary>
        public bool IsRunning
        {
            get { return Volatile.Read(ref _IsRunning); }
            //private set
            protected set
            {

                if (_IsRunning == value)
                    return;

                lock (_Locker)
                {
                    if (!_IsRunning.Equals(value))
                    {
                        _IsRunning = value;
                    }
                }

            }
        }

        protected IChannel _CurrentChannelHost;
        protected IEventLoopGroup _CurrentBossGroup;

        protected readonly TChannelContext _CurrentChannelContext;
        protected readonly TChannelOptions _CurrentChannelOptions;

        /// <summary>
        /// 初始化 Socket 宿主。
        /// </summary>
        /// <param name="serverChannelContext">当前宿主使用的通道上下文。</param>
        protected BaseSocketHost(TChannelContext serverChannelContext)
        {
            _CurrentChannelContext = serverChannelContext;

            _CurrentChannelOptions = _CurrentChannelContext.CurrentChannelOptions;

        }

        /// <summary>
        /// 启动宿主。
        /// </summary>
        public async Task StartAsync()
        {
            await _LifecycleSemaphore.WaitAsync();
            try
            {
                ThrowIfDisposed();

                if (IsRunning)
                {
                    return;
                }

                // 先进入运行态，再交给子类做具体启动；失败时会回滚状态。
                IsRunning = true;

                try
                {
                    await OnStartAsync();
                }
                catch
                {
                    IsRunning = false;
                    throw;
                }

            }
            finally
            {
                _LifecycleSemaphore.Release();
            }

        }

        /// <summary>
        /// 停止宿主。
        /// </summary>
        public async Task StopAsync()
        {
            await _LifecycleSemaphore.WaitAsync();
            try
            {
                if (!IsRunning)
                {
                    return;
                }

                // 先对外宣告停止，阻止后续逻辑继续发起新操作。
                IsRunning = false;

                await OnStopAsync();

            }
            finally
            {
                _LifecycleSemaphore.Release();
            }
        }



        protected abstract Task OnStartAsync();

        protected abstract Task OnStopAsync();

        protected abstract Task OnDisposeAsync();

        /// <summary>
        /// 释放宿主资源。
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await _LifecycleSemaphore.WaitAsync();
            try
            {
                if (_IsDisposed)
                {
                    return;
                }

                Exception stopException = null;
                Exception disposeException = null;

                if (IsRunning)
                {
                    IsRunning = false;

                    try
                    {
                        await OnStopAsync();
                    }
                    catch (Exception ex)
                    {
                        stopException = ex;
                    }
                }

                try
                {
                    await OnDisposeAsync();
                }
                catch (Exception ex)
                {
                    disposeException = ex;
                }

                _IsDisposed = true;

                if (stopException != null && disposeException != null)
                {
                    throw new AggregateException(stopException, disposeException);
                }

                if (stopException != null)
                {
                    ExceptionDispatchInfo.Capture(stopException).Throw();
                }

                if (disposeException != null)
                {
                    ExceptionDispatchInfo.Capture(disposeException).Throw();
                }
            }
            finally
            {
                _LifecycleSemaphore.Release();
            }

        }

        protected virtual void ThrowIfDisposed()
        {
            if (_IsDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }



    }

}
