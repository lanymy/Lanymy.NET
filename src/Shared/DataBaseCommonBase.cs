///********************************************************************

//时间: 2015年10月19日, AM 09:34:52

//作者: lanyanmiyu@qq.com

//描述: 数据库 辅助类 通用基类

//其它:     

//********************************************************************/



//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;

//namespace Lanymy.General.Extension
//{


//    /// <summary>
//    /// 数据库 辅助类 通用基类
//    /// </summary>
//    /// <typeparam name="TConnection">Connection数据类型</typeparam>
//    /// <typeparam name="TTransaction">Transaction数据类型</typeparam>
//    /// <typeparam name="TCommand">Command数据类型</typeparam>
//    /// <typeparam name="TParameter">Parameter数据类型</typeparam>
//    /// <typeparam name="TDataAdapter">DataAdapter数据类型</typeparam>
//    /// <typeparam name="TDataReader">DataReader数据类型</typeparam>
//    internal abstract class DataBaseCommonBase<TConnection, TTransaction, TCommand, TParameter, TDataAdapter, TDataReader, TCommandBuilder> : DataBaseCommonParameterCache<TConnection, TTransaction, TCommand, TParameter, TDataAdapter, TDataReader, TCommandBuilder>
//        where TConnection : DbConnection, new()
//        where TTransaction : DbTransaction
//        where TCommand : DbCommand, new()
//        where TParameter : DbParameter
//        where TDataAdapter : DbDataAdapter, new()
//        where TDataReader : DbDataReader, new()
//        where TCommandBuilder : DbCommandBuilder, new()
//    {

//        protected DataBaseCommonBase()
//        {

//        }



//        ///// <summary>
//        ///// 获取连接对象实例
//        ///// </summary>
//        ///// <param name="connectionString"></param>
//        ///// <returns></returns>
//        //protected abstract TConnection GetCreateInstance(string connectionString);


//        /// <summary> 
//        /// 将TParameter参数数组(参数值)分配给TCommand命令. 
//        /// 这个方法将给任何一个参数分配DBNull.Value; 
//        /// 该操作将阻止默认值的使用. 
//        /// </summary> 
//        /// <param name="command">命令名</param> 
//        /// <param name="commandParameters">TParameters数组</param> 
//        protected void AttachParameters(TCommand command, TParameter[] commandParameters)
//        {
//            if (command == null) throw new ArgumentNullException("command");
//            if (commandParameters != null)
//            {
//                foreach (var p in commandParameters)
//                {
//                    if (p != null)
//                    {
//                        // 检查未分配值的输出参数,将其分配以DBNull.Value. 
//                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) &&
//                            (p.Value == null))
//                        {
//                            p.Value = DBNull.Value;
//                        }
//                        command.Parameters.Add(p);
//                    }
//                }
//            }
//        }


//        /// <summary> 
//        /// 将DataRow类型的列值分配到TParameter参数数组. 
//        /// </summary> 
//        /// <param name="commandParameters">要分配值的TParameter参数数组</param> 
//        /// <param name="dataRow">将要分配给存储过程参数的DataRow</param> 
//        protected void AssignParameterValues(TParameter[] commandParameters, DataRow dataRow)
//        {
//            if ((commandParameters == null) || (dataRow == null))
//            {
//                return;
//            }

//            int i = 0;
//            // 设置参数值 
//            foreach (var commandParameter in commandParameters)
//            {
//                // 创建参数名称,如果不存在,只抛出一个异常. 
//                if (commandParameter.ParameterName == null ||
//                    commandParameter.ParameterName.Length <= 1)
//                    throw new Exception(
//                        string.Format("请提供参数{0}一个有效的名称{1}.", i, commandParameter.ParameterName));
//                // 从dataRow的表中获取为参数数组中数组名称的列的索引. 
//                // 如果存在和参数名称相同的列,则将列值赋给当前名称的参数. 
//                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
//                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
//                i++;
//            }
//        }


//        /// <summary> 
//        /// 将一个对象数组分配给TParameter参数数组. 
//        /// </summary> 
//        /// <param name="commandParameters">要分配值的TParameter参数数组</param> 
//        /// <param name="parameterValues">将要分配给存储过程参数的对象数组</param> 
//        protected void AssignParameterValues(TParameter[] commandParameters, object[] parameterValues)
//        {
//            if ((commandParameters == null) || (parameterValues == null))
//            {
//                return;
//            }

//            // 确保对象数组个数与参数个数匹配,如果不匹配,抛出一个异常. 
//            if (commandParameters.Length != parameterValues.Length)
//            {
//                throw new ArgumentException("参数值个数与参数不匹配.");
//            }

//            // 给参数赋值 
//            for (int i = 0, j = commandParameters.Length; i < j; i++)
//            {
//                // If the current array value derives from IDbDataParameter, then assign its Value property 
//                if (parameterValues[i] is IDbDataParameter)
//                {
//                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
//                    if (paramInstance.Value == null)
//                    {
//                        commandParameters[i].Value = DBNull.Value;
//                    }
//                    else
//                    {
//                        commandParameters[i].Value = paramInstance.Value;
//                    }
//                }
//                else if (parameterValues[i] == null)
//                {
//                    commandParameters[i].Value = DBNull.Value;
//                }
//                else
//                {
//                    commandParameters[i].Value = parameterValues[i];
//                }
//            }
//        }


//        //protected void PrepareCommand(TCommand command, TTransaction transaction, CommandType commandType, string commandText, TParameter[] commandParameters, out bool mustCloseConnection)
//        //{
//        //    PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
//        //}

//        /// <summary> 
//        /// 预处理用户提供的命令,数据库连接/事务/命令类型/参数 
//        /// </summary> 
//        /// <param name="command">要处理的TCommand</param> 
//        /// <param name="connection">数据库连接</param> 
//        /// <param name="transaction">一个有效的事务或者是null值</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
//        /// <param name="commandText">存储过程名或都T-SQL命令文本</param> 
//        /// <param name="commandParameters">和命令相关联的TParameter参数数组,如果没有参数为'null'</param> 
//        /// <param name="mustCloseConnection"><c>true</c> 如果连接是打开的,则为true,其它情况下为false.</param> 
//        protected void PrepareCommand(TCommand command, DbConnection connection, TTransaction transaction, CommandType commandType, string commandText, TParameter[] commandParameters, out bool mustCloseConnection)
//        {

