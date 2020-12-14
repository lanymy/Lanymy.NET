using System.ComponentModel;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{

    /// <summary>
    /// 管道事件节点基类
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class BasePipeLineEventHandler<TData> : IPipeLineEventHandler<TData> where TData : class
    {
        /// <summary>
        /// 管道事件节点 执行顺序 排序号 
        /// </summary>
        public abstract int OrderIndex { get; }

        /// <summary>
        /// 管道事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual async Task ProcessAsync(BasePipeLineDataContext<TData> context, CancelEventArgs e)
        {

            if (!e.Cancel)
            {
                await OnProcessAsync(context, e);
            }

            //await Task.CompletedTask;
        }

        /// <summary>
        /// 管道事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected abstract Task OnProcessAsync(BasePipeLineDataContext<TData> context, CancelEventArgs e);

    }

}
