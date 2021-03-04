using System.IO;
using System.Text;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// Lanymy 独立 存储区  操作器
    /// </summary>
    public class LanymyIsolatedStorage : BaseIsolatedStorage
    {

        /// <summary>
        /// 独立存储文件后缀名
        /// </summary>
        private const string ISOLATED_STORAGE_FILE_SUFFIX = ".isf";

        /// <summary>
        /// 自定义 独立存储区 根目录 文件夹名称
        /// </summary>
        private const string CUSTOM_ISOLATED_STORAGE_ROOT_DIRECTORY_NAME = "CustomIsolatedStorageFiles";



        /// <summary>
        /// Lanymy 独立 存储区  操作器 构造方法
        /// </summary>
        /// <param name="customIsolatedStorageRootDirectoryFullPath">自定义独立存储区 根目录 全路径 ; null 则使用托管的 独立存储区; 不为null 则使用 自定义独立存储区</param>
        public LanymyIsolatedStorage(string customIsolatedStorageRootDirectoryFullPath = null) : base(customIsolatedStorageRootDirectoryFullPath)
        {

        }


        /// <summary>
        /// 获取默认的 自定义独立存储区 跟目录 全路径
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultCustomIsolatedStorageRootDirectoryFullPath()
        {
            return Path.Combine(PathHelper.GetCallDomainPath(), CUSTOM_ISOLATED_STORAGE_ROOT_DIRECTORY_NAME);
        }


        /// <summary>
        /// 匹配文件名
        /// </summary>
        /// <returns></returns>
        protected virtual string MatchFileFullName(string fileName)
        {

            //return SecurityHelperOld.EncryptFileNameToBase64String(fileName, ISOLATED_STORAGE_FILE_SUFFIX, null, false);
            return SecurityHelper.EncryptFileNameToHashCodeString(fileName, ISOLATED_STORAGE_FILE_SUFFIX, null, false, null);

        }

        /// <summary>
        /// 获取数据实体类的默认文件名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual string GetModelDefaultFileName<T>() where T : class
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
        protected virtual void SaveStringToStream(Stream stream, string sourceString, string securityKey, Encoding encoding)
        {

            if (stream.IfIsNullOrEmpty() || sourceString.IfIsNullOrEmpty()) return;
            //byte[] buffer = CompressionHelper.CompressBytesToBytes(SecurityHelperOld.EncryptStringToBytes(sourceString, securityKey, true, encoding));
            var encryptModel = SecurityHelper.EncryptStringToBytes(sourceString, securityKey, true, encoding);
            byte[] buffer = CompressionHelper.CompressBytesToBytes(encryptModel.EncryptedBytes);
            stream.Write(buffer, 0, buffer.Length);

        }

        /// <summary>
        /// 从数据流中读取字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="securityKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        protected virtual string GetStringFromStream(Stream stream, string securityKey, Encoding encoding)
        {
            if (stream.IfIsNullOrEmpty()) return string.Empty;
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);

            //return SecurityHelperOld.DecryptStringFromBytes(CompressionHelper.DecompressBytesFromBytes(data), securityKey, encoding);

            var decryptModel = SecurityHelper.DecryptStringFromBytes(CompressionHelper.DecompressBytesFromBytes(data), securityKey, encoding);

            return decryptModel.SourceString;


        }


        /// <summary>
        /// 序列化 字符串 到 持久化文件
        /// </summary>
        /// <param name="sourceString">要序列化的字符串</param>
        /// <param name="token">持久化标识</param>
        /// <param name="encoding">编码 (Null表示使用 默认编码)</param>
        /// <param name="securityKey">密钥 (Null表示使用 默认密钥)</param>
        public override void SaveString(string sourceString, string token, string securityKey = null, Encoding encoding = null)
        {

            token = MatchFileFullName(token);
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;
            if (securityKey.IfIsNullOrEmpty()) securityKey = DefaultSettingKeys.DEFAULT_CRYPTO_KEY;

            if (IfIsCustomIsolatedStorageMode)
            {
                using (var fileStream = new FileStream(GetCustomIsolatedStorageFileFullPath(token), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    SaveStringToStream(fileStream, sourceString, securityKey, encoding);
                }
                return;
            }

            using (var store = GetSystemIsolatedStorage())
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
        public override string GetString(string token, string securityKey = null, Encoding encoding = null)
        {


            string result = string.Empty;
            token = MatchFileFullName(token);
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;
            if (securityKey.IfIsNullOrEmpty()) securityKey = DefaultSettingKeys.DEFAULT_CRYPTO_KEY;

            if (IfIsCustomIsolatedStorageMode)
            {
                string fileFullPath = GetCustomIsolatedStorageFileFullPath(token);
                if (File.Exists(fileFullPath))
                {
                    using (var fileStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        result = GetStringFromStream(fileStream, securityKey, encoding);
                    }
                }
                return result;
            }

            using (var store = GetSystemIsolatedStorage())
            {

                if (!store.IfIsNullOrEmpty())
                {
                    if (store.FileExists(token))
                    {
                        using (var fileStream = store.OpenFile(token, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            result = GetStringFromStream(fileStream, securityKey, encoding);
                        }
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
        public override void SaveModel<T>(T t, string token = null, string securityKey = null, Encoding encoding = null)
        {

            if (token.IfIsNullOrEmpty()) token = GetModelDefaultFileName<T>();
            SaveString(JsonSerializeHelper.SerializeToJson(t), token, securityKey, encoding);

        }

        /// <summary>
        ///从独立存储区中获取实体类
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="token">序列化标识 (Null 表示使用 默认 标识名)</param>
        /// <param name="encoding">编码 (Null 表示使用 默认 编码)</param>
        /// <param name="securityKey">密钥 (Null 表示使用 默认 密钥)</param>
        /// <returns></returns>
        public override T GetModel<T>(string token = null, string securityKey = null, Encoding encoding = null)
        {

            if (token.IfIsNullOrEmpty()) token = GetModelDefaultFileName<T>();
            return JsonSerializeHelper.DeserializeFromJson<T>(GetString(token, securityKey, encoding));

        }
    }
}
