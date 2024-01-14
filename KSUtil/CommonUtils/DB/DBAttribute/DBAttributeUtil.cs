using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    public class DBAttributeUtil
    {
        /// <summary>
        /// 传实体类返回添加sql
        /// </summary>
        /// <param name="class_name"></param>
        /// <returns></returns>
        public static string GetInsertSql(object models)
        {
            //已实例化的实体用GetType，如果未实例化的需要使用typeof
            Type type = models.GetType();//new过的对象                                        
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            string field = "";//字段       
            string value = "";//数据
            DateTime minval = DateTime.Parse("1900-01-01 00:00:00");
            var ids = new List<PropertyInfo>();
            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo property = info[i];
                var tableIgnore = property.GetCustomAttribute<TableIgnore>();
                var tableId = property.GetCustomAttribute<TableId>();
                var val = property.GetValue(models);
                if (tableIgnore == null)
                {
                    if (val != null && tableId == null && !val.Equals(DateTime.MinValue) && !val.Equals(minval))//为null不填
                    {
                        var fname = getTableField(property);
                        //获取字段和值
                        field += (field.Equals("") ? "" : ",") + fname;
                        if (info[i].PropertyType == typeof(bool))
                        {
                            value += (value.Equals("") ? "" : ",") + $"{info[i].GetValue(models).ToString()}";
                        }
                        else
                        {
                            value += (value.Equals("") ? "" : ",") + $"'{info[i].GetValue(models).ToString()}'";
                        }

                    }
                    if (tableId != null)
                    {
                        ids.Add(property);
                    }

                }
            }
            if (ids.Count > 1)
            {

                foreach (var property in ids)
                {
                    var val = property.GetValue(models);
                    var fname = getTableField(property);
                    //获取字段和值
                    field += (field.Equals("") ? "" : ",") + fname;
                    value += (value.Equals("") ? "" : ",") + $"'{val?.ToString()}'";
                }
            }
            else if (ids.Count == 1)
            {
                var tableId = ids[0];
                var val = tableId.GetValue(models);
                var fname = getTableField(tableId);
                if (!string.IsNullOrEmpty(val?.ToString()) && val?.ToString() != "0")
                {
                    //获取字段和值
                    field += (field.Equals("") ? "" : ",") + fname;
                    value += (value.Equals("") ? "" : ",") + $"'{val.ToString()}'";
                }

            }
            var attribute = getTableName(type);
            string sql = "insert into " + attribute + "(" + field + ") values(" + value + ")";
            return sql;
        }


        /// <summary>
        /// 传实体类返回添加sql
        /// </summary>
        /// <param name="class_name"></param>
        /// <returns></returns>
        public static (string, Dictionary<string, string>) GetParamInsertSql(object models)
        {
            //已实例化的实体用GetType，如果未实例化的需要使用typeof
            Type type = models.GetType();//new过的对象                                        
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            string field = "";//字段       
            string value = "";//数据
            DateTime minval = DateTime.Parse("1900-01-01 00:00:00");
            var ids = new List<PropertyInfo>();
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo property = info[i];
                var tableIgnore = property.GetCustomAttribute<TableIgnore>();
                var tableId = property.GetCustomAttribute<TableId>();
                var val = property.GetValue(models);
                if (tableIgnore == null)
                {
                    if (val != null && tableId == null && !val.Equals(DateTime.MinValue) && !val.Equals(minval))//为null不填
                    {
                        var fname = getTableField(property);
                        //获取字段和值
                        field += (field.Equals("") ? "" : ",") + fname;
                        value += (value.Equals("") ? "" : ",") + $"@{fname}";//'{info[i].GetValue(models).ToString()}'
                        dict.Add($"@{fname}", info[i].GetValue(models).ToString());
                    }
                    if (tableId != null)
                    {
                        ids.Add(property);
                    }

                }
            }
            if (ids.Count > 1)
            {

                foreach (var property in ids)
                {
                    var val = property.GetValue(models);
                    var fname = getTableField(property);
                    //获取字段和值
                    field += (field.Equals("") ? "" : ",") + fname;
                    value += (value.Equals("") ? "" : ",") + $"@{fname}";//val.ToString()
                    dict.Add($"@{fname}", val.ToString());
                }
            }
            else if (ids.Count == 1)
            {
                var tableId = ids[0];
                var val = tableId.GetValue(models);
                var fname = getTableField(tableId);
                if (!string.IsNullOrEmpty(val?.ToString()) && val?.ToString() != "0")
                {
                    //获取字段和值
                    field += (field.Equals("") ? "" : ",") + fname;
                    value += (value.Equals("") ? "" : ",") + $"@{fname}"; //val.ToString()
                    dict.Add($"@{fname}", val.ToString());
                }

            }
            var attribute = getTableName(type);
            string sql = "insert into " + attribute + "(" + field + ") values(" + value + ")";
            return (sql, dict);
        }



        /// <summary>
        /// 传实体类返回修改sql
        /// </summary>
        /// <param name="class_name"></param>
        /// <returns></returns>
        public static string GetUpdateSql(object models)
        {
            //已实例化的实体用GetType，如果未实例化的需要使用typeof
            Type type = models.GetType();//new过的对象                                        
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            string setSql = "";
            PropertyInfo id = null;//id
            var ids = new List<PropertyInfo>();
            DateTime minval = DateTime.Parse("1900-01-01 00:00:00");
            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo property = info[i];
                var tableIgnore = property.GetCustomAttribute<TableIgnore>();
                var tableId = property.GetCustomAttribute<TableId>();
                var val = property.GetValue(models);
                if (tableIgnore == null)
                {
                    if (val != null && tableId == null && !val.Equals(DateTime.MinValue) && !val.Equals(minval))//为null不填
                    {
                        var fname = getTableField(property);
                        if (property.PropertyType == typeof(bool))
                        {
                            setSql += (setSql.Equals("") ? "" : ",") + $"`{fname}`={property.GetValue(models)}";
                        }
                        else
                        {
                            setSql += (setSql.Equals("") ? "" : ",") + $"`{fname}`='{property.GetValue(models).ToString()}'";
                        }


                    }
                    if (tableId != null)
                    {
                        id = property;
                        ids.Add(property);
                    }
                }
            }
            if (ids.Count == 0) throw new Exception("没有设置主键");
            var tableName = getTableName(type);
            var whereSql = "";
            foreach (var item in ids)
            {
                var idName = getTableField(item);
                var idSql = $"{idName} = '{item.GetValue(models)}'";
                whereSql += string.IsNullOrEmpty(whereSql) ? "where " + idSql : " AND " + idSql;
            }
            string sql = $"update {tableName} set {setSql} {whereSql}";// where `{getTableField(id)}`='{id.GetValue(models).ToString()}'
            return sql;
        }


        /// <summary>
        /// 传实体类返回修改sql
        /// </summary>
        /// <param name="class_name"></param>
        /// <returns></returns>
        public static (string, Dictionary<string, string>) GetParamUpdateSql(object models)
        {
            //已实例化的实体用GetType，如果未实例化的需要使用typeof
            Type type = models.GetType();//new过的对象                                        
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            string setSql = "";
            PropertyInfo id = null;//id
            var ids = new List<PropertyInfo>();
            DateTime minval = DateTime.Parse("1900-01-01 00:00:00");
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo property = info[i];
                var tableIgnore = property.GetCustomAttribute<TableIgnore>();
                var tableId = property.GetCustomAttribute<TableId>();
                var val = property.GetValue(models);
                if (tableIgnore == null)
                {
                    if (val != null && tableId == null && !val.Equals(DateTime.MinValue) && !val.Equals(minval))//为null不填
                    {
                        var fname = getTableField(property);
                        setSql += (setSql.Equals("") ? "" : ",") + $"`{fname}`=@{fname}";//property.GetValue(models).ToString()
                        dict.Add($"@{fname}", property.GetValue(models).ToString());
                    }
                    if (tableId != null)
                    {
                        id = property;
                        ids.Add(property);
                    }
                }
            }
            if (ids.Count == 0) throw new Exception("没有设置主键");
            var tableName = getTableName(type);
            var whereSql = "";
            foreach (var item in ids)
            {
                var idName = getTableField(item);
                var idSql = $"{idName} = @{idName}"; //{item.GetValue(models)}
                dict.Add($"@{idName}", item.GetValue(models).ToString());
                whereSql += string.IsNullOrEmpty(whereSql) ? "where " + idSql : " AND " + idSql;
            }
            string sql = $"update {tableName} set {setSql} {whereSql}";// where `{getTableField(id)}`='{id.GetValue(models).ToString()}'
            return (sql, dict);
        }

        public static PropertyInfo getTableIdProperty(Type type)
        {
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            TableId tableId = null;
            PropertyInfo propertyInfo = null;
            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo property = info[i];
                tableId = property.GetCustomAttribute<TableId>();
                if (tableId != null)
                {
                    return property;
                }
            }
            return propertyInfo;
        }
        public static List<PropertyInfo> getTableIdsProperty(Type type)
        {
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            TableId tableId = null;
            PropertyInfo propertyInfo = null;
            return info.Where(property =>
            {
                tableId = property.GetCustomAttribute<TableId>();
                return tableId != null;
            }).ToList();
        }

        /// <summary>
        /// 传实体类和where条件返回修改sql     fid是默认不参与当条件
        /// </summary>
        /// <param name="models"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetUpdateSql(object models, string where)
        {
            //已实例化的实体用GetType，如果未实例化的需要使用typeof
            Type type = models.GetType();//new过的对象                                        
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            string setSql = "";

            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo property = info[i];
                var tableId = property.GetCustomAttribute<TableId>();
                if (property.GetValue(models) != null && tableId == null && getTableField(property) != "fid")//为null不填
                {
                    var fname = getTableField(property);
                    setSql += (setSql.Equals("") ? "" : ",") + $"`{fname}`='{property.GetValue(models).ToString()}'";
                }
            }
            var tableName = getTableName(type);
            string sql = $"update {tableName} set {setSql} where {where}";
            return sql;
        }



        public static string getTableField(PropertyInfo propertyInfo)
        {
            return DefaultVal(ConvertAttribute(propertyInfo.GetCustomAttribute<TableField>()).Value, propertyInfo.Name);
        }

        public static string getTableName(Type type)
        {
            return DefaultVal(ConvertAttribute(type.GetCustomAttribute<TableName>()).Value, type.Name);
        }

        public static T ConvertAttribute<T>(T input)
        {
            return input == null ? getAttributeInstance<T>() : input;
        }

        private static T getAttributeInstance<T>()
        {
            // 获取 TableField 自定义属性的构造函数
            ConstructorInfo constructor = typeof(T).GetConstructor(new Type[] { typeof(string) });
            // 构造函数参数值
            object[] args = new object[] { null };
            // 通过构造函数实例化 TableField 属性的新实例
            return (T)constructor.Invoke(args);
        }
        // 范型方法
        public static T DefaultVal<T>(T input, T defalut)
        {
            // 在这里可以使用 T 类型的通用操作
            // 返回 T 类型的结果
            return input == null ? defalut : input;
        }

       
        public static T fillData<T>(DataRow dataRow)
        {
            Type type = typeof(T);//new过的对象   
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            var model = Activator.CreateInstance(type);

            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo property = info[i];
                var fname = DBAttributeUtil.getTableField(property);
                if (dataRow.Table.Columns.Contains(fname))
                {
                    var val = dataRow[fname];
                    fillModelValByProperty(model, val, property);
                }
                else if (dataRow.Table.Columns.Contains(property.Name))
                {
                    var val = dataRow[property.Name];
                    fillModelValByProperty(model, val, property);
                }
            }
            return (T)model;
        }

    


        
        /// <summary>
        /// 转换DataTable为实体类集合
        /// </summary>
        /// <typeparam name="T">实体范型</typeparam>
        /// <param name="dataTable">DataTable</param>
        /// <returns>实体类集合</returns>
        public static List<T> fillData<T>(DataTable dataTable)
        {
            //已实例化的实体用GetType，如果未实例化的需要使用typeof
            Type type = typeof(T);//new过的对象                                        
            PropertyInfo[] info = type.GetProperties(); //获取所有的字段
            var list = new List<T>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                var model = Activator.CreateInstance(type);
                foreach (DataColumn column in dataTable.Columns)
                {
                    var fname = column.ColumnName;
                    PropertyInfo property = DBBaseModel<T>.getProperty(fname);
                    var val = dataTable.Rows[j][fname];
                    fillModelValByProperty(model, val, property);
                }
                list.Add((T)model);
            }
            return list;
        }
        /// <summary>
        /// 赋值方法
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="val">值</param>
        /// <param name="property">字段</param>
        private static void fillModelValByProperty(Object model, Object val, PropertyInfo property)
        {
            if (property != null)
            {
                if (!string.IsNullOrEmpty(val?.ToString()) && val != DBNull.Value && !val.Equals(DateTime.MinValue))
                {
                    object convertedValue = null;
                    try
                    {
                        convertedValue = Convert.ChangeType(val.ToString(), property.PropertyType);
                    }
                    catch
                    {
                        convertedValue = Convert.ChangeType(val, property.PropertyType);
                    }
                    property.SetValue(model, convertedValue);
                }
            }
        }


        /// <summary>
        /// 将数据库字段转换为驼峰命名
        /// </summary>
        /// <param name="dbField"></param>
        /// <returns></returns>
        public static string ToCamelCase(string dbField)
        {
            string[] words = dbField.Split('_');
            for (int i = 1; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }
            return string.Join("", words);
        }


        /// <summary>
        /// 将驼峰命名转换为数据库字段
        /// </summary>
        /// <param name="camelCaseField"></param>
        /// <returns></returns>
        public static string ToDatabaseField(string camelCaseField)
        {
            return Regex.Replace(camelCaseField, @"(?<!^)([A-Z])", "_$1").ToLower();
        }
    }
}
