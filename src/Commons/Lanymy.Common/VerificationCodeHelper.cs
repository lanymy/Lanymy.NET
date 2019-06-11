using System;
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

            if (verificationCodeLength < 4 || verificationCodeLength > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(verificationCodeLength), "验证码长度(4-10之间)");
            }

            return (isOnlyNumber ? BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0).ToString() : Guid.NewGuid().ToString("N").ToUpper()).RightSubString(verificationCodeLength);

        }

    }

}
