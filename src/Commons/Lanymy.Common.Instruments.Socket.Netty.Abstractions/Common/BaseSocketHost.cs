using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;


namespace Lanymy.Common.Instruments.Common
{



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


        protected readonly object _Locker = new object();
        protected readonly SemaphoreSlim _LifecycleSemaphore = new(1, 1);

        private bool _IsRunning = false;
        private bool _IsDisposed = false;

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


        protected BaseSocketHost(TChannelContext serverChannelContext)
        {

            _CurrentChannelContext = serverChannelContext;

            _CurrentChannelOptions = _CurrentChannelContext.CurrentChannelOptions;

        }



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


        public async Task StopAsync()
        {
            await _LifecycleSemaphore.WaitAsync();
            try
            {

                if (!IsRunning)
                {
                    return;
                }

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
