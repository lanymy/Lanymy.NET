using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Lanymy.Common
{
    /// <summary>
    /// 网络操作类
    /// </summary>
    public class NetworkHelper
    {

        /// <summary>
        /// PING IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool PingIP(string ip)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(ip, out ipAddress))
            {
                return PingIP(ipAddress);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// PING IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        /// <returns></returns>
        public static bool PingIP(IPAddress ip)
        {

            try
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ip, 5000);//ping 目标 IP 超时时间 5秒
                return pingReply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }

        }


    }
}
