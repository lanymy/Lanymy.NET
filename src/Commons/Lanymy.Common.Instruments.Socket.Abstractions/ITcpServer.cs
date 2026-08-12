using Lanymy.Common.Instruments.Common;

namespace Lanymy.Common.Instruments
{


    /// <summary>
    /// TCP 服务端接口。
    /// 对外暴露同步启动与同步/异步关闭入口，
    /// 子连接管理、启动回滚和全局关闭补偿由实现层负责保证。
    /// </summary>
    public interface ITcpServer : IBaseTcpServer, ITcp
    {

        /// <summary>
        /// 同步启动服务端监听。
        /// 如果启动过程中被关闭或初始化失败，具体实现应负责回滚本轮启动。
        /// </summary>
        void Start();

    }

}
