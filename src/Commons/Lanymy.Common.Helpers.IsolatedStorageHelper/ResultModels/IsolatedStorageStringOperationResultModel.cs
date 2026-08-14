using System;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 独立存储字符串读写结果。
    /// </summary>
    public class IsolatedStorageStringOperationResultModel
    {
        /// <summary>
        /// 请求的逻辑标识。
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 是否为写入操作。
        /// </summary>
        public bool IsWriteOperation { get; set; }

        /// <summary>
        /// 是否执行成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 操作异常。
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 内容级失败说明。
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 实际使用的存储文件名。
        /// </summary>
        public string StorageFileName { get; set; }

        /// <summary>
        /// 文件系统模式下的物理文件路径。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 是否使用文件系统模式保存。
        /// </summary>
        public bool IfUsesCustomIsolatedStorageMode { get; set; }

        /// <summary>
        /// 存储文件是否存在。
        /// </summary>
        public bool StorageFileExists { get; set; }

        /// <summary>
        /// 编码名称。
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// 是否使用默认编码。
        /// </summary>
        public bool UsedDefaultEncoding { get; set; }

        /// <summary>
        /// 是否使用默认密钥。
        /// </summary>
        public bool UsedDefaultSecurityKey { get; set; }

        /// <summary>
        /// 明文字符串内容。
        /// </summary>
        public string SourceString { get; set; }

        /// <summary>
        /// 当前存储实现类型名称。
        /// </summary>
        public string StorageImplementationTypeName { get; set; }
    }
}
