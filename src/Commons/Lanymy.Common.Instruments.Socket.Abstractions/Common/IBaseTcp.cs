using System;

namespace Lanymy.Common.Instruments.Common
{



    /// <summary>
    /// TCP 基础状态接口。
    /// 只表达当前实例的状态与底层 socket 视图，不直接约束具体生命周期动作。
    /// </summary>
    public interface IBaseTcp
    {

        /// <summary>
        /// 当前底层 socket 视图。
        /// 关闭尾声中该属性可能被实现层置空。
        /// </summary>
        System.Net.Sockets.Socket CurrentSocket { get; }

        /// <summary>
        /// 当前是否连接。
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 当前是否已经释放。
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// 当前接收缓冲区大小。
        /// </summary>
        int ReceiveBufferSize { get; }

        /// <summary>
        /// 当前发送缓冲区大小。
        /// </summary>
        int SendBufferSize { get; }

        /// <summary>
        /// 当前是否处于运行态。
        /// 该状态仅表示生命周期主状态，不等价于所有内部资源都仍然有效。
        /// </summary>
        bool IsRunning { get; }


    }



}
