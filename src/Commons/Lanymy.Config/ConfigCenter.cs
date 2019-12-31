//using System;
//using CacheManager.Core;

//namespace Lanymy.Config
//{

//    public class ConfigCenter
//    {

//        private static readonly object _Locker = new object();

//        #region  框架 全局配置表 单例


//        private static Config.ConfigModels.Config _Config = null;

//        public static Config.ConfigModels.Config ConfigInstance()
//        {

//            if (null == _Config)
//            {
//                lock (_Locker)
//                {
//                    if (null == _Config)
//                    {
//                        try
//                        {

//                            _Config = SerializeHelper.DeserializeFromJsonFile<Config.ConfigModels.Config>(Common.GlobalSettings.ConfigFileFullPath);
//                            if (_Config.IfIsNullOrEmpty())
//                            {
//                                _Config = new Config.ConfigModels.Config();
//                                _Config.SaveConfigFile();
//                                throw new Exception(string.Format("配置表文件 [ {0} ] , 不存在,以默认初始化新的配置表文件", Common.GlobalSettings.ConfigFileFullPath));
//                            }

//                            var cacheManagerCache = (CacheHelper.CacheManagerCacheInstance() as CacheManagerCache);

//                            if (cacheManagerCache.IfIsNullOrEmpty())
//                            {
//                                throw new Exception(string.Format("内置缓存操作器[ {0} ],初始化失败", nameof(CacheHelper.CacheManagerCacheInstance)));
//                            }

//                            cacheManagerCache.ReconfigCurrentCacheManager(settings =>
//                            {
//                                var cacheConfig = _Config.CacheConfig;
//                                settings.WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(cacheConfig.MemoryCacheExpirationTimeSeconds));
//                                if (cacheConfig.RedisConfig.IsEnabled)
//                                {

//                                    const string redisConfigurationKey = "redis";

//                                    settings.And
//                                        .WithRedisConfiguration(redisConfigurationKey, config =>
//                                        {

//                                            var redisConfig = cacheConfig.RedisConfig;
//                                            var redisConfigurationBuilder = config.WithAllowAdmin()
//                                                  .WithDatabase(0)
//                                                  .WithEndpoint(redisConfig.Host, redisConfig.Port);
//                                            if (!redisConfig.Password.IfIsNullOrEmpty())
//                                            {
//                                                redisConfigurationBuilder.WithPassword(redisConfig.Password);
//                                            }

//                                        })
//                                        .WithMaxRetries(cacheConfig.MaxRetries)
//                                        .WithRetryTimeout(cacheConfig.RetryTimeoutSeconds * 1000)
//                                        .WithRedisBackplane(redisConfigurationKey)
//                                        .WithRedisCacheHandle(redisConfigurationKey, true)
//                                        .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(cacheConfig.RedisCacheExpirationTimeSeconds));

//                                }
//                            });

//                        }
//                        catch (Exception e)
//                        {
//                            throw new Exception(Common.GlobalSettings.ConfigFileFullPath + " 全局配置表挂载失败!", e);
//                        }
//                    }
//                }
//            }

//            return _Config;

//        }

//        #endregion

//    }

//}
