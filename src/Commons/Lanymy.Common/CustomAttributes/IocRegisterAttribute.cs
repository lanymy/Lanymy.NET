using System;

namespace Lanymy.Common.CustomAttributes
{
    /// <summary>
    /// IOC 自动注册 特性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IocRegisterAttribute : Attribute
    {

        /// <summary>
        /// 注册名称标识
        /// </summary>
        public string RegisterName { get; }

        /// <summary>
        /// 注册类型
        /// </summary>
        public Type RegisterType { get; }

        //public InjectionMember[] InjectionMembers { get; }

        /// <summary>
        /// IOC 自动注册 特性标记 构造方法
        /// </summary>
        /// <param name="registerType">注册类型 如果为null 则以自己 为服务类型 注入</param>
        /// <param name="registerName">注册名称标识 如果为null 则表示 不设置</param>
        ///// <param name="injectionMembers"></param>
        //public IocRegisterAttribute(Type registerType, string registerName, InjectionMember[] injectionMembers)
        public IocRegisterAttribute(Type registerType = null, string registerName = null)
        {
            RegisterName = registerName;
            RegisterType = registerType;
            //InjectionMembers = injectionMembers;
        }

    }
}
