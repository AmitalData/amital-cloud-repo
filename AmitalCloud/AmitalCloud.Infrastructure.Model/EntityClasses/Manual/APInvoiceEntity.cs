
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class APInvoiceEntity
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string EntityId { get; set; }
        public string EntityReference { get; set; }
        public string ObjectTableId { get; set; }
        public string APInvoiceId { get; set; }
        public int IndexOrder { get; set; }

        [ForeignKey("ObjectTableId")]
        public virtual ObjectTable ObjectTable { get; set; }
        [ForeignKey("APInvoiceId")]
        public virtual APInvoice APInvoice { get; set; }
    }
}