///********************************************************************

//时间: 2015年10月09日, AM 09:22:22

//作者: lanyanmiyu@qq.com

//描述: MySql数据访问帮助类

//其它:     

//********************************************************************/






//using System.Data;
//using MySql.Data.MySqlClient;

//namespace Lanymy.General.Extension
//{


//    /// <summary>
//    /// MySql数据访问帮助类
//    /// </summary>
//    public sealed class MySqlHelper
//    {
//        #region 数据库连接字符串

//        public static readonly string DBConnectionString = string.Empty; //System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();

//        //public static readonly string DBConnectionString = "Server=localhost;DataBase=menagerie;Uid=de.cel;Pwd=de.cel";   //在MySql中localhost好像不能用"."代替，我试了一下，会出错。
//        #endregion

//        #region PrepareCommand

//        /// <summary>
//        /// Command预处理
//        /// </summary>
//        /// <param name="conn">MySqlConnection对象</param>
//        /// <param name="trans">MySqlTransaction对象，可为null</param>
//        /// <param name="cmd">MySqlCommand对象</param>
//        /// <param name="cmdType">CommandType，存储过程或命令行</param>
//        /// <param name="cmdText">SQL语句或存储过程名</param>
//        /// <param name="cmdParms">MySqlCommand参数数组，可为null</param>
//        private static void PrepareCommand(MySqlConnection conn, MySqlTransaction trans, MySqlCommand cmd, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
//        {
//            if (conn.State != ConnectionState.Open)

//                conn.Open();

//            cmd.Connection = conn;

//            cmd.CommandText = cmdText;

//            if (trans != null)

//                cmd.Transaction = trans;

//            cmd.CommandType = cmdType;

//            if (cmdParms != null)
//            {

//                foreach (MySqlParameter parm in cmdParms)

//                    cmd.Parameters.Add(parm);

//            }

//        }

//        #endregion

//        #region ExecuteNonQuery

//        /// <summary>
//        /// 执行命令
//        /// </summary>
//        /// <param name="connectionString">数据库连接字符串</param>
//        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
//        /// <param name="cmdText">SQL语句或存储过程名</param>
//        /// <param name="cmdParms">MySqlCommand参数数组</param>
//        /// <returns>返回受引响的记录行数</returns>

//        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
//        {


//            using (MySqlConnection conn = new MySqlConnection(connectionString))
//            {

//                MySqlCommand cmd = new MySqlCommand();
//                PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);

//                int val = cmd.ExecuteNonQuery();

//                cmd.Parameters.Clear();

//                return val;

//            }

//        }

//        /// <summary>
//        /// 执行命令
//        /// </summary>
//        /// <param name="conn">Connection对象</param>
//        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
//        /// <param name="cmdText">SQL语句或存储过程名</param>
//        /// <param name="cmdParms">MySqlCommand参数数组</param>
//        /// <returns>返回受引响的记录行数</returns>

//        public static int ExecuteNonQuery(MySqlConnection conn, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
//        {

//            MySqlCommand cmd = new MySqlCommand();

//            PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);

//            int val = cmd.ExecuteNonQuery();

//            cmd.Parameters.Clear();

//            return val;

//        }

//        /// <summary>
//        /// 执行事务
//        /// </summary>
//        /// <param name="trans">MySqlTransaction对象</param>
//        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
//        /// <param name="cmdText">SQL语句或存储过程名</param>
//        /// <param name="cmdParms">MySqlCommand参数数组</param>
//        /// <returns>返回受引响的记录行数</returns>

//        public static int ExecuteNonQuery(MySqlTransaction trans, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
//        {

//            MySqlCommand cmd = new MySqlCommand();

//            PrepareCommand(trans.Connection, trans, cmd, cmdType, cmdText, cmdParms);

//            int val = cmd.ExecuteNonQuery();

//            cmd.Parameters.Clear();

//            return val;

//        }

//        #endregion

//        #region ExecuteScalar

//        /// <summary>
//        /// 执行命令，返回第一行第一列的值
//        /// </summary>
//        /// <param name="connectionString">数据库连接字符串</param>
//        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
//        /// <param name="cmdText">SQL语句或存储过程名</param>
//        /// <param name="cmdParms">MySqlCommand参数数组</param>
//        /// <returns>返回Object对象</returns>

