using Lanymy.Common;
using Lanymy.Config.Interfaces;

namespace Lanymy.Config.ConfigModels
{
    /// <summary>
    /// 数据库配置信息
    /// </summary>
    public class DataBaseConfig : IDbConfig
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbTypeEnum CurrentDbType { get; set; } = DbTypeEnum.UnDefine;

        ///// <summary>
        ///// 数据库标记名称
        ///// </summary>
        //public string DataBaseKeyTag { get; set; }

        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public string ConnectionString { get; set; } = "Data Source=服务器IP;Initial Catalog=数据库名称;User=用户名;Password=密码";


    }
}
