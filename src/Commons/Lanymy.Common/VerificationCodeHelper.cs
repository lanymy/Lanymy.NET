using System;
using System.IO;
using Lanymy.Common.ConstKeys;
using Lanymy.Common.ExtensionFunctions;

namespace Lanymy.Common
{


    /// <summary>
    /// 验证码 辅助类
    /// </summary>
    public class VerificationCodeHelper
    {
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="verificationCodeLength">默认值4,验证码长度(4-10之间)</param>
        /// <param name="isOnlyNumber">True 纯数字;False 数字字母混合;默认值 False</param>
        /// <returns></returns>
        public static string CreateVerificationCode(byte verificationCodeLength = 4, bool isOnlyNumber = false)
        {

            const byte MIN_LENGTH = 4;
            const byte MAX_LENGTH = 10;

            if (verificationCodeLength < MIN_LENGTH || verificationCodeLength > MAX_LENGTH)
            {
                throw new ArgumentOutOfRangeException(nameof(verificationCodeLength), string.Format("验证码长度({0}-{1}之间)", MIN_LENGTH, MAX_LENGTH));
            }

            var randomCodeStr = Path.GetRandomFileName().Replace(".", "");

            if (isOnlyNumber)
            {
                randomCodeStr = BitConverter.ToUInt32(DefaultSettingKeys.DEFAULT_ENCODING.GetBytes(randomCodeStr), 0).ToString().PadLeft(MAX_LENGTH, '0');
            }

            return randomCodeStr.RightSubString(verificationCodeLength).ToUpper();

        }

    }

}