//        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
//        {


//            using (MySqlConnection connection = new MySqlConnection(connectionString))
//            {

//                MySqlCommand cmd = new MySqlCommand();

//                PrepareCommand(connection, null, cmd, cmdType, cmdText, cmdParms);

//                object val = cmd.ExecuteScalar();

//                cmd.Parameters.Clear();

//                return val;

//            }

//        }

//        /// <summary>

//        /// 执行命令，返回第一行第一列的值

//        /// </summary>

//        /// <param name="connectionString">数据库连接字符串</param>

//        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>

//        /// <param name="cmdText">SQL语句或存储过程名</param>

//        /// <param name="cmdParms">MySqlCommand参数数组</param>

//        /// <returns>返回Object对象</returns>

//        public static object ExecuteScalar(MySqlConnection conn, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
//        {

//            MySqlCommand cmd = new MySqlCommand();

//            PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);

//            object val = cmd.ExecuteScalar();

//            cmd.Parameters.Clear();

//            return val;

//        }

//        #endregion

//        #region ExecuteReader

//        /// <summary>
//        /// 执行命令或存储过程，返回MySqlDataReader对象
//        /// 注意MySqlDataReader对象使用完后必须Close以释放MySqlConnection资源
//        /// </summary>
//        /// <param name="connectionString">数据库连接字符串</param>
//        /// <param name="cmdType">命令类型（存储过程或SQL语句）</param>
//        /// <param name="cmdText">SQL语句或存储过程名</param>
//        /// <param name="cmdParms">MySqlCommand参数数组</param>
//        /// <returns></returns>

//        public static MySqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
//        {


//            using (MySqlConnection conn = new MySqlConnection(connectionString))
//            {
//                MySqlCommand cmd = new MySqlCommand();

//                PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);

//                MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

//                cmd.Parameters.Clear();

//                return dr;
//            }


//        }

//        #endregion

//        #region ExecuteDataSet

//        /// <summary>
//        /// 执行命令或存储过程，返回DataSet对象
//        /// </summary>
//        /// <param name="connectionString">数据库连接字符串</param>
//        /// <param name="cmdType">命令类型(存储过程或SQL语句)</param>
//        /// <param name="cmdText">SQL语句或存储过程名</param>
//        /// <param name="cmdParms">MySqlCommand参数数组(可为null值)</param>


//        public static DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
//        {


//            using (MySqlConnection conn = new MySqlConnection(connectionString))
//            {

//                MySqlCommand cmd = new MySqlCommand();

//                PrepareCommand(conn, null, cmd, cmdType, cmdText, cmdParms);

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

//                DataSet ds = new DataSet();

//                da.Fill(ds);

//                conn.Close();

//                cmd.Parameters.Clear();

//                return ds;

//            }

//        }

//        #endregion


//        /// <summary>
//        /// 批量同步DataTable数据 增删改
//        /// </summary>
//        /// <param name="connectionString">数据库连接字符串</param>
//        /// <param name="dt">数据</param>
//        /// <param name="selectCommandText">查询sql语句</param>
//        /// <param name="cmdParms">查询参数</param>
//        /// <returns></returns>
//        public static int UpdateDataTable(string connectionString, DataTable dt, string selectCommandText, params MySqlParameter[] cmdParms)
//        {
//            using (MySqlConnection conn = new MySqlConnection(connectionString))
//            {
//                using (MySqlCommand cmd = new MySqlCommand())
//                {

//                    PrepareCommand(conn, null, cmd, CommandType.Text, selectCommandText, cmdParms);

//                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
//                    {
//                        using (MySqlCommandBuilder sqlCmdBuilder = new MySqlCommandBuilder(da))
//                        {
//                            sqlCmdBuilder.GetDeleteCommand();
//                            sqlCmdBuilder.GetInsertCommand();
//                            sqlCmdBuilder.GetUpdateCommand();
//                            return da.Update(dt);
//                        }

//                    }

//                }

//            }
//        }





//    }//end class



//}
