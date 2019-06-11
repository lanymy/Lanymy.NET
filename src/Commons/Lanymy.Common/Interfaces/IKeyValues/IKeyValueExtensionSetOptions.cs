using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace Lanymy.Common.Interfaces.IKeyValues
{

    public interface IKeyValueExtensionSetOptions
    {

        /// <summary>
        /// 使用 默认Key 设置 Key的Value值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="options">缓存过期参数</param>
        void SetValue<T>(T value, MemoryCacheEntryOptions options);


        /// <summary>
        /// 根据 Key 设置 Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options">缓存过期参数</param>
        void SetValue<T>(string key, T value, MemoryCacheEntryOptions options);

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        ICacheEntry CreateEntry<T>(T value);

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ICacheEntry CreateEntry<T>(string key, T value);


    }

}
