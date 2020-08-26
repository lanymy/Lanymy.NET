using System;
using System.Reflection;
using Lanymy.Common.Abstractions.Models;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Helpers
{
    /// <summary>
    /// 版本号辅助方法
    /// </summary>
    public class VersionHelper
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
        /// 获取 应用程序域 版本号
        /// </summary>
        /// <returns></returns>
        public static Version GetCallDomainAssemblyVersion()
        {
            return GetAssemblyVersion(System.Reflection.Assembly.GetEntryAssembly());
        }


        /// <summary>
        /// 获取 调用当前正在执行的 方法的 版本号
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
