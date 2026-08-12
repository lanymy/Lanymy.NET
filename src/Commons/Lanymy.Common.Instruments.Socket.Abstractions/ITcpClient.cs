using System.Threading.Tasks;

namespace Lanymy.Common.Instruments
{


    /// <summary>
    /// 主动连接型 TCP 客户端接口。
    /// 对外暴露同步启动、同步/异步发送与同步/异步关闭入口，
    /// 生命周期回滚、桥接异常收口和资源释放补偿由实现层负责保证。
    /// </summary>
    public interface ITcpClient : ITcp
    {

        /// <summary>
        /// 目标服务端 IP。
        /// </summary>
        string ServerIP { get; }

        /// <summary>
        /// 目标服务端端口。
        /// </summary>
        int Port { get; }

        /// <summary>
        /// 同步发送数据。
        /// 具体实现可桥接到异步发送链，但失败应走错误通道而不是直接裸抛桥接异常。
        /// </summary>
        void Send(byte[] data);

        /// <summary>
        /// 异步发送数据。
        /// </summary>
        Task SendAsync(byte[] data);

        /// <summary>
        /// 同步启动客户端。
        /// 如果启动链中途失败，具体实现应负责回滚已创建的内部资源。
        /// </summary>
        void Start();


    }


}
