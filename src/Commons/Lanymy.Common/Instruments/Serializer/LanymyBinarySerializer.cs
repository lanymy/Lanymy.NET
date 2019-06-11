using System.Text;
using System.Threading.Tasks;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Interfaces.ISerializers;

namespace Lanymy.Common.Instruments.Serializer
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
            return encoding.GetBytes(SerializeHelper.SerializeToJson(t));
        }
        /// <summary>
        /// 异步 把对象序列化成二进制数据
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual async Task<byte[]> SerializeToBytesAsync<T>(T t, Encoding encoding = null) where T : class
        {
            return await GenericityHelper.DoTaskWorkAsync(SerializeToBytes, t, encoding);
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
            return SerializeHelper.DeserializeFromJson<T>(encoding.GetString(bytes));
        }
        /// <summary>
        /// 异步 反序列化二进制数据成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象 的 对象类型</typeparam>
        /// <param name="bytes">需要反序列化处理的二进制数据</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public virtual async Task<T> DeserializeFromBytesAsync<T>(byte[] bytes, Encoding encoding = null) where T : class
        {
            return await GenericityHelper.DoTaskWorkAsync(DeserializeFromBytes<T>, bytes, encoding);
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
            FileHelper.CreateBinaryFile(binaryFileFullPath, ifCompressBytes ? CompressionHelper.CompressBytesToBytes(SerializeToBytes(t, encoding)) : SerializeToBytes(t, encoding));
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
        public virtual async Task SerializeToBytesFileAsync<T>(T t, string binaryFileFullPath, Encoding encoding = null, bool ifCompressBytes = true) where T : class
        {
            await GenericityHelper.DoTaskWorkAsync(SerializeToBytesFile, t, binaryFileFullPath, encoding, ifCompressBytes);
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
            var bytes = FileHelper.GetBinaryFileBytes(binaryFileFullPath);
            if (ifDecompressBytes)
            {
                bytes = CompressionHelper.DecompressBytesFromBytes(bytes);
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
        public virtual async Task<T> DeserializeFromBytesFileAsync<T>(string binaryFileFullPath, Encoding encoding = null, bool ifDecompressBytes = true) where T : class
        {
            return await GenericityHelper.DoTaskWorkAsync(DeserializeFromBytesFile<T>, binaryFileFullPath, encoding, ifDecompressBytes);
        }


    }
}
