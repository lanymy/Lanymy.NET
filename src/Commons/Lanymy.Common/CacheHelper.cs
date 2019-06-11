using Lanymy.Common.Instruments.Cache;
using Lanymy.Common.Interfaces.ICaches;

namespace Lanymy.Common
{


    /// <summary>
    /// 数据缓存 辅助类
    /// </summary>
    public class CacheHelper
    {


        private static readonly object _Locker = new object();


        #region 自定义 内存模式数据缓存类 无过期参数(依赖项,过期时间等)  

        private static ICustomMemoryCache _CustomMemoryCache = null;


        /// <summary>
        /// 自定义 内存模式数据缓存类 无过期参数(依赖项,过期时间等)
        /// </summary>
        public static ICustomMemoryCache CustomMemoryCacheInstance()
        {

            if (null == _CustomMemoryCache)
            {
                lock (_Locker)
                {
                    if (null == _CustomMemoryCache)
                    {
                        _CustomMemoryCache = new CustomMemoryCache();
                    }
                }
            }

            return _CustomMemoryCache;
        }


        #endregion



        #region 微软原生内存缓存类 操作器 支持过期参数(依赖项,过期时间等)


        private static ICoreMemoryCache _CoreMemoryCache = null;


        /// <summary>
        /// 自定义 内存模式数据缓存类 无过期参数(依赖项,过期时间等)
        /// </summary>
        public static ICoreMemoryCache CoreMemoryCacheInstance()
        {

            if (null == _CoreMemoryCache)
            {
                lock (_Locker)
                {
                    if (null == _CoreMemoryCache)
                    {
                        _CoreMemoryCache = new CoreMemoryCache();
                    }
                }
            }

            return _CoreMemoryCache;
        }


        #endregion


        #region CacheManager 分布式缓存组件 单例 通过Redis支持 分布式同步缓存 策略 (通过配置表 支持超时时间)

        private static ICacheManager _CacheManagerCache = null;


        /// <summary>
        /// 自定义 内存模式数据缓存类 无过期参数(依赖项,过期时间等)
        /// </summary>
        public static ICacheManager CacheManagerCacheInstance()
        {

            if (null == _CacheManagerCache)
            {
                lock (_Locker)
                {
                    if (null == _CacheManagerCache)
                    {
                        _CacheManagerCache = new CacheManagerCache(nameof(CacheManagerCacheInstance));
                    }
                }
            }

            return _CacheManagerCache;
        }


        #endregion


    }


}
