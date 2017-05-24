///********************************************************************

//时间: 2015年10月19日, AM 11:08:51

//作者: lanyanmiyu@qq.com

//描述: 缓存存储过程参数,并能够在运行时从存储过程中探索参数

//其它:     

//********************************************************************/



//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Lanymy.General.Extension.ExtensionFunctions;
//using System.Reflection;

//namespace Lanymy.General.Extension
//{


//    /// <summary>
//    /// 缓存存储过程参数,并能够在运行时从存储过程中探索参数
//    /// </summary>
//    internal abstract class DataBaseCommonParameterCache<TConnection, TTransaction, TCommand, TParameter, TDataAdapter, TDataReader, TCommandBuilder>
//        where TConnection : DbConnection, new()
//        where TTransaction : DbTransaction
//        where TCommand : DbCommand, new()
//        where TParameter : DbParameter
//        where TDataAdapter : DbDataAdapter, new()
//        where TDataReader : DbDataReader, new()
//        where TCommandBuilder : DbCommandBuilder, new()
//    {

//        #region 私有方法,字段,构造函数


//        //// 私有构造函数,妨止类被实例化. 
//        //private DataBaseCommonParameterCache() { }

//        // 这个方法要注意 
//        private static Hashtable _ParamCache = Hashtable.Synchronized(new Hashtable());


//        private Type _CommandBuilderType;

//        protected Type CommandBuilderType
//        {
//            get
//            {
//                if (_CommandBuilderType.IfIsNullOrEmpty())
//                {
//                    _CommandBuilderType = typeof(TCommandBuilder);
//                }

//                return _CommandBuilderType;
//            }
//        }



//        protected void CommandBuilderDeriveParameters(TCommand cmd)
//        {
//            CommandBuilderType.InvokeMember("DeriveParameters", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { cmd });
//        }



//        /// <summary> 
//        /// 探索运行时的存储过程,返回SqlParameter参数数组. 
//        /// 初始化参数值为 DBNull.Value. 
//        /// </summary> 
//        /// <param name="connection">一个有效的数据库连接</param> 
//        /// <param name="spName">存储过程名称</param> 
//        /// <param name="includeReturnValueParameter">是否包含返回值参数</param> 
//        /// <returns>返回SqlParameter参数数组</returns> 
//        protected TParameter[] DiscoverSpParameterSet(TConnection connection, string spName, bool includeReturnValueParameter)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            //TCommand cmd = new TCommand(spName, connection);
//            TCommand cmd = new TCommand
//            {
//                CommandText = spName,
//                Connection = connection,
//                CommandType = CommandType.StoredProcedure
//            };



//            connection.Open();
//            // 检索cmd指定的存储过程的参数信息,并填充到cmd的Parameters参数集中. 
//            CommandBuilderDeriveParameters(cmd);
//            //TCommandBuilder.DeriveParameters(cmd);
//            connection.Close();
//            // 如果不包含返回值参数,将参数集中的每一个参数删除. 
//            if (!includeReturnValueParameter)
//            {
//                cmd.Parameters.RemoveAt(0);
//            }

//            // 创建参数数组 
//            TParameter[] discoveredParameters = new TParameter[cmd.Parameters.Count];
//            // 将cmd的Parameters参数集复制到discoveredParameters数组. 
//            cmd.Parameters.CopyTo(discoveredParameters, 0);

//            // 初始化参数值为 DBNull.Value. 
//            foreach (var discoveredParameter in discoveredParameters)
//            {
//                discoveredParameter.Value = DBNull.Value;
//            }
//            return discoveredParameters;
//        }




//        /// <summary> 
//        /// SqlParameter参数数组的深层拷贝. 
//        /// </summary> 
//        /// <param name="originalParameters">原始参数数组</param> 
//        /// <returns>返回一个同样的参数数组</returns> 
//        protected TParameter[] CloneParameters(TParameter[] originalParameters)
//        {
//            TParameter[] clonedParameters = new TParameter[originalParameters.Length];

//            for (int i = 0, j = originalParameters.Length; i < j; i++)
//            {
//                clonedParameters[i] = (TParameter)((ICloneable)originalParameters[i]).Clone();
//            }

//            return clonedParameters;
//        }

//        //#endregion 私有方法,字段,构造函数结束

//        //#region 缓存方法

