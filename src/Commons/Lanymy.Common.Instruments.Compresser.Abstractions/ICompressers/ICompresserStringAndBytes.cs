using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.ICompressers
{
    /// <summary>
    /// 压缩器 字符串 和 字节数组 压缩 功能 接口
    /// </summary>
    public interface ICompresserStringAndBytes
    {
        /// <summary>
        /// 压缩字符串 返回 压缩后的 字节数组
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        byte[] CompressStringToBytes(string compressString, Encoding encoding = null);
        /// <summary>
        /// 解压缩字节数组 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        string DecompressStringFromBytes(byte[] decompressBytes, Encoding encoding = null);
        /// <summary>
        /// 异步 压缩字符串 返回 压缩后的 字节数组
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        Task<byte[]> CompressStringToBytesAsync(string compressString, Encoding encoding = null);
        /// <summary>
        /// 异步 解压缩字节数组 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        Task<string> DecompressStringFromBytesAsync(byte[] decompressBytes, Encoding encoding = null);
    }
}
