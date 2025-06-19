using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    public class ShipmentPayableAmountTypePM : BaseEntityPM
    {
        public ShipmentPayableAmountTypePM() : base() { }
        public ShipmentPayableAmountTypePM(ShipmentPayableAmountType entity) : base()
        {
            Code = entity.Code;
            Name = entity.Name;
        }

        public string Code { get; set; }
        public string Name { get; set; }


    }
}
