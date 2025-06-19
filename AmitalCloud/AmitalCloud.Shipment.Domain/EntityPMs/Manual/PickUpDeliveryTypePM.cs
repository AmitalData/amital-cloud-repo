using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    public class PickUpDeliveryTypePM : BaseEntityPM
    {
        public PickUpDeliveryTypePM()
        {
        }
        public PickUpDeliveryTypePM(PickUpDeliveryType entity)
        {
            Code = entity.Code;
            Name = entity.Name;

        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
