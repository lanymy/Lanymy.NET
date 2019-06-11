using System;

namespace Lanymy.Common.Instruments.Logger
{
    /// <summary>
    /// log4net 日志操作器
    /// </summary>
    public class Log4NetLogger : BaseLog4NetLogger
    {


        //protected readonly ILog _Logger;

        //protected readonly string _ConfigFileFullPath;




        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="configFileFullPath">log4net.config文件全路径</param>
        public Log4NetLogger(string configFileFullPath, string loggerName = null) : base(configFileFullPath, loggerName)
        {

        }


        /// <summary>
        /// 堆栈信息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public override void Trace(object message)
        {
            Debug(message);
        }

        /// <summary>
        /// 堆栈信息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public override void Trace(object message, Exception ex)
        {
            Debug(message, ex);
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
