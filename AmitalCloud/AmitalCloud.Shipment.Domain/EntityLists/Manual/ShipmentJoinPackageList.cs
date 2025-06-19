using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AmitalCloud.Shipment.Domain.EntityLists
{
    public class ShipmentJoinPackageList
    {
        [Key]
        public string Id { get; set; }
        public string DirectionId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string ShipmentNumber { get; set; }
        public DateTime? MainCarriageETD { get; set; } //ETD
        public DateTime? MainCarriageATD { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string MainCarriageFromPortName { get; set; } //POL
        public string MainCarriageFinalDestinationPortName { get; set; } //POD
        public string MainCarriageFinalDestinationPortId{ get; set; } //POD
        public string ShipperName { get; set; }
        public string ShipperId { get; set; }
        public string MainCarriageCarrierName { get; set; }
        public string ContainerNumber { get; set; } //all the containers numbers + types
        public string ShipmentTypeId { get; set; }
        public string ShipmentTypeName { get; set; }
        public DateTime? MainCarriageFinalDestinationETA { get; set; } //ETA
        public string MasterNumber { get; set; }
        public string Voyage { get; set; } //main carriage carrier number
        public string StatusName { get; set; }
        public string AgentReference1 { get; set; }
        public string AgentReference2 { get; set; }
        public string ContainerCode { get; set; }
        public string ShipmentId { get; set; }
        public string PackageId { get; set; }
        public string TransportModeId { get; set; }
        public string MainCarriageCarrierPrefix { get; set; }
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public string ShipmentLevelCode { get; set; }
        public string VesselName { get; set; }
        public string CustomerReference1 { get; set; }
        public string CustomerReference2 { get; set; }
        public bool IsCancelled { get; set; }
        public string DescriptionofGoods { get; set; }
        public string AirlinePrefix { get; set; }

        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Field6 { get; set; }
        public string Field7 { get; set; }
        public string Field8 { get; set; }
        public string Field9 { get; set; }
        public string Field10 { get; set; }
        public string Field11 { get; set; }
        public string Field12 { get; set; }
        public string Field13 { get; set; }
        public string Field14 { get; set; }
        public string Field15 { get; set; }
        public string Field16 { get; set; }
        public string Field17 { get; set; }
        public string Field18 { get; set; }
        public string Field19 { get; set; }
        public string Field20 { get; set; }
        public string Field21 { get; set; }
        public string Field22 { get; set; }
        public string Field23 { get; set; }
        public string Field24 { get; set; }
        public string Field25 { get; set; }
        public string Field26 { get; set; }
        public string Field27 { get; set; }
        public string Field28 { get; set; }
        public string Field29 { get; set; }
        public string Field30 { get; set; }
        public string Field31 { get; set; }
        public string Field32 { get; set; }
        public string Field33 { get; set; }
        public string Field34 { get; set; }
        public string Field35 { get; set; }
        public string Field36 { get; set; }
        public string Field37 { get; set; }
        public string Field38 { get; set; }
        public string Field39 { get; set; }
        public string Field40 { get; set; }
        public string Field41 { get; set; }
        public string Field42 { get; set; }
        public string Field43 { get; set; }
        public string Field44 { get; set; }
        public string Field45 { get; set; }
        public string Field46 { get; set; }
        public string Field47 { get; set; }
        public string Field48 { get; set; }
        public string Field49 { get; set; }
        public string Field50 { get; set; }
        public string Field51 { get; set; }
        public string Field52 { get; set; }
        public string Field53 { get; set; }
        public string Field54 { get; set; }
        public string Field55 { get; set; }
        public string Field56 { get; set; }
        public string Field57 { get; set; }
        public string Field58 { get; set; }
        public string Field59 { get; set; }
        public string Field60 { get; set; }
        public string Field61 { get; set; }
        public string Field62 { get; set; }
        public string Field63 { get; set; }
        public string Field64 { get; set; }
        public string Field65 { get; set; }
        public string Field66 { get; set; }
        public string Field67 { get; set; }
        public string Field68 { get; set; }
        public string Field69 { get; set; }
        public string Field70 { get; set; }

        public string House { get; set; }
        public string ConsigneeName { get; set; }
        public string ShipmentPackageReference1 { get; set; }
        public string ShipmentPackageReference2 { get; set; }
        public string ShipmentPackageReference3 { get; set; }
        public string ShipmentPackageReference4 { get; set; }
        public string ContainerTypeName { get; set; }
        public string OnCarriageToPortId { get; set; }
        public string OnCarriageTo { get; set; }
        public string OnCarriageToPortCode { get; set; }
        public DateTime? ATD { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? OnCarriageATD { get; set; }
        public DateTime? OnCarriageATA { get; set; }
        public DateTime? OnCarriageETA { get; set; }
        public string ContainerNotes { get; set; }

        public string Transshipment3ToPortId { get; set; }
        public string Transshipment3ToPortName { get; set; }
        public string Transshipment2ToPortId { get; set; }
        public string Transshipment2ToPortName { get; set; }
        public string Transshipment1ToPortId { get; set; }
        public string Transshipment1ToPortName { get; set; }
        public string MainCarriageToPortId { get; set; }
        public string MainCarriageToPortName { get; set; }
        public double? PackagesGrossWeight { get; set; }
        public double? PackagesVolumetricWeight { get; set; }
        public int? PackagesQuantity { get; set; }
        
        public bool? ContainerFollowUp { get; set; }
        public bool? SplitOnCarriage { get; set; }
        public DateTime? PackageOnCarriageATD { get; set; }
        public DateTime? PackageOnCarriageATA { get; set; }
        public DateTime? PackageOnCarriageETA { get; set; }
        public string PackageDliveryId { get; set; }

        public string Transshipment3FromPortId { get; set; }
        public string Transshipment3FromPortName { get; set; }
        public string Transshipment2FromPortId { get; set; }
        public string Transshipment2FromPortName { get; set; }
        public string Transshipment1FromPortId { get; set; }
        public string Transshipment1FromPortName { get; set; }
        public string MainCarriageFromPortId { get; set; }

        public DateTime? Transshipment3ETA { get; set; }
        public DateTime? Transshipment2ETA { get; set; }
        public DateTime? Transshipment1ETA { get; set; }
        public DateTime? MainCarriageETA { get; set; }

        public DateTime? Transshipment3ATA { get; set; }
        public DateTime? Transshipment2ATA { get; set; }
        public DateTime? Transshipment1ATA { get; set; }
        public DateTime? MainCarriageATA { get; set; }

        public DateTime? Transshipment3ETD { get; set; }
        public DateTime? Transshipment2ETD { get; set; }
        public DateTime? Transshipment1ETD { get; set; }

        public DateTime? Transshipment3ATD { get; set; }
        public DateTime? Transshipment2ATD { get; set; }
        public DateTime? Transshipment1ATD { get; set; }

        public string Transshipment3VesselName { get; set; }
        public string Transshipment2VesselName { get; set; }
        public string Transshipment1VesselName { get; set; }
        public string MainCarriageVesselName { get; set; }
        public string BookingConfirmationNumber { get; set; }
        public string IncotermId { get; set; }
        public string ShipperAddressId { get; set; }
        public string ConsigneeAddressId { get; set; }
        public double? Volume { get; set; }

        public double? PackageVolume { get; set; }
        public string MainCarriageCarrierId { get; set; }

        public int? NumberOfContainers { get; set; }
        public string Reference1 { get; set; }
        public string CustomAgentImportId { get; set; }
        public string CustomAgentImportName { get; set; }
        public string CommodityNumber { get; set; }
        public string CommodityName { get; set; }
        public int? PackageQuantity { get; set; }
        public string MoveTypeName { get; set; }
        public double? PackageVolumeitricWeight { get; set; }
        public double? PackageChargeableWeight { get; set; } 
        public string MainCarriageCarrierCode { get; set; }
        public string MainCarriageCarrierNumber { get; set; }
        public DateTime? MainCarriageDateFilter { get; set; }
        public double? PackageWidth { get; set; }
        public double? PackageLength { get; set; }
        public double? PackageHeight { get; set; }
        public string ConsigneeId { get; set; }
        public string Routing { get; set; }
        public string OnForwardingTo { get; set; }
        public DateTime? OnForwardingATD { get; set; }
        public DateTime? OnForwardingATA { get; set; }
        public DateTime? OnForwardingETA { get; set; }
        public string OnForwardingToPortCode { get; set; }
        public string OnForwardingToPortId { get; set; }
        public DateTime? CutOffDate { get; set; }
    }
}
