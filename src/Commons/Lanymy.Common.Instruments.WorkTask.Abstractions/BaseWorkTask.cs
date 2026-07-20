using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{

    public abstract class BaseWorkTask : IDisposable
    {

        protected readonly object _Locker = new object();

        private bool _IsRunning = false;

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


        protected BaseWorkTask()
        {

        }



        public async Task StartAsync()
        {

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
            Exception stopException = null;
            Exception disposeException = null;

            try
            {
                TaskHelper.SyncWait(StopAsync());
            }
            catch (Exception ex)
            {
                stopException = ex;
            }

            try
            {
                TaskHelper.SyncWait(OnDisposeAsync());
            }
            catch (Exception ex)
            {
                disposeException = ex;
            }

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