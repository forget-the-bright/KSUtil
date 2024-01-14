namespace KSUtil.CommonUtils
{
    public static class RegexConstant
    {
        /// <summary>
        /// 数字正则
        /// </summary>
        public const string Number = @"^-?\d+(\.\d+)?$";
        /// <summary>
        /// 非空正则
        /// </summary>
        public const string NotEmpty = @"^.+$";
        /// <summary>
        /// 空正则
        /// </summary>
        public const string Empty = @"^$";
        /// <summary>
        /// 中国大陆手机号
        /// </summary>
        public const string ChinaPhone = @"^1[3-9]\d{9}$";
        /// <summary>
        /// 美国手机号
        /// </summary>
        public const string USPhone = @"^[2-9]\d{9}$";
        /// <summary>
        /// 印度手机号
        /// </summary>
        public const string INDPhone = @"^[6-9]\d{9}$";
        /// <summary>
        /// 英国手机号
        /// </summary>
        public const string UKPhone = @"^[7-9]\d{9}$";
        /// <summary>
        /// 邮箱正则
        /// </summary>
        public const string Mail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";


        /// <summary>
        /// 获取指定小数位的正则表达式
        /// </summary>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static string DecimalRegex(int decimalPlaces=2)
        {
            // 构建正则表达式
            string pattern = $@"^\d+(\.\d{{1,{decimalPlaces}}})?$";
            return pattern;
        }
    }
}