//            if (command == null) throw new ArgumentNullException("command");
//            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

//            // If the provided connection is not open, we will open it 
//            if (connection.State != ConnectionState.Open)
//            {
//                mustCloseConnection = true;
//                connection.Open();
//            }
//            else
//            {
//                mustCloseConnection = false;
//            }

//            // 给命令分配一个数据库连接. 
//            command.Connection = connection;

//            // 设置命令文本(存储过程名或SQL语句) 
//            command.CommandText = commandText;

//            // 分配事务 
//            if (transaction != null)
//            {
//                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//                command.Transaction = transaction;
//            }

//            // 设置命令类型. 
//            command.CommandType = commandType;

//            // 分配命令参数 
//            if (commandParameters != null)
//            {
//                AttachParameters(command, commandParameters);
//            }
//            return;

//        }


//        #region ExecuteNonQuery命令

//        /// <summary> 
//        /// 执行指定连接字符串,类型的TCommand. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders"); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
//        /// <param name="commandText">存储过程名称或SQL语句</param> 
//        /// <returns>返回命令影响的行数</returns> 
//        public int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
//        {
//            return ExecuteNonQuery(connectionString, commandType, commandText, null);
//        }


//        /// <summary> 
//        /// 执行指定连接字符串,类型的TCommand.如果没有提供参数,不返回结果. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
//        /// <param name="commandText">存储过程名称或SQL语句</param> 
//        /// <param name="commandParameters">TParameter参数数组</param> 
//        /// <returns>返回命令影响的行数</returns> 
//        public int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

//            using (TConnection connection = new TConnection())
//            //using (TConnection connection = new TConnection())
//            {

//                connection.ConnectionString = connectionString;
//                connection.Open();

//                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
//            }

//        }

//        /// <summary> 
//        /// 执行指定连接字符串的存储过程,将对象数组的值赋给存储过程参数, 
//        /// 此方法需要在参数缓存方法中探索参数并生成参数. 
//        /// </summary> 
//        /// <remarks> 
//        /// 这个方法没有提供访问输出参数和返回值. 
//        /// 示例:  
//        ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串/param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="parameterValues">分配到存储过程输入参数的对象数组</param> 
//        /// <returns>返回受影响的行数</returns> 
//        public int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果存在参数值 
//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // 从探索存储过程参数(加载到缓存)并分配给存储过程参数数组. 
//                TParameter[] commandParameters = GetSpParameterSet(connectionString, spName);

//                // 给存储过程参数赋值 
//                AssignParameterValues(commandParameters, parameterValues);

//                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                // 没有参数情况下 
//                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders"); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <returns>返回影响的行数</returns> 
//        public int ExecuteNonQuery(TConnection connection, CommandType commandType, string commandText)
//        {
//            return ExecuteNonQuery(connection, commandType, commandText, null);
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new TParameter("@prodid", 24));
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param> 
//        /// <param name="commandText">T存储过程名称或T-SQL语句</param> 
//        /// <param name="commandParameters">SqlParamter参数数组</param> 
//        /// <returns>返回影响的行数</returns> 
//        public int ExecuteNonQuery(TConnection connection, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");

//            // 创建TCommand命令,并进行预处理 
//            TCommand cmd = new TCommand();
//            bool mustCloseConnection = false;
//            PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);

//            // Finally, execute the command 
//            int retval = cmd.ExecuteNonQuery();

//            // 清除参数,以便再次使用. 
//            cmd.Parameters.Clear();
//            if (mustCloseConnection)
//                connection.Close();
//            return retval;
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令,将对象数组的值赋给存储过程参数. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值 
//        /// 示例:  
//        ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回影响的行数</returns> 
//        public int ExecuteNonQuery(TConnection connection, string spName, params object[] parameterValues)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果有参数值 
//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // 从缓存中加载存储过程参数 
//                TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//                // 给存储过程分配参数值 
//                AssignParameterValues(commandParameters, parameterValues);

//                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行带事务的TCommand. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例.:  
//        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders"); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <returns>返回影响的行数/returns> 
//        public int ExecuteNonQuery(TTransaction transaction, CommandType commandType, string commandText)
//        {
//            return ExecuteNonQuery(transaction, commandType, commandText, null);
//        }

//        /// <summary> 
//        /// 执行带事务的TCommand(指定参数). 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="commandParameters">SqlParamter参数数组</param> 
//        /// <returns>返回影响的行数</returns> 
//        public int ExecuteNonQuery(TTransaction transaction, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

//            // 预处理 
//            TCommand cmd = new TCommand();
//            bool mustCloseConnection = false;
//            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

//            // 执行 
//            int retval = cmd.ExecuteNonQuery();

//            // 清除参数集,以便再次使用. 
//            cmd.Parameters.Clear();
//            return retval;
//        }

//        /// <summary> 
//        /// 执行带事务的TCommand(指定参数值). 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值 
//        /// 示例:  
//        ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回受影响的行数</returns> 
//        public int ExecuteNonQuery(TTransaction transaction, string spName, params object[] parameterValues)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果有参数值 
//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//                // 给存储过程参数赋值 
//                AssignParameterValues(commandParameters, parameterValues);

//                // 调用重载方法 
//                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                // 没有参数值 
//                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
//            }
//        }

//        #endregion ExecuteNonQuery方法结束

//        #region ExecuteDataset方法

//        /// <summary> 
//        /// 执行指定数据库连接字符串的命令,返回DataSet. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders"); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <returns>返回一个包含结果集的DataSet</returns> 
//        public DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
//        {
//            return ExecuteDataset(connectionString, commandType, commandText, (TParameter[])null);
//        }

