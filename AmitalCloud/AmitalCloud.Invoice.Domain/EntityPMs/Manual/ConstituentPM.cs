using AmitalCloud.Infrastructure.Domain.BaseClasses;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class ConstituentPM : BaseEntityPM
    {
        [Key]
        public string Id { get; set; }
        public string ConsolidationInvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerRef { get; set; }
        public string MasterNumber { get; set; }
        public string HouseNumber { get; set; }
        public string MainEntityReference { get; set; }
        public double? AmountInInvoiceCurrency { get; set; }
        public double? TotalVATs { get; set; }
        public double? SubTotalInInvoiceCurrency { get; set; }
        public string ConcurrencyGUID { get; set; }
    }
}