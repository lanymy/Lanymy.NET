using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Lanymy.Common.ExtensionFunctions;

#if NET472

using System.Management;

#endif


namespace Lanymy.Common
{
    /// <summary>
    /// 获取本地机器信息的类
    /// </summary>
    public class PCInfoHelper
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


#if NET472

        /// <summary>
        /// 获取MAC地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddressByWmi()
        {
            string mac = "";
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
                ManagementObjectCollection queryCollection = query.Get();

                foreach (var o in queryCollection)
                {
                    var mo = (ManagementObject)o;
                    if (mo["IPEnabled"].ToString() == "True")
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
            }
            catch
            {
            }
            return mac;
        }

#endif





    }
}
