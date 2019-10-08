using Lanymy.Common.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Lanymy.Common.Console.CustomAttributes;
using Lanymy.Common.Console.ExtensionFunctions;

namespace Lanymy.Common.Console.Core.UnitTests
{

    [TestClass]
    public class ConsoleHelperTests
    {

        public enum ConsoleArgumentTypeEnum
        {

            /// <summary>
            /// 未定义
            /// </summary>
            UnDefine,

            /// <summary>
            /// 未知类型
            /// </summary>
            UnKnown,

            [ConsoleArgumentEnum("TargetFramework", "$(TargetFramework) 目标平台编译标识符", true)]
            TargetFrameworkArg,


            [ConsoleArgumentEnum("PublishRootDirectoryFullPath", "$(TargetDir) 目标编译输出的路径", true)]
            PublishRootDirectoryFullPathArg,


        }


        [TestMethod]
        public void ConsoleHelperTest()
        {

            var args = new[]
            {

                "-TargetFramework",
                "netstandard2.0",
                "-PublishRootDirectoryFullPath",
                @"E:\Lanymy.NET\src\Commons\Lanymy.Common\bin\Debug\netstandard2.0""",

            };

            var resultModel = ConsoleHelper.MatchConsoleArguments<ConsoleArgumentTypeEnum, ConsoleArgumentEnumAttribute>(args);

            var targetFramework = ConsoleArgumentTypeEnum.TargetFrameworkArg.GetConsoleInputArgumentData<ConsoleArgumentEnumAttribute>();
            var publishRootDirectoryFullPath = ConsoleArgumentTypeEnum.PublishRootDirectoryFullPathArg.GetConsoleInputArgumentData<ConsoleArgumentEnumAttribute>();


        }


    }
}
