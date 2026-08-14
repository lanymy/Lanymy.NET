using System;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// JSON 文件序列化/反序列化结果。
    /// </summary>
    /// <typeparam name="T">关联的模型类型</typeparam>
    public class JsonFileSerializationResultModel<T> where T : class
    {
        /// <summary>
        /// 目标文件路径。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 是否执行成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 操作异常。
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// JSON 文本内容。
        /// </summary>
        public string JsonContent { get; set; }

        /// <summary>
        /// 本次操作关联的模型。
        /// </summary>
        public T Model { get; set; }
    }
}
