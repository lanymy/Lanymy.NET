/********************************************************************

时间: 2016年05月25日, AM 10:15:58

作者: lanyanmiyu@qq.com

描述: 全局辅助类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanymy.General.Extension
{

    /// <summary>
    /// 全局辅助类
    /// </summary>
    public sealed class GlobalSettings
    {

        /// <summary>
        /// 默认时间格式化字符串yyyy-MM-dd hh:mm:ss.fff
        /// </summary>
        public const string DEFAULT_DATE_FORMAT_STRING  = "yyyy-MM-dd hh:mm:ss.fff";

        /// <summary>
        /// 默认编码UTF8
        /// </summary>
        public static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;


        /// <summary>
        /// 默认密钥
        /// </summary>
        internal const string DEFAULT_SECURITY_KEY = "lanyanmiyu@qq.com";

    }


}
