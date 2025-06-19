using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    public class AWBStatusPM : BaseEntityPM
    {
        public AWBStatusPM() : base() { }
        public AWBStatusPM(AWBStatus entity) : base()
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
