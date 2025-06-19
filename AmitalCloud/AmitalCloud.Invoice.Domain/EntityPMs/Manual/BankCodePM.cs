using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class BankCodePM : BaseEntityPM
    {
        public BankCodePM() : base() { }
        public BankCodePM(BankCode entity) : base()
        {
            Id = entity.Id;
            Code = entity.Code;
            EnglishName = entity.EnglishName;
            SearchFields = entity.SearchFields;
            Tenant = entity.Tenant;
            LocalName = entity.LocalName;
            Inactive = entity.Inactive;
            LogoId = entity.LogoId;
            DateFormat = entity.DateFormat;
        }
        public string Code { get; set; }
        public string EnglishName { get; set; }
        public string SearchFields { get; set; }
        public int Tenant { get; set; }
        public string LocalName { get; set; }
        public string Id { get; set; }
        public bool? Inactive { get; set; }
        public string LogoId { get; set; }
        public string DateFormat { get; set; }

    }
}
