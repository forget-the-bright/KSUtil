using System;

namespace KSUtil.CommonUtils.DB.DBAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TagField : Attribute
    {
        public string Value { get; }

        public TagField(string value)
        {
            Value = value;
        }
    }
}
