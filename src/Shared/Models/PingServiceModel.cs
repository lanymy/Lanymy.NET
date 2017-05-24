/********************************************************************

时间: 2016年11月29日, AM 09:57:38

作者: lanyanmiyu@qq.com

描述: ping服务器 使用的 数据实体类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.General.Extension.Models
{


    /// <summary>
    /// ping服务器 使用的 数据实体类
    /// <para>可供WCF直接使用</para>
    /// </summary>
    [DataContract]
    public class PingServiceModel
    {

        /// <summary>
        /// 客户端发起请求的时间
        /// </summary>
        [DataMember]
        public DateTime RequestDateTime { get; set; }


        /// <summary>
        /// 服务器端响应请求的时间
        /// </summary>
        [DataMember]
        public DateTime ResponseDateTime { get; set; }

        /// <summary>
        /// 接收到服务器返回数据的时间
        /// <para>这个属性 等服务器 返回数据实体后 需要本地赋值一次本地时间 获取得到本地时间</para>
        /// </summary>
        [DataMember]
        public DateTime LocalAcceptDateTime { get; set; }

        /// <summary>
        /// 发起请求的IP地址
        /// </summary>
        [DataMember]
        public string RequestIP { get; set; }

    }

}
