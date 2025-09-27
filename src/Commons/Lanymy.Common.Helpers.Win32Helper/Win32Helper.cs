#if !NETSTANDARD

using System;
using System.Runtime.InteropServices;
using Lanymy.Common.ConstKeys;

namespace Lanymy.Common.Helpers
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






        [DllImport(Win32FileKeys.KERNEL32)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport(Win32FileKeys.USER32)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT rc);

        [DllImport(Win32FileKeys.USER32)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool repaint);
        public struct RECT { public int left, top, right, bottom; }


        [DllImport(Win32FileKeys.USER32)]
        public static extern int GetSystemMetrics(int nIndex);
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;






    }

}



#endif