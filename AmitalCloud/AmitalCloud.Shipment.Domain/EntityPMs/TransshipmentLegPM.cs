using AmitalCloud.Infrastructure.Domain.BaseClasses;
using System;
namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    public partial class TransshipmentLegPM : BaseEntityPM
    {
        public string Id { get; set; }
        public int LegIndex { get; set; }
        public string FromPortId { get; set; }
        public string FromPortCode { get; set; }
        public string FromPortName { get; set; }
        public string ToPortId { get; set; }
        public string ToPortCode { get; set; }
        public string ToPortName { get; set; }
        public string CarrierId { get; set; }
        public string CarrierName { get; set; }
        public string CarrierNumber { get; set; }
        public string VesselId { get; set; }
        public string VesselName { get; set; }
        public string MasterNumber { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ATD { get; set; }
        public DateTime? ATA { get; set; }
        public string CarrierTypeName { get; set; }
    }
}
