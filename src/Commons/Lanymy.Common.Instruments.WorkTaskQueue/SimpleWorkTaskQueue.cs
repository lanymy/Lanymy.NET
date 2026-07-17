using System;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{


    public class SimpleWorkTaskQueue<TData> : BaseSimpleWorkTaskQueue<TData>
    {



        public SimpleWorkTaskQueue(Action<TData> workAction, int sleepIntervalMilliseconds = 10) : base(workAction, sleepIntervalMilliseconds)
        {


        }

        protected override async Task OnDisposeAsync()
        {
            await Task.CompletedTask;
        }

    }

}
