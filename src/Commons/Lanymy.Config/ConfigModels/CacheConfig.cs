namespace Lanymy.Config.ConfigModels
{



    /// <summary>
    /// 缓存配置
    /// </summary>
    public class CacheConfig
    {

        /// <summary>
        /// 内存 相对 过期时间 默认值 10 分钟
        /// </summary>
        public int MemoryCacheExpirationTimeSeconds { get; set; } = 60 * 10;
        /// <summary>
        /// Redis 相对 过期时间 默认值 一天 分钟
        /// </summary>
        public int RedisCacheExpirationTimeSeconds { get; set; } = 60 * 60 * 24;

        /// <summary>
        /// 最大重试次数 默认值 50
        /// </summary>
        public int MaxRetries { get; set; } = 50;

        /// <summary>
        /// 默认 重试 超时 时间 默认值
        /// </summary>
        public int RetryTimeoutSeconds { get; set; } = 5;

        /// <summary>
        /// Redis 配置
        /// </summary>
        public RedisConfig RedisConfig { get; set; } = new RedisConfig();

    }


}

