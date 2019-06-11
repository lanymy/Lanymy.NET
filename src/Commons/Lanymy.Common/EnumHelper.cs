using System;
using System.Collections.Generic;
using System.Linq;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Cache;
using Lanymy.Common.Interfaces.ICaches;
using Lanymy.Common.Models;

namespace Lanymy.Common
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public class EnumHelper
    {



        /// <summary>
        /// 枚举内存缓存器
        /// </summary>
        private static ICustomMemoryCache _DataMemoryCache = new CustomMemoryCache();

        /// <summary>
        /// 线程 对象 锁
        /// </summary>
        private static readonly object SynObject = new object();



        /// <summary>
        /// 获取 枚举类型 缓存 主键值
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        private static string GetEnumMapCacheKey(Type enumType)
        {
            return enumType.FullName;
        }


        /// <summary>
        /// 获取当前枚举映射导航器
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        private static EnumMapModel GetMapper(Type enumType)
        {
            string cacheKey = GetEnumMapCacheKey(enumType);
            var mapper = _DataMemoryCache.GetValue<EnumMapModel>(cacheKey);
            if (mapper.IfIsNullOrEmpty())
            {
                mapper = new EnumMapModel(enumType);
                _DataMemoryCache.SetValue(cacheKey, mapper);
            }
            return mapper;
        }


        /// <summary>
        /// 获取Enum指定项
        /// </summary>
        /// <param name="enumItem"></param>
        /// <returns></returns>
        public static EnumItem GetEnumItem(Enum enumItem)
        {
            return GetMapper(enumItem.GetType())[enumItem];
        }


        /// <summary>
        /// 获取Enum的子项 主键是 枚举子项 的字典类型集合
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyDictionary<Enum, EnumItem> GetEnumItemDictionary<TEnum>()
            where TEnum : IComparable, IFormattable, IConvertible
        {
            return GetMapper(typeof(TEnum)).DicEnumMap;
        }


        /// <summary>
        /// 获取Enum的子项集合
        /// </summary>
        /// <returns></returns>
        public static List<EnumItem> GetEnumItemList<TEnum>()
            where TEnum : IComparable, IFormattable, IConvertible
        {
            return GetMapper(typeof(TEnum)).DicEnumMap.Select(o => o.Value).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static List<EnumItem> GetEnumItemListRemoveEnumItems<TEnum>(params TEnum[] removeEnumItems)
            where TEnum : IComparable, IFormattable, IConvertible
        {

            var enumItemList = GetEnumItemList<TEnum>();

            if (!removeEnumItems.IfIsNullOrEmpty())
            {
                var list = removeEnumItems.Select(o => o.ToString()).ToList();
                enumItemList = enumItemList.Where(o => !list.Contains(o.CurrentEnum.ToString())).ToList();
            }

            //return GetMapper(typeof(TEnum)).DicEnumMap.Select(o => o.Value).Where(a => a.CurrentEnum.ToString() != "UnDefine").ToList();

            return enumItemList;

        }


        /// <summary>
        /// 获取枚举多选项列表 主键是 枚举子项 的字典类型集合
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Enum, EnumItem> GetEnumFlagsItemDictionary(Enum item)
        {
            List<string> flags = item.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToList();
            if (flags.IfIsNullOrEmpty())
            {
                return new Dictionary<Enum, EnumItem>();
            }
            var mapDic = GetMapper(item.GetType()).DicEnumMap;
            return mapDic.Where(o => flags.Contains(o.Value.CurrentEnum.ToString())).ToDictionary(dicItem => dicItem.Key, dicItem => dicItem.Value);
        }


        /// <summary>
        /// 获取枚举多选项列表
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<EnumItem> GetEnumFlagsItemList(Enum item)
        {
            return GetEnumFlagsItemDictionary(item).Select(o => o.Value).ToList();
        }



        //enum  flags 操作
        //http://blog.csdn.net/lindexi_gd/article/details/60744821

        /// <summary>
        /// 计算 是否 包含 标记
        /// </summary>
        /// <param name="itemSource">标记数据源</param>
        /// <param name="item">标记</param>
        /// <returns></returns>
        public static bool HasFlag(Enum itemSource, Enum item)
        {
            return itemSource.HasFlag(item);
        }



        //public static Dictionary<string, Enum> GetAttributeTitles()
        //{
        //    Dictionary<string, Enum> list = new Dictionary<string, Enum>();



        //    return list;
        //}


    }
}