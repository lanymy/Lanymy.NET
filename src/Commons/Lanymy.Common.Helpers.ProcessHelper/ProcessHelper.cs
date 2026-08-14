using System;
using System.Diagnostics;
using Lanymy.Common.Enums;
using Lanymy.Common.Helpers.ResultModels;

namespace Lanymy.Common.Helpers
{

    /// <summary>
    /// 进程辅助类
    /// </summary>
    public class ProcessHelper
    {


        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="applicationFileFullPath">应用程序全路径</param>
        /// <param name="createNoWindow">是否 显示启动 进程的界面 True 不显示 ; False 显示</param>
        /// <param name="useShellExecute">该值指示是否使用操作系统 shell 启动进程 默认值 False</param>
        /// <param name="args">启动应用程序 需要 传递的启动参数</param>
        /// <returns></returns>
        public static bool StartProcess(string applicationFileFullPath, bool createNoWindow, bool useShellExecute = false, params string[] args)
        {
            return StartProcessWithResult(applicationFileFullPath, createNoWindow, useShellExecute, args).IsSuccess;
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="processStartInfo"></param>
        /// <returns></returns>
        public static bool StartProcess(ProcessStartInfo processStartInfo)
        {
            return StartProcessWithResult(processStartInfo).IsSuccess;
        }

        /// <summary>
        /// 启动进程，并返回详细结果。
        /// </summary>
        /// <param name="applicationFileFullPath">应用程序全路径</param>
        /// <param name="createNoWindow">是否显示启动进程界面</param>
        /// <param name="useShellExecute">是否使用 shell 启动</param>
        /// <param name="args">启动参数</param>
        /// <returns>进程启动结果</returns>
        public static ProcessResultModel StartProcessWithResult(string applicationFileFullPath, bool createNoWindow, bool useShellExecute = false, params string[] args)
        {
            return StartProcessWithResult(GetProcessStartInfo(applicationFileFullPath, createNoWindow, useShellExecute, args));
        }

        /// <summary>
        /// 启动进程，并返回详细结果。
        /// </summary>
        /// <param name="processStartInfo">进程启动信息</param>
        /// <returns>进程启动结果</returns>
        public static ProcessResultModel StartProcessWithResult(ProcessStartInfo processStartInfo)
        {
            return ExecuteProcess(processStartInfo, false);
        }

        /// <summary>
        /// 启动进程并等待退出，返回退出结果。
        /// </summary>
        /// <param name="applicationFileFullPath">应用程序全路径</param>
        /// <param name="createNoWindow">是否显示启动进程界面</param>
        /// <param name="useShellExecute">是否使用 shell 启动</param>
        /// <param name="args">启动参数</param>
        /// <returns>进程执行结果</returns>
        public static ProcessResultModel RunProcessWithResult(string applicationFileFullPath, bool createNoWindow, bool useShellExecute = false, params string[] args)
        {
            return RunProcessWithResult(GetProcessStartInfo(applicationFileFullPath, createNoWindow, useShellExecute, args));
        }

        /// <summary>
        /// 启动进程并等待退出，返回退出结果。
        /// </summary>
        /// <param name="processStartInfo">进程启动信息</param>
        /// <returns>进程执行结果</returns>
        public static ProcessResultModel RunProcessWithResult(ProcessStartInfo processStartInfo)
        {
            return ExecuteProcess(processStartInfo, true);
        }

        private static ProcessResultModel ExecuteProcess(ProcessStartInfo processStartInfo, bool waitForExit)
        {
            var result = new ProcessResultModel();

            if (processStartInfo == null)
            {
                result.Exception = new ArgumentNullException(nameof(processStartInfo));
                return result;
            }

            result.ApplicationFileFullPath = processStartInfo.FileName;
            result.Arguments = processStartInfo.Arguments;
            result.CreateNoWindow = processStartInfo.CreateNoWindow;
            result.UseShellExecute = processStartInfo.UseShellExecute;
            result.WaitedForExit = waitForExit;

            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = processStartInfo;
                    result.IsStarted = process.Start();

                    if (!result.IsStarted)
                    {
                        result.ErrorMessage = "Process start returned false.";
                        return result;
                    }

                    result.ProcessId = process.Id;

                    if (waitForExit)
                    {
                        process.WaitForExit();
                        result.HasExited = true;
                        result.ExitCode = process.ExitCode;
                        if (result.ExitCode.GetValueOrDefault() != 0)
                        {
                            result.ErrorMessage = $"Process exited with code {result.ExitCode}.";
                        }
                    }
                    else if (TryGetExitCode(process, out var exitCode))
                    {
                        result.HasExited = true;
                        result.ExitCode = exitCode;
                        if (result.ExitCode.GetValueOrDefault() != 0)
                        {
                            result.ErrorMessage = $"Process exited with code {result.ExitCode}.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private static bool TryGetExitCode(Process process, out int exitCode)
        {
            exitCode = default;

            if (process == null)
            {
                return false;
            }

            try
            {
                if (!process.HasExited)
                {
                    return false;
                }

                exitCode = process.ExitCode;
                return true;
            }
            catch
            {
                return false;
            }
        }


        ///// <summary>
        ///// 打开指定目录
        ///// </summary>
        ///// <param name="directoryFullPath">目录路径</param>
        ///// <param name="args"></param>
        ///// <returns></returns>
        //public static bool StartExplorerProcess(string directoryFullPath, params string[] args)
        //{
        //    return StartProcess(GetExplorerProcessStartInfo(directoryFullPath, args));
        //}

        ///// <summary>
        ///// 打开指定目录
        ///// </summary>
        ///// <returns></returns>
        //internal static ProcessStartInfo GetExplorerProcessStartInfo(string directoryFullPath, params string[] args)
        //{

        //    //var currentProcess = new Process();
        //    //var startInfo = new ProcessStartInfo("explorer.exe", applicationFileFullPath);
        //    //currentProcess.StartInfo = startInfo;
        //    //currentProcess.StartInfo.UseShellExecute = useShellExecute;
        //    //return currentProcess;

        //    var argsList = new List<string>
        //    {
        //        directoryFullPath
        //    };

        //    if (args.Length > 0)
        //    {
        //        argsList.AddRange(args);
        //    }

        //    return GetProcessStartInfo("explorer.exe", false, false, argsList.ToArray());

        //}


        /// <summary>
        /// 获取匹配好的进程实体类
        /// </summary>
        /// <param name="applicationFileFullPath">应用程序全路径</param>
        /// <param name="createNoWindow">是否 显示启动 进程的界面 True 不显示 ; False 显示</param>
        /// <param name="useShellExecute">该值指示是否使用操作系统 shell 启动进程 默认值 False</param>
        /// <param name="args">启动应用程序 需要 传递的启动参数</param>
        /// <returns></returns>
        public static ProcessStartInfo GetProcessStartInfo(string applicationFileFullPath, bool createNoWindow, bool useShellExecute = false, params string[] args)
        {

            string strArgs = string.Empty;

            if (args.Length > 0)
            {
                strArgs = string.Join(" ", args);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = applicationFileFullPath,
                Arguments = strArgs,
            };

            if (createNoWindow)
            {

                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            }
            else
            {

                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = useShellExecute;

            }


            return startInfo;

        }


        ///// <summary>
        ///// 获取 当前进程的位数 x86 还是 x64
        ///// </summary>
        ///// <returns></returns>
        public static BitOperatingTypeEnum GetCurrentProcessBitOperatingSystemType()
        {

            return Environment.Is64BitProcess ? BitOperatingTypeEnum.x64 : BitOperatingTypeEnum.x86;

        }




    }
}
