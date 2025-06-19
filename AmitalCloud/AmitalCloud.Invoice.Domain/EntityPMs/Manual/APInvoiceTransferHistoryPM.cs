using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class APInvoiceTransferHistoryPM
    {
        [Key]
        public string Id { get; set; }
        public string APInvoiceId { get; set; }
        public string TransferNumber { get; set; }
        public DateTime? TransferDate { get; set; }
        public string FileName { get; set; }
    }
}
