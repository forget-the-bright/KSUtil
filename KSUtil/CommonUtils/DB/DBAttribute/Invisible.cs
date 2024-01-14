using System;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Invisible : Attribute
    {
        public string[] Values;
        public Invisible(params string[] values) 
        {
            Values = values;
        }
        public Invisible()
        {

        }
    }
}
