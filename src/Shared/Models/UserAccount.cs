/********************************************************************

时间: 2015年03月05日, AM 10:01:03

作者: lanyanmiyu@qq.com

描述: 用户帐号实体类

其它:     

********************************************************************/



using System;
using System.Runtime.Serialization;

namespace Lanymy.General.Extension.Models
{
    /// <summary>
    /// 用户帐号实体类
    /// </summary>
    [Serializable]
    [DataContract]
    public class UserAccount
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        public string UserPassWord { get; set; }
    }
}
