/********************************************************************

时间: 2016年11月09日, PM 03:11:32

作者: lanyanmiyu@qq.com

描述: 进程辅助类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.Models;

namespace Lanymy.General.Extension
{

    /// <summary>
    /// 进程辅助类
    /// </summary>
    public class ProcessFunctions
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

            try
            {
                GetStartProcess(applicationFileFullPath, useShellExecute, args).Start();
                return true;
            }
            catch
            {
                // ignored
            }

            return false;
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

            //if (Environment.Is64BitProcess)
            //{
            //    bitOperatingSystemTypeEnum = BitOperatingSystemTypeEnum.x64;
            //}
            //else
            //{
            //    bitOperatingSystemTypeEnum = BitOperatingSystemTypeEnum.x86;
            //}

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
