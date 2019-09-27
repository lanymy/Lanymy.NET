using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Models.AttachmentInfoModels
{
    /// <summary>
    /// 文件内容附件
    /// </summary>
    public class FileAttachmentInfoModel : BaseAttachmentDataInfoModel<string>
    {

        /// <summary>
        /// 文件内容附件 构造方法
        /// </summary>
        /// <param name="keyName">附件主键值</param>
        /// <param name="attachmentFileFullPath">附件文件全路径</param>
        public FileAttachmentInfoModel(string keyName, string attachmentFileFullPath) : base(keyName, attachmentFileFullPath)
        {

        }

    }
}
