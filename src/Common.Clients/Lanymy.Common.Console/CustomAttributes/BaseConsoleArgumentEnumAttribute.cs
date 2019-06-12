using Lanymy.Common.Console.Models;
using Lanymy.Common.CustomAttributes;

namespace Lanymy.Common.Console.CustomAttributes
{


    /// <summary>
    /// 控制台参数枚举特性 基类
    /// </summary>
    public abstract class BaseConsoleArgumentEnumAttribute : BaseEnumAttribute
    {

        /// <summary>
        /// 原始命令行参数名称
        /// </summary>
        public string SourceArgumentTitle { get; }

        /// <summary>
        /// 原始命令行参数名称 描述信息
        /// </summary>
        public string SourceArgumentDescription { get; }

        /// <summary>
        /// 必填项
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// 控制台 命令行参数 元数据 实体类
        /// </summary>
        public BaseConsoleArgumentModel ConsoleArgumentModel { get; set; }

        /// <summary>
        /// 控制台参数枚举特性 基类 构造方法
        /// </summary>
        /// <param name="sourceArgumentTitle">原始命令行参数名称</param>
        /// <param name="sourceArgumentDescription">原始命令行参数名称 描述信息</param>
        /// <param name="isRequired">必填项</param>
        protected BaseConsoleArgumentEnumAttribute(string sourceArgumentTitle, string sourceArgumentDescription, bool isRequired = false)
        {
            SourceArgumentTitle = sourceArgumentTitle;
            SourceArgumentDescription = sourceArgumentDescription;
            IsRequired = isRequired;
        }

    }

}
