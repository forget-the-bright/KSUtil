using KSUtil.CommonUtils.DB.DBAttribute;

namespace System.Collections.Generic
{
    public static class IEnumerableUtil
    {
        public static string joinList<T>(this IEnumerable<T> list,bool isConcatChar = true)
        {
            string str = "";
            foreach (var item in list)
            {
                if (isConcatChar)
                {
                    str += (str == "" ? "" : ",") + "\"" + item.ToString() + "\"";
                }
                else {
                    str += (str == "" ? "" : ",") + item.ToString() ;
                }
                
            }
            return str;
        }
        public static string join<T>(this IEnumerable<T> list,string joinChar=",", bool isConcatChar = true)
        {
            string str = "";
            foreach (var item in list)
            {
                if (isConcatChar)
                {
                    str += (str == "" ? "" : joinChar) + "\"" + item.ToString() + "\"";
                }
                else
                {
                    str += (str == "" ? "" : ",") + item.ToString();
                }
            }
            return str;
        }
       
    }
}
