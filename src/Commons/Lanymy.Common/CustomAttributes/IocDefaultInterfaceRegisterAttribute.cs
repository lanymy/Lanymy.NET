using Lanymy.Common.Interfaces;

namespace Lanymy.Common.CustomAttributes
{
    /// <summary>
    /// 默认 注入 基类接口 特性 标记
    /// </summary>
    public class IocDefaultInterfaceRegisterAttribute : IocRegisterAttribute
    {

        /// <summary>
        /// 默认 注入 基类接口 特性 标记
        /// </summary>
        /// <param name="registerName"></param>
        public IocDefaultInterfaceRegisterAttribute(string registerName = null) : base(typeof(IBaseIocRegister), registerName)
        {

        }

    }

}