//        /// <summary> 
//        /// 追加参数数组到缓存. 
//        /// </summary> 
//        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
//        /// <param name="commandText">存储过程名或SQL语句</param> 
//        /// <param name="commandParameters">要缓存的参数数组</param> 
//        protected void CacheParameterSet(string connectionString, string commandText, params TParameter[] commandParameters)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

//            string hashKey = connectionString + ":" + commandText;

//            _ParamCache[hashKey] = commandParameters;
//        }


//        /// <summary> 
//        /// 从缓存中获取参数数组. 
//        /// </summary> 
//        /// <param name="connectionString">一个有效的数据库连接字符</param> 
//        /// <param name="commandText">存储过程名或SQL语句</param> 
//        /// <returns>参数数组</returns> 
//        protected TParameter[] GetCachedParameterSet(string connectionString, string commandText)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

//            string hashKey = connectionString + ":" + commandText;

//            TParameter[] cachedParameters = _ParamCache[hashKey] as TParameter[];
//            if (cachedParameters == null)
//            {
//                return null;
//            }
//            else
//            {
//                return CloneParameters(cachedParameters);
//            }
//        }

//        //#endregion 缓存方法结束

//        //#region 检索指定的存储过程的参数集

//        /// <summary> 
//        /// 返回指定的存储过程的参数集 
//        /// </summary> 
//        /// <remarks> 
//        /// 这个方法将查询数据库,并将信息存储到缓存. 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <returns>返回SqlParameter参数数组</returns> 
//        protected TParameter[] GetSpParameterSet(string connectionString, string spName)
//        {
//            return GetSpParameterSet(connectionString, spName, false);
//        }

//        /// <summary> 
//        /// 返回指定的存储过程的参数集 
//        /// </summary> 
//        /// <remarks> 
//        /// 这个方法将查询数据库,并将信息存储到缓存. 
//        /// </remarks> 
//        /// <param name="connectionString">一个有效的数据库连接字符.</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <param name="includeReturnValueParameter">是否包含返回值参数</param> 
//        /// <returns>返回SqlParameter参数数组</returns> 
//        protected TParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
//        {
//            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            using (TConnection connection = new TConnection())
//            {
//                connection.ConnectionString = connectionString;
//                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
//            }
//        }

//        /// <summary> 
//        /// [内部]返回指定的存储过程的参数集(使用连接对象). 
//        /// </summary> 
//        /// <remarks> 
//        /// 这个方法将查询数据库,并将信息存储到缓存. 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接字符</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <returns>返回SqlParameter参数数组</returns> 
//        protected TParameter[] GetSpParameterSet(DbConnection connection, string spName)
//        {
//            return GetSpParameterSet(connection, spName, false);
//        }

//        /// <summary> 
//        /// [内部]返回指定的存储过程的参数集(使用连接对象) 
//        /// </summary> 
//        /// <remarks> 
//        /// 这个方法将查询数据库,并将信息存储到缓存. 
//        /// </remarks> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <param name="includeReturnValueParameter"> 
//        /// 是否包含返回值参数 
//        /// </param> 
//        /// <returns>返回SqlParameter参数数组</returns> 
//        protected TParameter[] GetSpParameterSet(DbConnection connection, string spName, bool includeReturnValueParameter)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            using (TConnection clonedConnection = (TConnection)((ICloneable)connection).Clone())
//            {
//                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
//            }
//        }

//        /// <summary> 
//        /// [私有]返回指定的存储过程的参数集(使用连接对象) 
//        /// </summary> 
//        /// <param name="connection">一个有效的数据库连接对象</param> 
//        /// <param name="spName">存储过程名</param> 
//        /// <param name="includeReturnValueParameter">是否包含返回值参数</param> 
//        /// <returns>返回SqlParameter参数数组</returns> 
//        protected TParameter[] GetSpParameterSetInternal(TConnection connection, string spName, bool includeReturnValueParameter)
//        {
//            if (connection == null) throw new ArgumentNullException("connection");
//            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

//            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

//            TParameter[] cachedParameters;

//            cachedParameters = _ParamCache[hashKey] as TParameter[];
//            if (cachedParameters == null)
//            {
//                TParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
//                _ParamCache[hashKey] = spParameters;
//                cachedParameters = spParameters;
//            }

//            return CloneParameters(cachedParameters);
//        }

//        #endregion 参数集检索结束

//    }




//}
