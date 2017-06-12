/********************************************************************

时间: 2017年06月12日, AM 10:42:04

作者: lanyanmiyu@qq.com

描述: 内存模式 数据缓存 功能 接口

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.Interfaces
{

    /// <summary>
    /// 内存模式 数据缓存 功能 接口
    /// </summary>
    public interface IDataMemoryCache : IKeyValue, IKeyValueExtension, IEnumKeyValue
    {

    }

}
