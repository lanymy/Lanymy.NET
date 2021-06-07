using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{

    public abstract class BaseWorkTask : IDisposable
    {

        public bool IsRunning { get; protected set; }

        protected BaseWorkTask()
        {

        }



        public async Task StartAsync()
        {

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

            try
            {

                await OnStopAsync();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsRunning = false;
            }
        }


        protected abstract Task OnStopAsync();


        protected abstract Task OnDisposeAsync();


        public void Dispose()
        {

            OnStopAsync().Wait();

            OnDisposeAsync().Wait();

        }


    }

}
