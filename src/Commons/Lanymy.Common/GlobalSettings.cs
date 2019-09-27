using System.IO;
using System.Text;
using Lanymy.Common.ConstKeys;

namespace Lanymy.Common
{
    /// <summary>
    /// 全局辅助类
    /// </summary>
    public class GlobalSettings
    {




        private static bool _IfCurrentAppDomainIsWeb = false;

        /// <summary>
        /// 当前应用程序域 是否 是 Web.    True:是Web应用程序 ; False:是Client应用程序
        /// </summary>
        public static bool IfCurrentAppDomainIsWeb
        {
            get { return _IfCurrentAppDomainIsWeb; }
            set
            {
                _IfCurrentAppDomainIsWeb = value;
                InitBasePathInfo();
            }
        }



        /// <summary>
        /// 程序域 根目录 全路径
        /// </summary>
        public static readonly string CallDomainBasePath;


        private static string _ConfigFolderFullPath;

        /// <summary>
        /// 配置表  文件夹  全路径
        /// </summary>
        public static string ConfigFolderFullPath => _ConfigFolderFullPath;


        private static string _Log4netConfigFileFullPath;

        /// <summary>
        /// 获取 log4net 配置 文件 全路径
        /// </summary>
        public static string Log4netConfigFileFullPath => _Log4netConfigFileFullPath;


        private static string _NLogConfigFileFullPath;

        /// <summary>
        /// 获取 NLog 配置 文件 全路径
        /// </summary>
        public static string NLogConfigFileFullPath => _NLogConfigFileFullPath;



        //private static string _ClientConfigFileFullPath;

        ///// <summary>
        ///// 客户端 配置表 文件 全路径
        ///// </summary>
        //public static string ClientConfigFileFullPath => _ClientConfigFileFullPath;


        //private static string _LanymyConfigFileFullPath;

        ///// <summary>
        ///// 全局 配置表 文件 全路径
        ///// </summary>
        //public static string LanymyConfigFileFullPath => _LanymyConfigFileFullPath;


        //private static string _LanymyWebAPIConfigFileFullPath;

        ///// <summary>
        ///// webApi配置 文件 全路径
        ///// </summary>
        //public static string LanymyWebApiConfigFileFullPath => _LanymyWebAPIConfigFileFullPath;


        private static string _CacheFolderFullPath;

        /// <summary>
        /// 客户端 缓存文件夹 全路径
        /// </summary>
        public static string CacheFolderFullPath => _CacheFolderFullPath;



        private static string _ImageCacheFolderFullPath;

        /// <summary>
        /// 客户端 缓存文件夹 全路径
        /// </summary>
        public static string ImageCacheFolderFullPath => _ImageCacheFolderFullPath;




        private static string _DriverFolderFullPath;

        /// <summary>
        /// 驱动 根 文件夹 全路径
        /// </summary>
        public static string DriverFolderFullPath => _DriverFolderFullPath;




        static GlobalSettings()
        {


            CallDomainBasePath = PathHelper.GetCallDomainPath();


            InitBasePathInfo();


        }

        /// <summary>
        /// 初始化路径信息
        /// </summary>
        private static void InitBasePathInfo()
        {


            //路径基础信息 初始化
            _ConfigFolderFullPath = _IfCurrentAppDomainIsWeb ? Path.Combine(CallDomainBasePath, DefaultFolderNameKeys.WEB_BIN_FOLDER_NAME, DefaultFolderNameKeys.CONFIG_FOLDER_NAME) : Path.Combine(CallDomainBasePath, DefaultFolderNameKeys.CONFIG_FOLDER_NAME);
            _CacheFolderFullPath = _IfCurrentAppDomainIsWeb ? Path.Combine(CallDomainBasePath, DefaultFolderNameKeys.WEB_BIN_FOLDER_NAME, DefaultFolderNameKeys.CACHE_FOLDER_NAME) : Path.Combine(CallDomainBasePath, DefaultFolderNameKeys.CACHE_FOLDER_NAME);
            _DriverFolderFullPath = _IfCurrentAppDomainIsWeb ? Path.Combine(CallDomainBasePath, DefaultFolderNameKeys.WEB_BIN_FOLDER_NAME, DefaultFolderNameKeys.DRIVER_FOLDER_NAME) : Path.Combine(CallDomainBasePath, DefaultFolderNameKeys.DRIVER_FOLDER_NAME);


            _ImageCacheFolderFullPath = _IfCurrentAppDomainIsWeb ? Path.Combine(_CacheFolderFullPath, DefaultFolderNameKeys.WEB_BIN_FOLDER_NAME, DefaultFolderNameKeys.IMAGE_CACHE_FOLDER_NAME) : Path.Combine(_CacheFolderFullPath, DefaultFolderNameKeys.IMAGE_CACHE_FOLDER_NAME);





            _Log4netConfigFileFullPath = Path.Combine(ConfigFolderFullPath, DefaultSettingKeys.LOG4NET_CONFIG_FILE_FULL_NAME);
            _NLogConfigFileFullPath = Path.Combine(ConfigFolderFullPath, DefaultSettingKeys.NLOG_CONFIG_FILE_FULL_NAME);


            //_ClientConfigFileFullPath = Path.Combine(ConfigFolderFullPath, DefaultLanymyNetSettingKeys.CLIENT_CONFIG_FILE_FULL_NAME);

            //_LanymyConfigFileFullPath = Path.Combine(ConfigFolderFullPath, DefaultLanymyNetSettingKeys.LANYMY_CONFIG_FILE_FULL_NAME);
            //_LanymyWebAPIConfigFileFullPath = Path.Combine(ConfigFolderFullPath, DefaultLanymyNetSettingKeys.LANYMY_WEBAPI_CONFIG_FILE_FULL_NAME);




        }




    }
}
