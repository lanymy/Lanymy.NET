using System;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 快捷方式操作结果。
    /// </summary>
    public class ShortcutOperationResultModel
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
        /// 是否成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 操作异常。
        /// </summary>
        public Exception Exception { get; set; }
    }
}
