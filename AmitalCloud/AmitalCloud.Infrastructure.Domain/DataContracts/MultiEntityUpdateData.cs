using System.Collections.Generic;
using System.Runtime.Serialization;
namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class MultiEntityUpdateData
    {
        [DataMember]
        public string ObjectTableId { get; set; }
        [DataMember]
        public string ObjectTableName { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public List<AutomationSetValue> SetValueLists { get; set; }

        [DataMember]
        public List<MultiEntityUpdateDataEntity> Entities { get; set; }
    }

    public class MultiEntityUpdateDataEntity
    {
        [DataMember]
        public string EntityId { get; set; }

        [DataMember]
        public string Tenant { get; set; }

        [DataMember]
        public string EntityNumber { get; set; }


        [DataMember]
        public bool HasException { get; set; }

        [DataMember]
        public string Exception { get; set; }

        [DataMember]
        public string StatusCode { get; set; }
    }
}
