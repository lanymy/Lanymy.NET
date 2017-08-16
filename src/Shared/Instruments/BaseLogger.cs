using System;
using System.Collections.Generic;
using System.Text;
using Lanymy.General.Extension.Interfaces;
using Lanymy.General.Extension.Models;

namespace Lanymy.General.Extension.Instruments
{
    /// <summary>
    /// 日志操作器 基类
    /// </summary>
    public abstract class BaseLogger : ILogger
    {




        /// <summary>
        /// 写日志消息
        /// </summary>
        /// <typeparam name="T">日志消息类型</typeparam>
        /// <param name="logMessageType">日志类别</param>
        /// <param name="message">日志消息</param>
        public virtual void WriteLogMessage<T>(LogMessageTypeEnum logMessageType, T message)
        {
            WriteLogMessage<T>(logMessageType, message, null);
        }



        /// <summary>
        /// 写日志消息
        /// </summary>
        /// <typeparam name="T">日志消息类型</typeparam>
        /// <param name="logMessageType">日志类别</param>
        /// <param name="message">日志消息</param>
        /// <param name="ex">异常</param>
        public virtual void WriteLogMessage<T>(LogMessageTypeEnum logMessageType, T message, Exception ex)
        {
            switch (logMessageType)
            {
                case LogMessageTypeEnum.Debug:
                    //action = Debug;
                    Debug(message, ex);
                    break;
                case LogMessageTypeEnum.Info:
                    //action = Info;
                    Info(message, ex);
                    break;
                case LogMessageTypeEnum.Warn:
                    //action = Warn;
                    Warn(message, ex);
                    break;
                case LogMessageTypeEnum.Error:
                    //action = Error;
                    Error(message, ex);
                    break;
                case LogMessageTypeEnum.Fatal:
                    //action = Fatal;
                    Fatal(message, ex);
                    break;
                default:
                    //action = Debug;
                    Debug(message, ex);
                    break;
            }

        }


        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public abstract void Debug(object message);


        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public abstract void Debug(object message, Exception ex);



        /// <summary>
        /// 内容消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public abstract void Info(object message);

        /// <summary>
        /// 内容消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public abstract void Info(object message, Exception ex);


        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public abstract void Warn(object message);

        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public abstract void Warn(object message, Exception ex);



        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public abstract void Error(object message);

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public abstract void Error(object message, Exception ex);


        /// <summary>
        /// 致命消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        public abstract void Fatal(object message);

        /// <summary>
        /// 致命消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        public abstract void Fatal(object message, Exception ex);


    }
}
