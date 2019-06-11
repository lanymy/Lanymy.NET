using System;
using CacheManager.Core;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Interfaces.ICaches;

namespace Lanymy.Common.Instruments.Cache
{


    /// <summary>
    /// CacheManager 缓存操作器
    /// </summary>
    public class CacheManagerCache : BaseCache, ICacheManager
    {


        /// <summary>
        /// 缓存实例名称
        /// </summary>
        public string CacheInstanceName { get; }

        public ICacheManager<object> CurrentCacheManager { get; private set; }

        /// <summary>
        /// CacheManager 缓存操作器 构造方法
        /// </summary>
        /// <param name="cacheInstanceName">缓存实例名称</param>
        /// <param name="settingsAction">配置信息方法 默认值 null 为使用内置默认的参数进行初始化</param>
        public CacheManagerCache(string cacheInstanceName, Action<ConfigurationBuilderCacheHandlePart> settingsAction = null)
        {
            if (cacheInstanceName.IfIsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(cacheInstanceName));
            }

            CacheInstanceName = cacheInstanceName;

            ReconfigCurrentCacheManager(settingsAction);

        }




        /// <summary>
        /// 重新配置 内置的 缓存操作器实例
        /// </summary>
        public void ReconfigCurrentCacheManager(Action<ConfigurationBuilderCacheHandlePart> settingsAction)
        {

            //释放当前 缓存操作器 实例
            if (!CurrentCacheManager.IfIsNullOrEmpty())
            {
                CurrentCacheManager.Dispose();
            }

            CurrentCacheManager = CacheFactory.Build(CacheInstanceName, settings =>
            {
                var configurationBuilderCacheHandlePart = settings
                    .WithJsonSerializer()
                    .WithMicrosoftMemoryCacheHandle();
                settingsAction?.Invoke(configurationBuilderCacheHandlePart);
            });

        }


        /// <summary>
        /// Key是否存在
        /// </summary>
        /// <param name="key">Key值</param>
        /// <returns></returns>
        public override bool IfHaveKey(string key)
        {
            return CurrentCacheManager.Exists(key);
        }

        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void SetValue(string key, object value)
        {
            CurrentCacheManager.Put(key, value);
        }

        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object GetValue(string key)
        {
            return CurrentCacheManager.Get(key);
        }

        /// <summary>
        /// 删除Key
        /// </summary>
        public override void RemoveValue(string key)
        {
            CurrentCacheManager.Remove(key);
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public override void Clear()
        {
            CurrentCacheManager.Clear();
        }


    }
}
