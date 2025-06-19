using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ARInvoiceLineAction
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public string LocalName { get; set; }
        public bool Inactive { get; set; }
        public string SearchFields { get; set; }
    }
}
