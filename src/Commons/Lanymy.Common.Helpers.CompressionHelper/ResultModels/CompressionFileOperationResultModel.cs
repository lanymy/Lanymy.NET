using System;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 文件压缩/解压操作结果。
    /// </summary>
    public class CompressionFileOperationResultModel
    {
        /// <summary>
        /// 源文件路径。
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// 目标文件路径。
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// 是否压缩操作。
        /// </summary>
        public bool IsCompressOperation { get; set; }

        /// <summary>
        /// 是否执行成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 相关异常。
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 目标文件是否存在。
        /// </summary>
        public bool TargetFileExists { get; set; }
    }
}
