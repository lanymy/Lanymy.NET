using System;

namespace Lanymy.Common.CustomAttributes
{
    /// <summary>
    /// 自定义 属性 特性 枚举基类,用于扩展枚举项的描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public abstract class BaseEnumAttribute : Attribute
    {

    }
}
