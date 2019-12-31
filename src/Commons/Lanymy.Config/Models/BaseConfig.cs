using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Lanymy.Common;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Serializer;

namespace Lanymy.Config.Models
{

    /// <summary>
    /// Config 基类
    /// </summary>
    public abstract class BaseConfig
    {


        /// <summary>
        /// 配置表文件全路径
        /// </summary>
        [JsonIgnore]
        public string ConfigFileFullPath { get; }

        [JsonIgnore]
        public ConfigFileTypeEnum ConfigFileType { get; }

        /// <summary>
        /// 序列化Json使用的格式化配置属性
        /// </summary>
        [JsonIgnore]
        public readonly JsonSerializerSettings CurrentJsonSerializerSettings;

        /// <summary>
        /// 配置表基类构造方法
        /// </summary>
        /// <param name="configFileFullPath">配置表文件全路径</param>
        /// <param name="configFileType">配置表文件类型</param>
        /// <param name="jsonSerializerSettings">JSON序列化配置信息实体类 默认值 null 使用默认配置信息</param>
        private BaseConfig(string configFileFullPath, ConfigFileTypeEnum configFileType, JsonSerializerSettings jsonSerializerSettings = null)
        {

            ConfigFileFullPath = configFileFullPath;
            ConfigFileType = configFileType;


            if (!jsonSerializerSettings.IfIsNullOrEmpty())
            {
                CurrentJsonSerializerSettings = jsonSerializerSettings;
            }
            else
            {

                var currentJsonSerializerSettings = JsonNetJsonSerializer.GetDefaultJsonSerializerSettings();
                currentJsonSerializerSettings.Formatting = Formatting.Indented;
                currentJsonSerializerSettings.Converters.Add(new StringEnumConverter());

                CurrentJsonSerializerSettings = currentJsonSerializerSettings;

            }


            //如果配置表文件 不存在 则直接生成一个 默认信息的 配置表文件
            if (!File.Exists(ConfigFileFullPath))
            {
                SaveConfigFile();
            }

        }




        /// <summary>
        /// JSON 配置表 反序列化的时候 List 类型 如果有默认值  直接就会被 默认值 覆盖掉,配置表上的自定义数据会失效
        /// 通过此方法 进行 默认值 重新赋值
        /// </summary>
        protected abstract void SetDefaultValuesOnSaveConfigFile();



        /// <summary>
        /// 保存配置表文件
        /// </summary>
        public virtual void SaveConfigFile()
        {

            SetDefaultValuesOnSaveConfigFile();

            var jsonNetJsonSerializer = new JsonNetJsonSerializer(CurrentJsonSerializerSettings);
            //var currentJsonSerializerSettings = jsonNetJsonSerializer.CurrentJsonSerializerSettings;
            //currentJsonSerializerSettings.Formatting = Formatting.Indented;
            //currentJsonSerializerSettings.Converters.Add(new StringEnumConverter());


            if (ConfigFileType == ConfigFileTypeEnum.Json)
            {
                SerializeHelper.SerializeToJsonFile(this, ConfigFileFullPath, null, jsonNetJsonSerializer);
            }
            //else if (ConfigFileType == ConfigFileTypeEnum.Xml)
            //{
            //    SerializeHelper.SerializeToXmlFile(this, ConfigFileFullPath);
            //}
            else
            {
                throw new NotSupportedException();
            }

        }


        /// <summary>
        /// 保存配置表文件
        /// </summary>
        /// <returns></returns>
        public virtual async Task SaveConfigFileAsync()
        {
            await GenericityHelper.DoTaskWorkAsync(SaveConfigFile);
        }



    }
}
