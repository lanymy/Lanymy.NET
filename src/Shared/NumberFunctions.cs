// *******************************************************************
// 创建时间：2015年01月14日, AM 10:04:09
// 作者：lanyanmiyu@qq.com
// 说明：数字类型辅助方法
// 其它:
// *******************************************************************



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lanymy.General.Extension
{

    /// <summary>
    /// 数字类型辅助方法
    /// </summary>
    /// <summary>
    /// 整型辅助方法
    /// </summary>
    public class NumberFunctions
    {


        #region ChineseCurrencyConverter 金额转人民币大写(比如叁亿万零捌佰)


        private static readonly string[] UNITS = { "", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", };
        private static readonly string[] CHINESE_NUMBER = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };

        /// <summary>
        /// 金额转人民币大写(比如叁亿万零捌佰)
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string ChineseCurrencyConverter(int amount)
        {
            if (amount >= 1e9 || amount < 0)
            {
                return "超出系统处理范围了。";
            }

            string text = "";
            for (int i = 8; i >= 0; i--)
            {
                int number = amount / ChineseCurrencyConverterE(i);
                if (number == 0 && string.IsNullOrEmpty(text))
                {
                    continue;
                }
                else if (number == 0 && text.EndsWith("零"))
                {
                    if (i == 4)
                    {
                        text = text.Substring(0, text.Length - 1);
                        if (!text.EndsWith("亿"))
                        {
                            text += "万";
                        }
                    }
                    continue;
                }
                else
                {
                    text += CHINESE_NUMBER[number];
                    if (i != 4 && number != 0)
                    {
                        text += UNITS[i];
                    }
                    else if (i == 4)
                    {
                        if (text.EndsWith("零"))
                        {
                            text = text.Substring(0, text.Length - 1);
                        }
                        text += "万";
                    }
                }
                amount -= number * ChineseCurrencyConverterE(i);
            }
            if (text.EndsWith("零"))
            {
                text = text.Substring(0, text.Length - 1);
            }
            text += "元整";
            return text;

        }

        private static int ChineseCurrencyConverterE(int len)
        {
            int result = 1;
            for (int i = 0; i < len; i++)
            {
                result *= 10;
            }
            return result;
        }




        #endregion //ChineseCurrencyConverter 金额转人民币大写(比如叁亿万零捌佰)


    }


}
