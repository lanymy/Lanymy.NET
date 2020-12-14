using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{
    //public interface IPipeLineHandlerContext<TPipeLineDataContext, TData, TPipeLineEventHandler>
    public interface IPipeLineHandlerContext<TData, TPipeLineEventHandler>
        //where TPipeLineDataContext : BasePipeLineDataContext<TData>
        where TData : class
        where TPipeLineEventHandler : BasePipeLineEventHandler<TData>
    {


        /// <summary>
        /// 当前管道所有事件集合
        /// </summary>
        List<TPipeLineEventHandler> CurrentPipeLineWorkList { get; set; }

        /// <summary>
        /// 执行当前管道
        /// </summary>
        /// <param name="data">管道要处理加工的数据实体类</param>
        /// <returns>返回当前管道的字典环境变量集合</returns>
        Task<Dictionary<string, object>> InvokeAsync(TData data);


        /// <summary>
        /// 编译当前管道
        /// </summary>
        void Build();

    }
}
