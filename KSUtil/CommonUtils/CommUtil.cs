using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace KSUtil.CommonUtils
{
    public class CommUtil
    {
        /// <summary>
        /// 通用转换类型方法
        /// </summary>
        /// <typeparam name="T">需要转换的类型</typeparam>
        /// <param name="obj">需要转换的类型对象</param>
        /// <param name="message">如果发生出错要抛出的异常信息，为空则不抛</param>
        /// <returns>转换类型的值</returns>
        /// <exception cref="Exception"></exception>
        public static T ConvertTo<T>(Object obj, string message = "")
        {
            T convertedValue = default(T);
            try
            {
                if (obj == null)
                {
                    return default(T);
                }
                if (obj == DBNull.Value)
                {
                    return default(T);
                }
                if (string.IsNullOrEmpty(obj.ToString()))
                {
                    return default(T);
                }
                Type type = typeof(T);
                convertedValue = (T)Convert.ChangeType(obj, type);
                return convertedValue;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    throw new Exception(message);
                }
                return convertedValue;
            }
        }

        /// <summary>
        /// 计算铝卷的长度
        /// </summary>
        /// <param name="width">宽度(mm)</param>
        /// <param name="thickness">厚度(mm)</param>
        /// <param name="weight">重量(kg)</param>
        /// <returns></returns>
        public static double getLength(double width, double thickness, double weight)
        {
            double density = 2.70; // 铝的密度（单位：克/立方厘米）

            // 将宽度和厚度转换为厘米
            double widthInCentimeters = width / 10.0;
            double thicknessInCentimeters = thickness / 10.0;
            // 将密度转换为千克/立方厘米
            double densityInKgPerCm3 = density / 1000.0;

            // 计算铝卷的长度 厘米
            double length = weight / (densityInKgPerCm3 * widthInCentimeters * thicknessInCentimeters);

            return Double.IsNaN(length) ? 0 : Convert.ToInt32(length / 100);
        }


        /// <summary>
        ///  获取dataTable 对应列的数据集合
        /// </summary>
        /// <param name="data">DataTable</param>
        /// <param name="name">字段名称</param>
        /// <returns>String[]</returns>
        public static String[] DataTableConvertListByProperty(DataTable data, string name)
        {
            return data.AsEnumerable().Select(row => row[name].ToString())
                                                 .ToArray();
        }
        /// <summary>
        /// DateTime 转换格式为yyyy-MM-dd 的字符串
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <returns>String</returns>
        public static String DefaultDataStr(DateTime dateTime)
        {
            return DefaultTimeStr(dateTime, "yyyy-MM-dd");
        }
        /// <summary>
        /// DateTime 转换格式为yyyy-MM-dd HH:mm:ss的字符串
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <returns>String</returns>
        public static String DefaultDataTimeStr(DateTime dateTime)
        {
            return DefaultTimeStr(dateTime, "yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// DateTime 转换指定格式的字符串
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <param name="format">format</param>
        /// <returns>String</returns>
        public static String DefaultTimeStr(DateTime dateTime, String format)
        {
            if (dateTime.Equals(DateTime.MinValue))
            {
                return "";
            }
            return dateTime.ToString(format);
        }
        /// <summary>
        /// 判断是否时 yyyy-MM-dd 格式字符串
        /// </summary>
        /// <param name="date"> 时间字符串</param>
        /// <returns></returns>
        public static bool IsDateStr(string date)
        {
            string pattern = @"^\d{4}-\d{2}-\d{2}$";
            return Regex.IsMatch(date, pattern);
        }


    }
}
