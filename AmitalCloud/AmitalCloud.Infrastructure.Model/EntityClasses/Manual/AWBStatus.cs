using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class AWBStatus
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public string SearchFields { get; set; }
        //public List<ShipmentCarrierStatus> ShipmentCarrierStatus { get; set; }
        //public List<Shipment> Shipments { get; set; }
    }
}
