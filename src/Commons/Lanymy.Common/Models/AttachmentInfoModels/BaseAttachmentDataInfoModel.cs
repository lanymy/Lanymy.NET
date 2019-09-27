using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Models.AttachmentInfoModels
{
    public abstract class BaseAttachmentDataInfoModel<TData> : BaseAttachmentInfoModel
    {

        /// <summary>
        /// 附件内容数据
        /// </summary>
        public TData AttachmentData { get; }

        /// <summary>
        /// 附件参数信息实体类 构造方法
        /// </summary>
        /// <param name="keyName">附件主键值</param>
        /// <param name="attachmentData">附件内容</param>
        protected BaseAttachmentDataInfoModel(string keyName, TData attachmentData) : base(keyName)
        {

            AttachmentData = attachmentData;

        }

    }

}
