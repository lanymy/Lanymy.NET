// *******************************************************************
// 创建时间：2013年3月20日, PM 02:05:45
// 作者：lanyanmiyu@qq.com
// 说明：字典扩展类
// 其它:
// *******************************************************************


using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace Lanymy.General.Extension.ExtensionFunctions
{



    /// <summary>
    /// 字典扩展类
    /// </summary>
    public static class DictionaryExtension
    {


        /// <summary>
        /// 将键和值添加或替换到字典中：如果不存在，则添加；存在，则替换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            dic[key] = value;
        }


        /// <summary>
        /// 获取与指定的键相关联的值，如果没有则返回输入的默认值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultValue = default(TValue))
        {
            return dic.ContainsKey(key) ? dic[key] : defaultValue;
        }

 
        /// <summary>
        /// 向字典中批量添加键值对
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="keyValuePairs"></param>
        /// <param name="replaceExisted">如果已存在，是否替换</param>
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs, bool replaceExisted = true)
        {

            foreach (var item in keyValuePairs)
            {
                if (replaceExisted || !dic.ContainsKey(item.Key))
                    dic.AddOrReplace(item.Key, item.Value);
            }
        }

    }


}
