using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.ConstKeys
{


    /// <summary>
    /// 默认 配置项 常量
    /// </summary>
    public class DefaultSettingKeys
    {


        /// <summary>
        /// 默认缓冲区大小 1024 * 4
        /// </summary>
        public const int DEFAULT_BUFFERSIZE = 1024 * 4;

        /// <summary>
        /// 默认时间格式化字符串yyyy-MM-dd hh:mm:ss.fff 
        /// </summary>
        public const string DEFAULT_DATE_FORMAT_STRING = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// http 路径中 默认时间格式化字符串 yyyyMMddHHmmssfff
        /// </summary>
        public const string DEFAULT_HTTP_URI_DATE_FORMAT_STRING = "yyyyMMddHHmmssfff";

        /// <summary>
        /// 默认编码UTF8
        /// </summary>
        public static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;



        /// <summary>
        /// log4net 配置 文件 全名称 log4net.config
        /// </summary>
        public const string LOG4NET_CONFIG_FILE_FULL_NAME = "log4net" + FileExtensionKeys.DLL_FILE_EXTENSION;


        /// <summary>
        /// NLog 配置 文件 全名称 NLog.config
        /// </summary>
        public const string NLOG_CONFIG_FILE_FULL_NAME = "NLog" + FileExtensionKeys.DLL_FILE_EXTENSION;



    }


}
