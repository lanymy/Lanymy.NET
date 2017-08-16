/********************************************************************

时间: 2016年08月12日, AM 11:19:58

作者: lanyanmiyu@qq.com

描述: 内存模式数据缓存辅助类

其它:     

********************************************************************/



using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;

namespace Lanymy.General.Extension.Instruments
{

    /// <summary>
    /// 内存模式数据缓存类
    /// </summary>
    public class DataMemoryCache : IDataMemoryCache
    {




        protected ConcurrentDictionary<string, object> _DicCache = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// key 格式化 格式 字符串 {0}_{1}
        /// </summary>
        private const string KEY_STRING_FORMAT = "{0}_{1}";


        /// <summary>
        /// Key是否存在
        /// </summary>
        /// <param name="key">Key值</param>
        /// <returns></returns>
        public virtual bool IfHaveKey(string key)
        {
            return _DicCache.ContainsKey(key);
        }


        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void SetValue(string key, object value)
        {
            _DicCache.AddOrUpdate(key, value, (k, v) => v);
        }


        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual object GetValue(string key)
        {
            object o;
            _DicCache.TryGetValue(key, out o);
            return o;
        }


        /// <summary>
        /// 删除Key
        /// </summary>
        public virtual void RemoveValue(string key)
        {
            object o;
            _DicCache.TryRemove(key, out o);
        }


        /// <summary>
        /// 获取默认Key值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual string GetDefaultKey<T>()
        {
            return typeof(T).FullName;
        }

        /// <summary>
        /// 获取默认Key值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string GetDefaultKey<T>(string key)
        {
            return string.Format(KEY_STRING_FORMAT, key, GetDefaultKey<T>());
        }

        /// <summary>
        /// 默认Key 是否存在
        /// </summary>
        /// <returns></returns>
        public virtual bool IfHaveKey<T>()
        {
            return IfHaveKey(GetDefaultKey<T>());
        }

        /// <summary>
        /// key是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool IfHaveKey<T>(string key)
        {
            return IfHaveKey(GetDefaultKey<T>(key));
        }

        /// <summary>
        /// 使用 默认Key 获取Key 的 Value值
        /// </summary>
        /// <returns></returns>
        public virtual T GetValue<T>()
        {
            return GetValue(GetDefaultKey<T>()).ConvertToType<T>();
        }

        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T GetValue<T>(string key)
        {
            return GetValue(GetDefaultKey<T>(key)).ConvertToType<T>();
        }


        /// <summary>
        /// 使用 默认Key 设置 Key的Value值
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetValue<T>(T value)
        {
            SetValue(GetDefaultKey<T>(), value);
        }

        /// <summary>
        /// 根据 Key 设置 Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void SetValue<T>(string key, T value)
        {
            SetValue(GetDefaultKey<T>(key), value as object);
        }

        /// <summary>
        /// 使用 默认Key 删除Key
        /// </summary>
        public virtual void RemoveValue<T>()
        {
            RemoveValue(GetDefaultKey<T>());
        }

        /// <summary>
        /// 删除Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public virtual void RemoveValue<T>(string key)
        {
            RemoveValue(GetDefaultKey<T>(key));
        }


        /// <summary>
        /// 获取 枚举项  默认Key 值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string GetEnumDefaultKey<T>(Enum key)
        {
            return GetDefaultKey<T>(string.Format(KEY_STRING_FORMAT, key.GetType().FullName, key));
        }

        /// <summary>
        /// 是否存在Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool IfHaveKey<T>(Enum key)
        {
            return IfHaveKey(GetEnumDefaultKey<T>(key));
        }

        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T GetValue<T>(Enum key)
        {
            return GetValue<T>(GetEnumDefaultKey<T>(key));
        }

        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        public virtual void SetValue<T>(Enum key, T t)
        {
            SetValue(GetEnumDefaultKey<T>(key), t);
        }


        /// <summary>
        /// 删除Key
        /// </summary>
        /// <param name="key"></param>
        public virtual void RemoveValue<T>(Enum key)
        {
            RemoveValue(GetEnumDefaultKey<T>(key));
        }


    }

}
