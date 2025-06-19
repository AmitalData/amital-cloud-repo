using System;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class TreeFilterQueryArgs
    {
        public string ObjectTableName { get; set; }
        public string AdditionalTreeFilter { get; set; }
        public int Tenant { get; set; }
        public string ParentEntityId { get; set; }
        public string ParentObjectTableName { get; set; }
        public object ParentEntity { get; set; }
        public Type Type { get; set; }
    }
}
