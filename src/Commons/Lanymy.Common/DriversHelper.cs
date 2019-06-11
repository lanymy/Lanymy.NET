using System;
using System.IO;
using System.Runtime.InteropServices;
using Lanymy.Common.ConstKeys;

namespace Lanymy.Common
{
    /// <summary>
    /// 驱动辅助类
    /// </summary>
    public class DriversHelper
    {



        /// <summary>
        /// 动态加载驱动DLL 文件 并返回 此DLL 文件的 句柄
        /// </summary>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        [DllImport(Win32FileKeys.KERNEL32, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadLibrary(string dllPath);


        /// <summary>
        /// 释放驱动DLL文件资源
        /// </summary>
        /// <param name="hDll"></param>
        /// <returns></returns>
        [DllImport(Win32FileKeys.KERNEL32, CharSet = CharSet.Auto)]
        public static extern bool FreeLibrary(IntPtr hDll);


        /// <summary>
        /// 挂载驱动文件 并 返回 句柄
        /// </summary>
        /// <param name="driverLibraryFileFullPath"></param>
        /// <returns></returns>
        public static IntPtr LoadDriverLibraryFile(string driverLibraryFileFullPath)
        {

            var intPtr = IntPtr.Zero;
            try
            {
                if (File.Exists(driverLibraryFileFullPath))
                {
                    intPtr = LoadLibrary(driverLibraryFileFullPath);
                }
            }
            catch
            {
            }

            return intPtr;

        }



    }
}
