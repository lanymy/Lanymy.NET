using System.Runtime.CompilerServices;
using System.Text;

//[assembly: InternalsVisibleTo("Lanymy.Common.Helpers.SecurityHelper")]
[assembly: InternalsVisibleTo("Lanymy.Common.Instruments.Crypto")]
[assembly: InternalsVisibleTo("Lanymy.Common.Instruments.IsolatedStorages")]

namespace Lanymy.Common.ConstKeys
{

    /// <summary>
    /// 默认 配置项 常量
    /// </summary>
    public class DefaultSettingKeys
    {


        /// <summary>
        /// 默认密钥
        /// </summary>
        internal const string DEFAULT_CRYPTO_KEY = "lanymy";


        /// <summary>
        /// 默认密钥长度
        /// </summary>
        internal const int DEFAULT_CRYPTO_KEY_SIZE = 16;


        /// <summary>
        /// MD5 加密 默认掩码
        /// </summary>
        internal const string DEFAULT_MD5_SALT = "5F207A0B";

        /// <summary>
        /// 默认加密分隔符
        /// </summary>
        internal const string DEFAULT_CRYPTO_SPLIT_STRING = "[$@lanymy@$]";


        /// <summary>
        /// 默认编码UTF8
        /// </summary>
        public static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;




    }
}
