using System;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 进程启动/执行结果信息。
    /// </summary>
    public class ProcessResultModel
    {
        /// <summary>
        /// 启动的应用程序全路径或命令名。
        /// </summary>
        public string ApplicationFileFullPath { get; set; }

        /// <summary>
        /// 启动参数字符串。
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// 是否已成功启动进程。
        /// </summary>
        public bool IsStarted { get; set; }

        /// <summary>
        /// 进程是否已退出。
        /// </summary>
        public bool HasExited { get; set; }

        /// <summary>
        /// 进程 ID。仅在成功启动时可用。
        /// </summary>
        public int? ProcessId { get; set; }

        /// <summary>
        /// 进程退出码。仅在已退出时可用。
        /// </summary>
        public int? ExitCode { get; set; }

        /// <summary>
        /// 启动或等待期间捕获到的异常。
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 是否成功。若仅启动未等待，则表示成功启动；
        /// 若已等待退出，则同时要求退出码为 0。
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return Exception == null
                       && IsStarted
                       && (!HasExited || ExitCode.GetValueOrDefault() == 0);
            }
        }
    }
}
