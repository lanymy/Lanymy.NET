namespace Lanymy.Common.Instruments
{


    /// <summary>
    /// 默认管道操作器
    /// </summary>
    /// <typeparam name="TData"></typeparam>
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