using System.IO;
using System.Reflection;
using System.Xml;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using log4net;
using log4net.Config;

namespace Lanymy.Common.Instruments.Logger
{



    /// <summary>
    /// Log4Net 日志组件 基类
    /// </summary>
    public abstract class BaseLog4NetLogger : BaseLogger
    {

        protected readonly ILog _Logger;


        protected BaseLog4NetLogger(string configFileFullPath, string loggerName = null) : base(configFileFullPath)
        {

            var repository = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            //在.net core 版本中 File log4net.config 配置表中  FileAppender 中 <param name= "File" value= "Logs/"/> 文件夹属性 相对路径 在vs 开发环境中 失效
            //因此 log4net.config 配置表中  FileAppender 中 <param name= "File" value= "Logs/"/> 文件夹属性 在内存中(不覆盖实体配置表文件)动态修改为配置表文件的绝对路径 统一行为


            var configXml = new XmlDocument();
            var settings = new XmlReaderSettings
            {
                //忽略文档里面的注释
                IgnoreComments = true
            };
            using (var reader = XmlReader.Create(configFileFullPath, settings))
            {
                configXml.Load(reader);
            }

            var nodesList = configXml.SelectNodes("//appender");

            foreach (XmlNode node in nodesList)
            {

                var typeAttribute = node.Attributes["type"];
                //log4net.Appender.FileAppender
                //log4net.Appender.RollingFileAppender
                if (!typeAttribute.IfIsNullOrEmpty() && typeAttribute.Value.EndsWith("FileAppender"))
                {

                    XmlNode fileXmlNode = node.SelectSingleNode("param[@name='File']");

                    if (!fileXmlNode.IfIsNullOrEmpty())
                    {
                        var paramValueAttribute = fileXmlNode.Attributes["value"];
                        paramValueAttribute.Value = Path.Combine(PathHelper.GetCallDomainPath(), paramValueAttribute.Value);
                    }
                    else
                    {

                        var paramElement = configXml.CreateElement("param");
                        paramElement.SetAttribute("name", "File");
                        paramElement.SetAttribute("value", Path.Combine(PathHelper.GetCallDomainPath(), DefaultFolderNameKeys.DEFAULT_LOG_FILES_FOLDER_NAME) + Path.DirectorySeparatorChar);

                        (node as XmlElement).PrependChild(paramElement);

                    }

                }

            }

            //configXml.Save(Path.Combine(Path.GetDirectoryName(configFileFullPath), string.Format("[{0}]", DateTime.Now.ToString(GlobalSettings.DEFAULT_HTTP_URI_DATE_FORMAT_STRING)) + Path.GetFileName(configFileFullPath)));


            //XmlConfigurator.Configure(repository, ToXmlDocument(configXml).DocumentElement);
            XmlConfigurator.Configure(repository, configXml.DocumentElement);
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(repo, new FileInfo(configFileFullPath));

            _Logger = loggerName.IfIsNullOrEmpty() ? LogManager.GetLogger(this.GetType()) : LogManager.GetLogger(repository.Name, loggerName);

            //CurrentLoggerName = _Logger.Logger.Name;


        }


        ///// <summary>
        ///// Converts a XDocument object into XmlDocument
        ///// </summary>
        ///// <param name="xDocument">The x document.</param>
        ///// <returns>The XDocument converted to XmlDocument</returns>
        //public static XmlDocument ToXmlDocument(XmlDocument xDocument)
        //{
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        xDocument.Save(memoryStream);
        //        memoryStream.Seek(0, SeekOrigin.Begin);
        //        var xmlDoc = new XmlDocument();
        //        xmlDoc.Load(memoryStream);
        //        return xmlDoc;
        //    }
        //}

    }


}
