namespace Lanymy.Common.Console.CustomAttributes
{

    /// <summary>
    /// 控制台参数枚举特性
    /// </summary>
    public class ConsoleArgumentEnumAttribute : BaseConsoleArgumentEnumAttribute
    {
        /// <summary>
        /// 控制台参数枚举特性 构造方法
        /// </summary>
        /// <param name="sourceArgumentTitle">原始命令行参数名称</param>
        /// <param name="sourceArgumentDescription">原始命令行参数名称 描述信息</param>
        /// <param name="isRequired">必填项</param>
        public ConsoleArgumentEnumAttribute(string sourceArgumentTitle, string sourceArgumentDescription, bool isRequired = false) : base(sourceArgumentTitle, sourceArgumentDescription, isRequired)
        {

        }

    }

}
