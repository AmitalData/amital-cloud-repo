using AmitalCloud.Infrastructure.Domain.BaseClasses;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class APPaymentInvoicePM : BaseEntityPM
    {
        [Key]
        public string Id { get; set; }
        public string APPaymentId { get; set; }
        public string APInvoiceId { get; set; }
        public double? LocalAmount { get; set; }
        public double? ForeignAmount { get; set; }
        public string ForeignCurrencyId { get; set; }
        public string APInvoiceNumber { get; set; }
        public double? PaymentAmount { get; set; }
        public double? ExchangeRate { get; set; }
        public string APInvoiceTransferStatusCode { get; set; }
    }
}
