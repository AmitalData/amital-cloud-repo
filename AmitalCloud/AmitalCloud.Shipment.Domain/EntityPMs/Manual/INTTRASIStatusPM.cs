using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    public class INTTRASIStatusPM : BaseEntityPM
    {
        public INTTRASIStatusPM() : base() { }
        public INTTRASIStatusPM(INTTRASIStatus entity) : base()
        {
            this.Code = entity.Code;
            this.Name = entity.Name;
            this.SearchFields = entity.SearchFields;
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string SearchFields { get; set; }
    }
}
