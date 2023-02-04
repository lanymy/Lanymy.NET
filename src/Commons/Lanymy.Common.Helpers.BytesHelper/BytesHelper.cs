using System;

namespace Lanymy.Common.Helpers
{
    public class BytesHelper
    {
        /// <summary>
        /// 16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hexString, string separator = " ")
        {

            hexString = hexString.Replace(separator, "");

            if ((hexString.Length % 2) != 0)
                hexString += " ";

            var returnBytes = new byte[hexString.Length / 2];

            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            return returnBytes;

        }


        /// <summary>
        /// 从字节数组转换成16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string HexStringFromBytes(byte[] bytes, string separator = " ")
        {
            return BitConverter.ToString(bytes, 0).Replace("-", separator).ToUpper();
        }

    }
}