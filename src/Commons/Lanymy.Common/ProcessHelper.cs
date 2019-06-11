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
        /// 启动一个进程
        /// </summary>
        /// <param name="applicationFileFullPath">应用程序全路径</param>
        /// <param name="useShellExecute">该值指示是否使用操作系统 shell 启动进程 默认值 False</param>
        /// <param name="args">要传递的启动参数</param>
        /// <returns></returns>
        public static bool StartProcess(string applicationFileFullPath, bool useShellExecute = false, params string[] args)
        {
            bool state = false;
            try
            {
                state = GetStartProcess(applicationFileFullPath, useShellExecute, args).Start();
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
        /// <param name="applicationFileFullPath">目录路径</param>
        /// <param name="useShellExecute">该值指示是否使用操作系统 shell 启动进程 默认值 False</param>
        /// <returns></returns>
        public static bool StartPathProcess(string applicationFileFullPath, bool useShellExecute = false)
        {
            bool state = false;
            try
            {
                state = GetStartPathProcess(applicationFileFullPath, useShellExecute).Start();
            }
            catch
            {
                state = false;
            }

            return state;
        }


        /// <summary>
        /// 获取匹配好的进程实体类
        /// </summary>
        /// <param name="applicationFileFullPath">应用程序全路径</param>
        /// <param name="useShellExecute">该值指示是否使用操作系统 shell 启动进程 默认值 False</param>
        /// <param name="args">启动应用程序 需要 传递的启动参数</param>
        /// <returns></returns>
        public static Process GetStartProcess(string applicationFileFullPath, bool useShellExecute = false, params string[] args)
        {
            string strArgs = string.Empty;

            if (args.Length > 0)
            {
                strArgs = string.Join(" ", args);
            }

            var currentProcess = new Process();
            var startInfo = new ProcessStartInfo(applicationFileFullPath, strArgs.Trim());
            currentProcess.StartInfo = startInfo;
            currentProcess.StartInfo.UseShellExecute = useShellExecute;

            return currentProcess;
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

        /// <summary>
        /// 打开指定目录
        /// </summary>
        /// <param name="applicationFileFullPath">目录路径</param>
        /// <param name="useShellExecute">该值指示是否使用操作系统 shell 启动进程 默认值 False</param>
        /// <returns></returns>
        private static Process GetStartPathProcess(string applicationFileFullPath, bool useShellExecute = false)
        {
            var currentProcess = new Process();
            var startInfo = new ProcessStartInfo("explorer.exe", applicationFileFullPath);
            currentProcess.StartInfo = startInfo;
            currentProcess.StartInfo.UseShellExecute = useShellExecute;
            return currentProcess;
        }


    }
}
