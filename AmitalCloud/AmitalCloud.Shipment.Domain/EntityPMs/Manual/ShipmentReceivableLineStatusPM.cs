using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    public class ShipmentReceivableLineStatusPM : BaseEntityPM
    {
        public ShipmentReceivableLineStatusPM() :base() { }
        public ShipmentReceivableLineStatusPM(ShipmentReceivableLineStatus entity) : base()
        {
            Code = entity.Code;
            Name = entity.Name;
            SearchFields = entity.SearchFields;
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string SearchFields { get; set; }
    }
}
