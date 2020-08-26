using System.Text;
using Lanymy.Common.Instruments;
using Lanymy.Common.Instruments.IIsolatedStorages;
using Xb.Common.Helpers;

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
            GenericityHelper.GetInterface(isolatedStorageString, DefaultIsolatedStorage).SaveString(sourceString, token, securityKey, encoding);
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
            return GenericityHelper.GetInterface(isolatedStorageString, DefaultIsolatedStorage).GetString(token, securityKey, encoding);
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
            GenericityHelper.GetInterface(isolatedStorageModel, DefaultIsolatedStorage).SaveModel(t, token, securityKey, encoding);
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
            return GenericityHelper.GetInterface(isolatedStorageModel, DefaultIsolatedStorage).GetModel<T>(token, securityKey, encoding);
        }


    }
}
