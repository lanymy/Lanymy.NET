using System;

namespace Lanymy.Common.Instruments.Logger
{


    public class NLogLogger : BaseNLogLogger
    {

        public NLogLogger(string configFileFullPath, string loggerName = null) : base(configFileFullPath, loggerName)
        {

        }

        /// <summary>
        /// 堆栈信息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public override void Trace(object message)
        {
            _Logger.Trace(GetLogMessageString(message));
        }

        /// <summary>
        /// 堆栈信息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public override void Trace(object message, Exception ex)
        {
            _Logger.Trace(ex, GetLogMessageString(message));
        }

        public override void Debug(object message)
        {
            _Logger.Debug(GetLogMessageString(message));
        }

        public override void Debug(object message, Exception ex)
        {

            _Logger.Debug(ex, GetLogMessageString(message));

        }

        public override void Info(object message)
        {
            _Logger.Info(GetLogMessageString(message));
        }

        public override void Info(object message, Exception ex)
        {
            _Logger.Info(ex, GetLogMessageString(message));
        }

        public override void Warn(object message)
        {
            _Logger.Warn(GetLogMessageString(message));
        }

        public override void Warn(object message, Exception ex)
        {
            _Logger.Warn(ex, GetLogMessageString(message));
        }

        public override void Error(object message)
        {
            _Logger.Error(GetLogMessageString(message));
        }

        public override void Error(object message, Exception ex)
        {
            _Logger.Error(ex, GetLogMessageString(message));
        }

        public override void Fatal(object message)
        {
            _Logger.Fatal(GetLogMessageString(message));
        }

        public override void Fatal(object message, Exception ex)
        {
            _Logger.Fatal(ex, GetLogMessageString(message));
        }

    }

}
