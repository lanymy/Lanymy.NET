using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers.ResultModels;

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
            return PingIPWithResult(ip).IsSuccess;
        }

        /// <summary>
        /// PING IP，并返回详细结果。
        /// </summary>
        /// <param name="ip">IP 字符串</param>
        /// <returns>Ping 结果</returns>
        public static PingResultModel PingIPWithResult(string ip)
        {
            var result = new PingResultModel
            {
                AddressText = ip,
            };

            if (ip.IfIsNullOrEmpty())
            {
                result.Exception = new ArgumentNullException(nameof(ip));
                return result;
            }

            if (!IPAddress.TryParse(ip, out var ipAddress))
            {
                result.Exception = new ArgumentException("Invalid IP address format.", nameof(ip));
                return result;
            }

            return PingIPWithResult(ipAddress, ip);
        }

        /// <summary>
        /// PING IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        /// <returns></returns>
        public static bool PingIP(IPAddress ip)
        {
            return PingIPWithResult(ip).IsSuccess;
        }

        /// <summary>
        /// PING IP，并返回详细结果。
        /// </summary>
        /// <param name="ip">IP 地址</param>
        /// <returns>Ping 结果</returns>
        public static PingResultModel PingIPWithResult(IPAddress ip)
        {
            return PingIPWithResult(ip, ip?.ToString());
        }

        private static PingResultModel PingIPWithResult(IPAddress ip, string addressText)
        {
            var result = new PingResultModel
            {
                Address = ip,
                AddressText = addressText,
            };

            if (ip == null)
            {
                result.Exception = new ArgumentNullException(nameof(ip));
                return result;
            }

            try
            {
                using (var ping = new Ping())
                {
                    var pingReply = ping.Send(ip, 5000);//ping 目标 IP 超时时间 5秒
                    result.Status = pingReply?.Status;
                    result.RoundtripTime = pingReply?.RoundtripTime;
                    result.IsSuccess = pingReply != null && pingReply.Status == IPStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }


        /// <summary>
        /// 通过 192.168.1.1 这种格式的IP字符串 转成 对应的 IPAddress 实例对象
        /// </summary>
        /// <param name="ipString"></param>
        /// <returns></returns>
        public static IPAddress GetIpAddressByIpString(string ipString)
        {
            var result = GetIpAddressByIpStringWithResult(ipString);

            if (!result.IsSuccess)
            {
                throw result.Exception;
            }

            return result.Address;
        }

        /// <summary>
        /// 通过 192.168.1.1 这种格式的 IP 字符串转成对应的 <see cref="IPAddress"/> 实例对象，并返回详细结果。
        /// </summary>
        /// <param name="ipString">IPv4 地址字符串</param>
        /// <returns>IP 地址解析结果</returns>
        public static IpAddressParseResultModel GetIpAddressByIpStringWithResult(string ipString)
        {
            var result = new IpAddressParseResultModel
            {
                Input = ipString,
            };

            if (ipString.IfIsNullOrEmpty())
            {
                result.Exception = new ArgumentNullException(nameof(ipString));
                return result;
            }

            if (!IPAddress.TryParse(ipString, out var ipAddress) || ipAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                result.Exception = new ArgumentException("Invalid IPv4 address format.", nameof(ipString));
                return result;
            }

            result.Address = ipAddress;
            result.IsSuccess = true;

            return result;

        }


        /// <summary>
        /// 获取本地所有IP地址列表
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalIpList()
        {
            var result = GetLocalIpListWithResult();
            if (!result.IsSuccess)
            {
                throw result.Exception;
            }

            return result.Addresses.ToList();
        }

        /// <summary>
        /// 获取本地所有 IP 地址列表，并返回详细结果。
        /// </summary>
        /// <returns>本地 IP 列表查询结果</returns>
        public static LocalIpListResultModel GetLocalIpListWithResult()
        {
            return GetLocalIpListWithResult(null);

        }

        /// <summary>
        /// 获取本地IPV4地址列表
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalIpV4List()
        {
            var result = GetLocalIpV4ListWithResult();
            if (!result.IsSuccess)
            {
                throw result.Exception;
            }

            return result.Addresses.ToList();
        }

        /// <summary>
        /// 获取本地 IPV4 地址列表，并返回详细结果。
        /// </summary>
        /// <returns>本地 IPV4 列表查询结果</returns>
        public static LocalIpListResultModel GetLocalIpV4ListWithResult()
        {
            return GetLocalIpListWithResult(AddressFamily.InterNetwork);
        }

        /// <summary>
        /// 获取本地IPV6地址列表
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalIpV6List()
        {
            var result = GetLocalIpV6ListWithResult();
            if (!result.IsSuccess)
            {
                throw result.Exception;
            }

            return result.Addresses.ToList();
        }

        /// <summary>
        /// 获取本地 IPV6 地址列表，并返回详细结果。
        /// </summary>
        /// <returns>本地 IPV6 列表查询结果</returns>
        public static LocalIpListResultModel GetLocalIpV6ListWithResult()
        {
            return GetLocalIpListWithResult(AddressFamily.InterNetworkV6);
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
            return GetLocalIPWithResult().AddressText ?? string.Empty;
        }

        /// <summary>
        /// 获取本机首个 IPV4 地址，并返回详细结果。
        /// </summary>
        /// <returns>本机 IP 查询结果</returns>
        public static LocalIpAddressResultModel GetLocalIPWithResult()
        {
            var ipListResult = GetLocalIpV4ListWithResult();
            var result = new LocalIpAddressResultModel
            {
                Addresses = ipListResult.Addresses,
                Exception = ipListResult.Exception,
            };

            if (ipListResult.Exception != null)
            {
                return result;
            }

            var ipAddress = ipListResult.Addresses.FirstOrDefault();
            if (ipAddress == null)
            {
                return result;
            }

            result.Address = ipAddress;
            result.AddressText = ipAddress.ToString();
            result.IsSuccess = true;

            return result;
        }

        private static LocalIpListResultModel GetLocalIpListWithResult(AddressFamily? addressFamily)
        {
            var result = new LocalIpListResultModel
            {
                AddressFamily = addressFamily,
                Addresses = Array.Empty<IPAddress>(),
            };

            try
            {
                var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                var addressList = hostEntry?.AddressList ?? Array.Empty<IPAddress>();

                if (addressFamily != null)
                {
                    addressList = addressList.Where(o => o.AddressFamily == addressFamily.Value).ToArray();
                }

                result.Addresses = addressList.ToList();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;

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
