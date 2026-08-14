using Lanymy.Common.Abstractions.ResultModels;

namespace Lanymy.Common.Helpers.ResultModels
{
    /// <summary>
    /// 二进制文件读取结果。
    /// </summary>
    public class BinaryFileReadResultModel : CommonResultModel
    {
        /// <summary>
        /// 读取目标文件路径。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 读取到的字节数组。
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// 读取到的字节长度。
        /// </summary>
        public int? BytesLength { get; set; }
    }
}
