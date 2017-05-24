/********************************************************************

时间: 2016年11月09日, PM 04:49:46

作者: lanyanmiyu@qq.com

描述: 版本号辅助方法

其它:     

********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Models;

namespace Lanymy.General.Extension
{


    /// <summary>
    /// 版本号辅助方法
    /// </summary>
    public class VersionFunctions
    {

        /// <summary>
        /// 获取程序集版本号
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Version GetAssemblyVersion(Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            var appVersion = assemblyName.Version;

            return appVersion;
        }


        /// <summary>
        /// 获取 调用当前正在执行的 方法的 方法的 版本号
        /// </summary>
        /// <returns></returns>
        public static Version GetCallingAssemblyVersion()
        {
            return GetAssemblyVersion(Assembly.GetCallingAssembly());
        }


        /// <summary>
        /// 获取文件字符串形式的版本号
        /// </summary>
        /// <param name="fileFullPath">文件全路径</param>
        /// <returns></returns>
        public static string GetFileVersionString(string fileFullPath)
        {
            return CustomFileInfo.GetCustomFileInfo(fileFullPath).FileVersionString;
        }

        /// <summary>
        /// 获取文件版本号
        /// </summary>
        /// <param name="fileFullPath">文件全路径</param>
        /// <returns></returns>
        public static Version GetFileVersion(string fileFullPath)
        {
            string versionString = GetFileVersionString(fileFullPath);
            return versionString.IfIsNullOrEmpty() ? null : new Version(versionString);
        }


    }


}
