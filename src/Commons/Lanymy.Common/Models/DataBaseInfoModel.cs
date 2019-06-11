using Lanymy.Common.Interfaces;

namespace Lanymy.Common.Models
{
    /// <summary>
    /// 数据库 基础信息 实体类
    /// </summary>
    public class DataBaseInfoModel : IAccountProperties
    {


        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbTypeEnum DbType { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataBaseName { get; set; }

        /// <summary>
        /// 数据库服务器地址
        /// </summary>
        public string ServerIp { get; set; }
        /// <summary>
        /// 数据库服务器端口号
        /// </summary>
        public ushort ServerPort { get; set; }


        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }

}
