using System.Collections.Generic;

namespace Lanymy.Common.Instruments
{


    /// <summary>
    /// 管道上下文对象基类
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class BasePipeLineDataContext<TData> where TData : class
    {


        //public readonly IDictionary<string, object> Environment = new Dictionary<string, object>();
        /// <summary>
        /// 管道开放式自定义环境变量
        /// </summary>
        public IDictionary<string, object> Environment { get; }
        /// <summary>
        /// 管道加工数据实体类
        /// </summary>
        public TData Data { get; }
        //public bool IsHandler { get; set; } = false;

        /// <summary>
        /// 管道上下文对象基类
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="data"></param>
        protected BasePipeLineDataContext(IDictionary<string, object> environment, TData data)
        {
            Environment = environment;
            Data = data;
        }


    }

}
