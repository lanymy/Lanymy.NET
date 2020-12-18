using System.Text;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common.Helpers
{

    public class BinarySerializeHelper
    {
        /// <summary>
        /// 把对象序列化成二进制数据
        /// </summary>
        /// <param name="t">要序列化的对象</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        public static byte[] SerializeToBytes<T>(T t, Encoding encoding = null) where T : class
        {
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;
            return encoding.GetBytes(JsonSerializeHelper.SerializeToJson(t));
        }

        /// <summary>
        /// 反序列化二进制数据成对象
        /// </summary>
        /// <typeparam name="T">要反序列化成对象 的 对象类型</typeparam>
        /// <param name="bytes">需要反序列化处理的二进制数据</param>
        /// <param name="encoding">编码 Null 使用默认编码</param>
        /// <returns></returns>
        public static T DeserializeFromBytes<T>(byte[] bytes, Encoding encoding = null) where T : class
        {
            if (encoding.IfIsNullOrEmpty()) encoding = DefaultSettingKeys.DEFAULT_ENCODING;
            return JsonSerializeHelper.DeserializeFromJson<T>(encoding.GetString(bytes));
        }

    }

}
