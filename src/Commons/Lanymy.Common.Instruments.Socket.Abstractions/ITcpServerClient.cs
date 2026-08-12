using System.Net;
using System.Threading.Tasks;
using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{

    /// <summary>
    /// TCP 服务端单连接接口。
    /// 事件用于对外通知业务状态，具体实现需保证事件异常不会直接打断核心收发或关闭链。
    /// </summary>
    public interface ITcpServerClient : IBaseTcpServerClient, ITcp
    {

        /// <summary>
        /// 子连接错误通知事件。
        /// </summary>
        event TcpServerClientErrorEvent ServerClientErrorEvent;

        /// <summary>
        /// 原始收包数据通知事件。
        /// </summary>
        event TcpReceiveDataEvent ReceiveDataEvent;

        /// <summary>
        /// 开始接收数据通知事件。
        /// </summary>
        event TcpStartReceiveEvent StartReceiveEvent;

        /// <summary>
        /// 关闭通知事件。
        /// </summary>
        event TcpCloseEvent CloseEvent;

        /// <summary>
        /// 心跳通知事件。
        /// </summary>
        event TcpHeartEvent HeartEvent;

        /// <summary>
        /// 同步发送数据。
        /// 具体实现可桥接到异步发送链，但桥接失败应进入错误通道。
        /// </summary>
        void Send(byte[] sendDataBytes);

        /// <summary>
        /// 异步发送数据。
        /// </summary>
        Task SendAsync(byte[] sendDataBytes);

    }

}
