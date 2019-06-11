using System;
using System.Collections.Generic;
using System.Text;
using Lanymy.Common.Interfaces;

namespace Lanymy.Common.Models
{

    /// <summary>
    /// 帐号 信息 实体类
    /// </summary>
    public class AccountInfoModel : IAccountProperties
    {

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

    }

}
