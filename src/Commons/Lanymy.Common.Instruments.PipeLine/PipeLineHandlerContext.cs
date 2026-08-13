namespace Lanymy.Common.Instruments
{


    /// <summary>
    /// 默认管道操作器
    /// </summary>
    /// <typeparam name="TPipeLineDataContext">管道数据上下文类型。</typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TPipeLineEventHandler">管道事件处理器类型。</typeparam>
    public class PipeLineHandlerContext<TPipeLineDataContext, TData, TPipeLineEventHandler> : BasePipeLineHandlerContext<TPipeLineDataContext, TData, TPipeLineEventHandler>
        where TPipeLineDataContext : BasePipeLineDataContext<TData>//, new()
        where TData : class
        where TPipeLineEventHandler : BasePipeLineEventHandler<TData>
    {



        //public override async Task InvokeAsync(TData data)
        //{

        //    //CurrentPipeLineDataContext = new PipeLineDataContext<TData>();

        //    foreach (var pipeLineEventHandler in CurrentPipeLineWorkList)
        //    {
        //        await pipeLineEventHandler.ProcessAsync();
        //    }

        //}

    }


}
