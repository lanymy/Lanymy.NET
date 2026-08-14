using System;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 面向外部使用的简单内存队列实现。
    /// </summary>
    public class SimpleWorkTaskQueue<TData> : BaseSimpleWorkTaskQueue<TData>
    {
        /// <summary>
        /// 初始化简单队列。
        /// </summary>
        /// <param name="workAction">队列元素处理委托。</param>
        /// <param name="sleepIntervalMilliseconds">空轮询间隔。</param>
        public SimpleWorkTaskQueue(Action<TData> workAction, int sleepIntervalMilliseconds = 10) : base(workAction, sleepIntervalMilliseconds)
        {
        }

        protected override async Task OnDisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
