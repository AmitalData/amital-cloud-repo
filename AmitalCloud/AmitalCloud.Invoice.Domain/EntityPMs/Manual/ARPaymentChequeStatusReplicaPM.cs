using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class ARPaymentChequeStatusReplicaPM
    {

        [Key]
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string SearchFields { get; set; }
        [DataMember]
        public string LocalName { get; set; }
        [DataMember]
        public string EnglishName { get; set; }
        [DataMember]
        public bool Inactive { get; set; }

    }
}
