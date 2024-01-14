using System;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotEmpty : Attribute
    {
        public NotEmpty() {
        }
    }

}
