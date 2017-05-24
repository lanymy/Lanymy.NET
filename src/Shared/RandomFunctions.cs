/********************************************************************

时间: 2016年08月15日, PM 06:39:08

作者: lanyanmiyu@qq.com

描述: 随机数辅助方法

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
    /// 随机数辅助方法
    /// </summary>
    public class RandomFunctions
    {

        /// <summary>
        /// 一次性 获取 不重复 随机数
        /// </summary>
        /// <param name="numCount">一次性获取多少个随机数</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns></returns>
        public static int[] GetNoRepeatRandomNum(int numCount, int minValue, int maxValue)
        {
            if (numCount > Math.Abs(maxValue - minValue))
            {
                throw new ArgumentOutOfRangeException("numCount", "numCount超出 minValue和maxValue之间的总数,必然会有重复数出现");
            }

            if (numCount <= 0)
            {
                throw new ArgumentException("numCount");
            }

            Random random = new Random(unchecked((int)DateTime.Now.Ticks));

            int[] numArray = new int[numCount];

            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = GetRandomNum(numArray, minValue, maxValue, random);
            }

            return numArray;

        }


        private static int GetRandomNum(int[] numArray, int minValue, int maxValue, Random random)
        {
            int num = random.Next(minValue, maxValue);
            if (numArray.Contains(num))
            {
                return GetRandomNum(numArray, minValue, maxValue, random);
            }
            else
            {
                return num;
            }
        }


    }

}
