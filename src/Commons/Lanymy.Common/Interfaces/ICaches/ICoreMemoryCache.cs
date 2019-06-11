
using Lanymy.Common.Interfaces.IKeyValues;
using Microsoft.Extensions.Caching.Memory;

namespace Lanymy.Common.Interfaces.ICaches
{


    /// <summary>
    /// 内存模式 数据缓存 功能 接口 支持过期参数(依赖项,过期时间等)
    /// </summary>
    public interface ICoreMemoryCache : ICustomMemoryCache, IEnumKeyValueSetOptions, IKeyValueExtensionSetOptions, IKeyValueSetOptions
    {
        Microsoft.Extensions.Caching.Memory.IMemoryCache CurrentMemoryCache { get; }

    }

}
