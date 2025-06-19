using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ContainerAnalytic
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdatedByUserId { get; set; }
        public string ContainerNumber { get; set; }
        public string MainCarriageCarrierId { get; set; }
        public DateTime? EstimatedEmptyPickupDate { get; set; }
        public DateTime? ActualEmptyPickupDate { get; set; }
        public string EmptyPickupLocationPortId { get; set; }
        public bool HasContainerException { get; set; }
        public string ShipmentPickupFrom { get; set; }
        public string ShipmentPickupTo { get; set; }
        public string PreCarriageLocationPortId { get; set; }
        public DateTime? PreCarriageETD { get; set; }
        public DateTime? PreCarriageATD { get; set; }
        public string POLLocation { get; set; }
        public DateTime? EstimatedPOLArrival { get; set; }
        public DateTime? ActualPOLArrival { get; set; }
        public DateTime? EstimatedPOLLoaded { get; set; }
        public DateTime? ActualPOLLoaded { get; set; }
        public DateTime? EstimatedPOLVesselDeparture { get; set; }
        public DateTime? ActualPOLVesselDeparture { get; set; }
        public int? TransshipmentCount { get; set; }
        public string Transshipment1LocationPortId { get; set; }
        public DateTime? EstimatedTrans1VesselArrival { get; set; }
        public DateTime? ActualTransshipment1VesselArrival { get; set; }
        public DateTime? EstimatedTransshipment1Discharge { get; set; }
        public DateTime? ActualTransshipment1Discharge { get; set; }
        public DateTime? EstimatedTransshipment1Loaded { get; set; }
        public DateTime? ActualTransshipment1Loaded { get; set; }
        public DateTime? EstimatedTrans1VesselDeparture { get; set; }
        public DateTime? ActualTrans1VesselDeparture { get; set; }
        public string Transshipment2LocationPortId { get; set; }
        public DateTime? ActualTransshipment2VesselArrival { get; set; }
        public DateTime? EstimatedTransshipment2Discharge { get; set; }
        public DateTime? ActualTransshipment2Discharge { get; set; }
        public DateTime? EstimatedTransshipment2Loaded { get; set; }
        public DateTime? ActualTransshipment2Loaded { get; set; }
        public DateTime? EstimatedTrans2VesselDeparture { get; set; }
        public DateTime? ActualTrans2VesselDeparture { get; set; }
        public DateTime? EstimatedTrans2VesselArrival { get; set; }
        public string Transshipment3LocationPortId { get; set; }
        public DateTime? EstimatedTrans3VesselArrival { get; set; }
        public DateTime? ActualTransshipment3VesselArrival { get; set; }
        public DateTime? EstimatedTransshipment3Discharge { get; set; }
        public DateTime? ActualTransshipment3Discharge { get; set; }
        public DateTime? EstimatedTransshipment3Loaded { get; set; }
        public DateTime? ActualTransshipment3Loaded { get; set; }
        public DateTime? EstimatedTrans3VesselDeparture { get; set; }
        public DateTime? ActualTrans3VesselDeparture { get; set; }
        public string Leg1VesselId { get; set; }
        public string Leg2VesselId { get; set; }
        public string Leg3VesselId { get; set; }
        public string Leg4VesselId { get; set; }
        public string Leg5VesselId { get; set; }
        public string Leg1Voyage { get; set; }
        public string Leg2Voyage { get; set; }
        public string Leg3Voyage { get; set; }
        public string Leg4Voyage { get; set; }
        public string Leg5Voyage { get; set; }
        public DateTime? EstimatedPODVesselArrival { get; set; }
        public DateTime? ActualPODVesselArrival { get; set; }
        public DateTime? EstimatedPODDischarge { get; set; }
        public DateTime? ActualPODDischarge { get; set; }
        public DateTime? EstimatedPODDeparture { get; set; }
        public DateTime? ActualPODDeparture { get; set; }
        public string OnCarriageLocationPortId { get; set; }
        public DateTime? OnCarriageETD { get; set; }
        public DateTime? OnCarriageATD { get; set; }
        public DateTime? GateIn { get; set; }
        public DateTime? GateOut { get; set; }
        public string EmptyReturnLocationPortId { get; set; }
        public DateTime? EstimatedEmptyReturn { get; set; }
        public DateTime? ActualEmptyReturn { get; set; }
        public DateTime? LastFreeDayDate { get; set; }
        public int? FreeDays { get; set; }
        public string ShipmentNumber { get; set; }
        public int? ContainersCount { get; set; }
        public string HandlerId { get; set; }
        public string CustomerId { get; set; }
        public bool OPClosed { get; set; }
        public string ShipmentTypeId { get; set; }
        public DateTime? ShipmentCreateDate { get; set; }
        public bool IsClosed { get; set; }
        public string StatusId { get; set; }
        public bool IsCancelled { get; set; }
        public string ShipmentDeliveryTruckerId { get; set; }
        public bool HasException { get; set; }
        public DateTime? PreCarriageGateIn { get; set; }
        public DateTime? OnCarriageGateOut { get; set; }
        public double? Volume { get; set; }
        public double? GrossWeight { get; set; }
        public string ShipmentStatusId { get; set; }

    }
}
