/********************************************************************

时间: 2016年11月13日, PM 07:22:12

作者: lanyanmiyu@qq.com

描述: Windows服务 操作 辅助类

其它:     

********************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Configuration.Install;
using System.ServiceProcess;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Models;
using System.Management;

namespace Lanymy.General.Extension
{

    /// <summary>
    /// Windows服务 操作 辅助类
    /// </summary>
    public class WindowsServiceFunctions
    {


        /// <summary>  
        /// 获取服务的路径  
        /// </summary>  
        /// <param name="serviceName">服务名称</param>  
        /// <returns></returns>  
        public static string GetWindowsServiceInstallPath(string serviceName)
        {

            try
            {
                string key = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
                string path = Registry.LocalMachine.OpenSubKey(key).GetValue("ImagePath").ToString();
                //替换掉双引号    
                path = path.Replace("\"", string.Empty);

                FileInfo fi = new FileInfo(path);
                return fi.Directory.ToString();
            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>  
        /// 根据服务名称获取服务状态。  
        /// </summary>  
        /// <param name="serviceName">服务名</param>  
        /// <returns>状态</returns>  
        public static ServiceStatusEnum GetServiceStatus(string serviceName)
        {
            ServiceStatusEnum status = ServiceStatusEnum.UnDefine;

            if (serviceName.IfIsNullOrEmpty()) return status;

            serviceName = serviceName.ToLower();
            var serviceController = ServiceController.GetServices().Where(o => o.ServiceName.ToLower() == serviceName).FirstOrDefault();

            if (serviceController.IfIsNullOrEmpty()) return status;

            status = serviceController.Status.ToString().ConvertToEnum<ServiceStatusEnum>();

            return status;

        }

        /// <summary>  
        /// 检查服务是否存在  
        /// </summary>  
        /// <param name="serviceName">服务名</param>  
        /// <returns>存在返回 true,否则返回 false;</returns>  
        public static bool WindowsServiceIsExists(string serviceName)
        {

            if (serviceName.IfIsNullOrEmpty()) return false;
            serviceName = serviceName.ToLower();
            var serviceController = ServiceController.GetServices().Where(o => o.ServiceName.ToLower() == serviceName).FirstOrDefault();
            return !serviceController.IfIsNullOrEmpty();
            //foreach (ServiceController s in services)
            //{
            //    if (s.ServiceName.ToLower() == serviceName.ToLower())
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }

        /// <summary>  
        /// 安装Windows服务  
        /// </summary>  
        /// <param name="stateSaver">集合</param>  
        /// <param name="windowsServiceFilePath">程序文件路径</param>  
        public static void InstallWindowsService(IDictionary stateSaver, string windowsServiceFilePath)
        {

            if (stateSaver.IfIsNullOrEmpty()) return;
            if (!File.Exists(windowsServiceFilePath)) return;

            using (var assemblyInstaller = new AssemblyInstaller())
            {
                assemblyInstaller.UseNewContext = true;
                assemblyInstaller.Path = windowsServiceFilePath;
                assemblyInstaller.Install(stateSaver);
                assemblyInstaller.Commit(stateSaver);
            }

        }
        /// <summary>  
        /// 卸载Windows服务  
        /// </summary>  
        /// <param name="windowsServiceFilePath">程序文件路径</param>  
        public static void UnInstallWindowsService(string windowsServiceFilePath)
        {
            if (!File.Exists(windowsServiceFilePath)) return;
            using (var assemblyInstaller = new AssemblyInstaller())
            {
                assemblyInstaller.UseNewContext = true;
                assemblyInstaller.Path = windowsServiceFilePath;
                assemblyInstaller.Uninstall(null);
            }
        }

        /// <summary>  
        /// 启动服务  
        /// </summary>  
        /// <param name="serviceName">服务名</param>  
        /// <returns>存在返回 true,否则返回 false;</returns>  
        public static bool RunService(string serviceName)
        {
            bool bo = true;
            try
            {
                ServiceController sc = new ServiceController(serviceName);
                if (sc.Status.Equals(ServiceControllerStatus.Stopped) || sc.Status.Equals(ServiceControllerStatus.StopPending))
                {
                    sc.Start();
                }
            }
            catch (Exception ex)
            {
                bo = false;
            }

            return bo;
        }

        /// <summary>  
        /// 停止服务  
        /// </summary>  
        /// <param name="serviceName">服务名</param>  
        /// <returns>存在返回 true,否则返回 false;</returns>  
        public static bool StopService(string serviceName)
        {
            bool bo = true;
            try
            {
                ServiceController sc = new ServiceController(serviceName);
                if (!sc.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    sc.Stop();
                }
            }
            catch (Exception ex)
            {
                bo = false;
            }

            return bo;
        }

        /// <summary>  
        /// 获取指定服务的版本号  
        /// </summary>  
        /// <param name="serviceName">服务名称</param>  
        /// <returns></returns>  
        public static Version GetWindowsServiceVersion(string serviceName)
        {
            if (serviceName.IfIsNullOrEmpty()) return null;

            try
            {
                string path = Path.Combine(GetWindowsServiceInstallPath(serviceName), serviceName + ".exe");
                Assembly assembly = Assembly.LoadFile(path);
                AssemblyName assemblyName = assembly.GetName();
                Version version = assemblyName.Version;
                return version;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        /// <summary>
        /// 获取Windows服务描述信息
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static string GetWindowsServiceDescription(string serviceName)
        {

            string description = string.Empty;

            if (!WindowsServiceIsExists(serviceName)) return description;

            using (var service = new ManagementObject(new ManagementPath(string.Format("Win32_Service.Name='{0}'", serviceName))))
            {
                description = service["Description"].ToString();
            }

            return description;

        }

    }


}
