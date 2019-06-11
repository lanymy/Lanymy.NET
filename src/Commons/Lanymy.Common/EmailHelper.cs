﻿using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Models;

namespace Lanymy.Common
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
        /// <returns></returns>
        public static CommonResultModel SendEmail(string smtpServer, string smtpMailUserName, string smtpPassword, string toEmailAddress, string mailSubject, string mailContent, bool ifEmailContentIsHtml = false, Encoding emailContentEncoding = null, bool enableSsl = false)
        {

            var resultModel = new CommonResultModel
            {
                IsSuccess = false
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


                using (var message = new MailMessage())
                {

                    var fromAddress = new MailAddress(smtpMailUserName);

                    message.Sender = fromAddress;
                    message.From = fromAddress;
                    message.To.Add(toEmailAddress);
                    message.Subject = mailSubject;//设置邮件主题 
                    message.IsBodyHtml = ifEmailContentIsHtml;//设置邮件正文为html格式 
                    message.Body = mailContent;//设置邮件内容 
                    message.BodyEncoding = emailContentEncoding;

                    using (var smtpClient = new SmtpClient(smtpServer))
                    {
                        smtpClient.Credentials = new NetworkCredential(smtpMailUserName, smtpPassword);
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.EnableSsl = enableSsl;
                        smtpClient.Send(message);
                    }

                }

                resultModel.IsSuccess = true;

            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Exception = ex;
            }

            return resultModel;
        }

        #endregion

    }

}
