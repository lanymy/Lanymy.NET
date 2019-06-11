using System.Text;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Instruments.Serializer
{
    /// <summary>
    /// 序列化器基类
    /// </summary>
    public abstract class BaseSerializer
    {
        /// <summary>
        /// 编码
        /// </summary>
        public readonly Encoding CurrentEncoding = DefaultSettingKeys.DEFAULT_ENCODING;

        /// <summary>
        /// 序列化器 构造方法
        /// </summary>
        /// <param name="encoding">编码</param>
        protected BaseSerializer(Encoding encoding)
        {
            if (!encoding.IfIsNullOrEmpty())
            {
                CurrentEncoding = encoding;
            }
        }

    }
}
