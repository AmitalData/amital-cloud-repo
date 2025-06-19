using AmitalCloud.Infrastructure.Domain.BaseClasses;
using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    public partial class ShipmentCarrierStatusPM : BaseEntityPM
    {
        [Key]
        public string Id { get; set; }
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
        public string Location { get; set; }
        public string AirlineName { get; set; }
        public string StatusName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string TimeOfDepartureInfo { get; set; }
        public string TimeOfArrivalInfo { get; set; }
    }
}