using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{

    public abstract class BaseWorkTask : IDisposable
    {

        protected readonly object _Locker = new object();

        private bool _IsRunning = false;
        private bool _IsDisposed = false;

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


        protected BaseWorkTask()
        {

        }



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


        public async Task StopAsync()
        {


            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;

            await OnStopAsync();



            //try
            //{

            //    await OnStopAsync();

            //}
            //catch (Exception ex)
            //{

            //}
            //finally
            //{
            //    IsRunning = false;
            //}

        }


        protected abstract Task OnStopAsync();


        protected abstract Task OnDisposeAsync();


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
