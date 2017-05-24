//// *******************************************************************
//// 创建时间：2015年01月14日, AM 11:07:37
//// 作者：lanyanmiyu@qq.com
//// 说明：发送电子邮件
//// 其它:
//// *******************************************************************

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Mail;
//using System.Text;
//using Lanymy.General.Extension.ExtensionFunctions;
//using Lanymy.General.Extension.Models;
//using MailKit.Search;
//using MailKit.Net.Imap;
//using MailKit;
//using MimeKit;
//using System.IO;

//namespace Lanymy.General.Extension
//{
//    /// <summary>
//    /// 提供发送电子邮件的常用方法
//    /// </summary>
//    public class EmailFunctions
//    {

//        #region 发送电子邮件


//        /// <summary>
//        /// SMTP协议发送邮件
//        /// </summary>
//        /// <param name="smtpServer">服务器地址</param>
//        /// <param name="smtpMailUserName">用户名</param>
//        /// <param name="smtpPassword">密码</param>
//        /// <param name="toEmailAddress">发送的邮件地址 多个用 "," 分割</param>
//        /// <param name="mailSubject">标题</param>
//        /// <param name="mailContent">内容</param>
//        /// <param name="ifEmailContentIsHtml">邮件内容是否是 Html 格式 默认值 False 为 文本内容</param>
//        /// <param name="emailContentEncoding">内容编码,默认值 UTF-8</param>
//        /// <param name="enableSsl">是否开启 SSL  默认值 False  不开启</param>
//        /// <returns></returns>
//        public static bool SendEmail(string smtpServer, string smtpMailUserName, string smtpPassword, string toEmailAddress, string mailSubject, string mailContent, bool ifEmailContentIsHtml = false, string emailContentEncoding = "UTF-8", bool enableSsl = false)
//        {
//            try
//            {

//                if (smtpServer.IfIsNullOrEmpty())
//                    throw new ArgumentNullException("smtpServer");

//                if (smtpMailUserName.IfIsNullOrEmpty())
//                    throw new ArgumentNullException("smtpMailUserName");

//                if (smtpPassword.IfIsNullOrEmpty())
//                    throw new ArgumentNullException("smtpPassword");

//                if (toEmailAddress.IfIsNullOrEmpty())
//                    throw new ArgumentNullException("toEmailAddress");

//                if (mailSubject.IfIsNullOrEmpty())
//                    throw new ArgumentNullException("mailSubject");

//                if (mailContent.IfIsNullOrEmpty())
//                    mailContent = mailSubject;

//                if (emailContentEncoding.IfIsNullOrEmpty())
//                    emailContentEncoding = "UTF-8";


//                MailAddress fromAddress = new MailAddress(smtpMailUserName);
//                MailMessage message = new MailMessage();
//                message.Sender = fromAddress;
//                message.From = fromAddress;
//                message.To.Add(toEmailAddress);
//                message.Subject = mailSubject;//设置邮件主题 
//                message.IsBodyHtml = ifEmailContentIsHtml;//设置邮件正文为html格式 
//                message.Body = mailContent;//设置邮件内容 
//                message.BodyEncoding = Encoding.GetEncoding(emailContentEncoding);

//                using (SmtpClient smtpClient = new SmtpClient(smtpServer))
//                {
//                    smtpClient.Credentials = new NetworkCredential(smtpMailUserName, smtpPassword);
//                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
//                    smtpClient.EnableSsl = enableSsl;
//                    smtpClient.Send(message);
//                }

//                return true;
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//        }

//        #endregion


//        #region 邮件下载


//        /// <summary>
//        /// 邮件下载 ( 注意 只匹配处理 邮箱服务器 "收件箱" 文件夹里的邮件 )
//        /// </summary>
//        /// <param name="imapServer">IMAP服务器</param>
//        /// <param name="userName">用户名</param>
//        /// <param name="userPassword">密码</param>
//        /// <param name="saveAttachmentLocalFolderFullPath">保存附件的目录全路径</param>
//        /// <param name="searchQuery">
//        /// imap command 级别 查询表达式 (不需要下载邮件 就可以查询 其它参数的搜索条件都是需要先把邮件下载后 匹配规则才能生效) 不同imap服务器 支持的 查询命令不同 如果报错 请尝试更换表达式
//        /// <para>表达式例子
//        /// <code>
//        /// var query = SearchQuery.DeliveredAfter (DateTime.Parse ("2013-01-12")).And (SearchQuery.SubjectContains ("MailKit")).And (SearchQuery.Seen);
//        /// </code>
//        /// </para>
//        /// </param>
//        /// <param name="ifUseSsl">是否开启SSL</param>
//        /// <param name="fromKeyWords">发件方 邮件 地址 关键字 列表 用于邮件匹配</param>
//        /// <param name="toKeyWords">收件方 邮件 地址 关键字 列表 用于邮件匹配</param>
//        /// <param name="subjectKeyWords">邮件标题关键字列表 用于邮件匹配</param>
//        /// <param name="dateTimes">邮件日期列表 用于邮件匹配</param>
//        /// <returns></returns>
//        public static List<MailInfoModel> GetEmails(string imapServer, string userName, string userPassword, string saveAttachmentLocalFolderFullPath, bool ifUseSsl = false, SearchQuery searchQuery = null, List<string> fromKeyWords = null, List<string> toKeyWords = null, List<string> subjectKeyWords = null, List<DateTime> dateTimes = null)
//        {

