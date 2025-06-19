using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class QuickbooksSyncRequestTicketPM
    {
        [Key]
        public string Ticket { get; set; }
        public int Tenant { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RequestCount { get; set; }
        public int ExternalTablesRequestCount { get; set; }
        public bool IsCurrentInvoiceChecked { get; set; }


    }
}