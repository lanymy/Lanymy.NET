using Lanymy.Config.Interfaces;

namespace Lanymy.Config.ConfigModels
{

    /// <summary>
    /// Redis 配置
    /// </summary>
    public class RedisConfig : IRedisConfig
    {

        /// <summary>
        /// 是否开启 Redis 功能
        /// </summary>
        public bool IsEnabled { get; set; } = false;

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Host { get; set; } = "127.0.0.1";

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; } = 6379;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "";

    }

}
