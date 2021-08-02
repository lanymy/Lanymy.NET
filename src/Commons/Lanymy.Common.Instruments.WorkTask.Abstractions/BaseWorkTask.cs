using System;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{

    public abstract class BaseWorkTask : IDisposable
    {

        protected readonly object _Locker = new();

        private bool _IsRunning = false;

        public bool IsRunning
        {
            get { return _IsRunning; }
            private set
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

            await OnStartAsync();

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

            //OnStopAsync().Wait();

            OnDisposeAsync().Wait();

        }


    }

}
