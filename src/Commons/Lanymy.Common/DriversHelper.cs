#if NET48


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
                    intPtr = Win32Helper.LoadLibrary(driverLibraryFileFullPath);
                }
            }
            catch
            {
            }

            return intPtr;

        }



    }
}


#endif
