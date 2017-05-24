/********************************************************************

时间: 2017年05月24日, PM 06:17:32

作者: lanyanmiyu@qq.com

描述: 压缩器 字节数组 压缩 功能 接口

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.General.Extension.Interfaces
{


    /// <summary>
    /// 压缩器 字节数组 压缩 功能 接口
    /// </summary>
    public interface ICompresserBytesAndBytes
    {

        /// <summary>
        /// 压缩字节数组 返回压缩后的 字节数组
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <returns></returns>
        byte[] CompressBytesToBytes(byte[] compressBytes);

        /// <summary>
        /// 解压缩字节数组 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <returns></returns>
        byte[] DecompressBytesFromBytes(byte[] decompressBytes);

        /// <summary>
        /// 异步 压缩字节数组 返回压缩后的 字节数组
        /// </summary>
        /// <param name="compressBytes">要压缩的字节数组</param>
        /// <returns></returns>
        Task<byte[]> CompressBytesToBytesAsync(byte[] compressBytes);

        /// <summary>
        /// 异步 解压缩字节数组 返回解压缩后的 字节数组
        /// </summary>
        /// <param name="decompressBytes">要解压缩的字节数组</param>
        /// <returns></returns>
        Task<byte[]> DecompressBytesFromBytesAsync(byte[] decompressBytes);


    }

}
