using System;
using System.Collections.Generic;
using System.Text;

namespace Lanymy.Common.Interfaces
{
    /// <summary>
    /// 数组 扩展 功能 接口
    /// </summary>
    public interface IArrayExtension
    {

        /// <summary>
        /// 数组合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstArray"></param>
        /// <param name="secondArray"></param>
        /// <param name="arrayParams"></param>
        /// <returns></returns>
        T[] MergerArray<T>(T[] firstArray, T[] secondArray, params T[][] arrayParams);


        /// <summary>
        /// 截取数据片段 默认 length=0;  length=0 startIndex 位置开始向左截取到最后 ; length&lt;0 startIndex 位置开始向左截取 length; length&gt;0 startIndex 位置开始向右截取 length;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceArray"></param>
        /// <param name="startIndex">截取起始索引值</param>
        /// <param name="length">默认 length=0;  length=0 startIndex 位置开始向左截取到最后 ; length&lt;0 startIndex 位置开始向左截取 length; length&gt;0 startIndex 位置开始向右截取 length;</param>
        /// <returns></returns>
        T[] SubArray<T>(T[] sourceArray, int startIndex, int length = 0);



        /// <summary>
        /// 数组是否完全相等 (包括数组内的值和顺序是否完全相等)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstArray"></param>
        /// <param name="secondArray"></param>
        /// <returns></returns>
        bool ArrayEquals<T>(T[] firstArray, T[] secondArray);



        /// <summary>
        /// 获取子集索引
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subset"></param>
        /// <returns></returns>
        int GetSubsetIndex(byte[] source, byte[] subset);



        /// <summary>
        /// 判断是否是子集  顺序也完全一致的 子集序列
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subset"></param>
        /// <returns></returns>
        bool IsSubset(byte[] source, byte[] subset);

    }
}
