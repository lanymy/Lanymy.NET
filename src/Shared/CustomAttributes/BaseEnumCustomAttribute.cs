/********************************************************************

时间: 2017年08月16日, PM 02:04:19

作者: lanyanmiyu@qq.com

描述: 枚举特性基类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.General.Extension.CustomAttributes
{



    /// <summary>
    /// 枚举特性基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public abstract class BaseEnumCustomAttribute : Attribute
    {

    }


}
