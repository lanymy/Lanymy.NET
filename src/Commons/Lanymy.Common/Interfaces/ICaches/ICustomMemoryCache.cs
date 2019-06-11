
using Lanymy.Common.Interfaces.IKeyValues;

namespace Lanymy.Common.Interfaces.ICaches
{


    /// <summary>
    /// 内存模式 数据缓存 功能 接口 无过期参数(依赖项,过期时间等)
    /// </summary>
    public interface ICustomMemoryCache : IKeyValue, IKeyValueExtension, IEnumKeyValue
    {

    }


}
