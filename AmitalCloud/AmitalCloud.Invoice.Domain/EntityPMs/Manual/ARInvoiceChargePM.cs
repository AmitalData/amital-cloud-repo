using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class ARInvoiceChargePM
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }

        public string InvoiceCurrencyCode { get; set; }
        public string LocalCurrencyCode { get; set; }
        public double? AmountInInvoiceCurrency { get; set; }
        public double? AmountInLocalCurrency { get; set; }
    }
}