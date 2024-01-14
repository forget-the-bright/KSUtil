using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Xml;

namespace KSUtil.CommonUtils.DB
{
    public  class DbHelperMySQL
    {
        /// <summary>
        /// 数据访问抽象基础类
        /// Copyright (C) 2004-2008 By zzzili 
        /// </summary>

        //数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.		
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        
        public static string connectionString = GetConnectionStringFromXml("./SystemConfig.xml");////"server=127.0.0.1;port=3306;database=test;user=root;password=Sinotoon;";
        private MySqlConnection connection;
        public static ThreadLocal<DbHelperMySQL> ThreadLocalDB = new ThreadLocal<DbHelperMySQL>();
        /// <summary>
        /// DbHelperMySQL无参数构造函数
        /// </summary>
        private DbHelperMySQL(MySqlConnection connection)
        {
              this.connection = connection;
        }
        public static DbHelperMySQL getDB(MySqlConnection connection) {
            return new DbHelperMySQL(connection);
        }
        public static string GetConnectionStringFromXml(string xmlFilePath)
        {
            try
            {
                if (!Directory.Exists(xmlFilePath))
                {
                    return "";
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);
                XmlNodeList connectionNodes = xmlDoc.SelectNodes("//config");
                bool isEncode = false;
                foreach (XmlNode configNode in connectionNodes)
                {
                    string name = configNode.Attributes["name"].Value;
                    string ivalue = configNode.Attributes["ivalue"].Value;
                    if (name.Equals("IsConStringEncode")) isEncode = Convert.ToBoolean(ivalue);
                    if (name.Equals("ConnString")) { 
                        if(isEncode)
                        {
                            return DecodeConnString(ivalue);
                        }
                        return ivalue;
                    } 
                }
                throw new Exception("No connection string found in the XML file.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading XML file: " + ex.Message);
            }
        }
        #region 解密数据库连接字符串
        public static string DecodeConnString(string connstring)
        {
            string decode_connstring = "";
            string[] arr = connstring.Split(';');
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == "")
                    continue;
                if (arr[i].ToLower().Contains("password"))
                {
                    decode_connstring += arr[i].Substring(0, 9) + Decode(arr[i].Substring(9, arr[i].Length - 9)) + ";";
                }
                else
                {
                    decode_connstring += arr[i] + ";";
                }
            }
            return decode_connstring;
        }

        public static string Encode(string str)
        {
            string text = "";
            int length = str.Length;
            for (int i = 0; i < str.Length; i++)
            {
                text += (char)(str[i] + length - i * 2);
            }

            return text;
        }

        public static string Decode(string str)
        {
            string text = "";
            int length = str.Length;
            for (int i = 0; i < str.Length; i++)
            {
                text += (char)(str[i] - length + i * 2);
            }

            return text;
        }
        #endregion
        #region 公用方法
        /// <summary>
        /// 获取表中最大的Id值
        /// </summary>
        /// <param name="FieldName">列名</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        /// <summary>
        /// 返回查询是否存在
        /// </summary>
        /// <param name="strSql">要查询的Sql语句</param>
        /// <returns></returns>
        public static bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
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
        /// 返回查询是否存在
        /// </summary>
        /// <param name="strSql">查询语句</param>
        /// <param name="cmdParms">参数互列表</param>
        /// <returns></returns>
        public static bool Exists(string strSql, params MySqlParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
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
        #endregion

        #region  执行简单SQL语句

        /// <summary>
        /// 返回新增加数据的Primary Key
        /// </summary>
        /// <param name="SQLString">要执行的INSERT SQL语句</param>
        /// <param name="TableName">表名</param>
        /// <param name="FieldName">字段名</param>
        /// <returns></returns>
        public static int ExecuteSqlReturnLastRowId(string SQLString, string TableName, string FieldName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();//int rows = 
                        cmd.ExecuteNonQuery();
                        int a = GetMaxID(FieldName, TableName);
                        return a;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }


        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString)
        {
           
            if (DbHelperMySQL.ThreadLocalDB.Value != null)
            {
                return DbHelperMySQL.ThreadLocalDB.Value.executeSql(SQLString);
            }
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
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
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteParamSql((string, Dictionary<string, string>) SQLString)
        {
           
            if (DbHelperMySQL.ThreadLocalDB.Value != null)
            {
                return DbHelperMySQL.ThreadLocalDB.Value.executeParamSql(SQLString);
            }
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString.Item1, connection))
                {
                    try
                    {
                        // 遍历 Dictionary
                        foreach (KeyValuePair<string, string> kvp in SQLString.Item2)
                        {
                            cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                        }
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行插入sql语句
        /// </summary>
        /// <param name="SQLString">执行插入sql语句</param>
        /// <returns>返回id</returns>
        public static int ExecuteSqlByInsert(string SQLString)
        {
           
            if (DbHelperMySQL.ThreadLocalDB.Value != null)
            {
                return DbHelperMySQL.ThreadLocalDB.Value.executeInsertSql(SQLString);
            }
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString+ "; SELECT LAST_INSERT_ID();", connection))
                {
                    try
                    {
                        connection.Open();
                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        return id;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行插入sql语句
        /// </summary>
        /// <param name="SQLString">执行插入sql语句</param>
        /// <returns>返回id</returns>
        public static int ExecuteSqlByInsert((string,Dictionary<string,string>) paramSQLString)
        {
            
            if (DbHelperMySQL.ThreadLocalDB.Value != null)
            {
                return DbHelperMySQL.ThreadLocalDB.Value.executeInsertSql(paramSQLString);
            }
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(paramSQLString.Item1 + "; SELECT LAST_INSERT_ID();", connection))
                {
                    try
                    {
                        // 遍历 Dictionary
                        foreach (KeyValuePair<string, string> kvp in paramSQLString.Item2)
                        {
                            cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                        }

                        connection.Open();
                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        return id;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }


        /// <summary>
        /// 指定时间执行
        /// </summary>
        /// <param name="SQLString">执行语句</param>
        /// <param name="Times">时间</param>
        /// <returns></returns>
        public static int ExecuteSqlByTime(string SQLString, int Times)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
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
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static int ExecuteSqlTran(List<String> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
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
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, string content)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(SQLString, connection);
                MySql.Data.MySqlClient.MySqlParameter myParameter = new MySql.Data.MySqlClient.MySqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
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
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static object ExecuteSqlGet(string SQLString, string content)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(SQLString, connection);
                MySql.Data.MySqlClient.MySqlParameter myParameter = new MySql.Data.MySqlClient.MySqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
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
                catch (MySql.Data.MySqlClient.MySqlException e)
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
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(strSQL, connection);
                MySql.Data.MySqlClient.MySqlParameter myParameter = new MySql.Data.MySqlClient.MySqlParameter("@fs", SqlDbType.Image);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
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

        //执行单条插入语句，并返回id，不需要返回id的用ExceuteNonQuery执行。
        /// <summary>
        /// 执行插入语句并返回Id，不需返回Id的用其它执行方法
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="parameters">此参数为Null时进行</param>
        /// <returns></returns>
        public static int ExecuteInsert(string sql, MySqlParameter[] parameters)
        {
            //Debug.WriteLine(sql);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                try
                {
                    connection.Open();
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = @"select LAST_INSERT_ID()";
                    //此方法估计也可直接用  select   @@identity 获得，两值基本都是全局服务，使用时不能关闭Connection
                    int value = Int32.Parse(cmd.ExecuteScalar().ToString());
                    return value;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            
            if (DbHelperMySQL.ThreadLocalDB.Value != null)
            {
                return DbHelperMySQL.ThreadLocalDB.Value.getSingle(SQLString);
            }
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
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
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        public  object getSingle(string SQLString)
        {
            using (MySqlCommand cmd = new MySqlCommand(SQLString, this.connection))
            {
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
        }


        /// <summary>
        /// 获取单一值
        /// </summary>
        /// <param name="SQLString">Sql语句</param>
        /// <param name="Times">时间</param>
        /// <returns>Object类型</returns>
        public static object GetSingle(string SQLString, int Times)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
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
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回MySqlDataReader ( 注意：调用该方法后，一定要对MySqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(string strSQL)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(strSQL, connection);
            try
            {
                connection.Open();
                MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public static DataTable Query(string SQLString)
        {
          
            if (DbHelperMySQL.ThreadLocalDB.Value != null)
            {
               return DbHelperMySQL.ThreadLocalDB.Value.query(SQLString);
            }
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                    dt = ds.Tables[0];
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return dt;
            }
        }

        /// <summary>
        /// 执行事务委托lambda语句，参数是DbHelperMySQL对象,通过执行对象的方法可以在事务的控制内
        /// </summary>
        /// <param name="transactionAction">lambda语句</param>
        /// <returns>void</returns>
        public static void ExecuteTransaction(Action<DbHelperMySQL> transactionAction)
        {

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        transactionAction.Invoke(new DbHelperMySQL(connection));
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Transaction failed: " + ex.Message);
                    }
                   
                }
               // connection.Close();
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(MySqlConnection connection, string SQLString)
        {
            using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
            {
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int executeSql(string SQLString)
        {
            using (MySqlCommand cmd = new MySqlCommand(SQLString, this.connection))
            {
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
        }  
        
        
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int executeParamSql((string, Dictionary<string, string>) SQLString)
        {
            using (MySqlCommand cmd = new MySqlCommand(SQLString.Item1, this.connection))
            {                // 遍历 Dictionary
                foreach (KeyValuePair<string, string> kvp in SQLString.Item2)
                {
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
        }    
        
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int executeInsertSql(string SQLString)
        {
            using (MySqlCommand cmd = new MySqlCommand(SQLString+ "; SELECT LAST_INSERT_ID();", this.connection))
            {
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                return id;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int executeInsertSql((string, Dictionary<string, string>) paramSQLString)
        {
            using (MySqlCommand cmd = new MySqlCommand(paramSQLString.Item1 + "; SELECT LAST_INSERT_ID();", this.connection))
            {
                // 遍历 Dictionary
                foreach (KeyValuePair<string, string> kvp in paramSQLString.Item2)
                {
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }
                
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                return id;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public DataTable query(string SQLString)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                command.Fill(ds, "ds");
                dt = ds.Tables[0];
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return dt;
        }

        /// <summary>
        /// 执行语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">Sql语句</param>
        /// <param name="Times">时间</param>
        /// <returns></returns>
        public static DataSet Query(string SQLString, int Times)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.SelectCommand.CommandTimeout = Times;
                    command.Fill(ds, "ds");
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="cmdParms">准备好的参数列表</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的MySqlParameter[]）</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
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
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的MySqlParameter[]）</param>
        public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int indentity = 0;
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (MySqlParameter q in cmdParms)
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
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <param name="cmdParms">准备好的参数列表</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
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
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        ///  执行查询语句，返回MySqlDataReader ( 注意：调用该方法后，一定要对MySqlDataReader进行Close )
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <param name="cmdParms">参数列表</param>
        /// <returns></returns>
        public static MySqlDataReader ExecuteReader(string SQLString, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                throw e;
            }
            //			finally
            //			{
            //				cmd.Dispose();
            //				connection.Close();
            //			}	

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <param name="cmdParms">参数列表</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// 准备命令语句
        /// </summary>
        /// <param name="cmd">Command命令</param>
        /// <param name="conn">Connection对象</param>
        /// <param name="trans">是否是事务</param>
        /// <param name="cmdText">语句</param>
        /// <param name="cmdParms">参数列表</param>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
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
                foreach (MySqlParameter parameter in cmdParms)
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

        #endregion

        #region 参数转换
        /// <summary>
        /// 准备MysqlCommad参数
        /// </summary>
        /// <param name="cmd">MySqlCommand cmd对象</param>
        /// <param name="conn">MySqlConnection conn对象</param>
        /// <param name="trans">数据库事务</param>
        /// <param name="cmdType">text or StoredProcedure</param>
        /// <param name="cmdText">存储过程名</param>
        /// <param name="cmdParms">SqlCommand的参数</param>
        private static void PrepareMySqlCommand(MySql.Data.MySqlClient.MySqlCommand cmd, MySql.Data.MySqlClient.MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
        {
            //连接状态是否打开
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            //Transact-SQL 事务
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        /// <summary>
        /// 放回一个MakeMySqlParameter
        /// </summary>
        /// <param name="name">参数名字</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数大小</param>
        /// <param name="value">参数值</param>
        /// <returns>SQLiteParameter的值</returns>
        public static MySqlParameter MakeMySqlParameter(string name, MySqlDbType type, int size, object value)
        {
            MySqlParameter parm = new MySqlParameter(name, type, size);
            parm.Value = value;
            return parm;
        }
        /// <summary>
        /// 创建MysqlParameter
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="type">数据类型</param>
        /// <param name="value">字段值</param>
        /// <returns>返回类型为MySqlParameter</returns>
        public static MySqlParameter MakeMySqlParameter(string name, MySqlDbType type, object value)
        {
            MySqlParameter parm = new MySqlParameter(name, type);
            parm.Value = value;
            return parm;
        }
        #endregion

        #region C#调用Mysql的存储过程--自修改
        #region 简单存储过程，返回结果中第一行第一列的值，要求第一行第一列有返回
        /// <summary>
        /// 简单存储过程，返回结果中第一行第一列的值，要求第一行第一列有返回
        /// </summary>
        /// <param name="ProcName">存储过程名</param>
        /// <returns></returns>
        public static int ExecuteProc(string ProcName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(ProcName, connection))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        connection.Open();//int rows = 
                        int result = int.Parse(cmd.ExecuteScalar().ToString());
                        return result;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        #endregion

        #region 执行带参数的存储过程
        /// <summary>
        /// 执行带参数的存储过程
        /// </summary>
        /// <param name="ProcName">存储过程名</param>
        /// <param name="cmdParms">参数MySqlParameter[] cmdParms</param>
        /// <returns></returns>
        public static string ExecuteProc(string ProcName, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.CommandText = ProcName;
                        PrepareMySqlCommand(cmd, connection, null, CommandType.StoredProcedure, ProcName, cmdParms);
                        // PrepareCommand(cmd, connection, null, ProcName, cmdParms);
                        //connection.Open();//int rows = 
                        string result = cmd.ExecuteScalar().ToString();
                        return result;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        #endregion

        #region 执行带参数的存储过程(返回DateSet)
        /// <summary>
        /// 执行带参数的存储过程
        /// </summary>
        /// <param name="ProcName">存储过程名</param>
        /// <param name="cmdParms">参数MySqlParameter[] cmdParms</param>
        /// <returns></returns>
        public static DataSet ExecuteProcDataset(string ProcName, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareMySqlCommand(cmd, connection, null, CommandType.StoredProcedure, ProcName, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }


        public static DataSet ExecuteProcDataset(string ProcName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareMySqlCommand(cmd, connection, null, CommandType.StoredProcedure, ProcName, null);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }
        #endregion
        #endregion
    }
}