//        /// <summary> 
//        /// 执行指定数据库连接字符串的命令,返回DataSet. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例: 
//        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="commandParameters">SqlParamters参数数组</param> 
//        /// <returns>返回一个包含结果集的DataSet</returns> 
//        public DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

//            // 创建并打开数据库连接对象,操作完成释放对象. 
//            using (TConnection connection = new TConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();

//                // 调用指定数据库连接字符串重载方法. 
//                return ExecuteDataset(connection, commandType, commandText, commandParameters);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库连接字符串的命令,直接提供参数值,返回DataSet. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值. 
//        /// 示例: 
//        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回一个包含结果集的DataSet</returns> 
//        public DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // 从缓存中检索存储过程参数 
//                TParameter[] commandParameters = GetSpParameterSet(connectionString, spName);

//                // 给存储过程参数分配值 
//                AssignParameterValues(commandParameters, parameterValues);

//                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令,返回DataSet. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders"); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名或T-SQL语句</param> 
//        /// <returns>返回一个包含结果集的DataSet</returns> 
//        public DataSet ExecuteDataset(TConnection connection, CommandType commandType, string commandText)
//        {
//            return ExecuteDataset(connection, commandType, commandText, (TParameter[])null);
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataSet. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名或T-SQL语句</param> 
//        /// <param name="commandParameters">SqlParamter参数数组</param> 
//        /// <returns>返回一个包含结果集的DataSet</returns> 
//        public DataSet ExecuteDataset(TConnection connection, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");

//            // 预处理 
//            TCommand cmd = new TCommand();
//            bool mustCloseConnection = false;
//            PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);

//            // 创建SqlDataAdapter和DataSet. 
//            using (TDataAdapter da = new TDataAdapter())
//            {

//                da.SelectCommand = cmd;

//                DataSet ds = new DataSet();

//                // 填充DataSet. 
//                da.Fill(ds);

//                cmd.Parameters.Clear();

//                if (mustCloseConnection)
//                    connection.Close();

//                return ds;

//            }

//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令,指定参数值,返回DataSet. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输入参数和返回值. 
//        /// 示例.:  
//        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回一个包含结果集的DataSet</returns> 
//        public DataSet ExecuteDataset(TConnection connection, string spName, params object[] parameterValues)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // 比缓存中加载存储过程参数 
//                TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//                // 给存储过程参数分配值 
//                AssignParameterValues(commandParameters, parameterValues);

//                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定事务的命令,返回DataSet. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders"); 
//        /// </remarks> 
//        /// <param name="transaction">事务</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名或T-SQL语句</param> 
//        /// <returns>返回一个包含结果集的DataSet</returns> 
//        public DataSet ExecuteDataset(TTransaction transaction, CommandType commandType, string commandText)
//        {
//            return ExecuteDataset(transaction, commandType, commandText, (TParameter[])null);
//        }

//        /// <summary> 
//        /// 执行指定事务的命令,指定参数,返回DataSet. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="transaction">事务</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名或T-SQL语句</param> 
//        /// <param name="commandParameters">SqlParamter参数数组</param> 
//        /// <returns>返回一个包含结果集的DataSet</returns> 
//        public DataSet ExecuteDataset(TTransaction transaction, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

//            // 预处理 
//            TCommand cmd = new TCommand();
//            bool mustCloseConnection = false;
//            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

//            // 创建 DataAdapter & DataSet 
//            using (TDataAdapter da = new TDataAdapter())
//            {
//                da.SelectCommand = cmd;
//                DataSet ds = new DataSet();
//                da.Fill(ds);
//                cmd.Parameters.Clear();
//                return ds;
//            }

//        }

//        /// <summary> 
//        /// 执行指定事务的命令,指定参数值,返回DataSet. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输入参数和返回值. 
//        /// 示例.:  
//        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36); 
//        /// </remarks> 
//        /// <param name="transaction">事务</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回一个包含结果集的DataSet</returns> 
//        public DataSet ExecuteDataset(TTransaction transaction, string spName, params object[] parameterValues)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // 从缓存中加载存储过程参数 
//                TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//                // 给存储过程参数分配值 
//                AssignParameterValues(commandParameters, parameterValues);

//                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
//            }
//        }

//        #endregion ExecuteDataset数据集命令结束

//        #region ExecuteReader 数据阅读器

//        ///// <summary> 
//        ///// 枚举,标识数据库连接是由SqlHelper提供还是由调用者提供 
//        ///// </summary> 
//        //private enum SqlConnectionOwnership
//        //{
//        //    /// <summary>由SqlHelper提供连接</summary> 
//        //    Internal,
//        //    /// <summary>由调用者提供连接</summary> 
//        //    External
//        //}

//        /// <summary> 
//        /// 执行指定数据库连接对象的数据阅读器. 
//        /// </summary> 
//        /// <remarks> 
//        /// 如果是SqlHelper打开连接,当连接关闭DataReader也将关闭. 
//        /// 如果是调用都打开连接,DataReader由调用都管理. 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="transaction">一个有效的事务,或者为 'null'</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名或T-SQL语句</param> 
//        /// <param name="commandParameters">TParameters参数数组,如果没有参数则为'null'</param> 
//        /// <param name="connectionOwnership">标识数据库连接对象是由调用者提供还是由SqlHelper提供</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        private TDataReader ExecuteReader(TConnection connection, TTransaction transaction, CommandType commandType, string commandText, TParameter[] commandParameters)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");

//            bool mustCloseConnection = false;
//            // 创建命令 
//            TCommand cmd = new TCommand();
//            try
//            {
//                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

//                // 创建数据阅读器 
//                TDataReader dataReader = (TDataReader)cmd.ExecuteReader();


//                //if (connectionOwnership == SqlConnectionOwnership.External)
//                //{
//                //    dataReader = cmd.ExecuteReader();
//                //}
//                //else
//                //{
//                //    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
//                //}

