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

namespace Lanymy.General.Extension
{

    /// <summary>
    /// 内存模式数据缓存辅助类
    /// </summary>
    public class DataMemoryCache
    {


        #region 单例

        private static DataMemoryCache _DataCache = null;

        private static readonly object SynObject = new object();

        /// <summary>
        /// 获取服务器通信数据中心对象
        /// </summary>
        public static DataMemoryCache Instance()
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

        private IDictionary<string, object> _DicCache = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        public void SetValue(string key, object o)
        {
            lock (SynObject)
            {
                _DicCache[key] = o;
            }
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            lock (SynObject)
            {
                return _DicCache.ContainsKey(key) ? _DicCache[key].ConvertToType<T>() : default(T);
            }
        }

        /// <summary>
        /// 删除缓存项
        /// </summary>
        /// <param name="key"></param>
        public void RemoveValue(string key)
        {

            lock (SynObject)
            {
                if (_DicCache.ContainsKey(key))
                {
                    _DicCache.Remove(key);
                }
            }

        }


        /// <summary>
        /// 主键枚举模式设置缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        public void SetValue(Enum key, object o)
        {
            SetValue(key.ToString(), o);
        }

        /// <summary>
        /// 主键枚举模式 获取 缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(Enum key)
        {
            return GetValue<T>(key.ToString());
        }

        /// <summary>
        /// 主键枚举模式 删除 缓存数据
        /// </summary>
        /// <param name="key"></param>
        public void RemoveValue(Enum key)
        {
            RemoveValue(key.ToString());
        }


        /// <summary>
        /// 使用默认键值 设置缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void SetValue<T>(T t)
        {
            SetValue(t.GetType().FullName, t);
        }

        /// <summary>
        /// 使用默认键值 获取 缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>()
        {
            return GetValue<T>(typeof(T).FullName);
        }

        /// <summary>
        /// 使用默认键值 删除 缓存数据
        /// </summary>
        public void RemoveValue<T>()
        {
            RemoveValue(typeof(T).FullName);
        }


    }

}
