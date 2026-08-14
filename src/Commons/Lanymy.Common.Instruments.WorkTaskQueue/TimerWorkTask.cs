using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 面向外部使用的定时工作任务实现。
    /// </summary>
    public class TimerWorkTask : BaseTimerWorkTask
    {
        /// <summary>
        /// 初始化定时任务。
        /// </summary>
        /// <param name="workFunc">每次 tick 执行的逻辑。</param>
        /// <param name="taskSleepMilliseconds">tick 间隔。</param>
        public TimerWorkTask(Func<TimerWorkTaskDataResult> workFunc, int taskSleepMilliseconds = 3000) : base(workFunc, taskSleepMilliseconds)
        {
        }

        protected override async Task OnDisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
