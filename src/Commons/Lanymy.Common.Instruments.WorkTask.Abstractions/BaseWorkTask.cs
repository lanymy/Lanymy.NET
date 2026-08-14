using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 定义可启动、可停止、可释放的后台任务生命周期基类。
    /// </summary>
    public abstract class BaseWorkTask : IDisposable
    {
        /// <summary>
        /// 保护运行态切换和释放流程的共享锁。
        /// </summary>
        protected readonly object _Locker = new object();

        private bool _IsRunning = false;
        private bool _IsDisposed = false;

        /// <summary>
        /// 指示当前任务是否处于运行态。
        /// </summary>
        public bool IsRunning
        {
            get { return _IsRunning; }
            //private set
            protected set
            {

                if (_IsRunning == value) return;

                lock (_Locker)
                {
                    if (!_IsRunning.Equals(value))
                    {
                        _IsRunning = value;
                    }
                }

            }
        }

        public bool IsDisposed
        {
            get { return _IsDisposed; }
        }

        /// <summary>
        /// 初始化后台任务。
        /// </summary>
        protected BaseWorkTask()
        {

        }

        /// <summary>
        /// 启动任务。
        /// </summary>
        public async Task StartAsync()
        {
            lock (_Locker)
            {
                if (_IsDisposed || _IsRunning)
                {
                    return;
                }

                _IsRunning = true;
            }

            try
            {
                // 先进入运行态，再把具体启动逻辑下发给子类；若启动失败会在 catch 中回滚。
                await OnStartAsync();
            }
            catch
            {
                lock (_Locker)
                {
                    _IsRunning = false;
                }
                throw;
            }

        }

        protected abstract Task OnStartAsync();

        /// <summary>
        /// 停止任务。
        /// </summary>
        public async Task StopAsync()
        {
            if (!IsRunning)
            {
                return;
            }

            // 先对外宣告停止，避免新的循环继续进入工作态。
            IsRunning = false;

            await OnStopAsync();
        }

        protected abstract Task OnStopAsync();

        protected abstract Task OnDisposeAsync();

        /// <summary>
        /// 释放任务资源。
        /// </summary>
        public void Dispose()
        {
            lock (_Locker)
            {
                if (_IsDisposed)
                {
                    return;
                }

                _IsDisposed = true;
            }

            Exception stopException = null;
            Exception disposeException = null;

            // Dispose 保证按 Stop -> Release 的顺序收口，并在最后统一回抛异常。
            stopException = TaskHelper.TrySyncWait(StopAsync);
            disposeException = TaskHelper.TrySyncWait(OnDisposeAsync);

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

    }
}
