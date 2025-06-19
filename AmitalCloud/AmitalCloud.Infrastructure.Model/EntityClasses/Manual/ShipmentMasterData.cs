
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentMasterData
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string Master { get; set; }
        public DateTime? MAWBOBLDate { get; set; }
        public bool MainCarriageIsFromStack { get; set; }
        public string BookingConfirmationNumber { get; set; }
        public string BookingConfirmedBy { get; set; }
        public string BookingConfirmationNotes { get; set; }
        public string MainCarriageFromPortId { get; set; }
        public string MainCarriageToPortId { get; set; }
        public DateTime? MainCarriageATD { get; set; }
        public DateTime? MainCarriageATA { get; set; }
        public DateTime? MainCarriageETD { get; set; }
        public DateTime? MainCarriageETA { get; set; }
        public string MainCarriageCarrierNumber { get; set; }
        public string MainCarriageCarrierId { get; set; }
        public string Transshipment1FromPortId { get; set; }
        public string Transshipment1ToPortId { get; set; }
        public DateTime? Transshipment1ATD { get; set; }
        public DateTime? Transshipment1ATA { get; set; }
        public DateTime? Transshipment1ETD { get; set; }
        public DateTime? Transshipment1ETA { get; set; }
        public string Transshipment1CarrierNumber { get; set; }
        public string Transshipment1CarrierId { get; set; }
        public string Transshipment2FromPortId { get; set; }
        public string Transshipment2ToPortId { get; set; }
        public DateTime? Transshipment2ATD { get; set; }
        public DateTime? Transshipment2ATA { get; set; }
        public DateTime? Transshipment2ETD { get; set; }
        public DateTime? Transshipment2ETA { get; set; }
        public string Transshipment2CarrierNumber { get; set; }
        public string Transshipment2CarrierId { get; set; }
        public string Transshipment3FromPortId { get; set; }
        public string Transshipment3ToPortId { get; set; }
        public DateTime? Transshipment3ATD { get; set; }
        public DateTime? Transshipment3ATA { get; set; }
        public DateTime? Transshipment3ETD { get; set; }
        public DateTime? Transshipment3ETA { get; set; }
        public string Transshipment3CarrierNumber { get; set; }
        public string Transshipment3CarrierId { get; set; }
        public string MainCarriageVesselId { get; set; }
        public string Transshipment1VesselId { get; set; }
        public string Transshipment2VesselId { get; set; }
        public string Transshipment3VesselId { get; set; }
        public string Transshipment1AdditionalMAWBOBLBL { get; set; }
        public string Transshipment2AdditionalMAWBOBLBL { get; set; }
        public string Transshipment3AdditionalMAWBOBLBL { get; set; }
        public string MainCarriageFinalDestinationPortId { get; set; }
        public DateTime? MainCarriageFinalDestinationETA { get; set; }
        public DateTime? MainCarriageFinalDestinationATA { get; set; }
        public DateTime? DepartureArrivalFromDate { get; set; }
        public DateTime? DepartureArrivalToDate { get; set; }


        public string MasterShipmentNumber { get; set; }
        public string FWBStatusCode { get; set; }
        public DateTime? FWBStatusDate { get; set; }
        public string MainCarriageFromPartnerId { get; set; }
        public string MainCarriageFromAddressId { get; set; }
        public string MainCarriageToPartnerId { get; set; }
        public string MainCarriageToAddressId { get; set; }
        public string Driver { get; set; }
        public string TruckNumber { get; set; }
        public string TrailerNumber { get; set; }
        public string Transshipment1TrailerNumber { get; set; }
        public string Transshipment2TrailerNumber { get; set; }
        public string Transshipment3TrailerNumber { get; set; }
        public DateTime? MainCarriageSTD { get; set; }
        public DateTime? MainCarriageSTA { get; set; }
        public DateTime? Transshipment1STD { get; set; }
        public DateTime? Transshipment1STA { get; set; }
        public DateTime? Transshipment2STD { get; set; }
        public DateTime? Transshipment2STA { get; set; }
        public DateTime? Transshipment3STD { get; set; }
        public DateTime? Transshipment3STA { get; set; }
        public string ImportManifest { get; set; }
        public string CarrierTransportDocumentNumber { get; set; }

        public string StatusId { get; set; }
        public DateTime? StatusDate { get; set; }
        public string StatusLocation { get; set; }
        public string PartialStatusAmount { get; set; }
        public string CargonautFWBStatusCode { get; set; }
        public DateTime? CargonautFWBStatusDate { get; set; }
        public string MainCarriageCarrierPrefix { get; set; }
        public string Transshipment1CarrierPrefix { get; set; }
        public string Transshipment2CarrierPrefix { get; set; }
        public string Transshipment3CarrierPrefix { get; set; }

        public bool IsKnownCargo { get; set; }
        public string RegulatedAgentRANumber { get; set; }
        public string KnownConsignorNumber { get; set; }
        public string ColoaderRANumber { get; set; }
        public DateTime? KCExpirationDate { get; set; }

        public string AWBPrintingSecurityStatusId { get; set; }
        public string AWBPrintingRANumber { get; set; }
        public string AdditionalHandlingInfo { get; set; }
        public bool AWBPrintingSecurityStatusEdited { get; set; }
        public bool AWBPrintingRANumberEdited { get; set; }
        public bool AdditionalHandlingInfoEdited { get; set; }
        public string InterlineId { get; set; }

        public string AirlinePrefix { get; set; }
        public string ManifestReason { get; set; }
        public string ManifestStatusCode { get; set; }
        public ManifestStatus ManifestStatus { get; set; }

        public bool ProrateReceivables { get; set; }

        public DateTime? DocumentsClosingDate { get; set; }
        public string OBLTypeCode { get; set; }
        public DateTime? AutomaticLastUpdateDate { get; set; }

        [ForeignKey("OBLTypeCode")]
        public OBLType OBLType { get; set; }

        public DateTime? CutoffDate { get; set; }

        #region Objects

        [ForeignKey("FWBStatusCode")]
        public FWBStatus FWBStatus { get; set; }

        [ForeignKey("CargonautFWBStatusCode")]
        public FWBStatus CargonautFWBStatus { get; set; }

        [ForeignKey("StatusId")]
        public virtual EntityStatus EntityStatus { get; set; }

        [ForeignKey("MainCarriageVesselId")]
        public virtual Vessel MainCarriageVessel { get; set; }
        [ForeignKey("Transshipment1VesselId")]
        public virtual Vessel Transshipment1Vessel { get; set; }
        [ForeignKey("Transshipment2VesselId")]
        public virtual Vessel Transshipment2Vessel { get; set; }
        [ForeignKey("Transshipment3VesselId")]
        public virtual Vessel Transshipment3Vessel { get; set; }
        [ForeignKey("Transshipment1FromPortId")]
        public virtual Port Transshipment1FromPort { get; set; }
        [ForeignKey("Transshipment1ToPortId")]
        public virtual Port Transshipment1ToPort { get; set; }
        [ForeignKey("Transshipment1CarrierId")]
        public virtual Card Transshipment1CarrierCard { get; set; }
        [ForeignKey("Transshipment2FromPortId")]
        public virtual Port Transshipment2FromPort { get; set; }
        [ForeignKey("Transshipment2ToPortId")]
        public virtual Port Transshipment2ToPort { get; set; }
        [ForeignKey("Transshipment2CarrierId")]
        public virtual Card Transshipment2CarrierCard { get; set; }
        [ForeignKey("Transshipment3FromPortId")]
        public virtual Port Transshipment3FromPort { get; set; }
        [ForeignKey("Transshipment3ToPortId")]
        public virtual Port Transshipment3ToPort { get; set; }
        [ForeignKey("Transshipment3CarrierId")]
        public virtual Card Transshipment3CarrierCard { get; set; }
        [ForeignKey("MainCarriageCarrierId")]
        public virtual Card MainCarriageCarrierCard { get; set; }
        [ForeignKey("MainCarriageFromPortId")]
        public virtual Port MainCarriageFromPort { get; set; }
        [ForeignKey("MainCarriageToPortId")]
        public virtual Port MainCarriageToPort { get; set; }
        [ForeignKey("MainCarriageFinalDestinationPortId")]
        public virtual Port MainCarriageFinalDestinationPort { get; set; }
        public Shipment Shipment { get; set; }
        [ForeignKey("MainCarriageFromPartnerId")]
        public virtual Card FromPartnerCard { get; set; }
        [ForeignKey("MainCarriageToPartnerId")]
        public virtual Card ToPartnerCard { get; set; }
        [ForeignKey("MainCarriageFromAddressId")]
        public virtual Address FromPartnerAddress { get; set; }
        [ForeignKey("MainCarriageToAddressId")]
        public virtual Address ToPartnerAddress { get; set; }

        public virtual AWBSpecialHandlingCode AWBSpecialHandlingCode { get; set; }

        [ForeignKey("InterlineId")]
        public virtual Card InterlineCard { get; set; }
        #endregion

        //pre carriage
        public string PreCarriageTransportModeId { get; set; }
        public string PreCarriageFromPortId { get; set; }
        public string PreCarriageToPortId { get; set; }
        public string PreCarriageCarrierId { get; set; }
        public string PreCarriageCarrierNumber { get; set; }
        public DateTime? PreCarriageETD { get; set; }
        public DateTime? PreCarriageATD { get; set; }
        public DateTime? PreCarriageETA { get; set; }
        public DateTime? PreCarriageATA { get; set; }
        public string PreCarriageVesselId { get; set; }

        //on carriage
        public string OnCarriageTransportModeId { get; set; }
        public string OnCarriageFromPortId { get; set; }
        public string OnCarriageToPortId { get; set; }
        public string OnCarriageCarrierId { get; set; }
        public string OnCarriageCarrierNumber { get; set; }
        public DateTime? OnCarriageETD { get; set; }
        public DateTime? OnCarriageATD { get; set; }
        public DateTime? OnCarriageETA { get; set; }
        public DateTime? OnCarriageATA { get; set; }
        public string OnCarriageVesselId { get; set; }
        public string OnCarriageAdditionalTransportModeCode { get; set; }
        public bool SplitOnCarriage { get; set; }

        public Vessel PreCarriageVessel { get; set; }
        public virtual TransportMode PreCarriageTransportMode { get; set; }
        public virtual Port PreCarriageFromPort { get; set; }
        public virtual Port PreCarriageToPort { get; set; }
        public virtual Card PreCarriageCarrierCard { get; set; }

        public Vessel OnCarriageVessel { get; set; }
        public virtual TransportMode OnCarriageTransportMode { get; set; }
        public virtual Port OnCarriageFromPort { get; set; }
        public virtual Port OnCarriageToPort { get; set; }
        public virtual Card OnCarriageCarrierCard { get; set; }
        public virtual PickUpDeliveryTransportMode OnCarriageAdditionalTransportMode { get; set; }

        public string InlandDomesticFromZipCode { get; set; }
        public string InlandDomesticToZipCode { get; set; }
        public string InlandDomesticFromCity { get; set; }
        public string InlandDomesticToCity { get; set; }
        public string InlandDomesticFromCountryId { get; set; }
        public string InlandDomesticToCountryId { get; set; }
        public string InlandDomesticFromTypeCode { get; set; }
        public string InlandDomesticToTypeCode { get; set; }
        public string MainCarriageFromPortAddress { get; set; }
        public string MainCarriageToPortAddress { get; set; }

        [ForeignKey("InlandDomesticFromTypeCode")]
        public PickUpDeliveryFromToType InlandDomesticFromType { get; set; }

        [ForeignKey("InlandDomesticToTypeCode")]
        public PickUpDeliveryFromToType InlandDomesticToType { get; set; }

        [ForeignKey("InlandDomesticFromCountryId")]
        public virtual Country InlandDomesticFromCountry { get; set; }

        [ForeignKey("InlandDomesticToCountryId")]
        public virtual Country InlandDomesticToCountry { get; set; }

        public string OperationalStatusId { get; set; }
        public string BillingStatusId { get; set; }

        public virtual EntityStatus OperationalStatus { get; set; }
        public virtual EntityStatus BillingStatus { get; set; }

        public string MainCarriageVesselName { get; set; }
        public string Transshipment1VesselName { get; set; }
        public string Transshipment2VesselName { get; set; }
        public string Transshipment3VesselName { get; set; }
        public string PreCarriageVesselName { get; set; }
        public string OnCarriageVesselName { get; set; }
        public string CarrierServiceLineId { get; set; }
        public virtual CarrierServiceLine CarrierServiceLine { get; set; }

        public string InlandDomesticToAddress1 { get; set; }
        public string InlandDomesticToAddress2 { get; set; }
        public string InlandDomesticToPhone { get; set; }
        public string InlandDomesticToFax { get; set; }
        public string InlandDomesticToStateId { get; set; }

        [ForeignKey("InlandDomesticToStateId")]
        public virtual State InlandDomesticToState { get; set; }


        public string InlandDomesticFromAddress1 { get; set; }
        public string InlandDomesticFromAddress2 { get; set; }
        public string InlandDomesticFromPhone { get; set; }
        public string InlandDomesticFromFax { get; set; }
        public string InlandDomesticFromStateId { get; set; }

        [ForeignKey("InlandDomesticFromStateId")]
        public virtual State InlandDomesticFromState { get; set; }
        public int? NumberOfTransshipments { get; set; }
    }
}