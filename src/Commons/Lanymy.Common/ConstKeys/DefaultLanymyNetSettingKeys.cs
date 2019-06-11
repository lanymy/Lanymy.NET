using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.ConstKeys
{

    /// <summary>
    /// Lanymy.Net 默认 配置  常量
    /// </summary>
    public class DefaultLanymyNetSettingKeys
    {


        /// <summary>
        /// 默认密钥
        /// </summary>
        internal const string DEFAULT_SECURITY_KEY = "lanymy";


        /// <summary>
        /// MD5 加密 默认掩码
        /// </summary>
        internal const string DEFAULT_MD5_SALT = "5F207A0B";



        /// <summary>
        /// 客户端配置表文件全名称 Lanymy.Client.config
        /// </summary>
        public const string CLIENT_CONFIG_FILE_FULL_NAME = "Lanymy.Client.config";

        /// <summary>
        /// 全局配置表文件全名称 Lanymy.config
        /// </summary>
        public const string LANYMY_CONFIG_FILE_FULL_NAME = "Lanymy.config";

        /// <summary>
        /// WEBAPI配置文件全名称 Lanymy.WebAPI.config
        /// </summary>
        public const string LANYMY_WEBAPI_CONFIG_FILE_FULL_NAME = "Lanymy.WebAPI.config";

        /// <summary>
        /// 版本信息 描述文件 全名称 version.lanymy
        /// </summary>
        public const string VERSION_FILE_FULL_NAME = "version.lanymy";
    }


}
