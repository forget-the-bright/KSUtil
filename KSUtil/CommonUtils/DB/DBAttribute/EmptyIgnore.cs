using System;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EmptyIgnore : Attribute
    {
        public EmptyIgnore() {
        }
    }

}
