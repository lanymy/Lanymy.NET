/********************************************************************

时间: 2016年02月26日, AM 10:04:42

作者: lanyanmiyu@qq.com

描述: 邮件 附件 信息 数据实体类

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
    /// 邮件 附件 信息 数据实体类
    /// </summary>
    public class MailAttachmentInfoModel
    {

        /// <summary>
        /// 附件原始文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 保存到本地附件 文件 全路径 信息
        /// </summary>
        public string LocalFileFullPath { get; set; }
    }



}


