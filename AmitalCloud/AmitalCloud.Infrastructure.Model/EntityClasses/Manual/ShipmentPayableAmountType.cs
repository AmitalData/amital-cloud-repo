using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentPayableAmountType
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }

        //public List<ShipmentPayable> ShipmentPayables { get; set; }
    }
}