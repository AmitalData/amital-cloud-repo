using AmitalCloud.Infrastructure.Model.Enums;
using System;

namespace AmitalCloud.Infrastructure.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DataBaseAttribute : Attribute
    {
        public DataBaseAttribute(AmitalCloudDBSchema name)
        {
            Name = name;
        }

        public AmitalCloudDBSchema Name { get; private set; }
    }
}