using System;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{


    /// <summary>
    /// TCP 基础生命周期接口。
    /// 同时暴露同步与异步关闭入口，具体实现允许用同步桥接异步关闭流程，
    /// 但桥接失败应由实现层转入各自的错误通道，而不是直接打穿调用方。
    /// </summary>
    public interface ITcp : IBaseTcp, IDisposable
    {

        /// <summary>
        /// 同步关闭当前 TCP 对象。
        /// 该入口的具体关闭动作可由实现层桥接到异步流程。
        /// </summary>
        void Close();

        /// <summary>
        /// 异步关闭当前 TCP 对象。
        /// </summary>
        Task CloseAsync();

    }

}
