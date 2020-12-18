using System.Text;

namespace Lanymy.Common.Helpers
{

    public class FileSerializeHelper
    {

        /// <summary>
        /// 把对象序列化成二进制文件
        /// </summary>
        /// <typeparam name="T">要序列化对象 的 对象类型</typeparam>
        /// <param name="binaryFileFullPath">要序列化成二进制文件的全路径</param>
        /// <param name="t">要序列化对象的实例</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifCompressBytes">是否压缩字节数组 默认值 True 压缩形式序列化字节数组</param>
        /// <returns></returns>
        public static void SerializeToBytesFile<T>(T t, string binaryFileFullPath, Encoding encoding = null, bool ifCompressBytes = true) where T : class
        {
            FileHelper.CreateBinaryFile(binaryFileFullPath, ifCompressBytes ? CompressionHelper.CompressBytesToBytes(BinarySerializeHelper.SerializeToBytes(t, encoding)) : BinarySerializeHelper.SerializeToBytes(t, encoding));
        }


        /// <summary>
        /// 反序列化二进制文件成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的 对象类型</typeparam>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifDecompressBytes">是否解压缩字节数组 默认值 True 解压缩形式反序列化字节数组</param>
        /// <returns></returns>
        public static T DeserializeFromBytesFile<T>(string binaryFileFullPath, Encoding encoding = null, bool ifDecompressBytes = true) where T : class
        {
            var bytes = FileHelper.GetBinaryFileBytes(binaryFileFullPath);
            if (ifDecompressBytes)
            {
                bytes = CompressionHelper.DecompressBytesFromBytes(bytes);
            }
            return BinarySerializeHelper.DeserializeFromBytes<T>(bytes, encoding);
        }

    }

}
