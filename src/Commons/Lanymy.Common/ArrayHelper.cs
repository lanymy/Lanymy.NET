using Lanymy.Common.Instruments;
using Lanymy.Common.Interfaces;

namespace Lanymy.Common
{
    /// <summary>
    /// 数组辅助类
    /// </summary>
    public class ArrayHelper
    {

        /// <summary>
        /// 默认数组辅助方法
        /// </summary>
        public static readonly IArrayExtension DefaultArrayExtension = new ArrayExtension();


        /// <summary>
        /// 数组合并
        /// </summary>
        /// <typeparam name="T">数组元数据类型</typeparam>
        /// <param name="firstArray">第一个数组</param>
        /// <param name="secondArray">第二个数组</param>
        /// <param name="arrayExtension">null 则使用默认数组扩展方法</param>
        /// <param name="arrayParams">其它多个数组</param>
        /// <returns></returns>
        public static T[] MergerArray<T>(T[] firstArray, T[] secondArray, IArrayExtension arrayExtension = null, params T[][] arrayParams)
        {
            return GenericityHelper.GetInterface(arrayExtension, DefaultArrayExtension).MergerArray(firstArray, secondArray, arrayParams);
        }

        /// <summary>
        /// 截取数据片段 默认 length=0;  length=0 startIndex 位置开始向左截取到最后 ; length&lt;0 startIndex 位置开始向左截取 length; length&gt;0 startIndex 位置开始向右截取 length;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceArray">源数组</param>
        /// <param name="startIndex">截取起始索引值</param>
        /// <param name="length">默认 length=0;  length=0 startIndex 位置开始向左截取到最后 ; length&lt;0 startIndex 位置开始向左截取 length; length&gt;0 startIndex 位置开始向右截取 length;</param>
        /// <param name="arrayExtension">null 则使用默认数组扩展方法</param>
        /// <returns></returns>
        public static T[] SubArray<T>(T[] sourceArray, int startIndex, int length = 0, IArrayExtension arrayExtension = null)
        {

            return GenericityHelper.GetInterface(arrayExtension, DefaultArrayExtension).SubArray(sourceArray, startIndex, length);

        }


        /// <summary>
        /// 数组是否完全相等 (包括数组内的值和顺序是否完全相等)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstArray"></param>
        /// <param name="secondArray"></param>
        /// <param name="arrayExtension">null 则使用默认数组扩展方法</param>
        /// <returns></returns>
        public static bool ArrayEquals<T>(T[] firstArray, T[] secondArray, IArrayExtension arrayExtension = null)
        {
            return GenericityHelper.GetInterface(arrayExtension, DefaultArrayExtension).ArrayEquals(firstArray, secondArray);
        }


        /// <summary>
        /// 获取子集索引
        /// </summary>
        /// <param name="source">源数组</param>
        /// <param name="subset">子数组</param>
        /// <param name="arrayExtension">null 则使用默认数组扩展方法</param>
        /// <returns></returns>
        public static int GetSubsetIndex(byte[] source, byte[] subset, IArrayExtension arrayExtension = null)
        {
            return GenericityHelper.GetInterface(arrayExtension, DefaultArrayExtension).GetSubsetIndex(source, subset);
        }


        /// <summary>
        /// 判断是否是子集  顺序也完全一致的 子集序列
        /// </summary>
        /// <param name="source">源数组</param>
        /// <param name="subset">子数组</param>
        /// <param name="arrayExtension">null 则使用默认数组扩展方法</param>
        /// <returns></returns>
        public static bool IsSubset(byte[] source, byte[] subset, IArrayExtension arrayExtension = null)
        {
            return GenericityHelper.GetInterface(arrayExtension, DefaultArrayExtension).IsSubset(source, subset);
        }


    }
}
