using System;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Interfaces;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 缓存 抽象 基类
    /// </summary>
    public abstract class BaseCache : ICustomMemoryCache
    {


        /// <summary>
        /// key 格式化 格式 字符串 {0}_{1}
        /// </summary>
        protected const string KEY_STRING_FORMAT = "{0}_{1}";


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
        /// 获取 枚举项  默认Key 值   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string GetEnumDefaultKey<T>(Enum key)
        {
            //return GetDefaultKey<T>(string.Format(KEY_STRING_FORMAT, key.GetType().FullName, key));
            return string.Format(KEY_STRING_FORMAT, key.GetType().FullName, key);
        }


        /// <summary>
        /// Key是否存在
        /// </summary>
        /// <param name="key">Key值</param>
        /// <returns></returns>
        public abstract bool IfHaveKey(string key);

        /// <summary>
        /// 默认Key 是否存在
        /// </summary>
        /// <returns></returns>
        public virtual bool IfHaveKey<T>()
        {
            return IfHaveKey(GetDefaultKey<T>());
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
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void SetValue(string key, object value);

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
        /// 根据 Key 设置 Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void SetValue<T>(string key, T value)
        {
            SetValue(key, value as object);
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
        /// 获取Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract object GetValue(string key);

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
        public virtual T GetValue<T>(Enum key)
        {
            return GetValue<T>(GetEnumDefaultKey<T>(key));
        }

        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T GetValue<T>(string key)
        {
            return GetValue(key).ConvertToType<T>();
        }


        /// <summary>
        /// 删除Key
        /// </summary>
        public abstract void RemoveValue(string key);

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
        /// <param name="key"></param>
        public virtual void RemoveValue<T>(Enum key)
        {
            RemoveValue(GetEnumDefaultKey<T>(key));
        }

        /// <summary>
        /// 删除Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public virtual void RemoveValue<T>(string key)
        {
            RemoveValue(key);
        }


        /// <summary>
        /// 清空数据
        /// </summary>
        public abstract void Clear();



    }
}
