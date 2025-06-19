
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentCarrierStatus
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string ShipmentId { get; set; }
        public string Status { get; set; }
        public string FromPortId { get; set; }
        public string ToPortId { get; set; }
        public string Details { get; set; }
        public string RecordHash { get; set; }
        public int Pieces { get; set; }
        public double Weight { get; set; }
        public DateTime ReceivingDate { get; set; }
        public DateTime? EventDate { get; set; }
        public bool Partial { get; set; }
        public string FlightNumber { get; set; }
        public string Location { get; set; }//location port id
        public string AirlineName { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string TimeOfDepartureInfo { get; set; }
        public string TimeOfArrivalInfo { get; set; }


        [ForeignKey("ShipmentId")]
        public virtual Shipment Shipment { get; set; }
        [ForeignKey("Status")]
        public virtual AWBStatus AWBStatus { get; set; }
        [ForeignKey("FromPortId")]
        public virtual Port FromPort { get; set; }
        [ForeignKey("ToPortId")]
        public virtual Port ToPort { get; set; }
        [ForeignKey("Location")]
        public virtual Port LocationPort { get; set; }




    }
}
