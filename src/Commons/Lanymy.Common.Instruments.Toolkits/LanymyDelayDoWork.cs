using System;
using System.Threading;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Abstractions;

namespace Lanymy.Common.Instruments
{


    /// <summary>
    /// 执行延迟任务
    /// </summary>
    public class LanymyDelayDoWork : BaseDelayDoWork
    {

        /// <summary>
        /// 执行延迟任务
        /// </summary>
        /// <param name="workAction">任务</param>
        /// <param name="delayMilliseconds">默认延迟时间</param>
        /// <exception cref="ArgumentNullException"></exception>
        public LanymyDelayDoWork(Action workAction, int delayMilliseconds = 3 * 1000) : base(workAction, delayMilliseconds)
        {

        }

    }

}
