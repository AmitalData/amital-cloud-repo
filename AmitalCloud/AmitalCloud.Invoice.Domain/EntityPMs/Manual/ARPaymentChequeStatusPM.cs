using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class ARPaymentChequeStatusPM : BaseEntityPM
    {
        public ARPaymentChequeStatusPM() : base() { }
        public ARPaymentChequeStatusPM(ARPaymentChequeStatus entity) : base()
        {
            Code = entity.Code;
            SearchFields = entity.SearchFields;
            LocalName = entity.LocalName;
            EnglishName = entity.EnglishName;
            Inactive  = entity.Inactive;
        }
        public string Code { get; set; }
        public string SearchFields { get; set; }
        public string LocalName { get; set; }
        public string EnglishName { get; set; }
        public bool Inactive { get; set; }

    }
}
