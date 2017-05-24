///********************************************************************

//时间: 2015年03月30日, PM 05:21:35

//作者: lanyanmiyu@qq.com

//描述: wcf辅助类

//其它:     

//********************************************************************/



//using System;
//using System.ServiceModel;
//using System.ServiceModel.Channels;

//namespace Lanymy.General.Extension
//{
//    /// <summary>
//    /// wcf辅助类
//    /// </summary>
//    public class WcfFunctions
//    {
//        /// <summary>
//        /// 匹配wcf自定义路由地址
//        /// </summary>
//        /// <param name="url">通信地址</param>
//        /// <param name="dnsIdentity">dns通信标识</param>
//        /// <param name="headers">报头</param>
//        /// <returns></returns>
//        public static EndpointAddress MatchWcfRemoteAddress(string url, string dnsIdentity, AddressHeaderCollection headers)
//        {
//            return new EndpointAddress(new Uri(url), EndpointIdentity.CreateDnsIdentity(dnsIdentity), headers);
//        }

//    }
//}
