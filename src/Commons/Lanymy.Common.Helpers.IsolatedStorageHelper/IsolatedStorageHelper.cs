using System;
using System.IO;
using System.Text;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Helpers.ResultModels;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.IIsolatedStorages;

namespace Lanymy.Common.Helpers
{
    /// <summary>
    /// 独立存储区操作类
    /// </summary>
    public class IsolatedStorageHelper
    {


        /// <summary>
        /// 默认 独立存储区 操作器
        /// </summary>
        public static readonly IIsolatedStorage DefaultIsolatedStorage = new LanymyIsolatedStorage();

        private static string GetModelToken<T>(string token) where T : class
        {
            return token.IfIsNullOrEmpty() ? typeof(T).Name : token;
        }

        private static string GetEncodingName(Encoding encoding)
        {
            return (encoding ?? DefaultSettingKeys.DEFAULT_ENCODING).WebName;
        }

        private static void FillFailureContext(IsolatedStorageStringOperationResultModel result, Exception ex)
        {
            if (result == null || ex == null)
            {
                return;
            }

            result.Exception = ex;
            if (result.ErrorMessage.IfIsNullOrEmpty())
            {
                result.ErrorMessage = ex.Message;
            }
        }

        private static void FillFailureContext<T>(IsolatedStorageModelOperationResultModel<T> result, Exception ex) where T : class
        {
            if (result == null || ex == null)
            {
                return;
            }

            result.Exception = ex;
            if (result.ErrorMessage.IfIsNullOrEmpty())
            {
                result.ErrorMessage = ex.Message;
            }
        }

        private static void FillStringStorageContext(IsolatedStorageStringOperationResultModel result, string token, IIsolatedStorageString isolatedStorageString)
        {
            if (result == null || isolatedStorageString == null)
            {
                return;
            }

            result.Token = token;
            result.StorageImplementationTypeName = isolatedStorageString.GetType().FullName;

            if (!(isolatedStorageString is LanymyIsolatedStorage lanymyIsolatedStorage))
            {
                return;
            }

            result.StorageFileName = lanymyIsolatedStorage.GetStorageFileName(token);
            result.FilePath = lanymyIsolatedStorage.GetStorageFileFullPath(token);
            result.IfUsesCustomIsolatedStorageMode = lanymyIsolatedStorage.IfIsCustomIsolatedStorageMode || !result.FilePath.IfIsNullOrEmpty();
            result.StorageFileExists = lanymyIsolatedStorage.StorageFileExists(token);
        }

        private static void FillModelStorageContext<T>(IsolatedStorageModelOperationResultModel<T> result, string token, IIsolatedStorageModel isolatedStorageModel) where T : class
        {
            if (result == null || isolatedStorageModel == null)
            {
                return;
            }

            result.Token = token;
            result.StorageImplementationTypeName = isolatedStorageModel.GetType().FullName;
            result.ModelTypeName = typeof(T).Name;
            result.ModelTypeFullName = typeof(T).FullName;

            if (!(isolatedStorageModel is LanymyIsolatedStorage lanymyIsolatedStorage))
            {
                return;
            }

            result.StorageFileName = lanymyIsolatedStorage.GetStorageFileName(token);
            result.FilePath = lanymyIsolatedStorage.GetStorageFileFullPath(token);
            result.IfUsesCustomIsolatedStorageMode = lanymyIsolatedStorage.IfIsCustomIsolatedStorageMode || !result.FilePath.IfIsNullOrEmpty();
            result.StorageFileExists = lanymyIsolatedStorage.StorageFileExists(token);
        }

