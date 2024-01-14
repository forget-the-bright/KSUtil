using System;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableIgnore : Attribute
    {
        public TableIgnore() {
        }
    }

}
