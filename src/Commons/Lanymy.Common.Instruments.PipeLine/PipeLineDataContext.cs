using System.Collections.Generic;

namespace Lanymy.Common.Instruments
{

    /// <summary>
    /// 默认管道上下文实体类
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class PipeLineDataContext<TData> : BasePipeLineDataContext<TData> where TData : class
    {
        /// <summary>
        /// 管道上下文对象基类
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="data"></param>
        public PipeLineDataContext(IDictionary<string, object> environment, TData data) : base(environment, data)
        {
        }
    }
}
