using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
#if NET8_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using Lanymy.Common.ConstKeys;
using Lanymy.Common.Enums;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Helpers
{

    /// <summary>
    /// 获取本地机器信息的类
    /// </summary>
    public class PcInfoHelper
    {
        /// <summary>
        /// 获取电脑主机名称
        /// </summary>
        /// <returns></returns>
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }

        private static string GetIP(System.Net.Sockets.AddressFamily addressFamily)
        {
            string ip = string.Empty;
            var ipAddress = Dns.GetHostAddresses(GetHostName());
            if (!ipAddress.IfIsNullOrEmpty())
            {
                var result = ipAddress.FirstOrDefault(p => p.AddressFamily == addressFamily);
                if (!result.IfIsNullOrEmpty())
                {
                    ip = result.ToString();
                }
            }
            return ip;
        }

        /// <summary>
        /// 获取IPV4
        /// </summary>
        /// <returns></returns>
        public static string GetIPV4()
        {
            return GetIP(System.Net.Sockets.AddressFamily.InterNetwork);
        }

        /// <summary>
        /// 获取IPV6
        /// </summary>
        /// <returns></returns>
        public static string GetIPV6()
        {
            return GetIP(System.Net.Sockets.AddressFamily.InterNetworkV6);
        }



        public static string GetLocalIpAddress()
        {
            UnicastIPAddressInformation mostSuitableIp = null;
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;
                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(address.Address))
                        continue;
                    return address.Address.ToString();
                }
            }

            return mostSuitableIp != null
                ? mostSuitableIp.Address.ToString()
                : "";
        }


        /// <summary>
        /// 获取本地一个随机可以用的端口号
        /// </summary>
        /// <param name="minPort"></param>
        /// <param name="maxPort"></param>
        /// <returns></returns>
        public static int GetRandomAvaliablePort(int minPort = 1024, int maxPort = 5000)
        {
            return GetRandomAvaliablePorts(minPort, maxPort)[0];
        }


        /// <summary>
        /// 批量获取本地随机可以用的端口号
        /// </summary>
        /// <param name="minPort"></param>
        /// <param name="maxPort"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] GetRandomAvaliablePorts(int minPort = 1024, int maxPort = 5000, int count = 1)
        {

            if (count <= 0)
            {
                count = 1;
            }

            var ports = new List<int>();

            var rand = new Random((int)DateTime.Now.Ticks);
            var index = 0;
            int port = 0;

            var localInUsedPorts = GetLocalInUsedPorts();

            while (index < count)
            {
                port = rand.Next(minPort, maxPort);
                if (!ports.Contains(port) && !localInUsedPorts.Contains(port))
                {
                    index++;
                    ports.Add(port);
                }
            }

            return ports.OrderBy(o => o).ToArray();

        }



        /// <summary>
        /// 获取本地占用端口号列表
        /// </summary>
        /// <returns></returns>
        public static int[] GetLocalInUsedPorts()
        {

            var localUsedPorts = new List<int>();

            var ipGlobalProps = IPGlobalProperties.GetIPGlobalProperties();
            localUsedPorts.AddRange(ipGlobalProps.GetActiveTcpListeners().Select(o => o.Port));
            localUsedPorts.AddRange(ipGlobalProps.GetActiveUdpListeners().Select(o => o.Port));
            localUsedPorts.AddRange(ipGlobalProps.GetActiveTcpConnections().Select(o => o.LocalEndPoint.Port));

            return localUsedPorts.Distinct().OrderBy(o => o).ToArray();

        }


        /// <summary>
        /// 本地端口号是否被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool IsPortInUsed(int port)
        {

            var ipGlobalProps = IPGlobalProperties.GetIPGlobalProperties();
            var ipsTCP = ipGlobalProps.GetActiveTcpListeners();

            if (ipsTCP.Any(p => p.Port == port))
            {
                return true;
            }

            var ipsUDP = ipGlobalProps.GetActiveUdpListeners();

            if (ipsUDP.Any(p => p.Port == port))
            {
                return true;
            }

            var tcpConnInfos = ipGlobalProps.GetActiveTcpConnections();
            if (tcpConnInfos.Any(conn => conn.LocalEndPoint.Port == port))
            {
                return true;
            }

            return false;

        }


        /// <summary>
        /// 获取 当前 系统 计算 位数
        /// </summary>
        /// <returns></returns>
        public static BitOperatingTypeEnum GetCurrentBitOperatingSystemType()
        {

            return Environment.Is64BitOperatingSystem ? BitOperatingTypeEnum.x64 : BitOperatingTypeEnum.x86;

        }


#if NET8_0_OR_GREATER


        /// <summary>
        /// 创建程序开机自启动
        /// </summary>
        /// <param name="sourceExeFileFullPath"></param>
        /// <param name="targetShortcutFileName"></param>
        /// <param name="isOverride">覆盖文件</param>
        /// <param name="arguments">参数</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static bool CreateBootAutoRun(string sourceExeFileFullPath, string targetShortcutFileName, string description = null, bool isOverride = true, string arguments = null)
        {
            var targetShortcutFileFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetShortcutFileName);

            return CreateShortcut(sourceExeFileFullPath, targetShortcutFileFullPath, description, isOverride, arguments);
        }





        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="sourceExeFileFullPath">源exe程序全路径</param>
        /// <param name="targetShortcutFileName">只需要快捷方式名称,自动补充.lnk文件后缀</param>
        /// <param name="isOverride">覆盖文件</param>
        /// <param name="arguments">参数</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static bool CreateDesktopShortcut(string sourceExeFileFullPath, string targetShortcutFileName, string description = null, bool isOverride = true, string arguments = null)
        {

            var targetShortcutFileFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), targetShortcutFileName);

            return CreateShortcut(sourceExeFileFullPath, targetShortcutFileFullPath, description, isOverride, arguments);

        }


        /// <summary>
        /// 创建一个快捷方式(用户级别)
        /// </summary>
        /// <param name="sourceExeFileFullPath">源exe程序全路径</param>
        /// <param name="targetShortcutFileFullPath">目标快件方式文件全路径,自动补充.lnk文件后缀</param>
        /// <param name="isOverride">覆盖文件</param>
        /// <param name="arguments">参数</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
#if NET8_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public static bool CreateShortcut(string sourceExeFileFullPath, string targetShortcutFileFullPath, string description = null, bool isOverride = true, string arguments = null)
        {

            try
            {

                if (!targetShortcutFileFullPath.ToLower().EndsWith(FileExtensionKeys.SHORTCUT_FILE_EXTENSION))
                {
                    targetShortcutFileFullPath += FileExtensionKeys.SHORTCUT_FILE_EXTENSION;
                }

                if (isOverride && File.Exists(targetShortcutFileFullPath))
                {
                    File.Delete(targetShortcutFileFullPath);
                }

                var shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType == null)
                {
                    return false;
                }

                dynamic shell = Activator.CreateInstance(shellType);
                var shortcut = shell.CreateShortcut(targetShortcutFileFullPath);
                shortcut.TargetPath = sourceExeFileFullPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(sourceExeFileFullPath);
                shortcut.Arguments = arguments;
                shortcut.Description = description;
                //shortcut.IconLocation = "图标路径";
                //shortcut.WindowStyle = FormWindowState.Normal;

                shortcut.Save();

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }



#endif



    }
}
