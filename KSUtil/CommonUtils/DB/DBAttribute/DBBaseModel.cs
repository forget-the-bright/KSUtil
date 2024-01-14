using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    public class DBBaseModel<T> : BaseModel
    {
        private static Type type;
        private static PropertyInfo tableId;
        private static List<PropertyInfo> tableIds;
        public static String tableName;
        private Dictionary<string, object> sqlfieldDict;
        private Dictionary<string, object> fieldDict;
        private static Dictionary<string, PropertyInfo> propertyDict;
        private static Dictionary<string, PropertyInfo> sqlPropertyDict;
        static DBBaseModel()
        {
            //已实例化的实体用GetType，如果未实例化的需要使用typeof
            type = typeof(T);//new过的对象    
            tableId = DBAttributeUtil.getTableIdProperty(type);
            tableIds = DBAttributeUtil.getTableIdsProperty(type);
            tableName = DBAttributeUtil.getTableName(type);
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            propertyDict = new Dictionary<string, PropertyInfo>();
            sqlPropertyDict = new Dictionary<string, PropertyInfo>();
            foreach (var item in info)
            {
                sqlPropertyDict.Add(DBAttributeUtil.getTableField(item), item);
                propertyDict.Add(item.Name, item);
            }
        }
        /// <summary>
        /// 根据表达式树获取字段名
        /// </summary>
        /// <param name="expression">表达式树</param>
        /// <returns></returns>
        private static string GetPropertyName(Expression<Func<T, object>> expression)
        {
            MemberExpression memberExpression;
            if (expression.Body is UnaryExpression unaryExpression)
            {
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)expression.Body;
            }
            return memberExpression.Member.Name;
        }
        /// <summary>
        /// 根据实体字段名获取sql字段名
        /// </summary>
        /// <param name="feildName">实体字段名</param>
        /// <returns></returns>
        public static string getSqlFeildName(string feildName)
        {
            var propertyInfo = getProperty(feildName);
            if (propertyInfo != null)
            {
                return DBAttributeUtil.getTableField(propertyInfo);
            }
            return feildName;
        }
        /// <summary>
        /// 根据表达式树获取sql字段名
        /// </summary>
        /// <param name="expression">表达式树</param>
        /// <returns></returns>
        public static string getSqlFeildName(Expression<Func<T, object>> expression)
        {
            var feildName = GetPropertyName(expression);
            return getSqlFeildName(feildName);
        }
        public static string getTableSqlFeildName(Expression<Func<T, object>> expression)
        {
            var feildName = GetPropertyName(expression);
            return getTableSqlFeildName(feildName);
        }
        public static string getTableSqlFeildName(string feildName)
        {
            var propertyInfo = getProperty(feildName);
            if (propertyInfo != null)
            {
                return DBAttributeUtil.getTableField(propertyInfo);
            }
            return $"{tableName}.{feildName}";
        }

        #region 验证方法
        /// <summary>
        /// 验证当前实体中所有标注NotEmpty的字段如果为空就提示
        /// </summary>
        /// <returns>为空返回false 不为空返回true</returns>
        public Boolean VerifyFieldNotEmpty()
        {
            foreach (var item in propertyDict.Values)
            {
                NotEmpty notEmpty = item.GetCustomAttribute<NotEmpty>();
                if (notEmpty != null)
                {
                    TableField tableField = item.GetCustomAttribute<TableField>();
                    var desc = tableField == null ? item.Name : tableField.Describe;
                    var objVal = item.GetValue(this);
                    if (objVal == null || objVal.ToString().Equals("") || objVal.ToString().Equals("0") || objVal.Equals(DateTime.MinValue))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 验证当前传入字段如果标注NotEmpty注解值如果为空就提示
        /// </summary>
        /// <returns>为空返回false 不为空返回true</returns>
        public Boolean VerifyFieldNotEmpty(string field)
        {
            var item = getProperty(field);
            if (item != null)
            {
                NotEmpty notEmpty = item.GetCustomAttribute<NotEmpty>();
                if (notEmpty != null)
                {
                    TableField tableField = item.GetCustomAttribute<TableField>();
                    var desc = tableField == null ? item.Name : tableField.Describe;
                    var objVal = item.GetValue(this);
                    if (objVal == null || objVal.ToString().Equals("") || objVal.ToString().Equals("0") || objVal.Equals(DateTime.MinValue))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 验证当前实体中所有标注EmptyIgnore外的字段如果为空就提示
        /// </summary>
        /// <returns>为空返回false 不为空返回true</returns>
        public Boolean VerifyFieldEmptyIgnore()
        {
            foreach (var item in propertyDict.Values)
            {
                EmptyIgnore emptyIgnore = item.GetCustomAttribute<EmptyIgnore>();
                if (emptyIgnore == null)
                {
                    TableField tableField = item.GetCustomAttribute<TableField>();
                    var desc = tableField == null ? item.Name : tableField.Describe;
                    var objVal = item.GetValue(this);
                    if (objVal == null || objVal.ToString().Equals("") || objVal.ToString().Equals("0") || objVal.Equals(DateTime.MinValue))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 验证当前实体中所有标注Verify的字段
        /// </summary>
        /// <returns>正则校验通过返回true，反正false</returns>
        public Boolean VerifyField(String field)
        {
            Verify verify = getVerifyByField(field);
            if (verify != null)
            {
                PropertyInfo value = getProperty(field);
                if (!Regex.IsMatch(value.GetValue(this).ToString(), verify.regexStr))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 获取该字段上的Verify注解
        /// </summary>
        /// <param name="field">字段名称</param>
        /// <returns>正则校验通过返回true，反正false</returns>
        public static Verify getVerifyByField(String field)
        {
            PropertyInfo value = getProperty(field);
            if (value != null)
            {
                return value.GetCustomAttribute<Verify>();
            }
            return null;
        }
        #endregion
        #region 数据库原生操作方法
        /// <summary>
        /// 事务方法，在此方法中使用DBBaseModel以及其派生类操作的数据库都有事务
        /// </summary>
        /// <param name="action">回调函数</param>
        /// <exception cref="Exception"></exception>
        public static void TransactionMethod(Action action, Action afterAction = null)
        {
            if (DbHelperMySQL.ThreadLocalDB.Value != null)
            {
                action();
                return;
            }
            using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        DbHelperMySQL.ThreadLocalDB.Value = DbHelperMySQL.getDB(connection);
                        action();
                        transaction.Commit();
                        if (afterAction != null) afterAction();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Transaction failed: " + ex.Message, ex);
                    }
                    finally
                    {
                        DbHelperMySQL.ThreadLocalDB.Value = null;
                    }
                }
            }
        }

        public static void TransactionMethod(Func<Action> action)
        {
            if (DbHelperMySQL.ThreadLocalDB.Value != null)
            {
                action();
                return;
            }
            using (MySqlConnection connection = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        DbHelperMySQL.ThreadLocalDB.Value = DbHelperMySQL.getDB(connection);
                        Action afterAction = action();
                        transaction.Commit();
                        if (afterAction != null) afterAction();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Transaction failed: " + ex.Message, ex);
                    }
                    finally
                    {
                        DbHelperMySQL.ThreadLocalDB.Value = null;
                    }
                }
            }
        }


        /// <summary>
        /// 查询方法DBBaseModel 如果当前DbHelperMySQL.ThreadLocalDB有对象 会用其对象查询 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable Query(string sql)
        {
            return DbHelperMySQL.Query(sql);
        }
        /// <summary>
        /// 执行方法DBBaseModel 如果当前DbHelperMySQL.ThreadLocalDB有对象 会用其对象执行
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteSql(string sql)
        {
            return DbHelperMySQL.ExecuteSql(sql);
        }

        /// <summary>
        /// 执行方法DBBaseModel 如果当前DbHelperMySQL.ThreadLocalDB有对象 会用其对象执行
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteParamSql((string, Dictionary<string, string>) sql)
        {
            return DbHelperMySQL.ExecuteParamSql(sql);
        }
        #endregion
        #region CRUD数据操作


        public static T getOne(String whereSql)
        {
            if (String.IsNullOrEmpty(whereSql))
            {
                throw new ArgumentNullException("筛选条件为空");
            }
            DataTable dataTable = Query($"select * from {tableName} where {whereSql}");
            if (dataTable.Rows.Count > 1)
            {
                throw new Exception("查询到多条数据");
            }
            else if (dataTable.Rows.Count == 0)
            {
                return default(T);
            }
            return DBAttributeUtil.fillData<T>(dataTable)[0];
        }
        public static List<T> getList(String whereSql = "")
        {
            if (!String.IsNullOrEmpty(whereSql))
            {
                whereSql = $"where {whereSql}";
            }
            DataTable dataTable = Query($"select * from {tableName} {whereSql}");
            return DBAttributeUtil.fillData<T>(dataTable);
        }
        public static List<T> getListPage(String whereSql, int pageSize = 10, int PageNo = 1)
        {
            if (String.IsNullOrEmpty(whereSql))
            {
                throw new ArgumentNullException("筛选条件为空");
            }
            DataTable dataTable = Query($"select * from {tableName} where {whereSql} limit {(1 - PageNo) * pageSize},{pageSize}");
            return DBAttributeUtil.fillData<T>(dataTable);
        }
        public static T getById(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return default(T);
                //throw new ArgumentNullException("id不能为空");
            }
            if (tableId == null)
            {
                throw new Exception("当前表未设置主键");
            }
            DataTable dataTable = Query($"select * from {tableName} where {DBAttributeUtil.getTableField(tableId)}='{id}'  limit 1");
            if (dataTable.Rows.Count > 1)
            {
                throw new Exception("查询到多条数据");
            }
            else if (dataTable.Rows.Count == 0)
            {
                return default(T);
            }
            return DBAttributeUtil.fillData<T>(dataTable)[0];
        }

        public static T getOneByProperty(Expression<Func<T, object>> expression, String val)
        {
            var propertyName = getSqlFeildName(expression);
            if (String.IsNullOrEmpty(val))
            {
                return default(T);
            }

            DataTable dataTable = Query($"select * from {tableName} where `{propertyName}`='{val}' limit 1");
            if (dataTable.Rows.Count > 1)
            {
                throw new Exception("查询到多条数据");
            }
            else if (dataTable.Rows.Count == 0)
            {
                return default(T);
            }
            return DBAttributeUtil.fillData<T>(dataTable)[0];
        }
        public static List<T> getListByProperty(Expression<Func<T, object>> expression, String val)
        {
            var propertyName = getSqlFeildName(expression);
            if (String.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException("值不能为空");
            }

            DataTable dataTable = Query($"select * from {tableName} where `{propertyName}`='{val}'");

            return DBAttributeUtil.fillData<T>(dataTable);
        }
        public static T getById(DBBaseModel<T> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("对象不能为空");
            }
            if (tableIds.Count == 0)
            {
                throw new Exception("当前表未设置主键");
            }
            var whereSql = "";
            foreach (var item in tableIds)
            {
                var idName = DBAttributeUtil.getTableField(item);
                var sql = $"{idName} = '{(obj).GetValue(idName)}'";
                whereSql += string.IsNullOrEmpty(whereSql) ? "where " + sql : " AND " + sql;
            }
            DataTable dataTable = Query($"select * from {tableName}  {whereSql}");
            if (dataTable.Rows.Count > 1)
            {
                throw new Exception("查询到多条数据");
            }
            else if (dataTable.Rows.Count == 0)
            {
                return default(T);
            }
            return DBAttributeUtil.fillData<T>(dataTable)[0];
        }
        public static int insert(DBBaseModel<T> t, bool isParam = false)
        {
            if (t == null)
            {
                throw new ArgumentNullException("对象不能为空");
            }

            t.SetVal("create_time", DateTime.Now);
            t.SetVal("CreateTime", DateTime.Now);

            var idval = 0;
            if (isParam)
            {
                idval = DbHelperMySQL.ExecuteSqlByInsert(DBAttributeUtil.GetParamInsertSql(t));
            }
            else
            {
                idval = DbHelperMySQL.ExecuteSqlByInsert(DBAttributeUtil.GetInsertSql(t));
            }

            if (tableIds.Count == 1 && tableId != null)
            {
                t.SetVal(tableId.Name, idval);
            }
            return idval;
        }
        public int insertOrUpdate(bool isParam = false)
        {
            return insertOrUpdate(this, isParam);
        }
        public static int insertOrUpdate(DBBaseModel<T> t, bool isParam = false)
        {
            if (t == null)
            {
                throw new ArgumentNullException("对象不能为空");
            }
            if (tableIds.Count == 0)
            {
                throw new Exception("当前表未设置主键");
            }
            t.refreshModel();
            var item = getById(t);
            if (item != null)
            {
                return updateById(t, isParam);
            }
            else
            {
                return insert(t, isParam);
            }
        }

        public int insert(bool isParam = false)
        {
            return insert(this, isParam);
        }
        public static int updateById(DBBaseModel<T> t, bool isParam = false)
        {
            if (t == null)
            {
                throw new ArgumentNullException("对象不能为空");
            }
            t.SetVal("update_time", DateTime.Now);
            t.SetVal("UpdateTime", DateTime.Now);
            if (isParam)
            {
                return ExecuteParamSql(DBAttributeUtil.GetParamUpdateSql(t));
            }
            return ExecuteSql(DBAttributeUtil.GetUpdateSql(t));
        }
        public int updateById(bool isParam = false)
        {
            return updateById(this, isParam);
        }
        public static int deleteById(DBBaseModel<T> t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("对象不能为空");
            }
            if (tableIds.Count == 0)
            {
                throw new Exception("当前表未设置主键");
            }
            var whereSql = "";
            foreach (var item in tableIds)
            {
                var idName = DBAttributeUtil.getTableField(item);
                var sql = $"{idName} = '{(t).GetValue(idName)}'";
                whereSql += string.IsNullOrEmpty(whereSql) ? "where " + sql : " AND " + sql;
            }
            // where {DBAttributeUtil.getTableField(tableId)} = '{tableId.GetValue(t).ToString()}'
            return ExecuteSql($"delete from {tableName} {whereSql}");
        }
        public static int deleteById(Object id)
        {
            if (tableIds.Count == 0)
            {
                throw new Exception("当前表未设置主键");
            }
            return ExecuteSql($"delete from {tableName} where {DBAttributeUtil.getTableField(tableId)} = '{id}'");
        }
        public int deleteById()
        {
            if (tableIds.Count == 0)
            {
                throw new Exception("当前表未设置主键");
            }
            return deleteById(this);
        }
        #endregion
        #region 数据格式转换
        /// <summary>
        /// 刷新sql字段值字典 和 实体字典值字典
        /// </summary>
        public void refreshModel()
        {
            this.convertDict();
            this.convertSqlDict();
        }
        /// <summary>
        /// bean拷贝，对应字段名的值拷贝到新的实体里面
        /// </summary>
        /// <typeparam name="X">新的实体类型</typeparam>
        /// <param name="obj">被拷贝的实体对象</param>
        /// <returns></returns>
        public static T beanCopy<X>(DBBaseModel<X> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("对象不能为空");
            }
            obj.refreshModel();
            var model = Activator.CreateInstance(type);
            foreach (var item in sqlPropertyDict)
            {
                var objVal = obj.get(item.Key);
                if (objVal != null)
                {
                    (model as DBBaseModel<T>).SetVal(item.Key, objVal);
                }
            }
            return (T)model;
        }
      
        

        /// <summary>
        /// 实体 转换 dgvRow
        /// </summary>
        /// <param name="dataGridViewRow"></param>
        /// <returns></returns>
        public void convertToDataTableRow(DataTable dataTable)
        {
            // 创建一个新的数据行
            DataRow newRow = dataTable.NewRow();

            // 获取 DataRow 中的列名
            foreach (DataColumn column in dataTable.Columns)
            {
                string columnName = column.ColumnName;
                PropertyInfo propInfo = DBBaseModel<T>.getProperty(columnName);
                if (propInfo != null)
                {
                    object propValue = propInfo.GetValue(this);
                    if (propValue != null)
                        newRow[columnName] = propValue;
                }
            }
            // 将新行添加到 DataTable
            dataTable.Rows.Add(newRow);
        }

        /// <summary>
        /// 将数据修改到DataRow
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="dr"></param>
        public void updateToDataRow(DataTable dataTable, DataRow dr)
        {
            // 获取 DataRow 中的列名
            foreach (DataColumn column in dataTable.Columns)
            {
                string columnName = column.ColumnName;
                PropertyInfo propInfo = DBBaseModel<T>.getProperty(columnName);
                if (propInfo != null)
                {
                    object propValue = propInfo.GetValue(this);
                    if (propValue != null)
                        dr[columnName] = propValue;
                }
            }
        }

        /// <summary>
        /// 实体 转换 dgvRow
        /// </summary>
        /// <param name="dataGridViewRow"></param>
        /// <returns></returns>
        public static T convertDataRow(DataRow dataRow)
        {
            DataColumnCollection columns = dataRow.Table.Columns;
            T model = (T)Activator.CreateInstance(type);
            // 获取 DataRow 中的列名
            foreach (DataColumn column in columns)
            {
                string columnName = column.ColumnName;
                PropertyInfo propertyInfo = DBBaseModel<T>.getProperty(columnName);
                if (propertyInfo != null)
                {
                    var val = dataRow[columnName];
                    if (val != null && val != DBNull.Value && !val.Equals(DateTime.MinValue))
                    {
                        var convertedValue = Convert.ChangeType(val, propertyInfo.PropertyType);
                        propertyInfo.SetValue(model, convertedValue);
                    }
                }
            }
            return model;
            // 将新行添加到 DataTable
            //dataTable.Rows.Add(newRow);
        }

        /// <summary>
        /// 根据模板表单输入框的值回写到实体类面
        /// </summary>
        /// <param name="dict">模板表单输入框的值字典</param>
        public void fillValByDict(Dictionary<string, (string, string)> dict)
        {
            foreach (var item in dict)
            {
                //Text
                var value1 = item.Value.Item1;
                //Tag
                var value2 = item.Value.Item2;
                //判定有没有指定TagName注解
                PropertyInfo propertyInfo = getProperty(item.Key);
                if (propertyInfo != null)
                {
                    var tagField = propertyInfo.GetCustomAttribute<TagField>();
                    if (tagField != null)
                    {   //给TagName注解指定的字段附上Text值
                        this.SetVal(tagField.Value, value1);
                        //给本身附上Tag值
                        this.SetVal(item.Key, value2);
                    }
                    else
                    {
                        //Tag不为空 就用Tag值 为空就用Text值
                        var value = string.IsNullOrEmpty(value2) ? value1 : value2;
                        this.SetVal(item.Key, value);
                    }
                }
            }
        }
        /// <summary>
        /// 根据字典 将key 和 实体字段名称或数据库字段名 赋值
        /// </summary>
        /// <param name="dict"></param>
        public void fillValByDict(Dictionary<string, string> dict)
        {
            foreach (var item in dict)
            {
                this.SetVal(item.Key, item.Value);
            }
        }
        public Dictionary<string, object> convertSqlDict()
        {
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            var result = new Dictionary<string, object>();
            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo property = info[i];
                var fname = DBAttributeUtil.getTableField(property);
                result.Add(fname, property.GetValue(this));
            }
            this.sqlfieldDict = result;
            return result;
        }
        public Dictionary<string, object> convertDict()
        {
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            var result = new Dictionary<string, object>();
            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo property = info[i];
                var fname = property.Name;
                result.Add(fname, property.GetValue(this));
            }
            this.fieldDict = result;
            return result;
        }
        #endregion
        #region 值获取操作
        public static PropertyInfo getProperty(String field)
        {
            PropertyInfo value = null;
            if (!sqlPropertyDict.TryGetValue(field, out value))
            {
                propertyDict.TryGetValue(field, out value);
            }
            return value;
        }
        public object get(string key)
        {
            Object value;
            if (this.sqlfieldDict == null)
            {
                convertSqlDict();
            }
            if (!this.sqlfieldDict.TryGetValue(key, out value))
            {
                if (this.fieldDict == null)
                {
                    convertDict();
                }
                this.fieldDict.TryGetValue(key, out value);
            }
            return value;
        }
        public Int64 getInt(string key)
        {
            return Convert.ToInt64(get(key));
        }
        public decimal getDecimal(string key)
        {
            return Convert.ToDecimal(get(key));
        }
        public String GetStr(string key)
        {
            return get(key) as string;
        }
        public (String, String) GetTagStr(string key)
        {
            var val = this.GetValue(key);
            if (val != null)
            {
                if (val is DateTime dateTime)
                {
                    if (dateTime.Equals(DateTime.MinValue)) return ("", "");
                    else return (dateTime.ToString(), "");
                }
                else
                {
                    //获取标签指定的文本值
                    var tagNameValue = this.GetTagNameValue(key);
                    if (tagNameValue != null)
                    {
                        return (tagNameValue.ToString(), val.ToString());
                    }
                    else
                    {
                        return (val.ToString(), "");
                    }
                }
            }
            return ("", "");
        }
        public (String, String) GetTagStr(Expression<Func<T, object>> expression)
        {
            var key = getSqlFeildName(expression);
            var val = this.GetValue(key);
            if (val != null)
            {
                if (val is DateTime dateTime)
                {
                    if (dateTime.Equals(DateTime.MinValue)) return ("", "");
                    else return (dateTime.ToString(), "");
                }
                else
                {
                    var tagNameValue = this.GetTagNameValue(key);
                    if (tagNameValue != null)
                    {
                        return (tagNameValue.ToString(), val.ToString());
                    }
                    else
                    {
                        return (val.ToString(), "");
                    }
                }
            }
            return ("", "");
        }
        public String GetDataStr(string key, string format = "yyyy-MM-dd")
        {
            object val = get(key);
            if (val != null && val is DateTime)
            {
                return CommUtil.DefaultTimeStr((DateTime)val, format);
            }
            return "";
        }
        public String GetDataTimeStr(string key)
        {
            return GetDataStr(key, "yyyy-MM-dd HH:mm:ss");
        }
        public Object GetValue(string key)
        {
            return get(key);
        }
        public Object GetTagNameValue(string key)
        {
            PropertyInfo propertyInfo = getProperty(key);
            var tagField = propertyInfo.GetCustomAttribute<TagField>();
            if (tagField != null)
            {
                return get(tagField.Value);
            }
            return null;
        }
        #endregion
        #region 赋值操作
        public void SetVal(string key, object value, bool isVerify = false)
        {
            if (isVerify && !(VerifyField(key) && VerifyFieldNotEmpty(key)))
            {
                return;
            }
            var property = getProperty(key);
            if (property != null && value != null)
            {
                var typeName = property.PropertyType.Name;
                object convertedValue = null;
                try
                {
                    if (string.IsNullOrEmpty(value?.ToString()))
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            convertedValue = value?.ToString();
                        }
                        else
                        {
                            convertedValue = Activator.CreateInstance(property.PropertyType);
                        }
                    }
                    else
                    {
                        convertedValue = Convert.ChangeType(value.ToString(), property.PropertyType);
                    }
                }
                catch (Exception ex)
                {
                    if (!(ex is MissingMethodException)) {
                        convertedValue = Activator.CreateInstance(property.PropertyType);
                    }
                }
                property.SetValue(this, convertedValue);
            }

        }
        #endregion
        #region 构建流水数据库操作
        public static Wrapper<T> Select()
        {
            return new Wrapper<T>();
        }
        public static UpdateWrapper<T> Update()
        {
            return new UpdateWrapper<T>();
        }
        public static DeleteWrapper<T> Delete()
        {
            return new DeleteWrapper<T>();
        }
        #endregion

    }
}
