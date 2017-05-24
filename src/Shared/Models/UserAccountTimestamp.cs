/********************************************************************

时间: 2015年03月09日, PM 04:31:35

作者: lanyanmiyu@qq.com

描述: 时间戳用户帐号实体类

其它:     

********************************************************************/


using System;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension.Models
{
    /// <summary>
    /// 时间戳用户帐号实体类
    /// </summary>
    [Serializable]
    public class UserAccountTimestamp : UserAccount
    {

        //[DataMember]
        //public int AMask { get; set; }


        //private DateTime? _Timestamp;

        //[DataMember]
        //public DateTime? Timestamp
        //{
        //    get { return _Timestamp; }
        //    set
        //    {
        //        _Timestamp = value;
        //        AMask = _Timestamp.HasValue ? _Timestamp.Value.Millisecond : DateTime.Now.Millisecond;
        //    }
        //}

        [DataMember]
        public DateTime? Timestamp { get; set; }

    }
}
