

using System;
using System.IO;
using Lanymy.Common.ConstKeys;

//using System.Runtime.InteropServices;
//using Lanymy.Common.ConstKeys;

namespace Lanymy.Common
{

    /// <summary>
    /// 驱动辅助类
    /// </summary>
    public class DriversHelper
    {


#if NET48

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

#endif


        /// <summary>
        /// 注册 非托管 DLL (如:驱动文件) 文件夹目录 到 当前应用程序域进程的环境变量中
        /// </summary>
        /// <param name="driverFullPath">非托管 DLL 文件夹目录 支持同时注册多个</param>
        public static void RegisterDriverFullPathToEnvironmentVariables(params string[] driverFullPath)
        {

            var target = EnvironmentVariableTarget.Process;

            var pathValue = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.PATH_KEY, target);

            if (!pathValue.EndsWith(EnvironmentVariableKeys.ENVIRONMENT_VARIABLE_SEPARATOR))
            {
                pathValue += EnvironmentVariableKeys.ENVIRONMENT_VARIABLE_SEPARATOR;
            }

            pathValue += string.Join(EnvironmentVariableKeys.ENVIRONMENT_VARIABLE_SEPARATOR, driverFullPath);

            Environment.SetEnvironmentVariable(EnvironmentVariableKeys.PATH_KEY, pathValue, target);

        }



    }

}