//                // 清除参数,以便再次使用.. 
//                // HACK: There is a problem here, the output parameter values are fletched 
//                // when the reader is closed, so if the parameters are detached from the command 
//                // then the SqlReader can磘 set its values. 
//                // When this happen, the parameters can磘 be used again in other command. 
//                bool canClear = true;
//                foreach (TParameter commandParameter in cmd.Parameters)
//                {
//                    if (commandParameter.Direction != ParameterDirection.Input)
//                        canClear = false;
//                }

//                if (canClear)
//                {
//                    cmd.Parameters.Clear();
//                }

//                return dataReader;
//            }
//            catch
//            {
//                if (mustCloseConnection)
//                    connection.Close();
//                throw;
//            }
//        }




//        /// <summary> 
//        /// 执行指定数据库连接字符串的数据阅读器. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  TDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders"); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名或T-SQL语句</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
//        {
//            return ExecuteReader(connectionString, commandType, commandText, (TParameter[])null);
//        }

//        /// <summary> 
//        /// 执行指定数据库连接字符串的数据阅读器,指定参数. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  TDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名或T-SQL语句</param> 
//        /// <param name="commandParameters">SqlParamter参数数组(new TParameter("@prodid", 24))</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            TConnection connection = null;
//            try
//            {
//                connection = new TConnection();
//                connection.ConnectionString = connectionString;
//                connection.Open();

//                return ExecuteReader(connection, null, commandType, commandText, commandParameters);
//            }
//            catch
//            {
//                // If we fail to return the SqlDatReader, we need to close the connection ourselves 
//                if (connection != null) connection.Close();
//                throw;
//            }

//        }

//        /// <summary> 
//        /// 执行指定数据库连接字符串的数据阅读器,指定参数值. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值参数. 
//        /// 示例:  
//        ///  TDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                TParameter[] commandParameters = GetSpParameterSet(connectionString, spName);

//                AssignParameterValues(commandParameters, parameterValues);

//                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的数据阅读器. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  TDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders"); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名或T-SQL语句</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReader(TConnection connection, CommandType commandType, string commandText)
//        {
//            return ExecuteReader(connection, commandType, commandText, null);
//        }

//        /// <summary> 
//        /// [调用者方式]执行指定数据库连接对象的数据阅读器,指定参数. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  TDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandParameters">SqlParamter参数数组</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReader(TConnection connection, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            return ExecuteReader(connection, null, commandType, commandText, commandParameters);
//        }

//        /// <summary> 
//        /// [调用者方式]执行指定数据库连接对象的数据阅读器,指定参数值. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值参数. 
//        /// 示例:  
//        ///  TDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">T存储过程名</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReader(TConnection connection, string spName, params object[] parameterValues)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//                AssignParameterValues(commandParameters, parameterValues);

//                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数值. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  TDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders"); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的连接事务</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReader(TTransaction transaction, CommandType commandType, string commandText)
//        {
//            return ExecuteReader(transaction, commandType, commandText, null);
//        }

//        /// <summary> 
//        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///   TDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的连接事务</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReader(TTransaction transaction, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

//            return ExecuteReader((TConnection)transaction.Connection, transaction, commandType, commandText, commandParameters);
//        }

//        /// <summary> 
//        /// [调用者方式]执行指定数据库事务的数据阅读器,指定参数值. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值参数. 
//        /// 
//        /// 示例:  
//        ///  TDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的连接事务</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReader(TTransaction transaction, string spName, params object[] parameterValues)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果有参数值 
//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//                AssignParameterValues(commandParameters, parameterValues);

//                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                // 没有参数值 
//                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
//            }
//        }

//        #endregion ExecuteReader数据阅读器

//        #region ExecuteScalar 返回结果集中的第一行第一列

//        /// <summary> 
//        /// 执行指定数据库连接字符串的命令,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount"); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
//        {
//            // 执行参数为空的方法 
//            return ExecuteScalar(connectionString, commandType, commandText, (TParameter[])null);
//        }

//        /// <summary> 
//        /// 执行指定数据库连接字符串的命令,指定参数,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            // 创建并打开数据库连接对象,操作完成释放对象. 
//            using (TConnection connection = new TConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();

//                // 调用指定数据库连接字符串重载方法. 
//                return ExecuteScalar(connection, commandType, commandText, commandParameters);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库连接字符串的命令,指定参数值,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值参数. 
//        /// 
//        /// 示例:  
//        ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果有参数值 
//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connectionString, spName);

//                // 给存储过程参数赋值 
//                AssignParameterValues(commandParameters, parameterValues);

