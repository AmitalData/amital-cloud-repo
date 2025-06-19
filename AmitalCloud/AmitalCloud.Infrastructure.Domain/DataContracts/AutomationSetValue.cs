using System.Runtime.Serialization;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{

    [DataContract(Namespace = "")]
    public class AutomationSetValue
    {

        [DataMember]
        public string OperatorCode { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string ObjectFieldId { get; set; }

        [DataMember]
        public string FieldName { get; set; }

        [DataMember]
        public string DataTypeCode { get; set; }


        [DataMember]
        public string ObjectFieldCode { get; set; }
        [DataMember]
        public bool IsCustomField { get; set; }


        [DataMember]
        public string PartnerObjectFieldCode { get; set; }

    }
}
