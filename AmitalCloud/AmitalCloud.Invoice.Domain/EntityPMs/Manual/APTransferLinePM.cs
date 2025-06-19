using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class APTransferLinePM
    {
        [Key]
        public int Id { get; set; }
        public string APInvoiceId { get; set; }
        public string Table { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string Description { get; set; }
        public string DescriptionValue { get; set; }
        public string DescriptionHelp { get; set; }
        public string FieldExternalTableId { get; set; }
    }
}
