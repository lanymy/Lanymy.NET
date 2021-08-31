using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{

    public class TimerWorkTask : BaseTimerWorkTask
    {

        public TimerWorkTask(Func<TimerWorkTaskDataResult> workFunc, int taskSleepMilliseconds = 3000) : base(workFunc, taskSleepMilliseconds)
        {
        }

        protected override async Task OnDisposeAsync()
        {
            await Task.CompletedTask;
        }

    }
}
