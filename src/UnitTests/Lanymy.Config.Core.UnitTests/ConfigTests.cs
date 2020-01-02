using System.IO;
using System.Threading.Tasks;
using Lanymy.Common;
using Lanymy.Config.ConfigModels;
using Lanymy.Config.Interfaces;
using Lanymy.Config.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Lanymy.Config.Core.UnitTests
{


    [TestClass]
    public class ConfigTests
    {

        public class MyDbConfigClass
        {
            public DataBaseConfig SqlDataBaseConfig { get; set; } = new DataBaseConfig();

            public DataBaseConfig LogMySqlDataBaseConfig { get; set; } = new DataBaseConfig();
        }

        public class MyConfigClass : BaseConfig, IDebugConfig
        {

            public bool IsDebugMode { get; set; } = false;

            public MyDbConfigClass MyDbConfig { get; set; } = new MyDbConfigClass();


            [JsonIgnore]
            public string TestJsonIgnore { get; set; }


            public MyConfigClass(string configFileFullPath) : base(configFileFullPath)
            {

            }

            protected override void SetDefaultValuesOnSaveConfigFile()
            {

            }





        }


        [TestMethod]
        public async Task ConfigTest()
        {

            var rootFullPath = PathHelper.GetCallDomainPath();
            var configFileFullName = string.Format("{0}.config", nameof(MyConfigClass));
            var configFileFullPath = Path.Combine(rootFullPath, "Configs", configFileFullName);

            PathHelper.InitDirectoryPath(configFileFullPath);

            //手动创建默认配置表
            //var myConfigClass = new MyConfigClass(configFileFullPath);
            ////await myConfigClass.SaveConfigFileAsync();

            //如果指定路径没有配置表 会自动创建 默认配置表
            var myConfigClass = new MyConfigClass(configFileFullPath);
            //myConfigClass.ReadConfigFile<MyConfigClass>();
            //var myConfigClassFromConfigFile = await myConfigClass.ReadConfigFileAsync<MyConfigClass>();

            myConfigClass.LoadFromConfigFile();

            myConfigClass.IsDebugMode = false;

            await myConfigClass.LoadFromConfigFileAsync();


        }

    }

}
