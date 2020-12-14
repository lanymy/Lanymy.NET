using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 管道操作器基类
    /// </summary>
    /// <typeparam name="TData">要加工的数据实体类</typeparam>
    /// <typeparam name="TPipeLineEventHandler">管道事件操作类</typeparam>
    /// <typeparam name="TPipeLineDataContext">管道上下文实体类</typeparam>
    //public abstract class BasePipeLineHandlerContext<TPipeLineDataContext, TData, TPipeLineEventHandler> : IPipeLineHandlerContext<TPipeLineDataContext, TData, TPipeLineEventHandler>
    public abstract class BasePipeLineHandlerContext<TPipeLineDataContext, TData, TPipeLineEventHandler> : IPipeLineHandlerContext<TData, TPipeLineEventHandler>
        where TPipeLineDataContext : BasePipeLineDataContext<TData>//, new()
        where TData : class
        where TPipeLineEventHandler : BasePipeLineEventHandler<TData>
    {

        //public List<BasePipeLineEventHandler<TData>> CurrentPipeLineWorkList { get; set; } = new List<BasePipeLineEventHandler<TData>>();
        /// <summary>
        /// 注册在管道中的事件集合
        /// </summary>
        public List<TPipeLineEventHandler> CurrentPipeLineWorkList { get; set; } = new List<TPipeLineEventHandler>();

        /// <summary>
        /// 当前管道上下文实体类
        /// </summary>
        //public BasePipeLineDataContext<TData> CurrentPipeLineDataContext { get; protected set; }
        //public TPipeLineDataContext CurrentPipeLineDataContext { get; protected set; }

        protected Type CurrentPipeLineDataContextType { get; }

        protected BasePipeLineHandlerContext()
        {
            CurrentPipeLineDataContextType = typeof(TPipeLineDataContext);
        }

        /// <summary>
        /// 执行管道加工
        /// </summary>
        /// <param name="data">要加工的数据实体类</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, object>> InvokeAsync(TData data)
        {


            //CurrentPipeLineDataContext = new PipeLineDataContext<TData>(new Dictionary<string, object>(), data);
            var dicEnvironment = new Dictionary<string, object>();
            //CurrentPipeLineDataContext = CurrentPipeLineDataContextType.Assembly.CreateInstance(CurrentPipeLineDataContextType.FullName, false, BindingFlags.CreateInstance, null, new object[] { dicEnvironment, data }, null, null) as TPipeLineDataContext;
            var pipeLineDataContext = CurrentPipeLineDataContextType.Assembly.CreateInstance(CurrentPipeLineDataContextType.FullName, false, BindingFlags.CreateInstance, null, new object[] { dicEnvironment, data }, null, null) as TPipeLineDataContext;
            var e = new CancelEventArgs(false);

            foreach (var pipeLineEventHandler in CurrentPipeLineWorkList)
            {
                //await pipeLineEventHandler.ProcessAsync(CurrentPipeLineDataContext, e);
                await pipeLineEventHandler.ProcessAsync(pipeLineDataContext, e);
            }

            return await Task.FromResult(dicEnvironment);

        }

        /// <summary>
        /// 自动注册当前管道事件操作基类的 所有管道事件
        /// </summary>
        public virtual void Build()
        {

            if (!CurrentPipeLineWorkList.IfIsNullOrEmpty())
            {
                CurrentPipeLineWorkList.Clear();
            }

            var pipeLineEventHandlerType = typeof(TPipeLineEventHandler);
            var assembly = pipeLineEventHandlerType.Assembly;

            var list = pipeLineEventHandlerType.Assembly.GetTypes()
                .Where(o => o.BaseType == pipeLineEventHandlerType)
                .Select(type => assembly.CreateInstance(type.FullName) as TPipeLineEventHandler)
                //.Cast<BasePipeLineEventHandler<TData>>()
                .OrderBy(o => o.OrderIndex)
                .ToList();

            CurrentPipeLineWorkList.AddRange(list);

            list.Clear();
            list = null;

        }


    }

}