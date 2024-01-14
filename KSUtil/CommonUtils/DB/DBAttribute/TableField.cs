using System;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableField : Attribute
    {
        public string Value { get; }
        public string Describe { get; }

        public TableField(string value)
        {
            Value = value;
        }
        public TableField(string value,string describe)
        {
            Value = value;
            Describe = describe;
        }
    }

}
