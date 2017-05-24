/********************************************************************

时间: 2015年03月05日, PM 02:25:11

作者: lanyanmiyu@qq.com

描述: 独立存储区操作类

其它:     

********************************************************************/


using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;
using Lanymy.General.Extension.ExtensionFunctions;
using System.Text;

namespace Lanymy.General.Extension
{
    /// <summary>
    /// 独立存储区操作类
    /// </summary>
    public class IsolatedStorageFunctions
    {


        /// <summary>
        /// 实体类加密保存文件后缀
        /// </summary>
        private const string Model_SECURITY_SUFFIX = ".ms";

        /// <summary>
        /// 自定义 独立存储区 根目录 文件夹名称
        /// </summary>
        private const string CUSTOM_ISOLATED_STORAGE_ROOT_DIRECTORY_NAME = "CustomIsolatedStorageFiles";

        /// <summary>
        /// 自定义 独立存储区 根目录 全路径
        /// </summary>
        private static string _CustomIsolatedStorageRootDirectoryFullPath = String.Empty;

        /// <summary>
        /// 获取 自定义 独立存储区 根目录 全路径
        /// </summary>
        /// <returns></returns>
        private static string GetCustomIsolatedStorageRootDirectoryFullPath()
        {

            if (_CustomIsolatedStorageRootDirectoryFullPath.IfIsNullOrEmpty())
            {
                _CustomIsolatedStorageRootDirectoryFullPath = Path.Combine(PathFunctions.GetCallDomainPath(), CUSTOM_ISOLATED_STORAGE_ROOT_DIRECTORY_NAME);
                PathFunctions.InitDirectoryPath(_CustomIsolatedStorageRootDirectoryFullPath, true);
            }

            return _CustomIsolatedStorageRootDirectoryFullPath;

        }

        /// <summary>
        /// 匹配 自定义 独立存储区 中文件 全路径
        /// </summary>
        /// <param name="fileFullName">文件全名称</param>
        /// <returns></returns>
        private static string GetCustomIsolatedStorageFileFullPath(string fileFullName)
        {
            return Path.Combine(GetCustomIsolatedStorageRootDirectoryFullPath(), fileFullName);
        }

        /// <summary>
        /// 获取独立存储区对象
        /// </summary>
        /// <returns></returns>
        private static IsolatedStorageFile GetIsolatedStorageFile()
        {

            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// 匹配文件名
        /// </summary>
        /// <returns></returns>
        private static string MatchFileName(string fileName)
        {
            return SecurityFunctions.EncryptFileNameToBase64String(fileName, Model_SECURITY_SUFFIX, null, false);
        }

        /// <summary>
        /// 获取数据实体类的默认文件名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static string GetModelDefaultFileName<T>() where T : class
        {
            return typeof(T).Name;
        }

        /// <summary>
        /// 字符串写入到数据流中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sourceString"></param>
        /// <param name="securityKey"></param>
        /// <param name="encoding"></param>
        private static void SaveStringToStream(Stream stream, string sourceString, string securityKey, Encoding encoding)
        {

            if (stream.IfIsNullOrEmpty() || sourceString.IfIsNullOrEmpty()) return;
            byte[] buffer = CompressionFunctions.CompressBytesToBytes(SecurityFunctions.EncryptStringToBytes(sourceString, securityKey, true, encoding));
            stream.Write(buffer, 0, buffer.Length);

        }

        /// <summary>
        /// 从数据流中读取字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="securityKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static string GetStringFromStream(Stream stream, string securityKey, Encoding encoding)
        {
            if (stream.IfIsNullOrEmpty()) return string.Empty;
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return SecurityFunctions.DecryptStringFromBytes(CompressionFunctions.DecompressBytesFromBytes(data), securityKey, encoding);
        }

        /// <summary>
        /// 序列化 字符串 到 持久化文件
        /// </summary>
        /// <param name="sourceString">要序列化的字符串</param>
        /// <param name="token">持久化标识</param>
        /// <param name="encoding">编码 (Null表示使用 默认编码)</param>
        /// <param name="securityKey">密钥 (Null表示使用 默认密钥)</param>
        public static void SaveString(string sourceString, string token, string securityKey = null, Encoding encoding = null)
        {

            token = MatchFileName(token);
            if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;
            if (securityKey.IfIsNullOrEmpty()) securityKey = GlobalSettings.DEFAULT_SECURITY_KEY;

            using (var store = GetIsolatedStorageFile())
            {
                if (!store.IfIsNullOrEmpty())
                {
                    using (var fileStream = store.OpenFile(token, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        SaveStringToStream(fileStream, sourceString, securityKey, encoding);
                    }
                }
                else
                {
                    using (var fileStream = new FileStream(GetCustomIsolatedStorageFileFullPath(token), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        SaveStringToStream(fileStream, sourceString, securityKey, encoding);
                    }
                }

            }

        }



        /// <summary>
        /// 从持久化文件 获取 字符串
        /// </summary>
        /// <param name="token">持久化标识</param>
        /// <param name="encoding">编码 (Null表示使用 默认编码)</param>
        /// <param name="securityKey">密钥 (Null表示使用 默认密钥)</param>
        /// <returns></returns>
        public static string GetString(string token, string securityKey = null, Encoding encoding = null)
        {

            string result = string.Empty;
            token = MatchFileName(token);
            if (encoding.IfIsNullOrEmpty()) encoding = GlobalSettings.DEFAULT_ENCODING;
            if (securityKey.IfIsNullOrEmpty()) securityKey = GlobalSettings.DEFAULT_SECURITY_KEY;

            using (var store = GetIsolatedStorageFile())
            {
                if (!store.IfIsNullOrEmpty() && store.FileExists(token))
                {
                    using (var fileStream = store.OpenFile(token, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        result = GetStringFromStream(fileStream, securityKey, encoding);
                    }
                }
                else
                {
                    string fileFullPath = GetCustomIsolatedStorageFileFullPath(token);
                    if (File.Exists(fileFullPath))
                    {
                        using (var fileStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            result = GetStringFromStream(fileStream, securityKey, encoding);
                        }
                    }
                }

            }
            return result;

        }








        /// <summary>
        /// 指定文件名 指定编码 指定密钥 保存实体类到独立存储区中
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="t">实体实例</param>
        /// <param name="token">序列化标识 (Null 表示使用 默认 标识名)</param>
        /// <param name="encoding">编码 (Null 表示使用 默认 编码)</param>
        /// <param name="securityKey">密钥 (Null 表示使用 默认 密钥)</param>
        public static void SaveModel<T>(T t, string token = null, string securityKey = null, Encoding encoding = null) where T : class
        {
            if (token.IfIsNullOrEmpty()) token = GetModelDefaultFileName<T>();
            SaveString(SerializeFunctions.SerializeToJson(t), token, securityKey, encoding);
        }


        /// <summary>
        ///从独立存储区中获取实体类
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="token">序列化标识 (Null 表示使用 默认 标识名)</param>
        /// <param name="encoding">编码 (Null 表示使用 默认 编码)</param>
        /// <param name="securityKey">密钥 (Null 表示使用 默认 密钥)</param>
        /// <returns></returns>
        public static T GetModel<T>(string token = null, string securityKey = null, Encoding encoding = null) where T : class
        {
            if (token.IfIsNullOrEmpty()) token = GetModelDefaultFileName<T>();
            return SerializeFunctions.DeserializeFromJson<T>(GetString(token, securityKey, encoding));
        }


    }

}
