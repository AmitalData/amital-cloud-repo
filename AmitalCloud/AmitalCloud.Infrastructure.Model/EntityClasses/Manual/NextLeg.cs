using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class NextLeg
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }

        //public List<Shipment> Shipments { get; set; }
    }
}