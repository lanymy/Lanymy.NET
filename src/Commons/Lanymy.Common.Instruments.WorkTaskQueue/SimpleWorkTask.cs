using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lanymy.Common.Instruments
{


    public class SimpleWorkTask : BaseSimpleWorkTask
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="workAction"></param>
        /// <param name="sleepIntervalMilliseconds">小于等于0为执行间隔不休眠,大于0每次执行间隔休眠时间</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SimpleWorkTask(Action<CancellationToken> workAction, int sleepIntervalMilliseconds = 0) : base(workAction, sleepIntervalMilliseconds)
        {

        }



    }

}
