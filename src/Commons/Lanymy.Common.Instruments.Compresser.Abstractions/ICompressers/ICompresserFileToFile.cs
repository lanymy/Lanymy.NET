using System.Threading.Tasks;

namespace Lanymy.Common.Instruments.ICompressers
{
    /// <summary>
    /// 压缩器 文件 和 文件 压缩 功能 接口
    /// </summary>
    public interface ICompresserFileToFile
    {

        /// <summary>
        /// 压缩源文件
        /// </summary>
        /// <param name="sourceFileFullPath">要压缩源文件的全路径</param>
        /// <param name="compressFileFullPath">压缩后文件全路径</param>
        /// <returns></returns>
        void CompressSourceFileToCompressFile(string sourceFileFullPath, string compressFileFullPath);

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="sourceFileFullPath">解压缩后 源文件 全路径</param>
        /// <param name="compressFileFullPath">要解压缩的文件</param>
        void DecompressSourceFileFromCompressFile(string sourceFileFullPath, string compressFileFullPath);
        /// <summary>
        /// 异步压缩源文件
        /// </summary>
        /// <param name="sourceFileFullPath">要压缩源文件的全路径</param>
        /// <param name="compressFileFullPath">压缩后文件全路径</param>
        /// <returns></returns>
        Task CompressSourceFileToCompressFileAsync(string sourceFileFullPath, string compressFileFullPath);
        /// <summary>
        /// 异步解压缩文件
        /// </summary>
        /// <param name="sourceFileFullPath">解压缩后 源文件 全路径</param>
        /// <param name="compressFileFullPath">要解压缩的文件</param>
        Task DecompressSourceFileFromCompressFileAsync(string sourceFileFullPath, string compressFileFullPath);


    }
}