//                // 调用重载方法 
//                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                // 没有参数值 
//                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount"); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalar(TConnection connection, CommandType commandType, string commandText)
//        {
//            // 执行参数为空的方法 
//            return ExecuteScalar(connection, commandType, commandText, (TParameter[])null);
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令,指定参数,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalar(TConnection connection, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");

//            // 创建TCommand命令,并进行预处理 
//            TCommand cmd = new TCommand();

//            bool mustCloseConnection = false;
//            PrepareCommand(cmd, connection, (TTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

//            // 执行TCommand命令,并返回结果. 
//            object retval = cmd.ExecuteScalar();

//            // 清除参数,以便再次使用. 
//            cmd.Parameters.Clear();

//            if (mustCloseConnection)
//                connection.Close();

//            return retval;
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令,指定参数值,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值参数. 
//        /// 
//        /// 示例:  
//        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalar(TConnection connection, string spName, params object[] parameterValues)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果有参数值 
//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//                // 给存储过程参数赋值 
//                AssignParameterValues(commandParameters, parameterValues);

//                // 调用重载方法 
//                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                // 没有参数值 
//                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库事务的命令,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount"); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的连接事务</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalar(TTransaction transaction, CommandType commandType, string commandText)
//        {
//            // 执行参数为空的方法 
//            return ExecuteScalar(transaction, commandType, commandText, (TParameter[])null);
//        }

//        /// <summary> 
//        /// 执行指定数据库事务的命令,指定参数,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的连接事务</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalar(TTransaction transaction, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

//            // 创建TCommand命令,并进行预处理 
//            TCommand cmd = new TCommand();
//            bool mustCloseConnection = false;
//            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

//            // 执行TCommand命令,并返回结果. 
//            object retval = cmd.ExecuteScalar();

//            // 清除参数,以便再次使用. 
//            cmd.Parameters.Clear();
//            return retval;
//        }

//        /// <summary> 
//        /// 执行指定数据库事务的命令,指定参数值,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值参数. 
//        /// 
//        /// 示例:  
//        ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的连接事务</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalar(TTransaction transaction, string spName, params object[] parameterValues)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果有参数值 
//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // PPull the parameters for this stored procedure from the parameter cache () 
//                TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//                // 给存储过程参数赋值 
//                AssignParameterValues(commandParameters, parameterValues);

//                // 调用重载方法 
//                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                // 没有参数值 
//                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
//            }
//        }

//        #endregion ExecuteScalar

//        #region ExecuteXmlReader XML阅读器


//        ///// <summary> 
//        ///// 执行指定数据库连接对象的TCommand命令,并产生一个XmlReader对象做为结果集返回. 
//        ///// </summary> 
//        ///// <remarks> 
//        ///// 示例:  
//        /////  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders"); 
//        ///// </remarks> 
//        ///// <param name="connection">一个有效的数据库连接对象</param> 
//        ///// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        ///// <param name="commandText">存储过程名称或T-SQL语句 using "FOR XML AUTO"</param> 
//        ///// <returns>返回XmlReader结果集对象.</returns> 
//        //public  XmlReader ExecuteXmlReader(TConnection connection, CommandType commandType, string commandText)
//        //{
//        //    // 执行参数为空的方法 
//        //    return ExecuteXmlReader(connection, commandType, commandText, (TParameter[])null);
//        //}

//        ///// <summary> 
//        ///// 执行指定数据库连接对象的TCommand命令,并产生一个XmlReader对象做为结果集返回,指定参数. 
//        ///// </summary> 
//        ///// <remarks> 
//        ///// 示例:  
//        /////  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new TParameter("@prodid", 24)); 
//        ///// </remarks> 
//        ///// <param name="connection">一个有效的数据库连接对象</param> 
//        ///// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        ///// <param name="commandText">存储过程名称或T-SQL语句 using "FOR XML AUTO"</param> 
//        ///// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
//        ///// <returns>返回XmlReader结果集对象.</returns> 
//        //public  XmlReader ExecuteXmlReader(TConnection connection, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        //{
//        //    if (connection == null) throw new ArgumentNullException("connection");

//        //    bool mustCloseConnection = false;
//        //    // 创建TCommand命令,并进行预处理 
//        //    TCommand cmd = new TCommand();
//        //    try
//        //    {
//        //        PrepareCommand(cmd, connection, (TTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

//        //        // 执行命令 
//        //        XmlReader retval = cmd.ExecuteXmlReader();

//        //        // 清除参数,以便再次使用. 
//        //        cmd.Parameters.Clear();

//        //        return retval;
//        //    }
//        //    catch
//        //    {
//        //        if (mustCloseConnection)
//        //            connection.Close();
//        //        throw;
//        //    }
//        //}

//        ///// <summary> 
//        ///// 执行指定数据库连接对象的TCommand命令,并产生一个XmlReader对象做为结果集返回,指定参数值. 
//        ///// </summary> 
//        ///// <remarks> 
//        ///// 此方法不提供访问存储过程输出参数和返回值参数. 
//        ///// 
//        ///// 示例:  
//        /////  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36); 
//        ///// </remarks> 
//        ///// <param name="connection">一个有效的数据库连接对象</param> 
//        ///// <param name="spName">存储过程名称 using "FOR XML AUTO"</param> 
//        ///// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        ///// <returns>返回XmlReader结果集对象.</returns> 
//        //public  XmlReader ExecuteXmlReader(TConnection connection, string spName, params object[] parameterValues)
//        //{
//        //    if (connection == null) throw new ArgumentNullException("connection");
//        //    if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//        //    // 如果有参数值 
//        //    if ((parameterValues != null) && (parameterValues.Length > 0))
//        //    {
//        //        // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//        //        TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//        //        // 给存储过程参数赋值 
//        //        AssignParameterValues(commandParameters, parameterValues);

//        //        // 调用重载方法 
//        //        return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
//        //    }
//        //    else
//        //    {
//        //        // 没有参数值 
//        //        return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
//        //    }
//        //}

//        ///// <summary> 
//        ///// 执行指定数据库事务的TCommand命令,并产生一个XmlReader对象做为结果集返回. 
//        ///// </summary> 
//        ///// <remarks> 
//        ///// 示例:  
//        /////  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders"); 
//        ///// </remarks> 
//        ///// <param name="transaction">一个有效的连接事务</param> 
//        ///// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        ///// <param name="commandText">存储过程名称或T-SQL语句 using "FOR XML AUTO"</param> 
//        ///// <returns>返回XmlReader结果集对象.</returns> 
//        //public  XmlReader ExecuteXmlReader(TTransaction transaction, CommandType commandType, string commandText)
//        //{
//        //    // 执行参数为空的方法 
//        //    return ExecuteXmlReader(transaction, commandType, commandText, (TParameter[])null);
//        //}

//        ///// <summary> 
//        ///// 执行指定数据库事务的TCommand命令,并产生一个XmlReader对象做为结果集返回,指定参数. 
//        ///// </summary> 
//        ///// <remarks> 
//        ///// 示例:  
//        /////  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new TParameter("@prodid", 24)); 
//        ///// </remarks> 
//        ///// <param name="transaction">一个有效的连接事务</param> 
//        ///// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        ///// <param name="commandText">存储过程名称或T-SQL语句 using "FOR XML AUTO"</param> 
//        ///// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
//        ///// <returns>返回XmlReader结果集对象.</returns> 
//        //public  XmlReader ExecuteXmlReader(TTransaction transaction, CommandType commandType, string commandText, params TParameter[] commandParameters)
//        //{
//        //    if (transaction == null) throw new ArgumentNullException("transaction");
//        //    if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

//        //    // 创建TCommand命令,并进行预处理 
//        //    TCommand cmd = new TCommand();
//        //    bool mustCloseConnection = false;
//        //    PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

//        //    // 执行命令 
//        //    XmlReader retval = cmd.ExecuteXmlReader();

//        //    // 清除参数,以便再次使用. 
//        //    cmd.Parameters.Clear();
//        //    return retval;
//        //}

//        ///// <summary> 
//        ///// 执行指定数据库事务的TCommand命令,并产生一个XmlReader对象做为结果集返回,指定参数值. 
//        ///// </summary> 
//        ///// <remarks> 
//        ///// 此方法不提供访问存储过程输出参数和返回值参数. 
//        ///// 
//        ///// 示例:  
//        /////  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36); 
//        ///// </remarks> 
//        ///// <param name="transaction">一个有效的连接事务</param> 
//        ///// <param name="spName">存储过程名称</param> 
//        ///// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        ///// <returns>返回一个包含结果集的DataSet.</returns> 
//        //public  XmlReader ExecuteXmlReader(TTransaction transaction, string spName, params object[] parameterValues)
//        //{
//        //    if (transaction == null) throw new ArgumentNullException("transaction");
//        //    if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//        //    if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//        //    // 如果有参数值 
//        //    if ((parameterValues != null) && (parameterValues.Length > 0))
//        //    {
//        //        // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//        //        TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//        //        // 给存储过程参数赋值 
//        //        AssignParameterValues(commandParameters, parameterValues);

//        //        // 调用重载方法 
//        //        return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
//        //    }
//        //    else
//        //    {
//        //        // 没有参数值 
//        //        return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
//        //    }
//        //}

//        #endregion ExecuteXmlReader 阅读器结束




//        #region FillDataset 填充数据集
//        /// <summary> 
//        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
//        /// <param name="tableNames">表映射的数据表数组 
//        /// 用户定义的表名 (可有是实际的表名.)</param> 
//        public void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (dataSet == null) throw new ArgumentNullException("dataSet");

//            // 创建并打开数据库连接对象,操作完成释放对象. 
//            using (TConnection connection = new TConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();

//                // 调用指定数据库连接字符串重载方法. 
//                FillDataset(connection, commandType, commandText, dataSet, tableNames);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集.指定命令参数. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
//        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
//        /// <param name="tableNames">表映射的数据表数组 
//        /// 用户定义的表名 (可有是实际的表名.) 
//        /// </param> 
//        public void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params TParameter[] commandParameters)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (dataSet == null) throw new ArgumentNullException("dataSet");
//            // 创建并打开数据库连接对象,操作完成释放对象. 
//            using (TConnection connection = new TConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();

//                // 调用指定数据库连接字符串重载方法. 
//                FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库连接字符串的命令,映射数据表并填充数据集,指定存储过程参数值. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值参数. 
//        /// 
//        /// 示例:  
//        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, 24); 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
//        /// <param name="tableNames">表映射的数据表数组 
//        /// 用户定义的表名 (可有是实际的表名.) 
//        /// </param>    
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        public void FillDataset(string connectionString, string spName,
//            DataSet dataSet, string[] tableNames,
//            params object[] parameterValues)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (dataSet == null) throw new ArgumentNullException("dataSet");
//            // 创建并打开数据库连接对象,操作完成释放对象. 
//            using (TConnection connection = new TConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();

//                // 调用指定数据库连接字符串重载方法. 
//                FillDataset(connection, spName, dataSet, tableNames, parameterValues);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
//        /// <param name="tableNames">表映射的数据表数组 
//        /// 用户定义的表名 (可有是实际的表名.) 
//        /// </param>    
//        public void FillDataset(TConnection connection, CommandType commandType,
//            string commandText, DataSet dataSet, string[] tableNames)
//        {
//            FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集,指定参数. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
//        /// <param name="tableNames">表映射的数据表数组 
//        /// 用户定义的表名 (可有是实际的表名.) 
//        /// </param> 
//        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
//        public void FillDataset(TConnection connection, CommandType commandType,
//            string commandText, DataSet dataSet, string[] tableNames,
//            params TParameter[] commandParameters)
//        {
//            FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
//        }

//        /// <summary> 
//        /// 执行指定数据库连接对象的命令,映射数据表并填充数据集,指定存储过程参数值. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值参数. 
//        /// 
//        /// 示例:  
//        ///  FillDataset(conn, "GetOrders", ds, new string[] {"orders"}, 24, 36); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
//        /// <param name="tableNames">表映射的数据表数组 
//        /// 用户定义的表名 (可有是实际的表名.) 
//        /// </param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        public void FillDataset(TConnection connection, string spName,
//            DataSet dataSet, string[] tableNames,
//            params object[] parameterValues)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (dataSet == null) throw new ArgumentNullException("dataSet");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果有参数值 
//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//                // 给存储过程参数赋值 
//                AssignParameterValues(commandParameters, parameterValues);

//                // 调用重载方法 
//                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
//            }
//            else
//            {
//                // 没有参数值 
//                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
//            }
//        }

//        /// <summary> 
//        /// 执行指定数据库事务的命令,映射数据表并填充数据集. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的连接事务</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
//        /// <param name="tableNames">表映射的数据表数组 
//        /// 用户定义的表名 (可有是实际的表名.) 
//        /// </param> 
//        public void FillDataset(TTransaction transaction, CommandType commandType,
//            string commandText,
//            DataSet dataSet, string[] tableNames)
//        {
//            FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
//        }

//        /// <summary> 
//        /// 执行指定数据库事务的命令,映射数据表并填充数据集,指定参数. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的连接事务</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
//        /// <param name="tableNames">表映射的数据表数组 
//        /// 用户定义的表名 (可有是实际的表名.) 
//        /// </param> 
//        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
//        public void FillDataset(TTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params TParameter[] commandParameters)
//        {
//            FillDataset((TConnection)transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
//        }

//        /// <summary> 
//        /// 执行指定数据库事务的命令,映射数据表并填充数据集,指定存储过程参数值. 
//        /// </summary> 
//        /// <remarks> 
//        /// 此方法不提供访问存储过程输出参数和返回值参数. 
//        /// 
//        /// 示例:  
//        ///  FillDataset(trans, "GetOrders", ds, new string[]{"orders"}, 24, 36); 
//        /// </remarks> 
//        /// <param name="transaction">一个有效的连接事务</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
//        /// <param name="tableNames">表映射的数据表数组 
//        /// 用户定义的表名 (可有是实际的表名.) 
//        /// </param> 
//        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
//        public void FillDataset(TTransaction transaction, string spName,
//            DataSet dataSet, string[] tableNames,
//            params object[] parameterValues)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//            if (dataSet == null) throw new ArgumentNullException("dataSet");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果有参数值 
//            if ((parameterValues != null) && (parameterValues.Length > 0))
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//                // 给存储过程参数赋值 
//                AssignParameterValues(commandParameters, parameterValues);

//                // 调用重载方法 
//                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
//            }
//            else
//            {
//                // 没有参数值 
//                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
//            }
//        }

//        /// <summary> 
//        /// [私有方法][内部调用]执行指定数据库连接对象/事务的命令,映射数据表并填充数据集,DataSet/TableNames/TParameters. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  FillDataset(conn, trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new TParameter("@prodid", 24)); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="transaction">一个有效的连接事务</param> 
//        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
//        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
//        /// <param name="dataSet">要填充结果集的DataSet实例</param> 
//        /// <param name="tableNames">表映射的数据表数组 
//        /// 用户定义的表名 (可有是实际的表名.) 
//        /// </param> 
//        /// <param name="commandParameters">分配给命令的SqlParamter参数数组</param> 
//        protected void FillDataset(TConnection connection, TTransaction transaction, CommandType commandType,
//            string commandText, DataSet dataSet, string[] tableNames,
//            params TParameter[] commandParameters)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (dataSet == null) throw new ArgumentNullException("dataSet");

//            // 创建TCommand命令,并进行预处理 
//            TCommand command = new TCommand();
//            bool mustCloseConnection = false;
//            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

//            // 执行命令 
//            using (TDataAdapter dataAdapter = new TDataAdapter())
//            {

//                dataAdapter.SelectCommand = command;

//                // 追加表映射 
//                if (tableNames != null && tableNames.Length > 0)
//                {
//                    string tableName = "Table";
//                    for (int index = 0; index < tableNames.Length; index++)
//                    {
//                        if (tableNames[index] == null || tableNames[index].Length == 0) throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
//                        dataAdapter.TableMappings.Add(tableName, tableNames[index]);
//                        tableName += (index + 1).ToString();
//                    }
//                }

//                // 填充数据集使用默认表名称 
//                dataAdapter.Fill(dataSet);

//                // 清除参数,以便再次使用. 
//                command.Parameters.Clear();
//            }

//            if (mustCloseConnection)
//                connection.Close();
//        }
//        #endregion

//        #region UpdateDataset 更新数据集
//        /// <summary> 
//        /// 执行数据集更新到数据库,指定inserted, updated, or deleted命令. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order"); 
//        /// </remarks> 
//        /// <param name="insertCommand">[追加记录]一个有效的T-SQL语句或存储过程</param> 
//        /// <param name="deleteCommand">[删除记录]一个有效的T-SQL语句或存储过程</param> 
//        /// <param name="updateCommand">[更新记录]一个有效的T-SQL语句或存储过程</param> 
//        /// <param name="dataSet">要更新到数据库的DataSet</param> 
//        /// <param name="tableName">要更新到数据库的DataTable</param> 
//        public void UpdateDataset(TCommand insertCommand, TCommand deleteCommand, TCommand updateCommand, DataSet dataSet, string tableName)
//        {
//            if (insertCommand == null) throw new ArgumentNullException("insertCommand");
//            if (deleteCommand == null) throw new ArgumentNullException("deleteCommand");
//            if (updateCommand == null) throw new ArgumentNullException("updateCommand");
//            if (tableName == null || tableName.Length == 0) throw new ArgumentNullException("tableName");

//            // 创建SqlDataAdapter,当操作完成后释放. 
//            using (TDataAdapter dataAdapter = new TDataAdapter())
//            {
//                // 设置数据适配器命令 
//                dataAdapter.UpdateCommand = updateCommand;
//                dataAdapter.InsertCommand = insertCommand;
//                dataAdapter.DeleteCommand = deleteCommand;

//                // 更新数据集改变到数据库 
//                dataAdapter.Update(dataSet, tableName);

//                // 提交所有改变到数据集. 
//                dataSet.AcceptChanges();
//            }
//        }
//        #endregion

//        #region CreateCommand 创建一条TCommand命令
//        /// <summary> 
//        /// 创建TCommand命令,指定数据库连接对象,存储过程名和参数. 
//        /// </summary> 
//        /// <remarks> 
//        /// 示例:  
//        ///  TCommand command = CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName"); 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="sourceColumns">源表的列名称数组</param> 
//        /// <returns>返回TCommand命令</returns> 
//        public TCommand CreateCommand(TConnection connection, string spName, params string[] sourceColumns)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 创建命令 
//            TCommand cmd = new TCommand
//            {
//                CommandText = spName,
//                Connection = connection,
//                CommandType = CommandType.StoredProcedure
//            };

//            // 如果有参数值 
//            if ((sourceColumns != null) && (sourceColumns.Length > 0))
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//                // 将源表的列到映射到DataSet命令中. 
//                for (int index = 0; index < sourceColumns.Length; index++)
//                    commandParameters[index].SourceColumn = sourceColumns[index];

//                // Attach the discovered parameters to the TCommand object 
//                AttachParameters(cmd, commandParameters);
//            }

//            return cmd;
//        }
//        #endregion

//        #region ExecuteNonQueryTypedParams 类型化参数(DataRow)
//        /// <summary> 
//        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回受影响的行数. 
//        /// </summary> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回影响的行数</returns> 
//        public int ExecuteNonQueryTypedParams(String connectionString, String spName, DataRow dataRow)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connectionString, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回受影响的行数. 
//        /// </summary> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回影响的行数</returns> 
//        public int ExecuteNonQueryTypedParams(TConnection connection, String spName, DataRow dataRow)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定连接数据库事物的存储过程,使用DataRow做为参数值,返回受影响的行数. 
//        /// </summary> 
//        /// <param name="transaction">一个有效的连接事务 object</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回影响的行数</returns> 
//        public int ExecuteNonQueryTypedParams(TTransaction transaction, String spName, DataRow dataRow)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // Sf the row has values, the store procedure parameters must be initialized 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
//            }
//        }
//        #endregion

//        #region ExecuteDatasetTypedParams 类型化参数(DataRow)
//        /// <summary> 
//        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回DataSet. 
//        /// </summary> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回一个包含结果集的DataSet.</returns> 
//        public DataSet ExecuteDatasetTypedParams(string connectionString, String spName, DataRow dataRow)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            //如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connectionString, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回DataSet. 
//        /// </summary> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回一个包含结果集的DataSet.</returns> 
//        /// 
//        public DataSet ExecuteDatasetTypedParams(TConnection connection, String spName, DataRow dataRow)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回DataSet. 
//        /// </summary> 
//        /// <param name="transaction">一个有效的连接事务 object</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回一个包含结果集的DataSet.</returns> 
//        public DataSet ExecuteDatasetTypedParams(TTransaction transaction, String spName, DataRow dataRow)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
//            }
//        }

//        #endregion

//        #region ExecuteReaderTypedParams 类型化参数(DataRow)
//        /// <summary> 
//        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回DataReader. 
//        /// </summary> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReaderTypedParams(String connectionString, String spName, DataRow dataRow)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connectionString, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
//            }
//        }


//        /// <summary> 
//        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回DataReader. 
//        /// </summary> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReaderTypedParams(TConnection connection, String spName, DataRow dataRow)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定连接数据库事物的存储过程,使用DataRow做为参数值,返回DataReader. 
//        /// </summary> 
//        /// <param name="transaction">一个有效的连接事务 object</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回包含结果集的TDataReader</returns> 
//        public TDataReader ExecuteReaderTypedParams(TTransaction transaction, String spName, DataRow dataRow)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
//            }
//        }
//        #endregion

//        #region ExecuteScalarTypedParams 类型化参数(DataRow)
//        /// <summary> 
//        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalarTypedParams(String connectionString, String spName, DataRow dataRow)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connectionString, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalarTypedParams(TConnection connection, String spName, DataRow dataRow)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
//            }
//        }

//        /// <summary> 
//        /// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回结果集中的第一行第一列. 
//        /// </summary> 
//        /// <param name="transaction">一个有效的连接事务 object</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="dataRow">使用DataRow作为参数值</param> 
//        /// <returns>返回结果集中的第一行第一列</returns> 
//        public object ExecuteScalarTypedParams(TTransaction transaction, String spName, DataRow dataRow)
//        {
//            if (transaction == null) throw new ArgumentNullException("transaction");
//            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            // 如果row有值,存储过程必须初始化. 
//            if (dataRow != null && dataRow.ItemArray.Length > 0)
//            {
//                // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//                TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//                // 分配参数值 
//                AssignParameterValues(commandParameters, dataRow);

//                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
//            }
//            else
//            {
//                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
//            }
//        }
//        #endregion

//        #region ExecuteXmlReaderTypedParams 类型化参数(DataRow)
//        ///// <summary> 
//        ///// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回XmlReader类型的结果集. 
//        ///// </summary> 
//        ///// <param name="connection">一个有效的数据库连接对象</param> 
//        ///// <param name="spName">存储过程名称</param> 
//        ///// <param name="dataRow">使用DataRow作为参数值</param> 
//        ///// <returns>返回XmlReader结果集对象.</returns> 
//        //public XmlReader ExecuteXmlReaderTypedParams(TConnection connection, String spName, DataRow dataRow)
//        //{
//        //    if (connection == null) throw new ArgumentNullException("connection");
//        //    if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//        //    // 如果row有值,存储过程必须初始化. 
//        //    if (dataRow != null && dataRow.ItemArray.Length > 0)
//        //    {
//        //        // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//        //        TParameter[] commandParameters = GetSpParameterSet(connection, spName);

//        //        // 分配参数值 
//        //        AssignParameterValues(commandParameters, dataRow);

//        //        return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
//        //    }
//        //    else
//        //    {
//        //        return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
//        //    }
//        //}

//        ///// <summary> 
//        ///// 执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回XmlReader类型的结果集. 
//        ///// </summary> 
//        ///// <param name="transaction">一个有效的连接事务 object</param> 
//        ///// <param name="spName">存储过程名称</param> 
//        ///// <param name="dataRow">使用DataRow作为参数值</param> 
//        ///// <returns>返回XmlReader结果集对象.</returns> 
//        //public XmlReader ExecuteXmlReaderTypedParams(TTransaction transaction, String spName, DataRow dataRow)
//        //{
//        //    if (transaction == null) throw new ArgumentNullException("transaction");
//        //    if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
//        //    if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//        //    // 如果row有值,存储过程必须初始化. 
//        //    if (dataRow != null && dataRow.ItemArray.Length > 0)
//        //    {
//        //        // 从缓存中加载存储过程参数,如果缓存中不存在则从数据库中检索参数信息并加载到缓存中. () 
//        //        TParameter[] commandParameters = GetSpParameterSet(transaction.Connection, spName);

//        //        // 分配参数值 
//        //        AssignParameterValues(commandParameters, dataRow);

//        //        return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
//        //    }
//        //    else
//        //    {
//        //        return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
//        //    }
//        //}




//        #endregion



//    }


//}