//            if (imapServer.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("imapServer");
//            }

//            if (userName.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("userName");
//            }

//            if (userPassword.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("userPassword");
//            }

//            if (saveAttachmentLocalFolderFullPath.IfIsNullOrEmpty())
//            {
//                throw new ArgumentNullException("saveAttachmentLocalFolderFullPath");
//            }

//            if (!PathFunctions.IsAbsolutePath(saveAttachmentLocalFolderFullPath))
//            {
//                throw new ArgumentException(string.Format("路径 [ {0} ] 不是有效的本地绝对路径", saveAttachmentLocalFolderFullPath));
//            }

//            List<MailInfoModel> resultList = new List<MailInfoModel>();

//            using (var client = new ImapClient())
//            {

//                client.Connect(imapServer, 0, ifUseSsl);
//                client.Authenticate(userName, userPassword);
//                var inbox = client.Inbox;
//                inbox.Open(FolderAccess.ReadOnly);

//                IList<IMessageSummary> messageList = searchQuery.IfIsNullOrEmpty() ? inbox.Fetch(0, -1, MessageSummaryItems.Envelope | MessageSummaryItems.UniqueId) : inbox.Fetch(inbox.Search(searchQuery), MessageSummaryItems.Envelope | MessageSummaryItems.UniqueId);


//                if (messageList.IfIsNullOrEmpty())
//                {
//                    return resultList;
//                }

//                //根据搜索条件过滤邮件摘要信息
//                //List<string> froms = null, 
//                //List<string> tos = null, 
//                //List<string> subjectKeyWords = null,
//                //List<DateTime> dateTimes = null

//                //bool ifHaveSearchQuery = !froms.IfIsNullOrEmpty() || !tos.IfIsNullOrEmpty() || !subjectKeyWords.IfIsNullOrEmpty() || !dateTimes.IfIsNullOrEmpty();



//                if (!fromKeyWords.IfIsNullOrEmpty() || !toKeyWords.IfIsNullOrEmpty() || !subjectKeyWords.IfIsNullOrEmpty() || !dateTimes.IfIsNullOrEmpty())
//                {
//                    bool ifMatchSuccess;
//                    List<IMessageSummary> messageSummaryList = new List<IMessageSummary>();
//                    //有匹配条件
//                    foreach (var messageSummary in messageList)
//                    {
//                        ifMatchSuccess = false;

//                        if (!fromKeyWords.IfIsNullOrEmpty())
//                        {
//                            foreach (var fromKeyWord in fromKeyWords)
//                            {

//                                foreach (var fromAddress in messageSummary.Envelope.From.Mailboxes)
//                                {
//                                    if (fromAddress.Address.ToLower().Contains(fromKeyWord.ToLower()))
//                                    {
//                                        ifMatchSuccess = true;
//                                        break;
//                                    }
//                                }

//                                if (ifMatchSuccess)
//                                {
//                                    break;
//                                }

//                            }
//                        }

//                        if (!ifMatchSuccess && !toKeyWords.IfIsNullOrEmpty())
//                        {
//                            foreach (var keyWord in toKeyWords)
//                            {
//                                foreach (var mailboxAddress in messageSummary.Envelope.To.Mailboxes)
//                                {
//                                    if (mailboxAddress.Address.ToLower().Contains(keyWord.ToLower()))
//                                    {
//                                        ifMatchSuccess = true;
//                                        break;
//                                    }
//                                }

//                                if (ifMatchSuccess)
//                                {
//                                    break;
//                                }
//                            }
//                        }

//                        if (!ifMatchSuccess && !subjectKeyWords.IfIsNullOrEmpty())
//                        {
//                            foreach (var subjectKeyWord in subjectKeyWords)
//                            {
//                                if (messageSummary.Envelope.Subject.ToLower().Contains(subjectKeyWord.ToLower()))
//                                {
//                                    ifMatchSuccess = true;
//                                    break;
//                                }


//                            }
//                        }

//                        if (!ifMatchSuccess && !dateTimes.IfIsNullOrEmpty())
//                        {
//                            foreach (var dateTime in dateTimes)
//                            {
//                                if (messageSummary.Envelope.Date.HasValue && dateTime.Date == messageSummary.Envelope.Date.Value.Date)
//                                {
//                                    ifMatchSuccess = true;
//                                    break;
//                                }
//                            }
//                        }

//                        if (ifMatchSuccess)
//                        {
//                            messageSummaryList.Add(messageSummary);
//                        }
//                    }

//                    //for (int i = messageList.Count - 1; i >= 0; i--)
//                    //{
//                    //    ifMatchSuccess = false;

//                    //    if (!fromKeyWords.IfIsNullOrEmpty())
//                    //    {
//                    //        foreach (var fromKeyWord in fromKeyWords)
//                    //        {

