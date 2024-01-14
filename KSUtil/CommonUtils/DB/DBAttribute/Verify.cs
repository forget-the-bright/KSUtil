using System;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Verify : Attribute
    {
        public string regexStr { get; }
        public string prompt { get; }


        public Verify(string regexStr, string prompt)
        {
            this.regexStr = regexStr;
            this.prompt = prompt;
        }
    }

}
