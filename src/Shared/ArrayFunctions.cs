/********************************************************************

时间: 2016年05月25日, AM 10:50:38

作者: lanyanmiyu@qq.com

描述: 数组辅助类

其它:     

********************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;

namespace Lanymy.General.Extension
{

    /// <summary>
    /// 数组辅助类
    /// </summary>
    public class ArrayFunctions
    {


        /// <summary>
        /// 数组合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstArray"></param>
        /// <param name="secondArray"></param>
        /// <param name="arrayParams"></param>
        /// <returns></returns>
        public static T[] MergerArray<T>(T[] firstArray, T[] secondArray, params T[][] arrayParams)
        {

            List<T> list = new List<T>();

            if (!firstArray.IfIsNullOrEmpty())
                list.AddRange(firstArray);
            if (!secondArray.IfIsNullOrEmpty())
                list.AddRange(secondArray);

            foreach (var arrayParam in arrayParams)
            {
                if (!arrayParam.IfIsNullOrEmpty())
                {
                    list.AddRange(arrayParam);
                }
            }

            return list.ToArray();

        }

        /// <summary>
        /// 截取数据片段 默认 length=0;  length=0 startIndex 位置开始向左截取到最后 ; length&lt;0 startIndex 位置开始向左截取 length; length&gt;0 startIndex 位置开始向右截取 length;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceArray"></param>
        /// <param name="startIndex">截取起始索引值</param>
        /// <param name="length">默认 length=0;  length=0 startIndex 位置开始向左截取到最后 ; length&lt;0 startIndex 位置开始向左截取 length; length&gt;0 startIndex 位置开始向右截取 length;</param>
        /// <returns></returns>
        public static T[] SubArray<T>(T[] sourceArray, int startIndex, int length = 0)
        {

            int lengthArray = sourceArray.Length;

            if (startIndex < 0 || startIndex >= lengthArray)
                throw new ArgumentOutOfRangeException("startIndex");

            int endIndex = startIndex + length;

            if (endIndex + 1 < 0 || endIndex - 1 >= lengthArray)
                throw new ArgumentOutOfRangeException("length");

            T[] result = null;

            if (length == 0)
            {
                result = sourceArray.Skip(startIndex).ToArray();
            }
            else if (length > 0)
            {
                result = sourceArray.Skip(startIndex).Take(length).ToArray();
            }
            else if (length < 0)
            {
                result = sourceArray.Skip(endIndex + 1).Take(Math.Abs(length)).ToArray();
            }

            return result;

        }


        /// <summary>
        /// 数组是否完全相等 (包括数组内的值和顺序是否完全相等)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstArray"></param>
        /// <param name="secondArray"></param>
        /// <returns></returns>
        public static bool ArrayEquals<T>(T[] firstArray, T[] secondArray)
        {
            return firstArray.SequenceEqual(secondArray);
        }


        /// <summary>
        /// 获取子集索引
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subset"></param>
        /// <returns></returns>
        public static int GetSubsetIndex(byte[] source, byte[] subset)
        {

            if (source.IfIsNullOrEmpty() || subset.IfIsNullOrEmpty())
            {
                return -1;
            }

            List<byte> sourceList = new List<byte>(source);


            int index = sourceList.IndexOf(subset[0], 0);

            while (index >= 0)
            {
                if (sourceList.Skip(index).Take(subset.Length).SequenceEqual(subset))
                {
                    return index;
                }

                index = sourceList.IndexOf(subset[0], index);
            }


            return -1;
        }


        /// <summary>
        /// 判断是否是子集  顺序也完全一致的 子集序列
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subset"></param>
        /// <returns></returns>
        public static bool IsSubset(byte[] source, byte[] subset)
        {
            return GetSubsetIndex(source, subset) >= 0;
        }


    }


}
