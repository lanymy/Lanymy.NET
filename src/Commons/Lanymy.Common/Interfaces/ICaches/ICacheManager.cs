using CacheManager.Core;

namespace Lanymy.Common.Interfaces.ICaches
{

    /// <summary>
    /// CacheManager 组件 功能 接口
    /// </summary>
    public interface ICacheManager : ICustomMemoryCache
    {

        ICacheManager<object> CurrentCacheManager { get; }

    }

}
