using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Interfaces
{
    /// <summary>
    /// 帐号信息属性接口
    /// </summary>
    public interface IAccountProperties
    {
        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        string Password { get; set; }
    }
}
