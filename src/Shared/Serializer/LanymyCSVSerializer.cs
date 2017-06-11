/********************************************************************

时间: 2017年05月28日, PM 12:00:15

作者: lanyanmiyu@qq.com

描述: CSV序列化器

其它:     

********************************************************************/



using Lanymy.General.Extension.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lanymy.General.Extension.CustomAttributes;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Models;

namespace Lanymy.General.Extension.Serializer
{

    /// <summary>
    /// CSV序列化器
    /// </summary>
    public class LanymyCsvSerializer<TModel> : ICsvSerializer<TModel> where TModel : class, new()
    {

        public static readonly Type CurrentTModelType = typeof(TModel);
        protected static readonly AttributeMapModel<TModel, CsvDescriptionAttribute> _AttributeMapModel;
        protected static readonly List<CsvDescriptionAttribute> _CSVDescriptionAttributeList;
        protected static readonly List<CSVSerializeMapModel<TModel>> _CSVSerializeMapModelList;

        static LanymyCsvSerializer()
        {
            _AttributeMapModel = new AttributeMapModel<TModel, CsvDescriptionAttribute>();
            _CSVDescriptionAttributeList = _AttributeMapModel.AttributeList.OrderBy(o => o.Index).ToList();
            foreach (var csvDescriptionAttribute in _CSVDescriptionAttributeList)
            {
                csvDescriptionAttribute.PropertyInfo = _AttributeMapModel.GetPropertyInfoByAttribute(csvDescriptionAttribute);
            }
            if (CsvSerializeMappings.DicCsvSerializeSettings.ContainsKey(CurrentTModelType))
            {
                _CSVSerializeMapModelList = CsvSerializeMappings.DicCsvSerializeSettings[CurrentTModelType] as List<CSVSerializeMapModel<TModel>>;
            }
        }

        /// <summary>
        /// 获取CSV数据的标题
        /// </summary>
        /// <returns></returns>
        public virtual string GetCsvTitle()
        {
            return GlobalSettings.CSV_ANNOTATION_SYMBOL + string.Join(",", _CSVDescriptionAttributeList.Select(o => o.Title).ToArray());
        }

        /// <summary>
        /// 异步 获取CSV数据的标题
        /// </summary>
        /// <returns></returns>
        public virtual Task<string> GetCsvTitleAsync()
        {
            return GenericityFunctions.DoTaskWork(GetCsvTitle);
        }


        /// <summary>
        /// 实体类序列化成CSV
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual string SerializeToCsv(TModel t)
        {

            if (t.IfIsNullOrEmpty())
                throw new ArgumentNullException(nameof(t));

            string[] strArray = new string[_CSVDescriptionAttributeList.Count];

            foreach (var csvDescriptionAttribute in _CSVDescriptionAttributeList)
            {

                //var propertyInfo = _AttributeMapModel.GetPropertyInfoByAttribute(csvDescriptionAttribute);

                var propertyInfo = csvDescriptionAttribute.PropertyInfo;

                object obj = propertyInfo.GetValue(t, null);

                //object obj = _AttributeMapModel.GetPropertyValueByAttribute(csvDescriptionAttribute, t);

                string vaule;

                if (obj.IfIsNullOrEmpty())
                {
                    vaule = "";
                }
                else
                {
                    if (!_CSVSerializeMapModelList.IfIsNullOrEmpty())
                    {
                        var currentMapModel = _CSVSerializeMapModelList.Where(o => o.PropertyName == propertyInfo.Name).FirstOrDefault();
                        if (!currentMapModel.IfIsNullOrEmpty() && !currentMapModel.PropertySerializeFunc.IfIsNullOrEmpty())
                        {
                            vaule = currentMapModel.PropertySerializeFunc(t);
                        }
                        else
                        {
                            vaule = obj.ToString();
                        }
                    }
                    else
                    {
                        vaule = obj.ToString();
                    }
                }

                strArray[csvDescriptionAttribute.Index] = vaule;
            }

            return string.Join(",", strArray);

        }

