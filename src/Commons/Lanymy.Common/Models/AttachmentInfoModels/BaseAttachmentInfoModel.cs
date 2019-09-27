using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Models.AttachmentInfoModels
{
    /// <summary>
    /// 附件参数信息实体类 基类
    /// </summary>
    public abstract class BaseAttachmentInfoModel
    {

        //public abstract AttachmentTypeEnum AttachmentType { get; }

        /// <summary>
        /// 附件主键值
        /// </summary>
        public string KeyName { get; }




        /// <summary>
        /// 附件参数信息实体类 构造方法
        /// </summary>
        /// <param name="keyName">附件主键值</param>
        protected BaseAttachmentInfoModel(string keyName)
        {

            KeyName = keyName;
        }


    }
}
