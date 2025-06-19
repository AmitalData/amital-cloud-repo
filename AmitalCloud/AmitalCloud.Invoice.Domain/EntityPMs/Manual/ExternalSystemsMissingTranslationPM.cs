using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class ExternalSystemsMissingTranslationPM
    {

        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string LogitudeTable { get; set; }
        public string LogitudeId { get; set; }
        public string ExternalCode { get; set; }
        public string Split1 { get; set; }
        public string Split2 { get; set; }
        public bool IsResolved { get; set; }

    }
}