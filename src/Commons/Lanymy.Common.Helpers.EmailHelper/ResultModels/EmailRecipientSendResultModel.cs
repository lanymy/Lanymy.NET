using System;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 单个收件人的邮件发送结果。
    /// </summary>
    public class EmailRecipientSendResultModel
    {
        /// <summary>
        /// 收件人地址。
        /// </summary>
        public string RecipientAddress { get; set; }

        /// <summary>
        /// 是否发送成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 发送异常。
        /// </summary>
        public Exception Exception { get; set; }
    }
}
