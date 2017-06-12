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

namespace Lanymy.General.Extension
{

    /// <summary>
    /// 内存模式数据缓存辅助类
    /// </summary>
    public class DataMemoryCache: IDataMemoryCache
    {


        #region 单例

        private static IDataMemoryCache _DataCache = null;

        private static readonly object SynObject = new object();

        /// <summary>
        /// 获取服务器通信数据中心对象
        /// </summary>
        public static IDataMemoryCache Instance()
        {

            if (null == _DataCache)
            {
                lock (SynObject)
                {
                    if (null == _DataCache)
                    {
                        _DataCache = new DataMemoryCache();
                    }
                }
            }

            return _DataCache;
        }
        #endregion

        protected ConcurrentDictionary<string, object> _DicCache = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 获取默认Key值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual string GetDefaultKey<T>()
        {
            return typeof(T).Name;
        }

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
            _DicCache.TryGetValue(key, out object o);
            return o;
        }

        /// <summary>
        /// 删除Key
        /// </summary>
        public virtual void RemoveValue(string key)
        {
            _DicCache.TryRemove(key,out object o);
        }


        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T GetValue<T>(string key)
        {
            _DicCache.TryGetValue(key , out object o);
            return o.ConvertToType<T>();
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
        /// 使用 默认Key 设置 Key的Value值
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetValue<T>(object value)
        {
            SetValue(GetDefaultKey<T>(), value);
        }
        /// <summary>
        /// 使用 默认Key 获取Key 的 Value值
        /// </summary>
        /// <returns></returns>
        public virtual object GetValue<T>()
        {
            return GetValue(GetDefaultKey<T>());
        }
        /// <summary>
        /// 使用 默认Key 删除Key
        /// </summary>
        public virtual void RemoveValue<T>()
        {
            RemoveValue(GetDefaultKey<T>());
        }

        /// <summary>
        /// 是否存在Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool IfHaveKey(Enum key)
        {
            return IfHaveKey(key.ToString());
        }
        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        public virtual void SetValue<T>(Enum key, T t)
        {
            SetValue(key.ToString(), t);
        }

        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T GetValue<T>(Enum key)
        {
            return GetValue<T>(key.ToString());
        }
        /// <summary>
        /// 删除Key
        /// </summary>
        /// <param name="key"></param>
        public virtual void RemoveValue(Enum key)
        {
            RemoveValue(key.ToString());
        }
    }

}
