using System.ComponentModel;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{



    /// <summary>
    /// 管道事件接口
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IPipeLineEventHandler<TData> where TData : class
    {

        /// <summary>
        /// 管道事件节点 执行顺序 排序号 
        /// </summary>
        int OrderIndex { get; }


        /// <summary>
        /// 管道事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task ProcessAsync(BasePipeLineDataContext<TData> context, CancelEventArgs e);

    }

}
