using System;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// AES 模型文件加解密结果。
    /// </summary>
    /// <typeparam name="T">模型类型。</typeparam>
    public class SecurityAesFileOperationResultModel<T> where T : class
    {
        /// <summary>
        /// 文件路径。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 是否加密操作。
        /// </summary>
        public bool IsEncryptOperation { get; set; }

        /// <summary>
        /// 关联模型。
        /// </summary>
        public T Model { get; set; }

        /// <summary>
        /// 模型类型名称。
        /// </summary>
        public string ModelTypeName { get; set; }

        /// <summary>
        /// 模型类型全名。
        /// </summary>
        public string ModelTypeFullName { get; set; }

        /// <summary>
        /// 是否执行成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 相关异常。
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 内容级失败说明。
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 目标文件是否存在。
        /// </summary>
        public bool TargetFileExists { get; set; }
    }
}
