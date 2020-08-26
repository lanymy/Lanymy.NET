using System;

namespace Lanymy.Common.Instruments.ResultModels
{

    /// <summary>
    /// CMD 执行 返回信息 实体类 基类
    /// </summary>
    public abstract class BaseCmdResultModel
    {

        /// <summary>
        /// 标识当前执行命令的ID
        /// </summary>
        public Guid CmdID { get; set; }

        /// <summary>
        /// 命令是否执行成功
        /// </summary>
        //public bool IsSuccess => ErrorDataString.IfIsNullOrEmpty();
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 执行的命令行完整字符串
        /// </summary>
        public string ExecuteCommandString { get; set; }

        /// <summary>
        /// 命令行返回的正常输出数据字符串
        /// </summary>
        public string OutputDataString { get; set; }


        /// <summary>
        /// 命令行返回的异常输出数据字符串
        /// </summary>
        public string ErrorDataString { get; set; }

        /// <summary>
        /// 开始执行命令时间戳
        /// </summary>
        public DateTime CmdStartDateTime { get; set; }

        /// <summary>
        /// 命令执行完毕时间戳
        /// </summary>
        public DateTime CmdEndDateTime { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }


        /// <summary>
        /// 获取完整输出信息 包括正常输出信息和异常输出信息
        /// </summary>
        /// <returns></returns>
        public string GetFullDataString()
        {
            return string.Format("{0}{1}{2}", OutputDataString, Environment.NewLine, ErrorDataString);
        }

    }

}
