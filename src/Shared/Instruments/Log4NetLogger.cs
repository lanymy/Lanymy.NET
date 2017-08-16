using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension.Instruments
{
    /// <summary>
    /// log4net 日志操作器
    /// </summary>
    public class Log4NetLogger : BaseLogger
    {


        protected readonly ILog _Logger;


        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="configFileFullPath">log4net.config文件全路径</param>
        public Log4NetLogger(string configFileFullPath = null, string loggerName = null)
        {
            if (!configFileFullPath.IfIsNullOrEmpty())
            {

                if (!File.Exists(configFileFullPath))
                {
                    throw new FileNotFoundException(configFileFullPath);
                }
           
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(configFileFullPath));

            }

            _Logger = loggerName.IfIsNullOrEmpty() ? LogManager.GetLogger(this.GetType()) : LogManager.GetLogger(loggerName);

        }



        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public override void Debug(object message)
        {
            _Logger.Debug(message);
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public override void Debug(object message, Exception ex)
        {
            _Logger.Debug(message, ex);
        }

        /// <summary>
        /// 内容消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public override void Info(object message)
        {
            _Logger.Info(message);
        }

        /// <summary>
        /// 内容消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public override void Info(object message, Exception ex)
        {
            _Logger.Info(message, ex);
        }

        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public override void Warn(object message)
        {
            _Logger.Warn(message);
        }

        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public override void Warn(object message, Exception ex)
        {
            _Logger.Warn(message, ex);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public override void Error(object message)
        {
            _Logger.Error(message);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public override void Error(object message, Exception ex)
        {
            _Logger.Error(message, ex);
        }


        /// <summary>
        /// 致命消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public override void Fatal(object message)
        {
            _Logger.Fatal(message);
        }


        /// <summary>
        /// 致命消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public override void Fatal(object message, Exception ex)
        {
            _Logger.Fatal(message, ex);
        }

    }
}
