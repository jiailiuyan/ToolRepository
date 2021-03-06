﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Common.Net.DbProvider
{
    /// <summary>
    /// SQLHelper数据访问抽象基础类 
    /// 该对象不通用
    /// </summary>
    public abstract class SQLHelper : IDisposable
    {
        /// <summary>
        /// 数据库连接字符串（未用）
        /// </summary>	
        private static string connectionString = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SQLHelper() { }

        /// <summary>
        /// 自定义数据库连接
        /// </summary>
        /// <param name="db">数据库名称</param>
        /// <returns></returns>
        private static SqlConnection MySelfSqlConnection(DataBase db)
        {
            SqlConnection connection = new SqlConnection();
            var conn = ConfigurationManager.ConnectionStrings[db.ToString()];
            if (conn != null)
                connection.ConnectionString = conn.ConnectionString;
            else
                throw new ConfigurationErrorsException("配置文件中没有名为" + db.ToString() + "的数据库连接字符串");
            return connection;
        }

        /// <summary>
        /// 得到最大值,使用该方法有线程安全问题 使用LAST_INSERT_ID
        /// </summary>
        /// <param name="FieldName">字段</param>
        /// <param name="TableName">表名</param>
        /// <returns>增长后的值</returns>
        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = ExecuteScalar(strsql);
            return obj == null ? 1 : int.Parse(obj.ToString());
        }

        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="SQLString">SQL</param>
        /// <returns></returns>
        public static bool Exists(string SQLString)
        {
            object obj = ExecuteScalar(SQLString);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 判断记录是否存在（基于SqlParameter）
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool Exists(string SQLString, DataBase db, params SqlParameter[] cmdParms)
        {
            object obj = ExecuteScalar(SQLString, db, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="connStr">数据库配置字符</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteNonQuery(string commandText, string connStr)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="db">数据库配置字符</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteNonQuery(string commandText, DataBase db = DataBase.None)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="commandTexts">Sql列表</param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static int ExecuteSqlTran(List<String> commandTexts, DataBase db = DataBase.None)
        {
            using (SqlConnection conn = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < commandTexts.Count; n++)
                    {
                        string strsql = commandTexts[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch
                {
                    tx.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="commandTexts">多条SQL语句</param>
        /// <param name="SqlParameterList">多条SQL参数</param>
        /// <returns>影响的行数</returns>
        public static int ExecuteSqlTran(List<String> commandTexts, List<SqlParameter[]> SqlParameterList, DataBase db = DataBase.None)
        {
            using (SqlConnection conn = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                SqlTransaction tx = conn.BeginTransaction();
                int ac = 0;
                try
                {
                    int count = 0;
                    for (int n = 0; n < commandTexts.Count; n++)
                    {
                        string strsql = commandTexts[n];
                        if (strsql.Trim().Length > 1)
                        {
                            PrepareCommand(cmd, conn, tx, strsql, SqlParameterList == null ? null : SqlParameterList[n]);
                            count += cmd.ExecuteNonQuery();
                            ac++;
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    var a = ac;
                    var b = commandTexts[ac];
                    var c = SqlParameterList[ac];
                    tx.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlInsertImg(string commandText, byte[] fs)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, connection);
                SqlParameter myParameter = new SqlParameter("@fs", SqlDbType.Image);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string SQLString)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(SQLString, connection);
            try
            {
                connection.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string SQLString, DataBase db = DataBase.None)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                SqlCommand cmd = new SqlCommand(SQLString, connection);
                try
                {
                    connection.Open();
                    SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    return myReader;
                }
                catch (SqlException e)
                {
                    throw e;
                }
            }
        }

        #region 返回DataSet方法

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="connStr">数据库连接字符串</param>
        /// <returns></returns>
        public static DataSet FindDataSet(string commandText, string connStr)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(commandText, connection);
                    command.SelectCommand.CommandTimeout = CommandTimeOut;
                    command.Fill(ds, "ds");
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="db">数据库连接字符串</param>
        /// <returns></returns>
        public static DataSet FindDataSet(string commandText, DataBase db = DataBase.None)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(commandText, connection);
                    command.SelectCommand.CommandTimeout = CommandTimeOut;
                    command.Fill(ds, "ds");
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行带参数查询语句，返回DataSet
        /// </summary>
        /// <param name="commandText">SQL</param>
        /// <param name="cmdParms">参数列表</param>
        /// <returns></returns>
        public static DataSet FindDataSet(string commandText, DataBase db = DataBase.None, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, commandText, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet FindDataSet(string commandText, List<SqlParameter> cmdParms, DataBase db = DataBase.None)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, commandText, cmdParms.ToArray());
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }

        #endregion

        /// <summary>
        /// 返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteNonQuery(string SQLString, DataBase db = DataBase.None, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行带参数返回受影响的行数Sql
        /// </summary>
        /// <param name="SQLString">SQL</param>
        /// <param name="parameters">参数</param>
        /// <param name="commandtext">执行类型</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string SQLString, List<SqlParameter> parameters, CommandType commandtext = CommandType.Text)
        {
            int results = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(SQLString, connection);
                command.CommandType = commandtext;
                if (parameters != null)
                    foreach (var p in parameters)
                        command.Parameters.Add(p);
                results = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            return results;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList, DataBase db = DataBase.None)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                            PrepareCommand(cmd, connection, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public static int ExecuteSqlTran(System.Collections.Generic.List<CommandInfo> cmdList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int count = 0;
                        //循环
                        foreach (CommandInfo myDE in cmdList)
                        {
                            string cmdText = myDE.CommandText;
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);

                            if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                            {
                                if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                                {
                                    trans.Rollback();
                                    return 0;
                                }

                                object obj = cmd.ExecuteScalar();
                                bool isHave = false;
                                if (obj == null && obj == DBNull.Value)
                                {
                                    isHave = false;
                                }
                                isHave = Convert.ToInt32(obj) > 0;

                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                continue;
                            }
                            int val = cmd.ExecuteNonQuery();
                            count += val;
                            if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                            {
                                trans.Rollback();
                                return 0;
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        return count;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public static void ExecuteSqlTranWithIndentity(System.Collections.Generic.List<CommandInfo> SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int indentity = 0;
                        //循环
                        foreach (CommandInfo myDE in SQLStringList)
                        {
                            string cmdText = myDE.CommandText;
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句.实现数据库事务.
        /// </summary>
        /// <param name="SqlList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public static void ExecuteSqlTranWithIndentity(Hashtable SqlList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int indentity = 0;
                        //循环
                        foreach (DictionaryEntry myDE in SqlList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句.实现数据库事务.
        /// </summary>
        /// <param name="SqlList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        /// <param name="db">数据库名称</param>
        public static void ExecuteSqlTranWithIndentity(Hashtable SqlList, DataBase db)
        {
            using (SqlConnection conn = MySelfSqlConnection(db))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int indentity = 0;
                        //循环
                        foreach (DictionaryEntry myDE in SqlList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object ExecuteScalar(string SQLString, DataBase db = DataBase.None)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="SQLString">>SQL</param>
        /// <param name="db">数据库</param>
        /// <param name="cmdParms">参数列表</param>
        /// <returns>返回结果</returns>
        public static object ExecuteScalar(string SQLString, DataBase db, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 填充Command参数
        /// </summary>
        /// <param name="cmd">SqlCommand</param>
        /// <param name="conn">SqlConnection</param>
        /// <param name="trans">SqlTransaction</param>
        /// <param name="cmdText">Sql</param>
        /// <param name="cmdParms">参数</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// 返回数据集合
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="SQLString">SQL</param>
        /// <param name="readFunc">FUN</param>
        /// <param name="parameters">参数</param>
        /// <param name="commandtext">执行类型</param>
        /// <returns></returns>
        public static List<T> FindList<T>(string SQLString, Func<SqlDataReader, T> readFunc, List<SqlParameter> parameters, CommandType commandtext = CommandType.Text, DataBase db = DataBase.None)
        {
            List<T> results = new List<T>();
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                connection.Open();
                var command = new SqlCommand(SQLString, connection);
                command.CommandType = commandtext;
                if (parameters != null)
                    foreach (var p in parameters)
                        command.Parameters.Add(p);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            results.Add(readFunc(reader));
                }
            }
            return results;
        }

        /// <summary>
        /// 返回一个实例对象
        /// </summary>
        /// <typeparam name="T">返回对象</typeparam>
        /// <param name="SQLString">Sql语句</param>
        /// <param name="readFunc">DataReader</param>
        /// <param name="parameters">参数列表</param>
        /// <param name="commandtext">命令类型</param>
        /// <returns></returns>
        public static T Find<T>(string SQLString, Func<SqlDataReader, T> readFunc, List<SqlParameter> parameters, CommandType commandtext = CommandType.Text, DataBase db = DataBase.None)
        {
            T results = default(T);
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                connection.Open();
                var command = new SqlCommand(SQLString, connection);
                command.CommandType = commandtext;
                if (parameters != null)
                    foreach (var p in parameters)
                        command.Parameters.Add(p);
                using (var reader = command.ExecuteReader())
                {
                    results = readFunc(reader);
                }
            }
            return results;
        }

        /// <summary>
        /// 返回一个实例对象
        /// </summary>
        /// <typeparam name="T">返回对象</typeparam>
        /// <param name="SQLString">Sql语句</param>
        /// <param name="readFunc">DataReader</param>
        /// <param name="parameters">参数列表</param>
        /// <param name="db">数据库名称</param>
        /// <returns></returns>
        public static T Find<T>(string SQLString, Func<SqlDataReader, T> readFunc, List<SqlParameter> parameters, DataBase db = DataBase.None)
        {
            T results = default(T);
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                connection.Open();
                var command = new SqlCommand(SQLString, connection);
                command.CommandType = CommandType.Text;
                if (parameters != null)
                    foreach (var p in parameters)
                        command.Parameters.Add(p);
                using (var reader = command.ExecuteReader())
                {
                    results = readFunc(reader);
                }
            }
            return results;
        }

        /// <summary>
        /// 执行带参数的SQL语句
        /// </summary>
        /// <param name="SQLString">Sql语句</param>
        /// <param name="readFunc">DataReader</param>
        /// <param name="parameters">参数列表</param>
        /// <param name="commandtext">命令类型</param>
        public static void FindList(string SQLString, Action<SqlDataReader> readFunc, List<SqlParameter> parameters, CommandType commandtext = CommandType.Text, DataBase db = DataBase.None)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                connection.Open();
                var command = new SqlCommand(SQLString, connection);
                command.CommandType = commandtext;
                command.CommandTimeout = CommandTimeOut;
                if (parameters != null)
                    foreach (var p in parameters)
                        command.Parameters.Add(p);
                using (var reader = command.ExecuteReader())
                {
                    readFunc(reader);
                }
            }
        }

        /// <summary>
        /// 执行带参数的SQL语句
        /// </summary>
        /// <param name="SQLString">Sql语句</param>
        /// <param name="readFunc">DataReader</param>
        /// <param name="parameters">参数列表</param>
        /// <param name="db">命令类型</param>
        public static void FindList(string SQLString, Action<SqlDataReader> readFunc, List<SqlParameter> parameters, DataBase db = DataBase.None)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                connection.Open();
                var command = new SqlCommand(SQLString, connection);
                command.CommandType = CommandType.Text;
                command.CommandTimeout = CommandTimeOut;
                if (parameters != null)
                    foreach (var p in parameters)
                        command.Parameters.Add(p);
                using (var reader = command.ExecuteReader())
                {
                    readFunc(reader);
                }
            }
        }

        #region 其它扩展公用代码
        /// <summary>
        /// 生成更新语句
        /// </summary>
        /// <param name="updateDictionary">要更新字段的字典</param>
        /// <param name="dbDataParameters">参数列表</param>
        /// <returns></returns>
        public static string ConvertSql(Dictionary<string, object> updateDictionary, List<IDbDataParameter> dbDataParameters)
        {
            StringBuilder setSql = new StringBuilder();
            if (updateDictionary != null && updateDictionary.Count > 0)
            {
                foreach (var item in updateDictionary)
                {
                    setSql.Append(",");
                    setSql.Append(item.Key);
                    setSql.Append("=");
                    setSql.Append("@");
                    setSql.Append(item.Key);
                    dbDataParameters.Add(new SqlParameter("@" + item.Key, item.Value));// 构建参数化列表
                }
            }
            string sql = string.Empty;
            if (setSql.Length > 0)
            {
                sql = setSql.Remove(0, 1).ToString();
            }
            setSql.Clear();
            return sql;
        }

        public void Dispose()
        {
            //Dispose();
        }
        #endregion

        #region 数据库批量操作
        /// <summary>
        /// 批量操作每批次记录数
        /// </summary>
        public static int BatchSize = 2000;

        /// <summary>
        /// 超时时间
        /// </summary>
        public static int CommandTimeOut = 600;
        /// <summary>
        ///大批量数据插入,返回成功插入行数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="table">数据表</param>
        /// <returns>返回成功插入行数</returns>
        public static int BulkInsert(DataTable table, DataBase db)
        {
            if (string.IsNullOrEmpty(table.TableName)) throw new Exception("请给DataTable的TableName属性附上表名称");
            if (table.Rows.Count == 0) return 0;
            int insertCount = 0;
            //string tmpPath = Path.GetTempFileName();
            //string csv = DataTableToCsv(table);
            //File.WriteAllText(tmpPath, csv);
            //using (SqlConnection conn = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            //{
            //    SqlTransaction tran = null;
            //    try
            //    {
            //        conn.Open();
            //        tran = conn.BeginTransaction();
            //        SqlBulkLoader bulk = new SqlBulkLoader(conn)
            //        {
            //            FieldTerminator = ",",
            //            FieldQuotationCharacter = '"',
            //            EscapeCharacter = '"',
            //            LineTerminator = "\n",
            //            FileName = tmpPath,
            //            NumberOfLinesToSkip = 0,
            //            TableName = table.TableName,
            //        };
            //        bulk.Columns.AddRange(table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList());
            //        insertCount = bulk.Load();
            //        tran.Commit();
            //    }
            //    catch (SqlException ex)
            //    {
            //        if (tran != null) tran.Rollback();
            //        throw ex;
            //    }
            //}
            //File.Delete(tmpPath);
            return insertCount;
        }
        /// <summary>
        ///将DataTable转换为标准的CSV
        /// </summary>
        /// <param name="table">数据表</param>
        /// <returns>返回标准的CSV</returns>
        private static string DataTableToCsv(DataTable table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else if (colum.DataType == typeof(DateTime))
                    {
                        sb.Append(Convert.ToDateTime(row[colum]).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// 使用SqlDataAdapter批量更新数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="db"></param>
        public static void BatchUpdate(DataTable table, DataBase db)
        {
            using (SqlConnection connection = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandTimeout = CommandTimeOut;
                command.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                SqlCommandBuilder commandBulider = new SqlCommandBuilder(adapter);
                commandBulider.ConflictOption = ConflictOption.OverwriteChanges;

                SqlTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    //设置批量更新的每次处理条数
                    adapter.UpdateBatchSize = BatchSize;
                    //设置事物
                    adapter.SelectCommand.Transaction = transaction;

                    if (table.ExtendedProperties["SQL"] != null)
                    {
                        adapter.SelectCommand.CommandText = table.ExtendedProperties["SQL"].ToString();
                    }
                    adapter.Update(table);
                    transaction.Commit();/////提交事务
                }
                catch (SqlException ex)
                {
                    if (transaction != null) transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }


        /// <summary> 
        /// 大批量插入数据(2000每批次) 
        /// 已采用整体事物控制 
        /// </summary> 
        /// <param name="connString">数据库链接字符串</param> 
        /// <param name="tableName">数据库服务器上目标表名</param> 
        /// <param name="dt">含有和目标数据库表结构完全一致(所包含的字段名完全一致即可)的DataTable</param> 
        public static void BulkCopy(DataTable dt, DataBase db = DataBase.None)
        {
            if (string.IsNullOrEmpty(dt.TableName)) throw new Exception("请给DataTable的TableName属性附上表名称");
            using (SqlConnection conn = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.BatchSize = 2000;
                        bulkCopy.BulkCopyTimeout = 120;
                        bulkCopy.DestinationTableName = dt.TableName;
                        try
                        {
                            foreach (DataColumn col in dt.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            bulkCopy.WriteToServer(dt);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
        }

        /// <summary> 
        /// 批量更新数据(每批次5000) 
        /// 若只是需要大批量插入数据使用bcp是最好的，若同时需要插入、删除、更新建议使用SqlDataAdapter我测试过有很高的效率，一般情况下这两种就满足需求了 
        /// </summary> 
        /// <param name="connString">数据库链接字符串</param> 
        /// <param name="dt"></param> 
        public static void BulkUpdate(DataTable dt, DataBase db = DataBase.None)
        {
            using (SqlConnection conn = (db == DataBase.None) ? new SqlConnection(connectionString) : MySelfSqlConnection(db))
            {
                SqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 120;
                comm.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter(comm);
                SqlCommandBuilder commandBulider = new SqlCommandBuilder(adapter);
                commandBulider.ConflictOption = ConflictOption.OverwriteChanges;
                try
                {
                    conn.Open();
                    //设置批量更新的每次处理条数 
                    adapter.UpdateBatchSize = 5000;
                    adapter.SelectCommand.Transaction = conn.BeginTransaction();/////////////////开始事务 
                    if (dt.ExtendedProperties["SQL"] != null)
                    {
                        adapter.SelectCommand.CommandText = dt.ExtendedProperties["SQL"].ToString();
                    }
                    adapter.Update(dt);
                    adapter.SelectCommand.Transaction.Commit();/////提交事务 
                }
                catch (Exception ex)
                {
                    if (adapter.SelectCommand != null && adapter.SelectCommand.Transaction != null)
                    {
                        adapter.SelectCommand.Transaction.Rollback();
                    }
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            };
        }
        #endregion 批量操作
    }
}
