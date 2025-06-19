using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class ARInvoiceLineActionPM : BaseEntityPM
    {
        public ARInvoiceLineActionPM() : base() { }
        public ARInvoiceLineActionPM(ARInvoiceLineAction entity) : base()
        {
            Code = entity.Code;
            Name = entity.Name;
            LocalName = entity.LocalName;
            Inactive = entity.Inactive;
            SearchFields = entity.SearchFields;
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string LocalName { get; set; }
        public bool Inactive { get; set; }
        public string SearchFields { get; set; }
    }
}
