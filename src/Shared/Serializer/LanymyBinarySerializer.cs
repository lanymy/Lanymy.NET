/********************************************************************

时间: 2017年05月24日, PM 09:50:45

作者: lanyanmiyu@qq.com

描述: 二进制序列化器

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;

namespace Lanymy.General.Extension.Serializer
{

    /// <summary>
    /// 二进制序列化器
    /// </summary>
    public class LanymyBinarySerializer : BaseSerializer, IBinarySerializer
    {

        /// <summary>
        /// 二进制序列化器 构造方法
        /// </summary>
        /// <param name="encoding">编码</param>
        public LanymyBinarySerializer(Encoding encoding = null) : base(encoding)
        {

        }

        /// <summary>
        /// 把对象序列化成二进制数据
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual byte[] SerializeToBytes<T>(T t, Encoding encoding = null) where T : class
        {
            if (encoding.IfIsNullOrEmpty()) encoding = CurrentEncoding;
            return encoding.GetBytes(SerializeFunctions.SerializeToJson(t));
        }
        /// <summary>
        /// 异步 把对象序列化成二进制数据
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual Task<byte[]> SerializeToBytesAsync<T>(T t, Encoding encoding = null) where T : class
        {
#if NET40
            return new Task<byte[]>(() => SerializeToBytes(t, encoding));
#else
            return Task.FromResult(SerializeToBytes(t, encoding));
#endif
        }
        /// <summary>
        /// 反序列化二进制数据成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象 的 对象类型</typeparam>
        /// <param name="bytes">需要反序列化处理的二进制数据</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual T DeserializeFromBytes<T>(byte[] bytes, Encoding encoding = null) where T : class
        {
            if (encoding.IfIsNullOrEmpty()) encoding = CurrentEncoding;
            return SerializeFunctions.DeserializeFromJson<T>(encoding.GetString(bytes));
        }
        /// <summary>
        /// 异步 反序列化二进制数据成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象 的 对象类型</typeparam>
        /// <param name="bytes">需要反序列化处理的二进制数据</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual Task<T> DeserializeFromBytesAsync<T>(byte[] bytes, Encoding encoding = null) where T : class
        {
#if NET40
            return new Task<T>(() => DeserializeFromBytes<T>(bytes, encoding));
#else
            return Task.FromResult(DeserializeFromBytes<T>(bytes, encoding));
#endif
        }
        /// <summary>
        /// 把对象序列化成二进制文件
        /// </summary>
        /// <typeparam name="T">要序列化对象 的 对象类型</typeparam>
        /// <param name="binaryFileFullPath">要序列化成二进制文件的全路径</param>
        /// <param name="t">要序列化对象的实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifCompressBytes">是否压缩字节数组 默认值 True 压缩形式序列化字节数组</param>
        /// <returns></returns>
        public virtual void SerializeToBytesFile<T>(T t, string binaryFileFullPath, Encoding encoding = null, bool ifCompressBytes = true) where T : class
        {
            FileFunctions.CreateBinaryFile(binaryFileFullPath, ifCompressBytes ? CompressionFunctions.CompressBytesToBytes(SerializeToBytes(t, encoding)) : SerializeToBytes(t, encoding));
        }
        /// <summary>
        /// 异步 把对象序列化成二进制文件
        /// </summary>
        /// <typeparam name="T">要序列化对象 的 对象类型</typeparam>
        /// <param name="binaryFileFullPath">要序列化成二进制文件的全路径</param>
        /// <param name="t">要序列化对象的实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifCompressBytes">是否压缩字节数组 默认值 True 压缩形式序列化字节数组</param>
        /// <returns></returns>
        public virtual Task SerializeToBytesFileAsync<T>(T t, string binaryFileFullPath, Encoding encoding = null,
            bool ifCompressBytes = true) where T : class
        {
#if NET40
            return new Task(() => SerializeToBytesFile(t, binaryFileFullPath, encoding));
#else
            return Task.Run(()=> SerializeToBytesFile(t, binaryFileFullPath, encoding));
#endif
        }

        /// <summary>
        /// 反序列化二进制文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifDecompressBytes">是否解压缩字节数组 默认值 True 解压缩形式反序列化字节数组</param>
        /// <returns></returns>
        public virtual T DeserializeFromBytesFile<T>(string binaryFileFullPath, Encoding encoding = null, bool ifDecompressBytes = true) where T : class
        {
            var bytes = FileFunctions.GetBinaryFileBytes(binaryFileFullPath);
            if (ifDecompressBytes)
            {
                bytes = CompressionFunctions.DecompressBytesFromBytes(bytes);
            }
            return DeserializeFromBytes<T>(bytes, encoding);
        }

        /// <summary>
        /// 异步 反序列化二进制文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifDecompressBytes">是否解压缩字节数组 默认值 True 解压缩形式反序列化字节数组</param>
        /// <returns></returns>
        public virtual Task<T> DeserializeFromBytesFileAsync<T>(string binaryFileFullPath, Encoding encoding = null,
            bool ifDecompressBytes = true) where T : class
        {
#if NET40
            return new Task<T>(() => DeserializeFromBytesFile<T>(binaryFileFullPath, encoding));
#else
            return Task.FromResult(DeserializeFromBytesFile<T>(binaryFileFullPath, encoding));
#endif
        }


    }
}
