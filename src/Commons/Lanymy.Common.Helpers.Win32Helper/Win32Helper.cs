#if NET48




using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

    /// <summary>
    /// Win32 dll API 引用辅助类库
    /// </summary>
    public class Win32Helper
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


        [DllImport(Win32FileKeys.IPHLPAPI, CharSet = CharSet.Auto)]
        public static extern int GetBestInterface(uint DestAddr, out uint BestIfIndex);


        [DllImport(Win32FileKeys.IPHLPAPI, ExactSpelling = true)]
        public static extern int SendARP(uint DestIP, uint SrcIP, byte[] pMacAddr, ref int PhyAddrLen);



    }

}



#endif