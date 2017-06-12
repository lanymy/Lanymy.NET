/********************************************************************

时间: 2016年05月24日, PM 07:09:09

作者: lanyanmiyu@qq.com

描述: 通过微软压缩API 压缩 字符串 字节数组

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.Instruments.Compresser;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;


namespace Lanymy.General.Extension
{


    /// <summary>
    /// 通过微软压缩API 压缩 字符串 字节数组
    /// </summary>
    public class CompressionFunctions
    {


        /// <summary>
        /// 默认压缩器
        /// </summary>
        public static readonly ICompresser DefaultCompresser = new LanymyCompresser();


        /// <summary>
        /// 压缩字节数组 返回压缩后的 字节数组
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <param name="compresserBytesAndBytes">压缩器 字节数组 压缩 功能 接口</param>
        /// <returns></returns>
        public static byte[] CompressBytesToBytes(byte[] compressBytes, ICompresserBytesAndBytes compresserBytesAndBytes = null)
        {
            return GenericityFunctions.GetInterface(compresserBytesAndBytes, DefaultCompresser).CompressBytesToBytes(compressBytes);
        }

        /// <summary>
        /// 解压缩字节数组 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <param name="compresserBytesAndBytes">压缩器 字节数组 压缩 功能 接口</param>
        /// <returns></returns>
        public static byte[] DecompressBytesFromBytes(byte[] decompressBytes, ICompresserBytesAndBytes compresserBytesAndBytes = null)
        {
            return GenericityFunctions.GetInterface(compresserBytesAndBytes, DefaultCompresser).DecompressBytesFromBytes(decompressBytes);
        }
        /// <summary>
        /// 异步 压缩字节数组 返回压缩后的 字节数组
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <param name="compresserBytesAndBytes">压缩器 字节数组 压缩 功能 接口</param>
        /// <returns></returns>
        public static Task<byte[]> CompressBytesToBytesAsync(byte[] compressBytes, ICompresserBytesAndBytes compresserBytesAndBytes = null)
        {
            return GenericityFunctions.GetInterface(compresserBytesAndBytes, DefaultCompresser).CompressBytesToBytesAsync(compressBytes);
        }


        /// <summary>
        /// 异步 解压缩字节数组 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <param name="compresserBytesAndBytes">压缩器 字节数组 压缩 功能 接口</param>
        /// <returns></returns>
        public static Task<byte[]> DecompressBytesFromBytesAsync(byte[] decompressBytes, ICompresserBytesAndBytes compresserBytesAndBytes = null)
        {
            return GenericityFunctions.GetInterface(compresserBytesAndBytes, DefaultCompresser).DecompressBytesFromBytesAsync(decompressBytes);
        }


        /// <summary>
        /// 压缩字节数组 返回 压缩后字节数组生成的 Base64 字符串
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <param name="compresserBytesAndBase64String">压缩器 字节数组 和 Base64字符串 压缩 功能 接口</param>
        /// <returns></returns>
        public static string CompressBytesToBase64String(byte[] compressBytes, ICompresserBytesAndBase64String compresserBytesAndBase64String = null)
        {
            return GenericityFunctions.GetInterface(compresserBytesAndBase64String, DefaultCompresser).CompressBytesToBase64String(compressBytes);
        }
        /// <summary>
        /// 解压缩 Base64 字符串 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressString"></param>
        /// <param name="compresserBytesAndBase64String">压缩器 字节数组 和 Base64字符串 压缩 功能 接口</param>
        /// <returns></returns>
        public static byte[] DecompressBytesFromBase64String(string decompressString, ICompresserBytesAndBase64String compresserBytesAndBase64String = null)
        {
            return GenericityFunctions.GetInterface(compresserBytesAndBase64String, DefaultCompresser).DecompressBytesFromBase64String(decompressString);
        }
        /// <summary>
        /// 异步 压缩字节数组 返回 压缩后字节数组生成的 Base64 字符串
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <param name="compresserBytesAndBase64String">压缩器 字节数组 和 Base64字符串 压缩 功能 接口</param>
        /// <returns></returns>
        public static Task<string> CompressBytesToBase64StringAsync(byte[] compressBytes, ICompresserBytesAndBase64String compresserBytesAndBase64String = null)
        {
            return GenericityFunctions.GetInterface(compresserBytesAndBase64String, DefaultCompresser).CompressBytesToBase64StringAsync(compressBytes);
        }
        /// <summary>
        /// 异步 解压缩 Base64 字符串 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressString"></param>
        /// <param name="compresserBytesAndBase64String">压缩器 字节数组 和 Base64字符串 压缩 功能 接口</param>
        /// <returns></returns>
        public static Task<byte[]> DecompressBytesFromBase64StringAsync(string decompressString, ICompresserBytesAndBase64String compresserBytesAndBase64String = null)
        {
            return GenericityFunctions.GetInterface(compresserBytesAndBase64String, DefaultCompresser).DecompressBytesFromBase64StringAsync(decompressString);
        }








        /// <summary>
        /// 压缩字符串 返回 压缩后的 字节数组
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="compresserStringAndBytes">压缩器 字符串 和 字节数组 压缩 功能 接口</param>
        /// <returns></returns>
        public static byte[] CompressStringToBytes(string compressString, Encoding encoding = null, ICompresserStringAndBytes compresserStringAndBytes = null)
        {
            return GenericityFunctions.GetInterface(compresserStringAndBytes, DefaultCompresser).CompressStringToBytes(compressString, encoding);
        }
        /// <summary>
        /// 解压缩字节数组 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="compresserStringAndBytes">压缩器 字符串 和 字节数组 压缩 功能 接口</param>
        /// <returns></returns>
        public static string DecompressStringFromBytes(byte[] decompressBytes, Encoding encoding = null, ICompresserStringAndBytes compresserStringAndBytes = null)
        {
            return GenericityFunctions.GetInterface(compresserStringAndBytes, DefaultCompresser).DecompressStringFromBytes(decompressBytes, encoding);
        }
        /// <summary>
        /// 异步 压缩字符串 返回 压缩后的 字节数组
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="compresserStringAndBytes">压缩器 字符串 和 字节数组 压缩 功能 接口</param>
        /// <returns></returns>
        public static Task<byte[]> CompressStringToBytesAsync(string compressString, Encoding encoding = null, ICompresserStringAndBytes compresserStringAndBytes = null)
        {
            return GenericityFunctions.GetInterface(compresserStringAndBytes, DefaultCompresser).CompressStringToBytesAsync(compressString, encoding);
        }
        /// <summary>
        /// 异步 解压缩字节数组 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="compresserStringAndBytes">压缩器 字符串 和 字节数组 压缩 功能 接口</param>
        /// <returns></returns>
        public static Task<string> DecompressStringFromBytesAsync(byte[] decompressBytes, Encoding encoding = null, ICompresserStringAndBytes compresserStringAndBytes = null)
        {
            return GenericityFunctions.GetInterface(compresserStringAndBytes, DefaultCompresser).DecompressStringFromBytesAsync(decompressBytes, encoding);
        }









        /// <summary>
        /// 压缩字符串 返回 压缩后的Base64 字符串
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="compresserStringAndBase64String">压缩器 字符串 和 Base64字符串 压缩 功能 接口</param>
        /// <returns></returns>
        public static string CompressStringToBase64String(string compressString, Encoding encoding = null, ICompresserStringAndBase64String compresserStringAndBase64String = null)
        {
            return GenericityFunctions.GetInterface(compresserStringAndBase64String, DefaultCompresser).CompressStringToBase64String(compressString, encoding);
        }
        /// <summary>
        /// 解压缩 Base64 字符串 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressString">要解压缩的Base64字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="compresserStringAndBase64String">压缩器 字符串 和 Base64字符串 压缩 功能 接口</param>
        /// <returns></returns>
        public static string DecompressStringFromBase64String(string decompressString, Encoding encoding = null, ICompresserStringAndBase64String compresserStringAndBase64String = null)
        {
            return GenericityFunctions.GetInterface(compresserStringAndBase64String, DefaultCompresser).DecompressStringFromBase64String(decompressString, encoding);
        }
        /// <summary>
        /// 异步 压缩字符串 返回 压缩后的Base64 字符串
        /// </summary>
        /// <param name="compressString">要压缩的字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="compresserStringAndBase64String">压缩器 字符串 和 Base64字符串 压缩 功能 接口</param>
        /// <returns></returns>
        public static Task<string> CompressStringToBase64StringAsync(string compressString, Encoding encoding = null, ICompresserStringAndBase64String compresserStringAndBase64String = null)
        {
            return GenericityFunctions.GetInterface(compresserStringAndBase64String, DefaultCompresser).CompressStringToBase64StringAsync(compressString, encoding);
        }
        /// <summary>
        /// 异步 解压缩 Base64 字符串 返回 解压缩后的字符串
        /// </summary>
        /// <param name="decompressString">要解压缩的Base64字符串</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="compresserStringAndBase64String">压缩器 字符串 和 Base64字符串 压缩 功能 接口</param>
        /// <returns></returns>
        public static Task<string> DecompressStringFromBase64StringAsync(string decompressString, Encoding encoding = null, ICompresserStringAndBase64String compresserStringAndBase64String = null)
        {
            return GenericityFunctions.GetInterface(compresserStringAndBase64String, DefaultCompresser).DecompressStringFromBase64StringAsync(decompressString, encoding);
        }


    }

}
