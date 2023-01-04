using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments.Abstractions
{



    /// <summary>
    /// 执行延迟任务抽象基类
    /// </summary>
    public abstract class BaseDelayDoWork : IDelayDoWork, IDisposable
    {


        protected readonly Action _CurrentWorkAction;
        protected readonly Timer _CurrentWorkTimer;
        protected readonly int _CurrentDelayMilliseconds;

        /// <summary>
        /// 执行延迟任务
        /// </summary>
        /// <param name="workAction">任务</param>
        /// <param name="delayMilliseconds">默认延迟时间</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected BaseDelayDoWork(Action workAction, int delayMilliseconds = 3 * 1000)
        {

            if (workAction.IfIsNull())
            {
                throw new ArgumentNullException(nameof(workAction));
            }

            _CurrentDelayMilliseconds = delayMilliseconds;
            _CurrentWorkAction = workAction;
            _CurrentWorkTimer = new Timer(OnWorkAction, null, Timeout.Infinite, Timeout.Infinite);

        }


        /// <summary>
        /// 执行当前任务
        /// </summary>
        /// <param name="state"></param>
        protected virtual void OnWorkAction(object state)
        {
            _CurrentWorkAction();
        }


        /// <summary>
        /// 执行延迟任务
        /// </summary>
        /// <param name="delayMilliseconds">延迟时间</param>
        /// <returns></returns>
        protected virtual bool OnRunDelayWork(int delayMilliseconds)
        {
            return _CurrentWorkTimer.Change(delayMilliseconds, Timeout.Infinite);
        }


        /// <summary>
        /// 延迟执行任务
        /// </summary>
        /// <param name="delayMilliseconds">延迟时间</param>
        /// <returns></returns>
        public bool RunDelayWork(int delayMilliseconds)
        {
            return OnRunDelayWork(delayMilliseconds);
        }

        /// <summary>
        /// 延迟执行任务
        /// </summary>
        /// <returns></returns>
        public bool RunDelayWork()
        {
            return RunDelayWork(_CurrentDelayMilliseconds);
        }


        public void Dispose()
        {
            _CurrentWorkTimer.Dispose();
        }


    }

}
