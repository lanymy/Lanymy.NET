using Lanymy.Common;
using System.Collections.Generic;

namespace Lanymy.Config.Interfaces
{


    /// <summary>
    /// 数据库相关属性接口
    /// </summary>
    public interface IDbConfig
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        DbTypeEnum CurrentDbType { get; set; }



        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        string ConnectionString { get; set; }

    }


}
