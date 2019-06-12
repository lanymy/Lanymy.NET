using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common;
using Lanymy.Common.Console;
using Lanymy.Common.Console.CustomAttributes;
using Lanymy.Common.ExtensionFunctions;
using TheFirstBuildLanymyNetSolution.Models;
using Lanymy.Common.Console.ExtensionFunctions;

namespace TheFirstBuildLanymyNetSolution
{



    class Program
    {


        private const string LANYMY_NET_SOLUTION_FILE_FULL_NAME = "Lanymy.NET.sln";
        private const string LANYMY_SHARED_BINARY_ROOT_DIRECTORY_FULL_NAME = "Lanymy.SharedBinary";
        private const string LANYMY_SHARED_BINARY_LANYMY_NET_DIRECTORY_FULL_NAME = "Lanymy.NET";

        private static string _ErrorMessage = string.Empty;


        static void Main(string[] args)
        {

            ////var aaa = new ConsoleArgumentModel
            ////{
            ////    LanymyNetSolutionRootDirectoryFullPath = "adsf",
            ////    IsDebugModel = true,
            ////};

            ////var json = SerializeHelper.SerializeToJson(aaa);
            //var json = "{'LanymyNetSolutionRootDirectoryFullPath':'adsf','IsDebugModel':0}";

            //var aaaaaaaa = SerializeHelper.DeserializeFromJson<ConsoleArgumentModel>(json);

            //Console.WriteLine(string.Join("|=|", args));

            //Console.WriteLine("aaaa");

            //Console.ReadKey();


            //return;

            _ErrorMessage = string.Empty;

            if (args.IfIsNullOrEmpty())
            {
                return;
            }

            var consoleArgumentMatchResultModel = ConsoleHelper.MatchConsoleArguments<ConsoleArgumentsTypeEnum, ConsoleArgumentEnumAttribute>(args);

            if (consoleArgumentMatchResultModel.IsSuccess)
            {
                OnMainCommand();
            }
            else
            {
                _ErrorMessage = consoleArgumentMatchResultModel.ErrorMessage;
            }


            if (!_ErrorMessage.IfIsNullOrEmpty())
            {

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("报错日志汇总");
                Console.WriteLine();
                Console.WriteLine(_ErrorMessage);
                Console.WriteLine();
                Console.WriteLine("按任意键退出");
                Console.WriteLine();
                Console.ReadKey();

            }



        }

        private static void OnMainCommand()
        {

            var consoleArgumentModel = SerializeHelper.DeserializeFromJson<ConsoleArgumentModel>(ConsoleArgumentsTypeEnum.ArgsJson.GetConsoleInputArgumentData<ConsoleArgumentEnumAttribute>().Replace(@"\", @"\\"));
            var lanymyNetSolutionRootDirectoryFullPath = consoleArgumentModel.LanymyNetSolutionRootDirectoryFullPath;
            var isDebugModel = consoleArgumentModel.IsDebugModel;

            if (isDebugModel)
            {
                return;
            }

            lanymyNetSolutionRootDirectoryFullPath = PathHelper.CombineDirectoryRelativePath(PathHelper.GetCallDomainPath(), @"..\..\..\..\");

            string lanymyNetSolutionFileFullPath = Path.Combine(lanymyNetSolutionRootDirectoryFullPath, LANYMY_NET_SOLUTION_FILE_FULL_NAME);
            if (!File.Exists(lanymyNetSolutionFileFullPath))
            {
                return;
            }

            string lanymySharedBinaryRootDirectoryFullPath = Path.Combine(PathHelper.CombineDirectoryRelativePath(lanymyNetSolutionRootDirectoryFullPath, @"..\..\"), LANYMY_SHARED_BINARY_ROOT_DIRECTORY_FULL_NAME);
            string lanymySharedBinaryLanymyNetDirectoryFullPath = Path.Combine(lanymySharedBinaryRootDirectoryFullPath, "src", LANYMY_SHARED_BINARY_LANYMY_NET_DIRECTORY_FULL_NAME);

            var targetFrameworks = new[]
            {
                "netcoreapp2.2",
                "net472",
                "netstandard2.0",
            };

            var lanymyProjectList = PathHelper.GetFilesFromFolder(lanymyNetSolutionRootDirectoryFullPath, "Lanymy.*.csproj");

        }


    }


}
