using System;

namespace KSUtil.CommonUtils
{
    public class TimeManagement
    {
        /// <summary>
        /// 传入时间或当前时间   返回上个月25日的时间  格式为yyyy-MM-dd
        /// </summary>
        /// <returns></returns>
        public static DateTime totime(DateTime time)
        {
            string strdd = time.ToString("dd");
            string strMM = time.ToString("MM");
            string stryyyy = time.ToString("yyyy");          
            if (Convert.ToInt32(strdd) <= 25)
            {
                if (Convert.ToInt32(strMM) - 1 >= 1)
                {
                    strMM = (Convert.ToInt32(strMM) - 1).ToString();
                    //不用减直接生成时间字段
                    return Convert.ToDateTime(stryyyy+"-"+strMM+"-"+strdd);
                }
                else
                {
                    stryyyy =( Convert.ToInt32(stryyyy) - 1).ToString();
                    //年减一  因为是1月了
                    return Convert.ToDateTime(stryyyy + "-" + "12" + "-" + strdd);
                }
            }
            else
            {               
                return Convert.ToDateTime(stryyyy + "-" + strMM + "-" + "25");
            }
        }
    }
}
