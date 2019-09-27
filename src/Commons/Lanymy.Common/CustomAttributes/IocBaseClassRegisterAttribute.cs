using System;

namespace Lanymy.Common.CustomAttributes
{

    /// <summary>
    /// 注册 基类 特性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class IocBaseClassRegisterAttribute : Attribute
    {

    }

}
