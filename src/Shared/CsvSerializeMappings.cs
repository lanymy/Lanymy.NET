/********************************************************************

时间: 2017年05月28日, PM 06:28:45

作者: lanyanmiyu@qq.com

描述: CSV序列化设置绑定

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Interfaces;
using Lanymy.General.Extension.Models;

namespace Lanymy.General.Extension
{


    /// <summary>
    /// CSV序列化设置绑定
    /// </summary>
    public static class CsvSerializeMappings
    {

        /// <summary>
        /// 全局 CSV序列化 设置绑定  映射字典
        /// </summary>
        internal static readonly Dictionary<Type, object> DicCsvSerializeSettings = new Dictionary<Type, object>();

        /// <summary>
        /// 开始属性映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static CsvMappingShellModel<T> StartMap<T>() where T : class
        {
            return new CsvMappingShellModel<T>();
        }


        /// <summary>
        /// CSV属性 自定义 序列化 映射
        /// </summary>
        /// <typeparam name="T">CSV数据实体类</typeparam>
        /// <param name="csvMappingShellModel">单纯 用于 自动关联 映射 方法  方便调用.</param>
        /// <param name="propertyNameExpre">要映射的属性表达式</param>
        /// <param name="propertySerializeFunc">CSV序列化自定义方法</param>
        /// <param name="propertyDeserializeFunc">CSV反序列化自定义方法</param>
        /// <returns></returns>
        public static CsvMappingShellModel<T> MapCSVSerializeProperty<T>(this CsvMappingShellModel<T> csvMappingShellModel, Expression<Func<T, object>> propertyNameExpre, Func<T, string> propertySerializeFunc, Func<string, object> propertyDeserializeFunc) where T : class
        {

            Type type = typeof(T);

            if (!DicCsvSerializeSettings.ContainsKey(type))
            {
                DicCsvSerializeSettings.AddOrReplace(type, new List<CSVSerializeMapModel<T>>());
            }

            string propertyName = ReflectionFunctions.GetPropertyName(propertyNameExpre);
            if (!propertyName.IfIsNullOrEmpty())
            {
                var list = DicCsvSerializeSettings[type] as List<CSVSerializeMapModel<T>>;
                var currentMapModel = list.Where(o => o.CsvModelType == type && o.PropertyName == propertyName).FirstOrDefault();

                if (currentMapModel.IfIsNullOrEmpty())
                {
                    list.Add(new CSVSerializeMapModel<T> { CsvModelType = type, PropertyName = propertyName, PropertySerializeFunc = propertySerializeFunc, PropertyDeserializeFunc = propertyDeserializeFunc });

                }
                else
                {
                    currentMapModel.PropertySerializeFunc = propertySerializeFunc;
                    currentMapModel.PropertyDeserializeFunc = propertyDeserializeFunc;
                }

            }

            return csvMappingShellModel;

        }

    }


}
