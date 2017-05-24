/********************************************************************

时间: 2017年05月24日, PM 06:21:22

作者: lanyanmiyu@qq.com

描述: 压缩器 字节数组 和 Base64字符串 压缩 功能 接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.General.Extension.Interfaces
{


    /// <summary>
    /// 压缩器 字节数组 和 Base64字符串 压缩 功能 接口
    /// </summary>
    public interface ICompresserBytesAndBase64String
    {

        /// <summary>
        /// 压缩字节数组 返回 压缩后字节数组生成的 Base64 字符串
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <returns></returns>
        string CompressBytesToBase64String(byte[] compressBytes);

        /// <summary>
        /// 解压缩 Base64 字符串 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressString"></param>
        /// <returns></returns>
        byte[] DecompressBytesFromBase64String(string decompressString);

        /// <summary>
        /// 异步 压缩字节数组 返回 压缩后字节数组生成的 Base64 字符串
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <returns></returns>
        Task<string> CompressBytesToBase64StringAsync(byte[] compressBytes);

        /// <summary>
        /// 异步 解压缩 Base64 字符串 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressString"></param>
        /// <returns></returns>
        Task<byte[]> DecompressBytesFromBase64StringAsync(string decompressString);


    }


}
