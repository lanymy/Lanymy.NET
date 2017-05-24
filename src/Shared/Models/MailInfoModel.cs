/********************************************************************

时间: 2016年02月26日, AM 09:47:27

作者: lanyanmiyu@qq.com

描述: 邮件信息数据实体类

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
    /// 邮件信息数据实体类
    /// </summary>
    public class MailInfoModel
    {


        /// <summary>
        /// 邮件ID
        /// </summary>
        public uint ID { get; set; }

        /// <summary>
        /// 邮件消息ID
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// 发件方列表
        /// </summary>
        public List<MailAddressModel> From { get; set; }

        /// <summary>
        /// 收件方列表
        /// </summary>
        public List<MailAddressModel> To { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Html 内容信息
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// 文本内容信息
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        /// 附件信息
        /// </summary>
        public List<MailAttachmentInfoModel> Attachments { get; set; }


    }

}
