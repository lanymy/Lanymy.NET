using Lanymy.Common.Console.CustomAttributes;

namespace AutoCopyReleaseDll
{

    public enum ConsoleArgumentsTypeEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,


        /// <summary>
        /// 参数实体类JSON字符串
        /// </summary>
        [ConsoleArgumentEnum(nameof(ArgsJson), "参数实体类JSON字符串", true)]
        ArgsJson,


    }


}
