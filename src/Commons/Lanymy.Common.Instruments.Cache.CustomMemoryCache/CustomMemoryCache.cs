using System.Collections.Concurrent;

namespace Lanymy.Common.Instruments
{
    /// <summary>
    /// 自定义 内存模式数据缓存类 无过期参数(依赖项,过期时间等)
    /// </summary>
    public class CustomMemoryCache : BaseCache
    {




        protected ConcurrentDictionary<string, object> _DicCache = new ConcurrentDictionary<string, object>();


        /// <summary>
        /// Key是否存在
        /// </summary>
        /// <param name="key">Key值</param>
        /// <returns></returns>
        public override bool IfHaveKey(string key)
        {
            return _DicCache.ContainsKey(key);
        }

        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void SetValue(string key, object value)
        {
            _DicCache.AddOrUpdate(key, value, (k, v) => v);
        }

        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object GetValue(string key)
        {
            _DicCache.TryGetValue(key, out object o);
            return o;
        }

        /// <summary>
        /// 删除Key
        /// </summary>
        public override void RemoveValue(string key)
        {
            _DicCache.TryRemove(key, out _);
        }


        /// <summary>
        /// 清空数据
        /// </summary>
        public override void Clear()
        {
            _DicCache.Clear();
        }

    }
}
