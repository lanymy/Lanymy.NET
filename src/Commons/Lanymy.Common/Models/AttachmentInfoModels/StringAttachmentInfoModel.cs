using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Models.AttachmentInfoModels
{
    /// <summary>
    /// 字符串内容附件
    /// </summary>
    public class StringAttachmentInfoModel : BaseAttachmentDataInfoModel<string>
    {


        /// <summary>
        /// 字符串内容附件
        /// </summary>
        /// <param name="keyName">附件主键值</param>
        /// <param name="stringAttachmentData">附件字符串内容</param>
        public StringAttachmentInfoModel(string keyName, string stringAttachmentData) : base(keyName, stringAttachmentData)
        {

        }

    }
}
