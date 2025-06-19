using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Shipment.Domain.EntityLists
{
    public class PickUpDeliveryTypeList
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}