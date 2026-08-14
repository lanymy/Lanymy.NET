using System;
using Lanymy.Common.Abstractions.ResultModels;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 二进制文件序列化/反序列化结果。
    /// </summary>
    /// <typeparam name="T">关联的模型类型</typeparam>
    public class BinaryFileSerializationResultModel<T> : CommonResultModel where T : class
    {
        /// <summary>
        /// 目标文件路径。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 是否启用了压缩或解压缩流程。
        /// </summary>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// 参与文件读写的字节数。
        /// </summary>
        public int? BytesLength { get; set; }

        /// <summary>
        /// 本次操作关联的模型。
        /// </summary>
        public T Model { get; set; }
    }
}
