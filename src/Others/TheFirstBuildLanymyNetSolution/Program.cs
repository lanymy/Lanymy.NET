using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.Common;
using Lanymy.Common.ExtensionFunctions;

namespace TheFirstBuildLanymyNetSolution
{



    class Program
    {


        private const string LANYMY_NET_SOLUTION_FILE_FULL_NAME = "Lanymy.NET.sln";
        private const string LANYMY_SHARED_BINARY_ROOT_DIRECTORY_FULL_NAME = "Lanymy.SharedBinary";
        private const string LANYMY_SHARED_BINARY_LANYMY_NET_DIRECTORY_FULL_NAME = "Lanymy.NET";

        static void Main(string[] args)
        {


            //todo: 第二个 参数 0  1  表示 是否开启Debug模式; 0 关闭Debug模式,程序正常执行; 1 开启Debug模式 跳过此程序编译自动执行,方便调试;

            if (args.IfIsNullOrEmpty())
            {
                return;
            }

            string lanymyNetSolutionFileFullPath = Path.Combine(args[0], LANYMY_NET_SOLUTION_FILE_FULL_NAME);
            if (!File.Exists(lanymyNetSolutionFileFullPath))
            {
                return;
            }

            string lanymyNetSolutionRootDirectoryFullPath = Path.GetDirectoryName(lanymyNetSolutionFileFullPath);
            string lanymySharedBinaryRootDirectoryFullPath = Path.Combine(PathHelper.CombineDirectoryRelativePath(@"..\..\"), LANYMY_SHARED_BINARY_ROOT_DIRECTORY_FULL_NAME);
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
