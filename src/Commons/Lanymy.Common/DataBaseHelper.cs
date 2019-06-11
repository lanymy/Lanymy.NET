using System;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Models;

namespace Lanymy.Common
{
    /// <summary>
    /// 数据库 辅助类 
    /// </summary>
    public class DataBaseHelper
    {
        /// <summary>
        /// 获取 数据库 基础信息 实例
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="dataBaseServerIp">数据库IP(如果 有端口信息 则带到 IP 信息中 , 如 : SqlServer 的 127.0.0.1,1433 , MySql 的 127.0.0.1:3306)</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="dataBaseName">数据库名称</param>
        /// <returns></returns>
        public static DataBaseInfoModel GetDataBaseInfoModel(DbTypeEnum dbType, string dataBaseServerIp = null, string userName = null, string password = null, string dataBaseName = null)
        {

            var dataBaseInfoModel = new DataBaseInfoModel
            {
                DbType = dbType
            };



            if (!dataBaseServerIp.IfIsNullOrEmpty())
            {

                string ip = string.Empty;
                string port = string.Empty;
                ushort defaultPort = 0;
                string portSeparator = string.Empty;

                if (dbType == DbTypeEnum.SqlServer)
                {
                    portSeparator = ",";
                    defaultPort = 1433;
                }

                else if (dbType == DbTypeEnum.MySql)
                {
                    portSeparator = ":";
                    defaultPort = 3306;
                }

                if (dataBaseServerIp.Contains(portSeparator))
                {

                    ip = dataBaseServerIp.LeftSubString(portSeparator);
                    port = dataBaseServerIp.LeftRemoveString(portSeparator);

                }
                else
                {
                    ip = dataBaseServerIp;
                }

                dataBaseInfoModel.ServerIp = ip;
                dataBaseInfoModel.ServerPort = port.ConvertToType<ushort>(defaultPort);

            }

            if (!userName.IfIsNullOrEmpty())
            {
                dataBaseInfoModel.UserName = userName;
            }

            if (!password.IfIsNullOrEmpty())
            {
                dataBaseInfoModel.Password = password;
            }

            if (!dataBaseName.IfIsNullOrEmpty())
            {
                dataBaseInfoModel.DataBaseName = dataBaseName;
            }

            return dataBaseInfoModel;

        }



        /// <summary>
        /// 根据 数据库 基础信息 实体类  获取 数据库 链接字符串
        /// </summary>
        /// <param name="dataBaseInfoModel"></param>
        /// <returns></returns>
        public static string GetDataBaseConnectionString(DataBaseInfoModel dataBaseInfoModel)
        {

            string dataBaseConnectionString = string.Empty;

            if (!dataBaseInfoModel.IfIsNullOrEmpty())
            {

                if (dataBaseInfoModel.DbType == DbTypeEnum.SqlServer)
                {
                    dataBaseConnectionString = string.Format("Data Source={0};User ID={1};pwd={2};Initial Catalog={3};Persist Security Info=True;"
                                                                , string.Format("{0},{1}", dataBaseInfoModel.ServerIp, dataBaseInfoModel.ServerPort)
                                                                , dataBaseInfoModel.UserName
                                                                , dataBaseInfoModel.Password
                                                                , dataBaseInfoModel.DataBaseName
                                                              );
                }
                else if (dataBaseInfoModel.DbType == DbTypeEnum.MySql)
                {
                    //dataBaseConnectionString = string.Format("server={0};port={1};user id={1};password={2};database={3};charset=utf8;persistsecurityinfo=True;");
                    dataBaseConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};charset=utf8;persistsecurityinfo=True;"
                                                                , dataBaseInfoModel.ServerIp
                                                                , dataBaseInfoModel.ServerPort
                                                                , dataBaseInfoModel.UserName
                                                                , dataBaseInfoModel.Password
                                                                , dataBaseInfoModel.DataBaseName
                                                              );
                }

            }

            return dataBaseConnectionString;

        }

        /// <summary>
        /// 根据连接字符串获取数据库ip、端口、用户名、密码
        /// 如果没有配置默认返回为空
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static DataBaseInfoModel GetDataBaseInfoModelFromConnctiongString(DbTypeEnum dbType, string connectionString)
        {
            var dataBaseInfoModel = new DataBaseInfoModel
            {
                DbType = dbType
            };

            if (!connectionString.IfIsNullOrEmpty())
            {
                string ip = string.Empty;
                ushort port = 0;
                string userName = string.Empty;
                string pwd = string.Empty;
                string[] strs = connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (strs.IfIsNullOrEmpty())
                {
                    return dataBaseInfoModel;
                }
                if (dbType == DbTypeEnum.MySql)
                {
                    foreach (var item in strs)
                    {
                        if (item.Contains("server="))
                        {
                            ip = item.Replace("server=", "");
                        }
                        if (item.Contains("port="))
                        {
                            ushort.TryParse(item.Replace("port=", ""), out port);
                        }
                        if (item.Contains("uid="))
                        {
                            userName = item.Replace("uid=", "");
                        }
                        if (item.Contains("user id="))
                        {
                            userName = item.Replace("user id=", "");
                        }
                        if (item.Contains("pwd="))
                        {
                            pwd = item.Replace("pwd=", "");
                        }
                        if (item.Contains("password="))
                        {
                            pwd = item.Replace("password=", "");
                        }
                    }
                    if (port == 0)
                    {
                        port = 3306;
                    }
                }
                else if (dbType == DbTypeEnum.SqlServer)
                {
                    foreach (var item in strs)
                    {
                        if (item.Contains("Data Source="))
                        {
                            ip = item.Replace("Data Source=", "");
                        }
                        if (item.Contains("User ID="))
                        {
                            userName = item.Replace("User ID=", "");
                        }
                        if (item.Contains("pwd="))
                        {
                            pwd = item.Replace("pwd=", "");
                        }
                    }
                    string portSeparator = ",";
                    if (ip.Contains(portSeparator))
                    {
                        ip = ip.LeftSubString(portSeparator);
                        ushort.TryParse(ip.LeftRemoveString(portSeparator), out port);
                    }
                    if (port == 0)
                    {
                        port = 1433;
                    }
                }

                if (ip == "服务器地址")
                {
                    ip = "";
                }
                if (userName == "用户名")
                {
                    userName = "";
                }
                if (pwd == "密码")
                {
                    pwd = "";
                }

                dataBaseInfoModel.ServerIp = ip;
                dataBaseInfoModel.ServerPort = port;
                dataBaseInfoModel.UserName = userName;
                dataBaseInfoModel.Password = pwd;
            }

            return dataBaseInfoModel;
        }
    }
}