        /// <summary>
        /// 序列化 字符串 到 持久化文件
        /// </summary>
        /// <param name="sourceString">要序列化的字符串</param>
        /// <param name="token">持久化标识</param>
        /// <param name="encoding">编码 (Null表示使用 默认编码)</param>
        /// <param name="securityKey">密钥 (Null表示使用 默认密钥)</param>
        /// <param name="isolatedStorageString">独立存储 字符串 功能 接口</param>
        public static void SaveString(string sourceString, string token, string securityKey = null, Encoding encoding = null, IIsolatedStorageString isolatedStorageString = null)
        {
            var result = SaveStringWithResult(sourceString, token, securityKey, encoding, isolatedStorageString);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 序列化字符串到持久化文件，并返回详细结果。
        /// </summary>
        /// <param name="sourceString">要序列化的字符串。</param>
        /// <param name="token">持久化标识。</param>
        /// <param name="securityKey">密钥。</param>
        /// <param name="encoding">编码。</param>
        /// <param name="isolatedStorageString">独立存储接口实现。</param>
        /// <returns>独立存储字符串写入结果。</returns>
        public static IsolatedStorageStringOperationResultModel SaveStringWithResult(string sourceString, string token, string securityKey = null, Encoding encoding = null, IIsolatedStorageString isolatedStorageString = null)
        {
            var currentIsolatedStorage = GenericityHelper.GetInterface(isolatedStorageString, DefaultIsolatedStorage);
            var result = new IsolatedStorageStringOperationResultModel
            {
                IsWriteOperation = true,
                SourceString = sourceString,
                EncodingName = GetEncodingName(encoding),
                UsedDefaultEncoding = encoding.IfIsNullOrEmpty(),
                UsedDefaultSecurityKey = securityKey.IfIsNullOrEmpty(),
            };

            FillStringStorageContext(result, token, currentIsolatedStorage);

            try
            {
                currentIsolatedStorage.SaveString(sourceString, token, securityKey, encoding);
                FillStringStorageContext(result, token, currentIsolatedStorage);
                result.IsSuccess = currentIsolatedStorage is LanymyIsolatedStorage ? result.StorageFileExists : true;
            }
            catch (Exception ex)
            {
                FillFailureContext(result, ex);
            }

            return result;
        }

        /// <summary>
        /// 从持久化文件 获取 字符串
        /// </summary>
        /// <param name="token">持久化标识</param>
        /// <param name="encoding">编码 (Null表示使用 默认编码)</param>
        /// <param name="securityKey">密钥 (Null表示使用 默认密钥)</param>
        /// <param name="isolatedStorageString">独立存储 字符串 功能 接口</param>
        /// <returns></returns>
        public static string GetString(string token, string securityKey = null, Encoding encoding = null, IIsolatedStorageString isolatedStorageString = null)
        {
            var result = GetStringWithResult(token, securityKey, encoding, isolatedStorageString);
            if (!result.IsSuccess && result.Exception != null && !(result.Exception is FileNotFoundException))
            {
                throw result.Exception;
            }

            return result.Exception is FileNotFoundException ? string.Empty : result.SourceString;
        }

        /// <summary>
        /// 从持久化文件获取字符串，并返回详细结果。
        /// </summary>
        /// <param name="token">持久化标识。</param>
        /// <param name="securityKey">密钥。</param>
        /// <param name="encoding">编码。</param>
        /// <param name="isolatedStorageString">独立存储接口实现。</param>
        /// <returns>独立存储字符串读取结果。</returns>
        public static IsolatedStorageStringOperationResultModel GetStringWithResult(string token, string securityKey = null, Encoding encoding = null, IIsolatedStorageString isolatedStorageString = null)
        {
            var currentIsolatedStorage = GenericityHelper.GetInterface(isolatedStorageString, DefaultIsolatedStorage);
            var result = new IsolatedStorageStringOperationResultModel
            {
                IsWriteOperation = false,
                EncodingName = GetEncodingName(encoding),
                UsedDefaultEncoding = encoding.IfIsNullOrEmpty(),
                UsedDefaultSecurityKey = securityKey.IfIsNullOrEmpty(),
            };

            FillStringStorageContext(result, token, currentIsolatedStorage);

            try
            {
                if (currentIsolatedStorage is LanymyIsolatedStorage lanymyIsolatedStorage)
                {
                    result.StorageFileExists = lanymyIsolatedStorage.StorageFileExists(token);
                    if (!result.StorageFileExists)
                    {
                        FillFailureContext(result, new FileNotFoundException("Isolated storage file does not exist.", result.FilePath.IfIsNullOrEmpty() ? result.StorageFileName : result.FilePath));
                        return result;
                    }

                    var storageBytes = lanymyIsolatedStorage.GetStorageFileBytes(token);
                    var decryptModel = SecurityHelper.DecryptStringFromBytes(CompressionHelper.DecompressBytesFromBytes(storageBytes), securityKey, encoding);
                    result.SourceString = decryptModel.SourceString;
                    result.ErrorMessage = decryptModel.ErrorMessage;
                    result.IsSuccess = decryptModel.IsSuccess;
                    return result;
                }

                result.SourceString = currentIsolatedStorage.GetString(token, securityKey, encoding);
                result.IsSuccess = result.SourceString != null;
            }
            catch (Exception ex)
            {
                FillFailureContext(result, ex);
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
        /// <param name="isolatedStorageModel">独立存储 Model 功能 接口</param>
        public static void SaveModel<T>(T t, string token = null, string securityKey = null, Encoding encoding = null, IIsolatedStorageModel isolatedStorageModel = null) where T : class
        {
            var result = SaveModelWithResult(t, token, securityKey, encoding, isolatedStorageModel);
            if (!result.IsSuccess && result.Exception != null)
            {
                throw result.Exception;
            }
        }

        /// <summary>
        /// 保存实体类到独立存储，并返回详细结果。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="t">实体实例。</param>
        /// <param name="token">序列化标识。</param>
        /// <param name="securityKey">密钥。</param>
        /// <param name="encoding">编码。</param>
        /// <param name="isolatedStorageModel">独立存储模型接口。</param>
        /// <returns>独立存储模型写入结果。</returns>
        public static IsolatedStorageModelOperationResultModel<T> SaveModelWithResult<T>(T t, string token = null, string securityKey = null, Encoding encoding = null, IIsolatedStorageModel isolatedStorageModel = null) where T : class
        {
            var currentIsolatedStorage = GenericityHelper.GetInterface(isolatedStorageModel, DefaultIsolatedStorage);
            var effectiveToken = GetModelToken<T>(token);
            var result = new IsolatedStorageModelOperationResultModel<T>
            {
                IsWriteOperation = true,
                Model = t,
                EncodingName = GetEncodingName(encoding),
                UsedDefaultEncoding = encoding.IfIsNullOrEmpty(),
                UsedDefaultSecurityKey = securityKey.IfIsNullOrEmpty(),
            };

            FillModelStorageContext(result, effectiveToken, currentIsolatedStorage);

            try
            {
                result.SerializedString = JsonSerializeHelper.SerializeToJson(t);

                if (currentIsolatedStorage is IIsolatedStorageString isolatedStorageString)
                {
                    var stringResult = SaveStringWithResult(result.SerializedString, effectiveToken, securityKey, encoding, isolatedStorageString);
                    result.Exception = stringResult.Exception;
                    result.ErrorMessage = stringResult.ErrorMessage;
                    result.StorageFileName = stringResult.StorageFileName;
                    result.FilePath = stringResult.FilePath;
                    result.IfUsesCustomIsolatedStorageMode = stringResult.IfUsesCustomIsolatedStorageMode;
                    result.StorageFileExists = stringResult.StorageFileExists;
                    result.StorageImplementationTypeName = stringResult.StorageImplementationTypeName;
                    result.IsSuccess = stringResult.IsSuccess;
                    return result;
                }

                currentIsolatedStorage.SaveModel(t, effectiveToken, securityKey, encoding);
                FillModelStorageContext(result, effectiveToken, currentIsolatedStorage);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                FillFailureContext(result, ex);
            }

            return result;
        }

        /// <summary>
        ///从独立存储区中获取实体类
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="token">序列化标识 (Null 表示使用 默认 标识名)</param>
        /// <param name="encoding">编码 (Null 表示使用 默认 编码)</param>
        /// <param name="securityKey">密钥 (Null 表示使用 默认 密钥)</param>
        /// <param name="isolatedStorageModel">独立存储 Model 功能 接口</param>
        /// <returns></returns>
        public static T GetModel<T>(string token = null, string securityKey = null, Encoding encoding = null, IIsolatedStorageModel isolatedStorageModel = null) where T : class
        {
            var result = GetModelWithResult<T>(token, securityKey, encoding, isolatedStorageModel);
            if (!result.IsSuccess && result.Exception != null && !(result.Exception is FileNotFoundException))
            {
                throw result.Exception;
            }

            return result.Model;
        }

        /// <summary>
        /// 从独立存储区获取实体类，并返回详细结果。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="token">序列化标识。</param>
        /// <param name="securityKey">密钥。</param>
        /// <param name="encoding">编码。</param>
        /// <param name="isolatedStorageModel">独立存储模型接口。</param>
        /// <returns>独立存储模型读取结果。</returns>
        public static IsolatedStorageModelOperationResultModel<T> GetModelWithResult<T>(string token = null, string securityKey = null, Encoding encoding = null, IIsolatedStorageModel isolatedStorageModel = null) where T : class
        {
            var currentIsolatedStorage = GenericityHelper.GetInterface(isolatedStorageModel, DefaultIsolatedStorage);
            var effectiveToken = GetModelToken<T>(token);
            var result = new IsolatedStorageModelOperationResultModel<T>
            {
                IsWriteOperation = false,
                EncodingName = GetEncodingName(encoding),
                UsedDefaultEncoding = encoding.IfIsNullOrEmpty(),
                UsedDefaultSecurityKey = securityKey.IfIsNullOrEmpty(),
            };

            FillModelStorageContext(result, effectiveToken, currentIsolatedStorage);

            try
            {
                if (currentIsolatedStorage is IIsolatedStorageString isolatedStorageString)
                {
                    var stringResult = GetStringWithResult(effectiveToken, securityKey, encoding, isolatedStorageString);
                    result.Exception = stringResult.Exception;
                    result.ErrorMessage = stringResult.ErrorMessage;
                    result.StorageFileName = stringResult.StorageFileName;
                    result.FilePath = stringResult.FilePath;
                    result.IfUsesCustomIsolatedStorageMode = stringResult.IfUsesCustomIsolatedStorageMode;
                    result.StorageFileExists = stringResult.StorageFileExists;
                    result.StorageImplementationTypeName = stringResult.StorageImplementationTypeName;
                    result.SerializedString = stringResult.SourceString;

                    if (!stringResult.IsSuccess)
                    {
                        return result;
                    }

                    result.Model = JsonSerializeHelper.DeserializeFromJson<T>(stringResult.SourceString);
                    result.IsSuccess = true;
                    return result;
                }

                result.Model = currentIsolatedStorage.GetModel<T>(effectiveToken, securityKey, encoding);
                result.SerializedString = JsonSerializeHelper.SerializeToJson(result.Model);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                FillFailureContext(result, ex);
            }

            return result;
        }


    }
}
