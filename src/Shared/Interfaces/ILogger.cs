using System;
using System.Collections.Generic;
using System.Text;
using Lanymy.General.Extension.Models;

namespace Lanymy.General.Extension.Interfaces
{


    /// <summary>
    /// 日志功能接口
    /// </summary>
    public interface ILogger
    {


        /// <summary>
        /// 写日志消息
        /// </summary>
        /// <typeparam name="T">日志消息类型</typeparam>
        /// <param name="logMessageType">日志类别</param>
        /// <param name="message">日志消息</param>
        void WriteLogMessage<T>(LogMessageTypeEnum logMessageType, T message);


        /// <summary>
        /// 写日志消息
        /// </summary>
        /// <typeparam name="T">日志消息类型</typeparam>
        /// <param name="logMessageType">日志类别</param>
        /// <param name="message">日志消息</param>
        /// <param name="ex">异常</param>
        void WriteLogMessage<T>(LogMessageTypeEnum logMessageType, T message, Exception ex);



        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        void Debug(object message);
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        void Debug(object message, Exception ex);


        /// <summary>
        /// 内容消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        void Info(object message);

        /// <summary>
        /// 内容消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        void Info(object message, Exception ex);


        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message">日志消息内容</param>
        void Warn(object message);

        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        void Warn(object message, Exception ex);





        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">日志消息内容</param>
        void Error(object message);
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        void Error(object message, Exception ex);



        /// <summary>
        /// 致命消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        void Fatal(object message);
        /// <summary>
        /// 致命消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="ex">异常对象</param>
        void Fatal(object message, Exception ex);


    }
}
