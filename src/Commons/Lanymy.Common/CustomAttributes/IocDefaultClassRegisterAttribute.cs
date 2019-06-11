using Lanymy.Common.Models;

namespace Lanymy.Common.CustomAttributes
{
    /// <summary>
    /// 默认 注入 基类 特性 标记
    /// </summary>
    public class IocDefaultClassRegisterAttribute : IocRegisterAttribute
    {
        public IocDefaultClassRegisterAttribute(string registerName = null) : base(typeof(BaseIocRegister), registerName)
        {
        }
    }
}
