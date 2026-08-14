using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Lanymy.Common.Abstractions.ResultModels;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers.ResultModels;

namespace Lanymy.Common.Helpers
{
    /// <summary>
    /// 邮件扩展类
    /// </summary>
    public class EmailHelper
    {

        #region 发送电子邮件

        /// <summary>
        /// SMTP协议发送邮件
        /// </summary>
        /// <param name="smtpServer">服务器地址</param>
        /// <param name="smtpMailUserName">用户名</param>
        /// <param name="smtpPassword">密码</param>
        /// <param name="toEmailAddress">发送的邮件地址 多个用 "," 分割</param>
        /// <param name="mailSubject">标题</param>
        /// <param name="mailContent">内容</param>
        /// <param name="ifEmailContentIsHtml">邮件内容是否是 Html 格式 默认值 False 为 文本内容</param>
        /// <param name="emailContentEncoding">内容编码,默认值 null 为 UTF-8 编码</param>
        /// <param name="enableSsl">是否开启 SSL  默认值 False  不开启</param>
        /// <param name="port">端口号,默认值:25</param>
        /// <param name="isUnpackSendMail">是否 把多个地址一次性发送一个邮件 拆分成 一个地址发一个邮件的形式; 默认值 false 批量一次性发一个邮件;</param>
        /// <param name="senderDisplayName">发送人要显示的名称</param>
        /// <returns></returns>
        public static CommonResultModel SendEmail(string smtpServer, string smtpMailUserName, string smtpPassword, string toEmailAddress, string mailSubject, string mailContent, bool ifEmailContentIsHtml = false, Encoding emailContentEncoding = null, bool enableSsl = false, int port = 25, bool isUnpackSendMail = false, string senderDisplayName = null)
        {
            return SendEmailWithResult(smtpServer, smtpMailUserName, smtpPassword, toEmailAddress, mailSubject, mailContent, ifEmailContentIsHtml, emailContentEncoding, enableSsl, port, isUnpackSendMail, senderDisplayName);
        }

