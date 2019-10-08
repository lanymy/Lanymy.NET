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
            /// δ����
            /// </summary>
            UnDefine,

            /// <summary>
            /// δ֪����
            /// </summary>
            UnKnown,

            [ConsoleArgumentEnum("TargetFramework", "$(TargetFramework) Ŀ��ƽ̨�����ʶ��", true)]
            TargetFrameworkArg,


            [ConsoleArgumentEnum("PublishRootDirectoryFullPath", "$(TargetDir) Ŀ����������·��", true)]
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
