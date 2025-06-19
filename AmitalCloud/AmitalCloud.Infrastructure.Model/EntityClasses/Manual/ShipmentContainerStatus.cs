
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentContainerStatus
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string ShipmentId { get; set; }
        public string StatusCode { get; set; }
        public string FromPortId { get; set; }
        public string ToPortId { get; set; }
        public string Details { get; set; }
        public string RecordHash { get; set; }
        public int Pieces { get; set; }
        public double Weight { get; set; }
        public DateTime ReceivingDate { get; set; }
        public DateTime? EventDate { get; set; }
        public bool Partial { get; set; }
        public string VoyageNumber { get; set; }
        public string VesselName { get; set; }
        public string Location { get; set; }
        public string ShippingLineName { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string TimeOfDepartureInfo { get; set; }
        public string TimeOfArrivalInfo { get; set; }
        public string ContainerId { get; set; }
        public string ContainerNumber { get; set; }
        public string StatusSource { get; set; }
        public string ContainerStatusCode { get; set; }

        [ForeignKey("ShipmentId")]
        public virtual Shipment Shipment { get; set; }

        [ForeignKey("ContainerId")]
        public virtual ShipmentPackage Container { get; set; }

        [ForeignKey("StatusCode")]
        public virtual INTTRAStatus INTTRAStatus { get; set; }

        [ForeignKey("FromPortId")]
        public virtual Port FromPort { get; set; }

        [ForeignKey("ToPortId")]
        public virtual Port ToPort { get; set; }

        [ForeignKey("Location")]
        public virtual Port LocationPort { get; set; }

        [ForeignKey("StatusSource")]
        public virtual ContainerStatusSource ContainerStatusSource { get; set; }

        [ForeignKey("ContainerStatusCode")]
        public virtual ContainerStatus ContainerStatus { get; set; }

    }
}
