
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class MappedShipmentDirections
    {
        [Key]
        [Column("Tenant", Order = 1)]
        public int Tenant { get; set; }

        [Key]
        [ForeignKey("Direction")]
        [Column("ShipmentDirectionId", Order = 2)]
        public string ShipmentDirectionId { get; set; }

        public virtual Direction Direction { get; set; }

        public DateTime UpdateDateTime { get; set; }
    }
}
