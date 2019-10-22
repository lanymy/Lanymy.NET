using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Lanymy.Common;

namespace Lanymy.Common
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
            return StartProcess(GetProcessStartInfo(applicationFileFullPath, createNoWindow, useShellExecute, args));
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="processStartInfo"></param>
        /// <returns></returns>
        public static bool StartProcess(ProcessStartInfo processStartInfo)
        {

            bool state = false;

            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = processStartInfo;
                    state = process.Start();
                }
            }
            catch
            {
                state = false;
            }

            return state;

        }


        /// <summary>
        /// 打开指定目录
        /// </summary>
        /// <param name="directoryFullPath">目录路径</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool StartExplorerProcess(string directoryFullPath, params string[] args)
        {
            return StartProcess(GetExplorerProcessStartInfo(directoryFullPath, args));
        }

        /// <summary>
        /// 打开指定目录
        /// </summary>
        /// <returns></returns>
        internal static ProcessStartInfo GetExplorerProcessStartInfo(string directoryFullPath, params string[] args)
        {

            //var currentProcess = new Process();
            //var startInfo = new ProcessStartInfo("explorer.exe", applicationFileFullPath);
            //currentProcess.StartInfo = startInfo;
            //currentProcess.StartInfo.UseShellExecute = useShellExecute;
            //return currentProcess;

            var argsList = new List<string>
            {
                directoryFullPath
            };

            if (args.Length > 0)
            {
                argsList.AddRange(args);
            }

            return GetProcessStartInfo("explorer.exe", false, false, argsList.ToArray());

        }


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

            //var currentProcess = new Process();
            //var startInfo = new ProcessStartInfo(applicationFileFullPath, strArgs.Trim());
            //currentProcess.StartInfo = startInfo;
            //currentProcess.StartInfo.UseShellExecute = useShellExecute;

            //return currentProcess;

            return startInfo;

        }


        /// <summary>
        /// 获取 当前进程的位数 x86 还是 x64
        /// </summary>
        /// <returns></returns>
        public static BitOperatingSystemTypeEnum GetCurrentProcessBitOperatingSystemType()
        {

            BitOperatingSystemTypeEnum bitOperatingSystemTypeEnum;

            if (IntPtr.Size == 8)
            {
                bitOperatingSystemTypeEnum = BitOperatingSystemTypeEnum.x64;
            }
            else if (IntPtr.Size == 4)
            {
                bitOperatingSystemTypeEnum = BitOperatingSystemTypeEnum.x86;
            }
            else
            {
                bitOperatingSystemTypeEnum = BitOperatingSystemTypeEnum.UnKnow;
            }

            return bitOperatingSystemTypeEnum;

        }




    }
}
