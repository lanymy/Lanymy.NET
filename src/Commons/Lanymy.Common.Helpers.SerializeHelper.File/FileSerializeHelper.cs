using System;
using System.IO;
using System.Text;
using Lanymy.Common.Helpers.ResultModels;

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
            var result = SerializeToBytesFileWithResult(t, binaryFileFullPath, encoding, ifCompressBytes);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 把对象序列化成二进制文件，并返回详细结果。
        /// </summary>
        /// <typeparam name="T">要序列化对象的对象类型</typeparam>
        /// <param name="t">要序列化对象的实例</param>
        /// <param name="binaryFileFullPath">要序列化成二进制文件的全路径</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifCompressBytes">是否压缩字节数组 默认值 True</param>
        /// <returns>二进制文件序列化结果</returns>
        public static BinaryFileSerializationResultModel<T> SerializeToBytesFileWithResult<T>(T t, string binaryFileFullPath, Encoding encoding = null, bool ifCompressBytes = true) where T : class
        {
            var result = new BinaryFileSerializationResultModel<T>
            {
                FilePath = binaryFileFullPath,
                IsCompressed = ifCompressBytes,
                Model = t,
            };

            try
            {
                var bytes = BinarySerializeHelper.SerializeToBytes(t, encoding);
                if (ifCompressBytes)
                {
                    bytes = CompressionHelper.CompressBytesToBytes(bytes);
                }

                result.BytesLength = bytes?.Length ?? 0;

                var fileResult = FileHelper.CreateBinaryFileWithResult(binaryFileFullPath, bytes);
                result.IsSuccess = fileResult.IsSuccess;
                result.Exception = fileResult.Exception;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
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
            var result = DeserializeFromBytesFileWithResult<T>(binaryFileFullPath, encoding, ifDecompressBytes);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }

            return result.Model;
        }

        /// <summary>
        /// 反序列化二进制文件成对象，并返回详细结果。
        /// </summary>
        /// <typeparam name="T">要反序列化成对象的对象类型</typeparam>
        /// <param name="binaryFileFullPath">二进制文件全路径</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <param name="ifDecompressBytes">是否解压缩字节数组 默认值 True</param>
        /// <returns>二进制文件反序列化结果</returns>
        public static BinaryFileSerializationResultModel<T> DeserializeFromBytesFileWithResult<T>(string binaryFileFullPath, Encoding encoding = null, bool ifDecompressBytes = true) where T : class
        {
            var result = new BinaryFileSerializationResultModel<T>
            {
                FilePath = binaryFileFullPath,
                IsCompressed = ifDecompressBytes,
            };

            try
            {
                var fileReadResult = FileHelper.GetBinaryFileBytesWithResult(binaryFileFullPath);
                result.BytesLength = fileReadResult.BytesLength;

                if (!fileReadResult.IsSuccess)
                {
                    throw fileReadResult.Exception ?? new FileNotFoundException("Binary file does not exist.", binaryFileFullPath);
                }

                var bytes = fileReadResult.Bytes;

                if (ifDecompressBytes)
                {
                    bytes = CompressionHelper.DecompressBytesFromBytes(bytes);
                }

                result.Model = BinarySerializeHelper.DeserializeFromBytes<T>(bytes, encoding);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

    }

}
