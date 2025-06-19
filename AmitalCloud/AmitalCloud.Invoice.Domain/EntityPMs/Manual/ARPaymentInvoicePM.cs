using AmitalCloud.Infrastructure.Domain.BaseClasses;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class ARPaymentInvoicePM : BaseEntityPM
    {
        [Key]
        public string Id { get; set; }
        public string ARPaymentId { get; set; }
        public string ARInvoiceId { get; set; }
        public double? LocalAmount { get; set; }
        public double? ForeignAmount { get; set; }
        public double? PaymentAmount { get; set; }
        public string ForeignCurrencyId { get; set; }
        public double? ExchangeRate { get; set; }
        public string ARInvoiceNumber { get; set; }
        public string ExternalAccountingEntityId { get; set; }
        public string ARInvoiceMetodoPagoCode { get; set; }
        public string ARInvoiceTransferStatusCode { get; set; }

    }
}
