using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
        //public JsonSerializerSettings CurrentJsonSerializerSettings { get; private set; }

        /// <summary>
        /// 配置表基类构造方法
        /// </summary>
        /// <param name="configFileFullPath">配置表文件全路径</param>
        /// <param name="configFileType">配置表文件类型</param>
        /// <param name="jsonSerializerSettings">JSON序列化配置信息实体类 默认值 null 使用默认配置信息</param>
        protected BaseConfig(string configFileFullPath, ConfigFileTypeEnum configFileType = ConfigFileTypeEnum.Json, JsonSerializerSettings jsonSerializerSettings = null)
        {

            //// 过滤JSON反序列化操作; JSON反序列化的时候 此两个属性都为 空
            //if (configFileFullPath.IfIsNullOrEmpty() && configFileType == ConfigFileTypeEnum.UnDefine)
            // 过滤JSON反序列化操作; JSON反序列化的时候 此属性都为 空
            if (configFileFullPath.IfIsNullOrEmpty())
            {
                return;
            }

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


        /// <summary>
        /// 从 新配置表实体实例 克隆 可序列化的属性 到 当前实例中
        /// </summary>
        /// <param name="newConfigModel">新配置表实体实例</param>
        public virtual void CloneFromNewConfigModel(BaseConfig newConfigModel)
        {

            var type = this.GetType();

            var properties = type.GetProperties()
                .Where(o => o.CanRead && o.CanWrite)
                .Where(o => o.GetCustomAttribute<JsonIgnoreAttribute>().IfIsNullOrEmpty())
                .ToList();

            foreach (var propertyInfo in properties)
            {
                propertyInfo.SetValue(this, propertyInfo.GetValue(newConfigModel));
            }

        }


        /// <summary>
        /// 从配置表文件中反序列化出属性
        /// </summary>
        public virtual void LoadFromConfigFile()
        {


            try
            {

                var type = this.GetType();

                var json = File.ReadAllText(ConfigFileFullPath);
                var configModel = JsonConvert.DeserializeObject(json, type, CurrentJsonSerializerSettings) as BaseConfig;

                CloneFromNewConfigModel(configModel);

                //propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(t), nullableConverter.UnderlyingType), null);

                //var configModel = SerializeHelper.DeserializeFromJsonFile<TConfigModel>(ConfigFileFullPath);

                //if (configModel.IfIsNullOrEmpty())
                //{
                //    SaveConfigFile();
                //    throw new Exception(string.Format("配置表文件 [ {0} ] , 不存在,以默认初始化新的配置表文件", ConfigFileFullPath));
                //}

                //return configModel;

            }
            catch (Exception e)
            {
                throw new Exception(ConfigFileFullPath + " 配置表挂载失败!", e);
            }

        }


        /// <summary>
        /// 异步 从配置表文件中反序列化出属性
        /// </summary>
        public virtual async Task LoadFromConfigFileAsync()
        {

            await GenericityHelper.DoTaskWorkAsync(LoadFromConfigFile);

        }

        ///// <summary>
        ///// 反序列化配置文件 到 配置信息实体类
        ///// </summary>
        ///// <typeparam name="TConfigModel">配置信息实体类</typeparam>
        ///// <returns></returns>
        //public virtual TConfigModel ReadConfigFile<TConfigModel>() where TConfigModel : BaseConfig
        //{

        //    try
        //    {

        //        var configModel = SerializeHelper.DeserializeFromJsonFile<TConfigModel>(ConfigFileFullPath);

        //        CloneOtherValues(this, configModel);

        //        if (configModel.IfIsNullOrEmpty())
        //        {
        //            SaveConfigFile();
        //            throw new Exception(string.Format("配置表文件 [ {0} ] , 不存在,以默认初始化新的配置表文件", ConfigFileFullPath));
        //        }

        //        return configModel;

        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(ConfigFileFullPath + " 配置表挂载失败!", e);
        //    }

        //}


        /////// <summary>
        /////// 反序列化配置文件 到 配置信息实体类
        /////// </summary>
        /////// <typeparam name="TConfigModel"></typeparam>
        /////// <returns></returns>
        ////public virtual async Task<TConfigModel> ReadConfigFileAsync<TConfigModel>() where TConfigModel : BaseConfig
        ////{
        ////    return await GenericityHelper.DoTaskWorkAsync(ReadConfigFile<TConfigModel>);
        ////}


    }

}
