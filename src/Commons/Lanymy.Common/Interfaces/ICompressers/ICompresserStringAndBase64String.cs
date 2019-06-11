using System.Text;
using System.Threading.Tasks;

namespace Lanymy.Common.Interfaces.ICompressers
{
    /// <summary>
    /// 压缩器 字符串 和 Base64字符串 压缩 功能 接口
    /// </summary>
    public interface ICompresserStringAndBase64String
    {
        /// <summary>
        /// 压缩字符串 返回 压缩后的Base64 字符串
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        string CompressStringToBase64String(string compressString, Encoding encoding = null);

        /// <summary>
        /// 解压缩 Base64 字符串 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressString">要解压缩的Base64字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        string DecompressStringFromBase64String(string decompressString, Encoding encoding = null);
        /// <summary>
        /// 异步 压缩字符串 返回 压缩后的Base64 字符串
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        Task<string> CompressStringToBase64StringAsync(string compressString, Encoding encoding = null);

        /// <summary>
        /// 异步 解压缩 Base64 字符串 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressString">要解压缩的Base64字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        Task<string> DecompressStringFromBase64StringAsync(string decompressString, Encoding encoding = null);

    }
}
