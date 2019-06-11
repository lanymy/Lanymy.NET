namespace Lanymy.Common.Interfaces.IKeyValues
{
    /// <summary>
    /// Key Value 操作 功能 接口
    /// </summary>
    public interface IKeyValue
    {

        /// <summary>
        /// Key是否存在
        /// </summary>
        /// <param name="key">Key值</param>
        /// <returns></returns>
        bool IfHaveKey(string key);

        /// <summary>
        /// 设置Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetValue(string key, object value);


        /// <summary>
        /// 获取Key的Value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetValue(string key);

        ///// <summary>
        ///// 获取Key的Value值
        ///// </summary>
        ///// <typeparam name="T">Value 类型</typeparam>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //T GetValue<T>(string key);

        /// <summary>
        /// 删除Key
        /// </summary>
        void RemoveValue(string key);

        /// <summary>
        /// 清空数据
        /// </summary>
        void Clear();

    }
}
