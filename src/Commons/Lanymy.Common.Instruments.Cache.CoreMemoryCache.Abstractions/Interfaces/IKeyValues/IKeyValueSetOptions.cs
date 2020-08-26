using Microsoft.Extensions.Caching.Memory;

namespace Lanymy.Common.Instruments.Interfaces.IKeyValues
{

    public interface IKeyValueSetOptions
    {

        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options">缓存过期参数</param>
        void SetValue(string key, object value, MemoryCacheEntryOptions options);

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ICacheEntry CreateEntry(object key);

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ICacheEntry CreateEntry(string key);

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ICacheEntry CreateEntry(string key, object value);


    }

}
