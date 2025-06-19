using System.Collections.Generic;
namespace AmitalCloud.Infrastructure.Data.Services
{
    public class ChildEntitiesCustomFieldArgs
    {
        public int Tenant { get; set; }
        public string EntityId { get; set; }
        public string ChildEntityId { get; set; }
        public string ObjectTableName { get; set; }
        public string ChildObjectTableName { get; set; }
        public List<object> ChildEntities { get; set; }
    }
}
