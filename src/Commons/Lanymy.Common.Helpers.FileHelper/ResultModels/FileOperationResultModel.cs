using System;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 文件或目录操作结果。
    /// </summary>
    public class FileOperationResultModel
    {
        /// <summary>
        /// 源路径。
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// 目标路径。
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// 是否执行成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 操作异常。
        /// </summary>
        public Exception Exception { get; set; }
    }
}
