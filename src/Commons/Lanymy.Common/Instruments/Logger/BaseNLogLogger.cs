using System.IO;
using System.Linq;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using NLog;
using NLog.Config;

namespace Lanymy.Common.Instruments.Logger
{


    /// <summary>
    /// NLog日志操作器 抽象基类
    /// </summary>
    public abstract class BaseNLogLogger : BaseLogger
    {


        protected readonly ILogger _Logger;

        /// <summary>
        /// NLog日志操作器 抽象基类 构造方法
        /// </summary>
        /// <param name="configFileFullPath">NLog.config 配置表文件 全路径</param>
        /// <param name="loggerName">日志操作器名称 默认 null</param>
        protected BaseNLogLogger(string configFileFullPath, string loggerName = null) : base(configFileFullPath)
        {

            var nLoggingConfiguration = LogManager.Configuration;

            bool isRightConfigFileFullPath;

            if (nLoggingConfiguration.IfIsNullOrEmpty())
            {
                isRightConfigFileFullPath = false;
            }
            else
            {
                var currentConfigFileFullPath = nLoggingConfiguration.FileNamesToWatch.FirstOrDefault();
                //Directory.GetDirectories(currentConfigFileFullPath);
                isRightConfigFileFullPath = Path.GetDirectoryName(currentConfigFileFullPath).EndsWith(Path.DirectorySeparatorChar + DefaultFolderNameKeys.CONFIG_FOLDER_NAME);
            }



            if (!isRightConfigFileFullPath)
            {
                LogManager.Configuration = new XmlLoggingConfiguration(configFileFullPath);
            }



            if (nLoggingConfiguration.IfIsNullOrEmpty())
            {
                LogManager.Configuration = new XmlLoggingConfiguration(configFileFullPath);
            }
            _Logger = loggerName.IfIsNullOrEmpty() ? LogManager.GetCurrentClassLogger() : LogManager.GetLogger(loggerName);
            //CurrentLoggerName = _Logger.Name;

        }


        protected string GetLogMessageString(object message)
        {

            if (message is string messageString)
            {
                return messageString;
            }

            return SerializeHelper.SerializeToJson(message);

        }


    }

}