        /// <summary>
        /// SMTP 协议发送邮件，并返回详细结果。
        /// </summary>
        /// <param name="smtpServer">服务器地址</param>
        /// <param name="smtpMailUserName">用户名</param>
        /// <param name="smtpPassword">密码</param>
        /// <param name="toEmailAddress">发送的邮件地址 多个用 "," 分割</param>
        /// <param name="mailSubject">标题</param>
        /// <param name="mailContent">内容</param>
        /// <param name="ifEmailContentIsHtml">邮件内容是否是 Html 格式 默认值 False 为 文本内容</param>
        /// <param name="emailContentEncoding">内容编码,默认值 null 为 UTF-8 编码</param>
        /// <param name="enableSsl">是否开启 SSL 默认值 False 不开启</param>
        /// <param name="port">端口号,默认值:25</param>
        /// <param name="isUnpackSendMail">是否把多个地址拆成逐个发送</param>
        /// <param name="senderDisplayName">发送人显示名称</param>
        /// <returns>邮件发送结果</returns>
        public static EmailSendResultModel SendEmailWithResult(string smtpServer, string smtpMailUserName, string smtpPassword, string toEmailAddress, string mailSubject, string mailContent, bool ifEmailContentIsHtml = false, Encoding emailContentEncoding = null, bool enableSsl = false, int port = 25, bool isUnpackSendMail = false, string senderDisplayName = null)
        {

            var resultModel = new EmailSendResultModel
            {
                IsSuccess = false,
                SmtpServer = smtpServer,
                SenderAddress = smtpMailUserName,
                Subject = mailSubject,
                SenderDisplayName = senderDisplayName,
                Port = port,
                EnableSsl = enableSsl,
                IsUnpackSendMail = isUnpackSendMail,
            };

            try
            {

                if (smtpServer.IfIsNullOrEmpty())
                    throw new ArgumentNullException(nameof(smtpServer));

                if (smtpMailUserName.IfIsNullOrEmpty())
                    throw new ArgumentNullException(nameof(smtpMailUserName));

                if (smtpPassword.IfIsNullOrEmpty())
                    throw new ArgumentNullException(nameof(smtpPassword));

                if (toEmailAddress.IfIsNullOrEmpty())
                    throw new ArgumentNullException(nameof(toEmailAddress));

                if (mailSubject.IfIsNullOrEmpty())
                    throw new ArgumentNullException(nameof(mailSubject));

                if (mailContent.IfIsNullOrEmpty())
                    mailContent = mailSubject;

                if (emailContentEncoding.IfIsNullOrEmpty())
                    emailContentEncoding = Encoding.UTF8;

                resultModel.Subject = mailSubject;
                resultModel.BodyEncodingName = emailContentEncoding.WebName;

                var toEmailAddressList = NormalizeRecipientAddresses(toEmailAddress);
                resultModel.RequestedRecipientAddresses = toEmailAddressList;

                if (toEmailAddressList.Count <= 0)
                {
                    throw new ArgumentException("At least one recipient address is required.", nameof(toEmailAddress));
                }


                using (var message = new MailMessage())
                {

                    var fromAddress = senderDisplayName.IfIsNullOrEmpty() ? new MailAddress(smtpMailUserName) : new MailAddress(smtpMailUserName, senderDisplayName);

                    message.Sender = fromAddress;
                    message.From = fromAddress;
                    message.Subject = mailSubject;//设置邮件主题 
                    message.IsBodyHtml = ifEmailContentIsHtml;//设置邮件正文为html格式 
                    message.Body = mailContent;//设置邮件内容 
                    message.BodyEncoding = emailContentEncoding;

                    using (var smtpClient = new SmtpClient(smtpServer, port))
                    {

                        smtpClient.Credentials = new NetworkCredential(smtpMailUserName, smtpPassword);
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.EnableSsl = enableSsl;

                        if (isUnpackSendMail)
                        {

                            var messageTo = message.To;
                            var recipientResults = new List<EmailRecipientSendResultModel>();
                            foreach (var toEmailAddressItem in toEmailAddressList)
                            {
                                var recipientResult = new EmailRecipientSendResultModel
                                {
                                    RecipientAddress = toEmailAddressItem,
                                };

                                try
                                {

                                    messageTo.Clear();
                                    messageTo.Add(toEmailAddressItem);
                                    smtpClient.Send(message);
                                    recipientResult.IsSuccess = true;

                                }
                                catch (Exception ex)
                                {
                                    recipientResult.Exception = ex;

                                    if (resultModel.Exception == null)
                                    {
                                        resultModel.Exception = ex;
                                    }
                                }
                                finally
                                {
                                    recipientResults.Add(recipientResult);
                                }

                            }

                            resultModel.RecipientResults = recipientResults;
                            resultModel.IsSuccess = recipientResults.All(item => item.IsSuccess);

                        }
                        else
                        {

                            foreach (var toEmailAddressItem in toEmailAddressList)
                            {
                                message.To.Add(toEmailAddressItem);
                            }

                            smtpClient.Send(message);
                            resultModel.RecipientResults = toEmailAddressList
                                .Select(item => new EmailRecipientSendResultModel
                                {
                                    RecipientAddress = item,
                                    IsSuccess = true,
                                })
                                .ToList();
                            resultModel.IsSuccess = true;

                        }

                    }

                }


            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                if (resultModel.Exception == null)
                {
                    resultModel.Exception = ex;
                }

                if ((resultModel.RecipientResults == null || resultModel.RecipientResults.Count <= 0)
                    && resultModel.RequestedRecipientAddresses != null
                    && resultModel.RequestedRecipientAddresses.Count > 0)
                {
                    resultModel.RecipientResults = resultModel.RequestedRecipientAddresses
                        .Select(item => new EmailRecipientSendResultModel
                        {
                            RecipientAddress = item,
                            Exception = ex,
                        })
                        .ToList();
                }
            }

            return resultModel;

        }

        private static IReadOnlyList<string> NormalizeRecipientAddresses(string toEmailAddress)
        {
            return (toEmailAddress ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item?.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();
        }

        #endregion

    }
}