//                    //            foreach (var fromAddress in messageList[i].Envelope.From.Mailboxes)
//                    //            {
//                    //                if (fromAddress.Address.ToLower().Contains(fromKeyWord.ToLower()))
//                    //                {
//                    //                    ifMatchSuccess = true;
//                    //                    break;
//                    //                }
//                    //            }

//                    //            if (ifMatchSuccess)
//                    //            {
//                    //                break;
//                    //            }

//                    //        }
//                    //    }

//                    //    if (!ifMatchSuccess && !toKeyWords.IfIsNullOrEmpty())
//                    //    {
//                    //        foreach (var keyWord in toKeyWords)
//                    //        {
//                    //            foreach (var mailboxAddress in messageList[i].Envelope.To.Mailboxes)
//                    //            {
//                    //                if (mailboxAddress.Address.ToLower().Contains(keyWord.ToLower()))
//                    //                {
//                    //                    ifMatchSuccess = true;
//                    //                    break;
//                    //                }
//                    //            }

//                    //            if (ifMatchSuccess)
//                    //            {
//                    //                break;
//                    //            }
//                    //        }
//                    //    }

//                    //    if (!ifMatchSuccess && !subjectKeyWords.IfIsNullOrEmpty())
//                    //    {
//                    //        foreach (var subjectKeyWord in subjectKeyWords)
//                    //        {
//                    //            if (messageList[i].Envelope.Subject.ToLower().Contains(subjectKeyWord.ToLower()))
//                    //            {
//                    //                ifMatchSuccess = true;
//                    //                break;
//                    //            }


//                    //        }
//                    //    }

//                    //    if (!ifMatchSuccess && !dateTimes.IfIsNullOrEmpty())
//                    //    {
//                    //        foreach (var dateTime in dateTimes)
//                    //        {
//                    //            if (messageList[i].Envelope.Date.HasValue && dateTime.Date == messageList[i].Envelope.Date.Value.Date)
//                    //            {
//                    //                ifMatchSuccess = true;
//                    //                break;
//                    //            }
//                    //        }
//                    //    }

//                    //    if (ifMatchSuccess)
//                    //    {
//                    //        messageSummaryList.Add(messageList[i]);
//                    //    }

//                    //}

//                    //if (!messageSummaryList.IfIsNullOrEmpty())
//                    //{
//                    //    messageList = messageSummaryList;
//                    //}

//                    messageList = messageSummaryList;

//                }


//                foreach (var messageSummary in messageList)
//                {

//                    var message = inbox.GetMessage(messageSummary.UniqueId);

//                    MailInfoModel mailInfoModel = new MailInfoModel
//                    {
//                        ID = messageSummary.UniqueId.Id,
//                        MessageId = message.MessageId,
//                        From = messageSummary.Envelope.From.Mailboxes.Select(o => new MailAddressModel { Address = o.Address, Name = o.Name }).ToList(),
//                        To = messageSummary.Envelope.To.Mailboxes.Select(o => new MailAddressModel { Address = o.Address, Name = o.Name }).ToList(),
//                        DateTime = message.Date.DateTime,
//                        HtmlBody = message.HtmlBody,
//                        Subject = message.Subject,
//                        TextBody = message.TextBody,
//                    };

//                    List<MailAttachmentInfoModel> mailAttachmentList = new List<MailAttachmentInfoModel>();

//                    foreach (var attachment in message.Attachments.OfType<MimePart>())
//                    {
//                        MailAttachmentInfoModel mailAttachmentInfo = new MailAttachmentInfoModel();
//                        mailAttachmentInfo.FileName = attachment.FileName;
//                        mailAttachmentInfo.LocalFileFullPath = Path.Combine(saveAttachmentLocalFolderFullPath, userName, string.Format("[{0}].{1}", mailInfoModel.ID, mailAttachmentInfo.FileName));
//                        mailAttachmentList.Add(mailAttachmentInfo);

//                        PathFunctions.InitDirectoryPath(mailAttachmentInfo.LocalFileFullPath);

//                        using (var stream = File.Create(mailAttachmentInfo.LocalFileFullPath))
//                        {
//                            attachment.ContentObject.DecodeTo(stream);
//                        }

//                    }

//                    mailInfoModel.Attachments = mailAttachmentList;
//                    resultList.Add(mailInfoModel);

//                }


//                client.Disconnect(true);

//            }


//            return resultList;

//        }


//        #endregion


//        /// <summary>
//        /// 检查邮件来源地址 是否在 邮件地址白名单中
//        /// </summary>
//        /// <param name="fromMailAddress">要效验的邮件来源邮箱地址</param>
//        /// <param name="fromMailAddressWhiteList">邮件来源地址白名单</param>
//        /// <param name="separator">白名单分隔符 默认值 ","</param>
//        /// <returns></returns>
//        public static bool CheckFromMailAddressByWhiteList(string fromMailAddress, string fromMailAddressWhiteList, string separator = ",")
//        {
//            const string formatStr = "{0}{1}{0}";
//            return string.Format(formatStr, separator, fromMailAddressWhiteList).ToLower().Contains(string.Format(formatStr, separator, fromMailAddress).ToLower());
//        }


//    }
//}
