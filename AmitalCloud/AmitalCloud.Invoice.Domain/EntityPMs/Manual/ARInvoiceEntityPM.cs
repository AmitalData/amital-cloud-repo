using AmitalCloud.Infrastructure.Model.EntityClasses;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class ARInvoiceEntityPM
    {
        private ARInvoiceEntityPM a;

        public ARInvoiceEntityPM(ARInvoice a)
        {

        }

        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string ARInvoiceId { get; set; }
        public string EntityId { get; set; }
        public string ObjectTableId { get; set; }
        public string EntityReference { get; set; }
    }
}