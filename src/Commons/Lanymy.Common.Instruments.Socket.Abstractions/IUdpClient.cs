using System;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// UDP 客户端接口。
    /// 对外暴露同步启动、同步/异步发送与同步/异步关闭入口，
    /// 队列 stop/dispose 拆分和关闭失败后的补偿释放由实现层负责保证。
    /// </summary>
    public interface IUdpClient : IDisposable
    {

        /// <summary>
        /// 当前监听端口。
        /// </summary>
        int Port { get; }

        /// <summary>
        /// 当前是否处于接收态。
        /// </summary>
        bool IsAccept { get; }

        /// <summary>
        /// 当前是否已经释放。
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// 同步启动 UDP 客户端。
        /// </summary>
        void Start();

        /// <summary>
        /// 同步发送数据到指定 IP 和端口。
        /// </summary>
        bool Send(byte[] data, string remoteIP, int remotePort);

        /// <summary>
        /// 同步广播发送数据。
        /// </summary>
        bool SendBroadcast(byte[] data, int broadcastPort);

        /// <summary>
        /// 同步发送 UDP 数据模型。
        /// 具体实现可桥接到异步发送链，但失败应由错误通道负责上报。
        /// </summary>
        bool Send(SendUdpDataModel sendUdpDataModel);

        /// <summary>
        /// 异步发送 UDP 数据模型。
        /// </summary>
        Task SendAsync(SendUdpDataModel sendUdpDataModel);

        /// <summary>
        /// 同步关闭 UDP 客户端。
        /// </summary>
        void Close();

        /// <summary>
        /// 异步关闭 UDP 客户端。
        /// </summary>
        Task CloseAsync();


    }

}
