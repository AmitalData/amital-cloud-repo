using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class PickUpDeliveryType
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }

        //public List<ShipmentPickUpDelivery> ShipmentPickUpDeliveries { get; set; }

    }
}