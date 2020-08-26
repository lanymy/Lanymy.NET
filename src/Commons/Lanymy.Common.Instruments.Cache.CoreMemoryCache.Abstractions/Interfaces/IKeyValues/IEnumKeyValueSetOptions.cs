using System;
using Microsoft.Extensions.Caching.Memory;

namespace Lanymy.Common.Instruments.Interfaces.IKeyValues
{
    public interface IEnumKeyValueSetOptions
    {

        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="options">缓存过期参数</param>
        void SetValue<T>(Enum key, T t, MemoryCacheEntryOptions options);

        /// <summary>
        ///创建或者重写缓存项实体实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        ICacheEntry CreateEntry<T>(Enum key, T t);

    }

}
