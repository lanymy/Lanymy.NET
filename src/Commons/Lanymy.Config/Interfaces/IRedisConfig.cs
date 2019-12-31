namespace Lanymy.Config.Interfaces
{

    /// <summary>
    /// Redis 相关属性 配置 接口
    /// </summary>
    public interface IRedisConfig
    {

        /// <summary>
        /// 是否开启 Redis 功能
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        string Password { get; set; }

    }

}
