using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCopyReleaseDll.Models;
using Lanymy.Common;
using Lanymy.Common.Console;
using Lanymy.Common.Console.CustomAttributes;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Console.ExtensionFunctions;
using Lanymy.Common.Models;

namespace AutoCopyReleaseDll
{

    class Program
    {

        private static string _ErrorMessage = string.Empty;


        static void Main(string[] args)
        {

            _ErrorMessage = string.Empty;

            if (args.IfIsNullOrEmpty())
            {
                return;
            }

            //args = new[]
            //{
            //    "-ArgsJson",
            //    "{'ProjectFileFullName':'AutoCopyReleaseDll.exe','TargetDirFullPath':'c:\\','AutoCopyDirFullPath':'c:\\','IgnoreFolderNames':'a;b','IsDebugModel':1}",
            //};

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

            //xcopy "$(TargetDir)..\$(ConfigurationName)" "$(SolutionDir)..\Build\$(ProjectName)\" /e /d /y

            var consoleArgumentModel = SerializeHelper.DeserializeFromJson<ConsoleArgumentModel>(ConsoleArgumentsTypeEnum.ArgsJson.GetConsoleInputArgumentData<ConsoleArgumentEnumAttribute>().Replace(@"\", @"\\"));

            var projectFileFullName = consoleArgumentModel.ProjectFileFullName;
            var projectXmlFileFullName = string.Format("{0}{1}", Path.GetFileNameWithoutExtension(projectFileFullName), ".xml");
            var targetDirFullPath = consoleArgumentModel.TargetDirFullPath;
            var autoCopyDirFullPath = consoleArgumentModel.AutoCopyDirFullPath;
            var ignoreFolderNameList = consoleArgumentModel.IgnoreFolderNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            targetDirFullPath = PathHelper.CombineDirectoryRelativePath(targetDirFullPath);
            autoCopyDirFullPath = PathHelper.CombineDirectoryRelativePath(autoCopyDirFullPath);

            ShowParameterValueMessage(nameof(projectFileFullName), projectFileFullName);
            ShowParameterValueMessage(nameof(projectXmlFileFullName), projectXmlFileFullName);
            ShowParameterValueMessage(nameof(targetDirFullPath), targetDirFullPath);
            ShowParameterValueMessage(nameof(autoCopyDirFullPath), autoCopyDirFullPath);
            ShowParameterValueMessage(nameof(ignoreFolderNameList), string.Join(";", ignoreFolderNameList));

            //for (int i = 0; i < 20; i++)
            //{

            //    var a1 = Path.GetRandomFileName();

            //    Console.WriteLine(string.Format("[ {0} ]-[ {1} ]", a1.Length, a1));

            //}

            var folderNameList = Directory.GetDirectories(targetDirFullPath).Select(o => new DirectoryInfo(o).Name).Where(o => !ignoreFolderNameList.Contains(o)).ToList();

            ShowParameterValueMessage(nameof(folderNameList), string.Join(";", folderNameList));

            var scheduleFileList = new List<ScheduleFileInfoModel>();

            foreach (var folderName in folderNameList)
            {

                var scheduleFileInfoModel = new ScheduleFileInfoModel
                {
                    SourceFileFullPath = Path.Combine(targetDirFullPath, folderName, projectFileFullName),
                    TargetFileFullPath = Path.Combine(autoCopyDirFullPath, folderName, projectFileFullName),
                };

                if (File.Exists(scheduleFileInfoModel.SourceFileFullPath))
                {
                    scheduleFileList.Add(scheduleFileInfoModel);
                }

            }

            foreach (var folderName in folderNameList)
            {

                var scheduleFileInfoModel = new ScheduleFileInfoModel
                {
                    SourceFileFullPath = Path.Combine(targetDirFullPath, folderName, projectXmlFileFullName),
                    TargetFileFullPath = Path.Combine(autoCopyDirFullPath, folderName, projectXmlFileFullName),
                };

                if (File.Exists(scheduleFileInfoModel.SourceFileFullPath))
                {
                    scheduleFileList.Add(scheduleFileInfoModel);
                }

            }


            foreach (var scheduleFileInfoModel in scheduleFileList)
            {
                Console.WriteLine(string.Format("[ {0} ] - [ {1} ] - [ {2} ]", Path.GetFileName(scheduleFileInfoModel.SourceFileFullPath), scheduleFileInfoModel.SourceFileFullPath, scheduleFileInfoModel.TargetFileFullPath));
                Console.WriteLine();
            }


            if (consoleArgumentModel.IsDebugModel)
            {
                Console.ReadKey();
            }
            else
            {
                FileHelper.CopyFiles(scheduleFileList);
            }


        }


        private static void ShowParameterValueMessage(string parameterName, string parameterValue)
        {
            Console.WriteLine();
            Console.WriteLine(string.Format("[ {0} ] - [ {1} ]", parameterName, parameterValue));
            Console.WriteLine();
        }


    }

}
