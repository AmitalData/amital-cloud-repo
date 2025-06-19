using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class APInvoiceEntityPM
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string EntityId { get; set; }
        public string EntityReference { get; set; }
        public string ObjectTableId { get; set; }
        public string APInvoiceId { get; set; }
        public int IndexOrder { get; set; }
    }
}