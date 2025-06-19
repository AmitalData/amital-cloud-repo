using System.Runtime.Serialization;
namespace AmitalCloud.Infrastructure.Domain.EntityPMs
{
    public partial class ContactPM
    {
        public bool IsAPIContact { get; set; }
        [DataMember]
        public bool IsUserAdditionalPackagesOnly { get; set; }

        [DataMember]
        public bool IsLicencedUser { get; set; }
        public string OldSimilarInactiveContactId { get; set; }
        [DataMember]
        public string DigitalPortalCardId { get; set; }
    }
}
