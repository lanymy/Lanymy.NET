using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Helpers
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


        /// <summary>
        /// 通过 192.168.1.1 这种格式的IP字符串 转成 对应的 IPAddress 实例对象
        /// </summary>
        /// <param name="ipString"></param>
        /// <returns></returns>
        public static IPAddress GetIpAddressByIpString(string ipString)
        {

            return new IPAddress(ipString.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.ConvertToType<byte>()).ToArray());

        }


        /// <summary>
        /// 获取本地所有IP地址列表
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalIpList()
        {

            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());

            return hostEntry.AddressList.ToList();

        }

        /// <summary>
        /// 获取本地IPV4地址列表
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalIpV4List()
        {
            return GetLocalIpList().Where(o => o.AddressFamily == AddressFamily.InterNetwork).ToList();
        }

        /// <summary>
        /// 获取本地IPV6地址列表
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalIpV6List()
        {
            return GetLocalIpList().Where(o => o.AddressFamily == AddressFamily.InterNetworkV6).ToList();
        }



        public static byte[] GetMacAddressBytes(string macAddressStr, char separatorChar = ':')
        {

            if (string.IsNullOrEmpty(macAddressStr)) return null;

            return macAddressStr.Split(separatorChar).Select(o => Convert.ToByte(o, 16)).ToArray();

        }

        public static byte[] GetIpAddressBytes(string ipAddressStr)
        {

            if (string.IsNullOrEmpty(ipAddressStr)) return null;

            return ipAddressStr.Split('.').Select(byte.Parse).ToArray();

        }

        public static string GetLocalIP()
        {

            string ip = string.Empty;

            try
            {

                //var ipEntry = Dns.GetHostEntry(Dns.GetHostName());

                //var ipAddress = ipEntry.AddressList.Where(o => o.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();

                var ipAddress = GetLocalIpV4List().FirstOrDefault();


                if (ipAddress != null)
                {
                    ip = ipAddress.ToString();
                }

            }
            catch
            {

            }

            return ip;

        }




#if !NETSTANDARD



        //public static IPAddress GetGatewayAddresses(IPAddress currentIpAddress)
        //{

        //    uint bestInterfaceIndex;
        //    IPAddress address = null;
        //    int bestInterface = Win32Helper.GetBestInterface(BitConverter.ToUInt32(currentIpAddress.GetAddressBytes(), 0), out bestInterfaceIndex);
        //    if (bestInterface != 0)
        //    {
        //        throw new Win32Exception(bestInterface);
        //    }

        //    var allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        //    foreach (var networkInterface in allNetworkInterfaces)
        //    {

        //        var pProperties = networkInterface.GetIPProperties();

        //        if (pProperties != null)
        //        {

        //            //if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
        //            //{
        //            //    foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
        //            //    {
        //            //        if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
        //            //        {
        //            //            currentIpAddressOut = unicastAddress.Address;
        //            //            break;
        //            //        }
        //            //    }
        //            //}

        //            var gatewayAddresses = pProperties.GatewayAddresses;
        //            if (gatewayAddresses != null)
        //            {
        //                var gatewayIPAddressInformation = gatewayAddresses.FirstOrDefault();
        //                if (gatewayIPAddressInformation != null)
        //                {
        //                    address = gatewayIPAddressInformation.Address;
        //                }
        //            }
        //            else
        //            {
        //                address = null;
        //            }

        //            IPAddress pAddress = address;
        //            if (pAddress != null)
        //            {
        //                if (networkInterface.Supports(NetworkInterfaceComponent.IPv4))
        //                {

        //                    IPv4InterfaceProperties pv4Properties = pProperties.GetIPv4Properties();
        //                    if (pv4Properties != null && pv4Properties.Index == bestInterfaceIndex)
        //                    {
        //                        return pAddress;
        //                    }

        //                }
        //                if (networkInterface.Supports(NetworkInterfaceComponent.IPv6))
        //                {
        //                    IPv6InterfaceProperties pv6Properties = pProperties.GetIPv6Properties();
        //                    if (pv6Properties != null && pv6Properties.Index == bestInterfaceIndex)
        //                    {
        //                        return pAddress;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}

        //public static string GetMacAddressByIpAddress(IPAddress ipaddress)
        //{
        //    int length = 6;
        //    byte[] numArray = new byte[length];
        //    if (Win32Helper.SendARP((uint)ipaddress.Address, 0, numArray, ref length) != 0)
        //    {
        //        return null;
        //    }
        //    return BitConverter.ToString(numArray, 0, 6);
        //}

        //public static IpInfoModel GetIpInfoModel(string ip)
        //{

        //    IpInfoModel ipInfoModel = null;

        //    try
        //    {
        //        IPAddress ipAddress = IPAddress.Parse(ip);
        //        string ipMacAddress = GetMacAddressByIpAddress(ipAddress);
        //        if (!string.IsNullOrEmpty(ipMacAddress))
        //        {
        //            string hostName;
        //            try
        //            {
        //                hostName = Dns.GetHostEntry(ipAddress).HostName;
        //            }
        //            catch
        //            {
        //                hostName = "";
        //            }

        //            ipInfoModel = new IpInfoModel
        //            {
        //                IpAddress = ipAddress,
        //                IpMacAddress = ipMacAddress,
        //                HostName = hostName,
        //            };

        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }

        //    return ipInfoModel;

        //}

        //public static List<IpInfoModel> GetLanIpInfoList(string gatewayAddressString, Action<IpInfoModel> action = null)
        //{
        //    return GetLanIpInfoList(GetIpAddressByIpString(gatewayAddressString), action);
        //}

        //public static List<IpInfoModel> GetLanIpInfoList(IPAddress gatewayAddress, Action<IpInfoModel> action = null)
        //{

        //    var ipInfoList = new List<IpInfoModel>();
        //    //string str2 = string.Concat(string.Join(".", ipaddress.ToString().Split('.').Take(3)), ".");
        //    string gatewayAddressesString = gatewayAddress.ToString();
        //    string prefixGatewayAddresses = string.Join(".", gatewayAddressesString.Split('.').Take(3)) + ".";

        //    Parallel.For(1, 254, new ParallelOptions()
        //    {
        //        MaxDegreeOfParallelism = 255
        //    }, (lastIpNum) =>
        //    {

        //        string ipStr = prefixGatewayAddresses + lastIpNum;

        //        //不处理网关IP
        //        if (ipStr == gatewayAddressesString)
        //        {
        //            return;
        //        }

        //        var ipInfoModel = GetIpInfoModel(ipStr);

        //        if (ipInfoModel.IfIsNullOrEmpty())
        //        {
        //            return;
        //        }

        //        lock (ipInfoList)
        //        {
        //            ipInfoList.Add(ipInfoModel);
        //            action?.Invoke(ipInfoModel);
        //        }

        //    });

        //    return ipInfoList;

        //}


#endif


    }
}
