using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    [DataContract(Namespace = "")]
    public class QueryFilterItem
    {
        [DataMember]
        public string FieldName { get; set; }
        [DataMember]
        public object FieldValue { get; set; }
        [DataMember]
        public object FieldValue2 { get; set; }
        [DataMember]
        public object FieldValue3 { get; set; }
        [DataMember]
        public string Operator { get; set; }
        [DataMember]
        public bool IsCustom { get; set; }
        [DataMember]
        public bool DisplayInList { get; set; }
        [DataMember]
        public bool IsCustomField { get; set; }
        [DataMember]
        public string FieldDataType { get; set; }
        [DataMember]
        public string FilterType { get; set; }
        [DataMember]
        public string DateGroupCode { get; set; }
        [DataMember]
        public bool IsListFilter { get; set; }
        [DataMember]
        public bool IsAnalyticsMetadatas { get; set; }

        [DataMember]
        public List<QueryFilterItem> QueryFilterItems { get; set; }

    }
}
