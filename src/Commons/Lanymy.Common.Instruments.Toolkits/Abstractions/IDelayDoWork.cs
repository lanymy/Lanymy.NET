using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lanymy.Common.Instruments.Abstractions
{


    public interface IDelayDoWork
    {

        /// <summary>
        /// 延迟执行任务
        /// </summary>
        /// <param name="delayMilliseconds">延迟时间</param>
        /// <returns></returns>
        bool RunDelayWork(int delayMilliseconds);


        /// <summary>
        /// 延迟执行任务
        /// </summary>
        /// <returns></returns>
        bool RunDelayWork();

    }

}
