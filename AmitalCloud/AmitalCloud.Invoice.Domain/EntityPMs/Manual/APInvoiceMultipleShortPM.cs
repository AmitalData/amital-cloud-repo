using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class APInvoiceMultipleShortPM
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string ShipmentId { get; set; }
        public string StatusCode { get; set; }
        public string VendorId { get; set; }
        public string ProfitCurrencyId { get; set; }
        public string InvoiceCurrencyId { get; set; }
        public double? InvoiceCurrencyExchangeRate { get; set; }
        public double? ProfitCurrencyExchangeRate { get; set; }
        public double? SubTotalInLocalCurrency { get; set; }
        public double? SubTotalInInvoiceCurrency { get; set; }

        private bool isMultipleEntities = true;
        public bool IsMultipleEntities
        {
            get { return isMultipleEntities; }
            set
            {
                isMultipleEntities = true;
            }
        }

        private List<APInvoiceLinePM> invoiceLines;
        
        
        [Association("APInvoiceShortAPInvoiceLines", "Id", "APInvoiceId")]
        public virtual List<APInvoiceLinePM> InvoiceLines
        {
            get
            {
                if (invoiceLines == null)
                {
                    invoiceLines = new List<APInvoiceLinePM>();
                }

                return this.invoiceLines;
            }

            set
            {
                if (value != null)
                {
                    invoiceLines = value;
                }
            }
        }

    }
}
