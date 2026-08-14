using System.Collections.Generic;
using System.Linq;
using Lanymy.Common.Abstractions.ResultModels;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 邮件发送结果。
    /// </summary>
    public class EmailSendResultModel : CommonResultModel
    {
        /// <summary>
        /// SMTP 服务器地址。
        /// </summary>
        public string SmtpServer { get; set; }

        /// <summary>
        /// 发送人地址。
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// 邮件主题。
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 发送人显示名称。
        /// </summary>
        public string SenderDisplayName { get; set; }

        /// <summary>
        /// 发信端口。
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 是否开启 SSL。
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// 是否逐个收件人拆分发送。
        /// </summary>
        public bool IsUnpackSendMail { get; set; }

        /// <summary>
        /// 正文编码名称。
        /// </summary>
        public string BodyEncodingName { get; set; }

        /// <summary>
        /// 本次请求的收件人地址列表。
        /// </summary>
        public IReadOnlyList<string> RequestedRecipientAddresses { get; set; } = new List<string>();

        /// <summary>
        /// 本次请求的收件人数量。
        /// </summary>
        public int RequestedRecipientCount => RequestedRecipientAddresses?.Count ?? 0;

        /// <summary>
        /// 收件人发送结果明细。
        /// </summary>
        public IReadOnlyList<EmailRecipientSendResultModel> RecipientResults { get; set; } = new List<EmailRecipientSendResultModel>();

        /// <summary>
        /// 成功数量。
        /// </summary>
        public int SuccessCount => RecipientResults?.Count(item => item != null && item.IsSuccess) ?? 0;

        /// <summary>
        /// 失败数量。
        /// </summary>
        public int FailureCount => RecipientResults?.Count(item => item == null || !item.IsSuccess) ?? 0;
    }
}
