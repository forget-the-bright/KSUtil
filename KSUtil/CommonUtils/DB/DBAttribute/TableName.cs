using System;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableName : Attribute
    {
        public string Value { get; }

        public TableName(string value)
        {
            Value = value;
        }
    }
}
