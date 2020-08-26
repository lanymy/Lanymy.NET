using System.Collections.Generic;

namespace Lanymy.Common.Helpers
{
    public class DataTableSerializeHelper
    {
        #region DataTable 序列化

        /// <summary>
        /// 序列化实体类数据集合成 DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="ifClearList">是否清空 数据源 集合 释放资源</param>
        /// <returns></returns>
        public static System.Data.DataTable SerializeToDataTable<T>(List<T> list, bool ifClearList = false) where T : class
        {
            var dt = JsonSerializeHelper.DeserializeFromJson<System.Data.DataTable>(JsonSerializeHelper.SerializeToJson(list));

            if (ifClearList)
            {
                list?.Clear();
            }

            return dt;
        }

        /// <summary>
        /// 反序列化 DataTable 成 实体类数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="ifClearDataTable">是否清空 数据源 集合 释放资源</param>
        /// <returns></returns>
        public static List<T> DeserializeFromDataTable<T>(System.Data.DataTable dt, bool ifClearDataTable = false) where T : class
        {
            var list = JsonSerializeHelper.DeserializeFromJson<List<T>>(JsonSerializeHelper.SerializeToJson(dt));

            if (ifClearDataTable)
            {
                dt?.Clear();
            }

            return list;
        }

        #endregion
    }
}
