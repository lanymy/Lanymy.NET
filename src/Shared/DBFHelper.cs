/********************************************************************

时间: 2015年10月19日, AM 08:57:01

作者: lanyanmiyu@qq.com

描述: Microsoft Visual FoxPro（Dbf）数据库 辅助类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.IO;

namespace Lanymy.General.Extension
{


    ///// <summary>
    ///// DBF数据库文件的连接字符串模式枚举
    ///// </summary>
    //public enum DBFConnectModeTypeEnum
    //{
    //    /// <summary>
    //    /// 有界面程序 模式 连接字符串
    //    /// </summary>
    //    Application,
    //    /// <summary>
    //    /// 无界面 模式  连接字符串
    //    /// </summary>
    //    Machine,
    //    /// <summary>
    //    /// Windows服务模式连接字符串
    //    /// </summary>
    //    WindowsService,
    //}


    /// <summary>
    /// <para>注意: DBF 驱动 只能在  x86  的程序上成功 挂载 x64 位报错 未在本地计算机上注册“vfpoledb”提供程序</para>   
    /// Microsoft Visual FoxPro（Dbf）数据库 辅助类
    /// </summary>
    public sealed class DBFHelper
    {


        #region 数据库连接字符串

        /// <summary>
        /// DBF数据库 连接 格式化  字符串
        /// </summary>
        private static string strConnFormat = @"Provider=vfpoledb;Data Source={0};Collating Sequence=machine;";

        //private const string strConnFormat = @"Provider=vfpoledb;Data Source={0};Collating Sequence=machine;";
        //string connStr = @"Driver={Microsoft Visual FoxPro Driver};Provider=VFPOLEDB;Data Source=" + mulu + ";Collating Sequence=machine;";
        //string connStr = @"Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;SourceDB=" + mulu + ";Exclusive=No;NULL=NO;Collate=Machine;BACKGROUNDFETCH=NO;DELETED=NO";
        //string strConn = @"Provider=vfpoledb;Data Source=" + DatasourceDirectory + ";Collating Sequence=machine;";



        //private const string strConnFormatApplication = @"Provider=vfpoledb;Data Source={0};Collating Sequence=machine;";

        //private const string strConnFormatMachine = @"Driver={{Microsoft Visual FoxPro Driver}};Provider=VFPOLEDB;Data Source={0};Collating Sequence=machine;";

        //private const string strConnFormatWindowsService = @"Driver={{Microsoft Visual FoxPro Driver}};SourceType=DBF;Provider=VFPOLEDB;SourceDB={0};Exclusive=No;NULL=NO;Collate=Machine;BACKGROUNDFETCH=NO;DELETED=NO";

        //private static DBFConnectModeTypeEnum _DBFConnectModeType;

        ///// <summary>
        ///// DBF数据库文件的连接字符串模式  应用场景不同 连接字符串不同 如果 提示 未在本地计算机上注册“vfpoledb”提供程序  尝试切换 连接模式
        ///// </summary>
        //public static DBFConnectModeTypeEnum DBFConnectModeType
        //{
        //    get { return _DBFConnectModeType; }
        //    set
        //    {
        //        _DBFConnectModeType = value;
        //        switch (_DBFConnectModeType)
        //        {
        //            case DBFConnectModeTypeEnum.Application:
        //                strConnFormat = strConnFormatApplication;
        //                break;
        //            case DBFConnectModeTypeEnum.Machine:
        //                strConnFormat = strConnFormatMachine;
        //                break;
        //            case DBFConnectModeTypeEnum.WindowsService:
        //                strConnFormat = strConnFormatWindowsService;
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}


        //public static readonly string DBConnectionString = string.Empty; //System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();

        //public static readonly string DBConnectionString = "Server=localhost;DataBase=menagerie;Uid=de.cel;Pwd=de.cel";   //在OleDb中localhost好像不能用"."代替，我试了一下，会出错。

        #endregion

        #region PrepareCommand

        /// <summary>
        /// Command预处理
        /// </summary>
        /// <param name="conn">OleDbConnection对象</param>
        /// <param name="trans">OleDbTransaction对象，可为null</param>
        /// <param name="cmd">OleDbCommand对象</param>
        /// <param name="cmdType">CommandType，存储过程或命令行</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">OleDbCommand参数数组，可为null</param>
        private static void PrepareCommand(OleDbConnection conn, OleDbTransaction trans, OleDbCommand cmd, CommandType cmdType, string cmdText, OleDbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)

                conn.Open();

            cmd.Connection = conn;

            cmd.CommandText = cmdText;

            if (trans != null)

                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {

                foreach (OleDbParameter parm in cmdParms)

                    cmd.Parameters.Add(parm);

            }

        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="datasourceDirectory">DBF数据库 目录 路径</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">OleDbCommand参数数组</param>
        /// <returns>返回受引响的记录行数</returns>
        public static int ExecuteNonQuery(string datasourceDirectory, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {

            if (!Directory.Exists(datasourceDirectory))
            {
                throw new DirectoryNotFoundException(datasourceDirectory);
            }

            string connectionString = string.Format(strConnFormat, datasourceDirectory);

            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {

                PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);

                int val = cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();

                return val;

            }

        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="conn">Connection对象</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">OleDbCommand参数数组</param>
        /// <returns>返回受引响的记录行数</returns>
        public static int ExecuteNonQuery(OleDbConnection conn, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {

            OleDbCommand cmd = new OleDbCommand();

            PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);

            int val = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();

            return val;

        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="trans">OleDbTransaction对象</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">OleDbCommand参数数组</param>
        /// <returns>返回受引响的记录行数</returns>
        public static int ExecuteNonQuery(OleDbTransaction trans, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {

            OleDbCommand cmd = new OleDbCommand();

            PrepareCommand(trans.Connection, trans, cmd, cmdType, cmdText, cmdParms);

            int val = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();

            return val;

        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 执行命令，返回第一行第一列的值
        /// </summary>
        /// <param name="datasourceDirectory">数据库根目录</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">OleDbCommand参数数组</param>
        /// <returns>返回Object对象</returns>
        public static object ExecuteScalar(string datasourceDirectory, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {

            if (!Directory.Exists(datasourceDirectory))
            {
                throw new DirectoryNotFoundException(datasourceDirectory);
            }

            string connectionString = string.Format(strConnFormat, datasourceDirectory);



            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {

                PrepareCommand(connection, null, cmd, cmdType, cmdText, cmdParms);

                object val = cmd.ExecuteScalar();

                cmd.Parameters.Clear();

                return val;

            }

        }

        /// <summary>
        /// 执行命令，返回第一行第一列的值
        /// </summary>
        /// <param name="conn">数据源连接对象</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">OleDbCommand参数数组</param>
        /// <returns>返回Object对象</returns>
        public static object ExecuteScalar(OleDbConnection conn, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {

            OleDbCommand cmd = new OleDbCommand();

            PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);

            object val = cmd.ExecuteScalar();

            cmd.Parameters.Clear();

            return val;

        }

        #endregion

        #region ExecuteReader

        /// <summary>
        /// 执行命令或存储过程，返回OleDbDataReader对象
        /// 注意OleDbDataReader对象使用完后必须Close以释放OleDbConnection资源
        /// </summary>
        /// <param name="datasourceDirectory">数据库根目录</param>
        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">OleDbCommand参数数组</param>
        /// <returns></returns>
        public static OleDbDataReader ExecuteReader(string datasourceDirectory, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {

            if (!Directory.Exists(datasourceDirectory))
            {
                throw new DirectoryNotFoundException(datasourceDirectory);
            }

            string connectionString = string.Format(strConnFormat, datasourceDirectory);



            using (OleDbCommand cmd = new OleDbCommand())
            {
                OleDbConnection conn = new OleDbConnection(connectionString);

                try
                {

                    PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);

                    OleDbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    cmd.Parameters.Clear();

                    return dr;

                }
                catch
                {

                    conn.Close();

                    throw;

                }
            }

        }

        #endregion

        #region ExecuteDataSet

        /// <summary>
        /// 执行命令或存储过程，返回DataSet对象
        /// </summary>
        /// <param name="datasourceDirectory">数据库根目录</param>
        /// <param name="cmdType">命令类型(存储过程或SQL语句)</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdParms">OleDbCommand参数数组(可为null值)</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string datasourceDirectory, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {

            if (!Directory.Exists(datasourceDirectory))
            {
                throw new DirectoryNotFoundException(datasourceDirectory);
            }

            string connectionString = string.Format(strConnFormat, datasourceDirectory);



            using (OleDbCommand cmd = new OleDbCommand())
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {

                    PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);

                    DataSet ds = new DataSet();

                    da.Fill(ds);

                    conn.Close();

                    cmd.Parameters.Clear();

                    return ds;

                }
            }



        }

        #endregion
    }


}
