using System;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableId : Attribute
    {
        public string Value { get; }

        public TableId(string value)
        {
            Value = value;
        }
        public TableId() {
            Value = null;
        }
    }

}