        /// <summary>
        /// 异步 实体类序列化成CSV
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual Task<string> SerializeToCsvAsync(TModel t)
        {
            return GenericityFunctions.DoTaskWork(SerializeToCsv,t);
        }


        /// <summary>
        /// 反序列化CSV成实体类
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns></returns>
        public virtual TModel DeserializeFromCsv(string csvString)
        {

            if (csvString.IfIsNullOrEmpty())
                throw new ArgumentNullException(nameof(csvString));

            string[] csv = csvString.Split(',');

            TModel model = new TModel();

            for (int i = 0; i < csv.Length; i++)
            {
                var currentCSVDescriptionAttribute = _CSVDescriptionAttributeList[i];
                //var propertyInfo = _AttributeMapModel.GetPropertyInfoByAttribute(currentCSVDescriptionAttribute);
                var propertyInfo = currentCSVDescriptionAttribute.PropertyInfo;

                if (!_CSVSerializeMapModelList.IfIsNullOrEmpty())
                {
                    var currentMapModel = _CSVSerializeMapModelList.Where(o => o.PropertyName == propertyInfo.Name).FirstOrDefault();

                    if (!currentMapModel.IfIsNullOrEmpty() && !currentMapModel.PropertyDeserializeFunc.IfIsNullOrEmpty())
                    {
                        propertyInfo.SetValue(model, currentMapModel.PropertyDeserializeFunc(csv[i]), null);
                    }
                    else
                    {
                        propertyInfo.SetValue(model, csv[i], null);
                    }
                }
                else
                {
                    propertyInfo.SetValue(model, csv[i], null);
                }

            }

            return model;

        }


        /// <summary>
        /// 异步 反序列化CSV成实体类
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns></returns>
        public virtual Task<TModel> DeserializeFromCsvAsync(string csvString)
        {
            return GenericityFunctions.DoTaskWork(DeserializeFromCsv, csvString);
        }

        /// <summary>
        /// 序列化到CSV文件
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="list">要序列化的实体类数据集合</param>
        /// <param name="ifWriteTitle">是否写入 标题 </param>
        public virtual void SerializeToCsvFile(string csvFileFullPath, IEnumerable<TModel> list, bool ifWriteTitle = true)
        {
            using (var writer = new FileReadWriteHelper(csvFileFullPath, true))
            {
                if (ifWriteTitle)
                {
                    writer.WriteLine(SerializeFunctions.GetCsvTitle<TModel>());
                }
                foreach (var item in list)
                {
                    writer.WriteLine(SerializeFunctions.SerializeToCsv(item));
                }
            }
        }
        /// <summary>
        /// 异步 序列化到CSV文件
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="list">要序列化的实体类数据集合</param>
        /// <param name="ifWriteTitle">是否写入 标题 </param>
        public virtual Task SerializeToCsvFileAsync(string csvFileFullPath, IEnumerable<TModel> list, bool ifWriteTitle = true)
        {
            return GenericityFunctions.DoTaskWork(SerializeToCsvFile, csvFileFullPath, list, ifWriteTitle);
        }
        /// <summary>
        /// 从CSV文件反序列化数据
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="csvAnnotationSymbol">行首 注释符 默认 '#'</param>
        /// <returns></returns>
        public virtual List<TModel> DeserializeFromCsvFile(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL)
        {
            List<TModel> list = new List<TModel>();
            foreach (var csvStr in CsvFunctions.ReadCsvFile(csvFileFullPath, csvAnnotationSymbol))
            {
                list.Add(DeserializeFromCsv(csvStr));
            }
            return list;
        }
        /// <summary>
        /// 异步 从CSV文件反序列化数据
        /// </summary>
        /// <param name="csvFileFullPath">CSV文件全路径</param>
        /// <param name="csvAnnotationSymbol">行首 注释符 默认 '#'</param>
        /// <returns></returns>
        public virtual Task<List<TModel>> DeserializeFromCsvFileAsync(string csvFileFullPath, string csvAnnotationSymbol = GlobalSettings.CSV_ANNOTATION_SYMBOL)
        {
            return GenericityFunctions.DoTaskWork(DeserializeFromCsvFile, csvFileFullPath, csvAnnotationSymbol);
        }
    }


}
