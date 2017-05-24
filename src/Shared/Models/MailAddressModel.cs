/********************************************************************

时间: 2016年02月26日, AM 09:58:25

作者: lanyanmiyu@qq.com

描述: 邮件地址 数据信息 实体类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.General.Extension.Models
{

    /// <summary>
    /// 邮件地址 数据信息 实体类
    /// </summary>
    public class MailAddressModel
    {

        /// <summary>
        /// 地址昵称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        public string Address { get; set; }
    }


}
