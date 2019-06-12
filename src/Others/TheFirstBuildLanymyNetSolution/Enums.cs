using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.Console.CustomAttributes;

namespace TheFirstBuildLanymyNetSolution
{

    public enum ConsoleArgumentsTypeEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,



        ///// <summary>
        ///// Lanymy.NET解决方案根文件夹全路径
        ///// </summary>
        //[ConsoleArgumentEnum(nameof(LanymyNetSolutionRootDirectoryFullPath), "Lanymy.NET解决方案根文件夹全路径", true)]
        //LanymyNetSolutionRootDirectoryFullPath,

        ///// <summary>
        ///// 0关闭调试模式;1开启调试模式
        ///// </summary>
        //[ConsoleArgumentEnum(nameof(IsDebugModel), "0关闭调试模式;1开启调试模式", true)]
        //IsDebugModel,

        /// <summary>
        /// 参数实体类JSON字符串
        /// </summary>
        [ConsoleArgumentEnum(nameof(ArgsJson), "参数实体类JSON字符串", true)]
        ArgsJson,


    }


}
