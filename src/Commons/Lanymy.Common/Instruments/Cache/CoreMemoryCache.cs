using System;
using Lanymy.Common.Interfaces.ICaches;
using Microsoft.Extensions.Caching.Memory;


namespace Lanymy.Common.Instruments.Cache
{


    /// <summary>
    /// 微软原生内存缓存类 操作器 支持过期参数(依赖项,过期时间等)
    /// </summary>
    public class CoreMemoryCache : BaseCache, ICoreMemoryCache
    {

        /// <summary>
        /// 当前缓存实例
        /// </summary>
        public IMemoryCache CurrentMemoryCache { get; } = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());


        /// <summary>
        /// Key是否存在
        /// </summary>
        /// <param name="key">Key值</param>
        /// <returns></returns>
        public override bool IfHaveKey(string key)
        {
            return CurrentMemoryCache.TryGetValue(key, out _);
        }


        /// <summary>
        /// 设置Key的Value值 此方法缓存的值为 NeverRemove 永不过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void SetValue(string key, object value)
        {
            SetValue(key, value, new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove));
        }


        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options">缓存过期参数</param>
        public void SetValue(string key, object value, MemoryCacheEntryOptions options)
        {
            CurrentMemoryCache.Set(key, value, options);
        }

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ICacheEntry CreateEntry(object key)
        {
            return CurrentMemoryCache.CreateEntry(key);
        }

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ICacheEntry CreateEntry(string key)
        {
            return CreateEntry(key as object);
        }

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICacheEntry CreateEntry(string key, object value)
        {
            var entry = CreateEntry(key);
            entry.Value = value;
            return entry;
        }

        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="options">缓存过期参数</param>
        public void SetValue<T>(Enum key, T t, MemoryCacheEntryOptions options)
        {
            SetValue(GetEnumDefaultKey<T>(key), t as object, options);
        }

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public ICacheEntry CreateEntry<T>(Enum key, T t)
        {
            return CreateEntry(GetEnumDefaultKey<T>(key), t);
        }

        /// <summary>
        /// 使用 默认Key 设置 Key的Value值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="options">缓存过期参数</param>
        public void SetValue<T>(T value, MemoryCacheEntryOptions options)
        {
            SetValue(GetDefaultKey<T>(), value, options);
        }

        /// <summary>
        /// 根据 Key 设置 Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options">缓存过期参数</param>
        public void SetValue<T>(string key, T value, MemoryCacheEntryOptions options)
        {
            SetValue(key, value as object, options);
        }

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICacheEntry CreateEntry<T>(T value)
        {
            return CreateEntry(GetDefaultKey<T>(), value);
        }

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICacheEntry CreateEntry<T>(string key, T value)
        {
            return CreateEntry(key, value as object);
        }


        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object GetValue(string key)
        {
            CurrentMemoryCache.TryGetValue(key, out object o);
            return o;
        }


        /// <summary>
        /// 删除Key
        /// </summary>
        public override void RemoveValue(string key)
        {
            CurrentMemoryCache.Remove(key);
        }

        /// <summary>
        /// 清空缓存 (只能清除 除 CacheItemPriority.NeverRemove 外的其他缓存项)
        /// </summary>
        public override void Clear()
        {

            (CurrentMemoryCache as MemoryCache).Compact(1);
            //throw new NotSupportedException("此方法不受支持,请通过设置缓存过期参数来控制缓存清除策略");

        }


    }

}
