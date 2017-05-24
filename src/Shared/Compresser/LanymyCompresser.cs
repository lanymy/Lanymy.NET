/********************************************************************

时间: 2017年05月24日, PM 07:22:20

作者: lanyanmiyu@qq.com

描述: 默认压缩器

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;

namespace Lanymy.General.Extension.Compresser
{


    /// <summary>
    /// 默认压缩器
    /// </summary>
    public class LanymyCompresser: ICompresser
    {

        /// <summary>
        /// 编码
        /// </summary>
        public readonly Encoding CurrentEncoding = GlobalSettings.DEFAULT_ENCODING;

        /// <summary>
        /// 默认压缩器 构造方法
        /// </summary>
        /// <param name="encoding">编码</param>
        public LanymyCompresser(Encoding encoding = null)
        {
            if (!encoding.IfIsNullOrEmpty())
            {
                CurrentEncoding = encoding;
            }
        }

        /// <summary>
        /// 压缩字节数组 返回压缩后的 字节数组
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <returns></returns>
        public virtual byte[] CompressBytesToBytes(byte[] compressBytes)
        {

            if (compressBytes.IfIsNullOrEmpty())
                throw new ArgumentNullException(nameof(compressBytes));

            byte[] result;

            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream compressZipStream = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    compressZipStream.Write(compressBytes, 0, compressBytes.Length);
                }

                result = ms.ToArray();

            }

            return result;
        }

        /// <summary>
        /// 解压缩字节数组 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <returns></returns>
        public virtual byte[] DecompressBytesFromBytes(byte[] decompressBytes)
        {
            if (decompressBytes.IfIsNullOrEmpty())
                throw new ArgumentNullException(nameof(decompressBytes));

            byte[] result = null;

            using (MemoryStream ms = new MemoryStream(), decompressSourceMemoryStream = new MemoryStream(decompressBytes))
            {

                using (GZipStream decompressZipStream = new GZipStream(decompressSourceMemoryStream, CompressionMode.Decompress))
                {
                    decompressZipStream.CopyTo(ms);
                    result = ms.ToArray();
                }

            }

            return result;
        }
        /// <summary>
        /// 异步 压缩字节数组 返回压缩后的 字节数组
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <returns></returns>
        public virtual Task<byte[]> CompressBytesToBytesAsync(byte[] compressBytes)
        {
#if NET40
            return new Task<byte[]>(()=> CompressBytesToBytes(compressBytes));
#else
            return Task.FromResult(CompressBytesToBytes(compressBytes));
#endif
        }
        /// <summary>
        /// 异步 解压缩字节数组 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <returns></returns>
        public virtual Task<byte[]> DecompressBytesFromBytesAsync(byte[] decompressBytes)
        {
#if NET40
            return new Task<byte[]>(() => DecompressBytesFromBytes(decompressBytes));
#else
            return Task.FromResult(DecompressBytesFromBytes(decompressBytes));
#endif
        }
        /// <summary>
        /// 压缩字节数组 返回 压缩后字节数组生成的 Base64 字符串
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <returns></returns>
        public virtual string CompressBytesToBase64String(byte[] compressBytes)
        {
            return Convert.ToBase64String(CompressBytesToBytes(compressBytes));
        }
        /// <summary>
        /// 解压缩 Base64 字符串 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressString"></param>
        /// <returns></returns>
        public virtual byte[] DecompressBytesFromBase64String(string decompressString)
        {
            return DecompressBytesFromBytes(Convert.FromBase64String(decompressString));
        }
        /// <summary>
        /// 异步 压缩字节数组 返回 压缩后字节数组生成的 Base64 字符串
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <returns></returns>
        public virtual Task<string> CompressBytesToBase64StringAsync(byte[] compressBytes)
        {
#if NET40
            return new Task<string>(() => CompressBytesToBase64String(compressBytes));
#else
            return Task.FromResult(CompressBytesToBase64String(compressBytes));
#endif
        }
        /// <summary>
        /// 异步 解压缩 Base64 字符串 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressString"></param>
        /// <returns></returns>
        public virtual Task<byte[]> DecompressBytesFromBase64StringAsync(string decompressString)
        {
#if NET40
            return new Task<byte[]>(() => DecompressBytesFromBase64String(decompressString));
#else
            return Task.FromResult(DecompressBytesFromBase64String(decompressString));
#endif
        }
        /// <summary>
        /// 压缩字符串 返回 压缩后的 字节数组
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual byte[] CompressStringToBytes(string compressString, Encoding encoding = null)
        {
            if (encoding.IfIsNullOrEmpty()) encoding = CurrentEncoding;
            return CompressBytesToBytes(encoding.GetBytes(compressString));
        }
        /// <summary>
        /// 解压缩字节数组 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual string DecompressStringFromBytes(byte[] decompressBytes, Encoding encoding = null)
        {
            if (encoding.IfIsNullOrEmpty()) encoding = CurrentEncoding;
            byte[] result = DecompressBytesFromBytes(decompressBytes);
            return encoding.GetString(result, 0, result.Length);
        }
        /// <summary>
        /// 异步 压缩字符串 返回 压缩后的 字节数组
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual Task<byte[]> CompressStringToBytesAsync(string compressString, Encoding encoding = null)
        {
#if NET40
            return new Task<byte[]>(() => CompressStringToBytes(compressString, encoding));
#else
            return Task.FromResult(CompressStringToBytes(compressString, encoding));
#endif
        }
        /// <summary>
        /// 异步 解压缩字节数组 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual Task<string> DecompressStringFromBytesAsync(byte[] decompressBytes, Encoding encoding = null)
        {
#if NET40
            return new Task<string>(() => DecompressStringFromBytes(decompressBytes, encoding));
#else
            return Task.FromResult(DecompressStringFromBytes(decompressBytes, encoding));
#endif
        }
        /// <summary>
        /// 压缩字符串 返回 压缩后的Base64 字符串
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual string CompressStringToBase64String(string compressString, Encoding encoding = null)
        {
            return Convert.ToBase64String(CompressStringToBytes(compressString, encoding));
        }
        /// <summary>
        /// 解压缩 Base64 字符串 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressString">要解压缩的Base64字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual string DecompressStringFromBase64String(string decompressString, Encoding encoding = null)
        {
            return DecompressStringFromBytes(Convert.FromBase64String(decompressString), encoding);
        }
        /// <summary>
        /// 异步 压缩字符串 返回 压缩后的Base64 字符串
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual Task<string> CompressStringToBase64StringAsync(string compressString, Encoding encoding = null)
        {
#if NET40
            return new Task<string>(() => CompressStringToBase64String(compressString, encoding));
#else
            return Task.FromResult(CompressStringToBase64String(compressString, encoding));
#endif
        }
        /// <summary>
        /// 异步 解压缩 Base64 字符串 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressString">要解压缩的Base64字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual Task<string> DecompressStringFromBase64StringAsync(string decompressString, Encoding encoding = null)
        {
#if NET40
            return new Task<string>(() => DecompressStringFromBase64String(decompressString, encoding));
#else
            return Task.FromResult(DecompressStringFromBase64String(decompressString, encoding));
#endif
        }


    }


}
