using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentFollowUpDataView
    {
        private string id;

        public string Id
        {
            get
            {

                return id;
            }
            set
            {
                id = value;
            }
        }
        private string shipmentFollowUpId;
        [Key]
        public string ShipmentFollowUpId { get { return Id + FollowUpId; } set { shipmentFollowUpId = Id + FollowUpId; } }
        #region Fields
        public string MainCarriageAirlinePrefix { get; set; }
        public string ShipmentMasterDataId { get; set; }
        public int? ShipmentMasterDataTenant { get; set; }
        public double? ChargeableWeightInKG { get; set; }
        public double? GrossWeightInKG { get; set; }
        public double? GrossWeightPerStorageDays { get; set; }
        public double? ChargeableWeight { get; set; }
        public double? GrossWeight { get; set; }

        public string CurrentUserId { get; set; }
        public int Tenant { get; set; }
        public string BasketId { get; set; }

        public string ShipmentNumber { get; set; }
        public string MasterShipmentNumber { get; set; }


        public string DirectionId { get; set; }
        public string DirectionName { get; set; }

        public string TransportModeId { get; set; }
        public string TransportModeName { get; set; }


        public string MasterId { get; set; }


        public string ShipmentTypeId { get; set; }
        public string ShipmentTypeName { get; set; }

        public string House { get; set; }
        public DateTime CreateDateTime { get; set; }
        public bool MAWBTakenFromStack { get; set; }
        public bool MAWBReturnedToStack { get; set; }


        public string BranchId { get; set; }


        public string IncotermId { get; set; }

        //[CustomValidation(typeof(ValidationClass), "ValidateClass")]
        //public string CurrencyValueOfGoodsId { get; set; }

        public double? ValueOfGoods { get; set; }


        public string SalesmanUserId { get; set; }


        public string CreatedByUserId { get; set; }


        public string DepartmentId { get; set; }


        public string Notes { get; set; }


        public string DescriptionOfGoods { get; set; }

        //[Timestamp]
        //public byte[] LastModified { get; set; }
        public DateTime? HAWBDate { get; set; }
        public string MAWBStackNumber { get; set; }

        public bool IsOperationalClosed { get; set; }

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


        public string SearchFields { get; set; }


        public string Master { get; set; }

        public string LongMaster { get; set; }

        public string FreightPrepaidCollectId { get; set; }
        public string OtherPrepaidCollectId { get; set; }
        public string GrossWeightUnitCode { get; set; }
        public string ChargeableWeightUnitCode { get; set; }
        public string DimensionsUnitCode { get; set; }

        public double? VolumetricWeight { get; set; }
        public double? VolumeInCBM { get; set; }

        public int? PackagesQuantity { get; set; }
        public int? NumberOfPackages { get; set; }
        public int? NumberOfContainers { get; set; }
        public int? NumberOfFollowUps { get; set; }
        public double? Ratio { get; set; }

        public DateTime? MAWBOBLDate { get; set; }
        public bool GrossWeightEdited { get; set; }
        public bool ChargeableWeightEdited { get; set; }
        public string VolumeUnitCode { get; set; }

        public string StatusId { get; set; }
        public string ShipmentStatusId { get; set; }
        public string ShipmentStatusName { get; set; }
        public int? ShipmentStatusWeight { get; set; }
        public DateTime? ShipmentStatusDate { get; set; }
        public string ShipmentStatusLocation { get; set; }

        public string ShipmentMasterDataStatusId { get; set; }
        public string ShipmentMasterDataStatusName { get; set; }
        public int? ShipmentMasterDataStatusWeight { get; set; }
        public DateTime? ShipmentMasterDataStatusDate { get; set; }
        public string ShipmentMasterDataStatusLocation { get; set; }

        public string QuoteId { get; set; }
        public string TotalContainers { get; set; }

        public string ShipmentType { get; set; }
        public string ShipmentPMId { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool NewMessage { get; set; }
        public bool IsAnyConversation { get; set; }
        public int NumberOfShipments { get; set; }
        public string MainCarriageFinalDestinationPortId { get; set; }
        public string MainCarriageFinalDestinationPortCode { get; set; }
        public string MainCarriageFinalDestinationPortName { get; set; }


        public string MainHarmonize { get; set; }
        public bool IsDangerous { get; set; }
        public string DangerousClassNumber { get; set; }
        public string DangerousUnNumber { get; set; }
        public string DangerousPackagingGroup { get; set; }
        public string DangerousIMDGCode { get; set; }
        public string DangerousFlashPoint { get; set; }
        public string DangerousMaterialDescription { get; set; }
        //public bool FTCWEdited { get; set; }
        public bool LTCWEdited { get; set; }
        public int ShipmentPickUpIndex { get; set; }
        public int ShipmentDeliveryIndex { get; set; }
        public string ShipmentPayableStatusCode { get; set; }
        public string ShipmentReceivableStatusCode { get; set; }
        public string ShipmentReceivableStatusName { get; set; }
        public string ShipmentPayableStatusName { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsAccountingClosed { get; set; }
        public DateTime AccessDate { get; set; }
        public string UpdatedByUserId { get; set; }
        public DateTime LastUpdateDate { get; set; }

        #region financial props
        public double? OpenReceivablesInLocalCurrency { get; set; }
        public double? AccountedReceivablesInLocalCurrency { get; set; }

        public double? OpenReceivablesInProfitCurrency { get; set; }
        public double? AccountedReceivablesInProfitCurrency { get; set; }

        public double? OpenPayablesInLocalCurrency { get; set; }
        public double? AccountedPayablesInLocalCurrency { get; set; }
        public double? OpenPayablesInProfitCurrency { get; set; }
        public double? AccountedPayablesInProfitCurrency { get; set; }

        public double ProfitInLocalCurrency { get; set; }
        public double? EstimateProfitInLocalCurrency { get; set; }

        public double? ProfitInProfitCurrency { get; set; }
        public double? EstimateProfitInProfitCurrency { get; set; }

        #endregion



        public string ProfitCurrencyId { get; set; }
        public string UpdatedByUserName { get; set; }
        public string EventNote { get; set; }

        public string NextLegCode { get; set; }
        public string NextLegName { get; set; }
        public DateTime? NextETD { get; set; }
        public DateTime? NextETA { get; set; }

        //public bool IsMaster { get; set; }
        //public bool IsDirect { get; set; }
        public string ShipmentLevelCode { get; set; }
        public string ShipmentLevelName { get; set; }
        public string MasterShipmentDataId { get; set; }
        #endregion

        #region Partners
        public string ShipmentCustomerTypeCode { get; set; }
        public bool ConsigneeAddressOneTime { get; set; }
        public bool ShipperAddressOneTime { get; set; }
        //public string PartnerToBill { get; set; }

        public string CustomerId { get; set; }
        public string CustomerAddressId { get; set; }
        public string CustomerContactId { get; set; }
        public string CustomerReference1 { get; set; }
        public string CustomerReference2 { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNote { get; set; }

        public string FreightForwarderId { get; set; }
        public string FreightForwarderAddressId { get; set; }
        public string FreightForwarderContactId { get; set; }
        public string FreightForwarderReference { get; set; }
        public string FreightForwarderName { get; set; }
        public string FreightForwarderNote { get; set; }

        public string ShipperId { get; set; }
        public string ShipperAddressId { get; set; }
        public string ShipperContactId { get; set; }
        public string ShipperReference1 { get; set; }
        public string ShipperReference2 { get; set; }
        public string ShipperName { get; set; }
        public string ShipperNote { get; set; }

        public string ConsigneeId { get; set; }
        public string ConsigneeAddressId { get; set; }
        public string ConsigneeContactId { get; set; }
        public string ConsigneeReference1 { get; set; }
        public string ConsigneeReference2 { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneeNote { get; set; }

        public string AgentId { get; set; }
        public string AgentAddressId { get; set; }
        public string AgentContactId { get; set; }
        public string AgentReference1 { get; set; }
        public string AgentReference2 { get; set; }
        public string AgentName { get; set; }
        public string AgentNote { get; set; }

        public string CustomAgentExportId { get; set; }
        public string CustomAgentExportAddressId { get; set; }
        public string CustomAgentExportContactId { get; set; }
        public string CustomAgentExportReference { get; set; }
        public string CustomAgentExportName { get; set; }
        public string CustomAgentExportNote { get; set; }

        public string CustomAgentImportId { get; set; }
        public string CustomAgentImportAddressId { get; set; }
        public string CustomAgentImportContactId { get; set; }
        public string CustomAgentImportReference { get; set; }
        public string CustomAgentImportName { get; set; }
        public string CustomAgentImportNote { get; set; }

        public string Notify1Id { get; set; }
        public string Notify1AddressId { get; set; }
        public string Notify1ContactId { get; set; }
        public string Notify1Name { get; set; }
        public string Notify1Note { get; set; }
        public string Notify1Reference { get; set; }
        public string Notify1Reference2 { get; set; }
        public string Notify2Id { get; set; }
        public string Notify2AddressId { get; set; }
        public string Notify2ContactId { get; set; }
        public string Notify2Name { get; set; }
        public string Notify2Note { get; set; }
        public string Notify2Reference { get; set; }

        public string ShipperNotExporterId { get; set; }
        public string ShipperNotExporterAddressId { get; set; }
        public string ShipperNotExporterContactId { get; set; }
        public string ShipperNotExporterName { get; set; }
        public string ShipperNotExporterNote { get; set; }
        public string ShipperNotExporterReference { get; set; }

        public string ConsigneeNotImporterId { get; set; }
        public string ConsigneeNotImporterAddressId { get; set; }
        public string ConsigneeNotImporterContactId { get; set; }
        public string ConsigneeNotImporterName { get; set; }
        public string ConsigneeNotImporterNote { get; set; }
        public string ConsigneeNotImporterReference { get; set; }
        public string ProjectNumber { get; set; }

        public string ConsolidatorId { get; set; }
        public string ConsolidatorAddressId { get; set; }
        public string ConsolidatorContactId { get; set; }
        public string ConsolidatorReference { get; set; }
        public string ConsolidatorName { get; set; }
        public string ConsolidatorNote { get; set; }
        #endregion

        #region Routings

        public string FromPortId { get; set; }
        public string ToPortId { get; set; }
        public DateTime? PreCarriageETD { get; set; }
        public DateTime? PreCarriageATD { get; set; }
        public DateTime? PreCarriageETA { get; set; }
        public DateTime? PreCarriageATA { get; set; }
        public DateTime? OnCarriageETD { get; set; }
        public DateTime? OnCarriageATD { get; set; }
        public DateTime? OnCarriageETA { get; set; }
        public DateTime? OnCarriageATA { get; set; }

        public string MainCarriageTransportModeId { get; set; }
        public string MainCarriageFromPortId { get; set; }
        public string MainCarriageToPortId { get; set; }
        public string MainCarriageCarrierName { get; set; }
        public string MainCarriageCarrierCode { get; set; }
        public string MainCarriageFromPortCode { get; set; }
        public string MainCarriageFromPortName { get; set; }
        public string MainCarriageFromPortCountryName { get; set; }
        public string MainCarriageFromPortCountryCode { get; set; }
        public string MainCarriageToPortCode { get; set; }
        public string MainCarriageToPortName { get; set; }
        public string MainCarriageToPortCountryCode { get; set; }
        public string MainCarriageToPortCountryName { get; set; }
        public string MainCarriageVesselId { get; set; }
        public string MainCarriageVesselName { get; set; }
        public bool? MainCarriageIsFromStack { get; set; }
        public string MainCarriageCarrierNumber { get; set; }
        public string MainCarriageCarrierId { get; set; }
        public DateTime? MainCarriageATD { get; set; }
        public DateTime? MainCarriageATA { get; set; }
        public DateTime? MainCarriageETD { get; set; }
        public DateTime? MainCarriageETA { get; set; }
        public string MainCarriageCarrierWebSite { get; set; }

        public string Transshipment1FromPortId { get; set; }
        public string Transshipment1ToPortId { get; set; }
        public DateTime? Transshipment1ATD { get; set; }
        public DateTime? Transshipment1ATA { get; set; }
        public DateTime? Transshipment1ETD { get; set; }
        public DateTime? Transshipment1ETA { get; set; }
        public string Transshipment1CarrierNumber { get; set; }
        public string Transshipment1CarrierId { get; set; }
        public string Transshipment1CarrierName { get; set; }
        public string Transshipment1CarrierCode { get; set; }
        public string Transshipment1FromPortCode { get; set; }
        public string Transshipment1FromPortName { get; set; }
        public string Transshipment1FromPortCountryCode { get; set; }
        public string Transshipment1FromPortCountryName { get; set; }
        public string Transshipment1ToPortCode { get; set; }
        public string Transshipment1ToPortName { get; set; }
        public string Transshipment1ToPortCountryCode { get; set; }
        public string Transshipment1ToPortCountryName { get; set; }
        public string Transshipment1CarrierWebSite { get; set; }
        public string Transshipment2CarrierWebSite { get; set; }
        public string Transshipment3CarrierWebSite { get; set; }

        public string Transshipment2FromPortId { get; set; }
        public string Transshipment2ToPortId { get; set; }
        public DateTime? Transshipment2ATD { get; set; }
        public DateTime? Transshipment2ATA { get; set; }
        public DateTime? Transshipment2ETD { get; set; }
        public DateTime? Transshipment2ETA { get; set; }

        public string Transshipment2CarrierNumber { get; set; }
        public string Transshipment2CarrierId { get; set; }
        public string Transshipment2CarrierName { get; set; }
        public string Transshipment2CarrierCode { get; set; }
        public string Transshipment2FromPortCode { get; set; }
        public string Transshipment2FromPortName { get; set; }
        public string Transshipment2FromPortCountryCode { get; set; }
        public string Transshipment2FromPortCountryName { get; set; }
        public string Transshipment2ToPortCode { get; set; }
        public string Transshipment2ToPortName { get; set; }
        public string Transshipment2ToPortCountryCode { get; set; }
        public string Transshipment2ToPortCountryName { get; set; }

        public string Transshipment3FromPortId { get; set; }
        public string Transshipment3ToPortId { get; set; }
        public DateTime? Transshipment3ATD { get; set; }
        public DateTime? Transshipment3ATA { get; set; }
        public DateTime? Transshipment3ETD { get; set; }
        public DateTime? Transshipment3ETA { get; set; }

        public string Transshipment3CarrierNumber { get; set; }
        public string Transshipment3CarrierId { get; set; }
        public string Transshipment3CarrierName { get; set; }
        public string Transshipment3CarrierCode { get; set; }
        public string Transshipment3FromPortCode { get; set; }
        public string Transshipment3FromPortName { get; set; }
        public string Transshipment3FromPortCountryCode { get; set; }
        public string Transshipment3FromPortCountryName { get; set; }
        public string Transshipment3ToPortCode { get; set; }
        public string Transshipment3ToPortName { get; set; }
        public string Transshipment3ToPortCountryCode { get; set; }
        public string Transshipment3ToPortCountryName { get; set; }

        public string FinalDistenationPortId { get; set; }



        public string Transshipment1VesselId { get; set; }
        public string Transshipment2VesselId { get; set; }
        public string Transshipment3VesselId { get; set; }
        public string Transshipment1AdditionalMAWBOBLBL { get; set; }
        public string Transshipment2AdditionalMAWBOBLBL { get; set; }
        public string Transshipment3AdditionalMAWBOBLBL { get; set; }

        public string FromPort { get; set; }
        public string FromPortName { get; set; }
        public string FromPortCode { get; set; }
        public string FromPortCountryCode { get; set; }

        public string ToPort { get; set; }
        public string ToPortName { get; set; }
        public string ToPortCode { get; set; }
        public string ToPortCountryCode { get; set; }
        #endregion
        public string TruckNumber { get; set; }
        public double? Volume { get; set; }
        public string BranchName { get; set; }
        public string MoveTypeName { get; set; }
        public string AgentComputed { get; set; }

        public string LocalCustomsTransmissionsStatusCode { get; set; }
        public string LocalCustomsTransmissionsStatusName { get; set; }
        public string LocalCustomsTransmissionsStatusError { get; set; }
        public DateTime? LocalCustomsTransmissionsStatusDate { get; set; }
        public DateTime? FinalArrivalDate { get; set; }
        public DateTime? EstimatedFinalArrivalDate { get; set; }
        public DateTime? ActualFinalArrivalDate { get; set; }

        #region Booking

        public double? OrderGrossWeight { get; set; }
        public double? BookingVolume { get; set; }
        public int? BookingNumberOfPackages { get; set; }
        public bool OrderIsDangerouseGoods { get; set; }
        public string BookingConfirmationNumber { get; set; }
        public string BookingConfirmedBy { get; set; }
        public string BookingConfirmationNotes { get; set; }
        public double? OrderVolumetricWeight { get; set; }
        public double? OrderChargeableWeight { get; set; }
        #endregion

        public string Routing { get; set; }

        #region AWB
        public string AWBCurrencyId { get; set; }
        public string AWBCurrencyCode { get; set; }

        public double? AWBFreightAmountPrepaid { get; set; }
        public double? AWBFreightAmountCollect { get; set; }

        public string AWBCarrierTarrifReference { get; set; }
        public string AWBDeclaredValueForCarriage { get; set; }
        public string AWBDeclaredValueForCustoms { get; set; }
        public string AWBAccountingInformation { get; set; }
        public string AWBInsurrenceValue { get; set; }
        public string AWBHandlingInformation { get; set; }
        public string SCI { get; set; }
        public bool AWBPrint { get; set; }
        public string CarrierLastStatusCode { get; set; }
        public string CarrierLastStatusName { get; set; }
        public DateTime? CarrierLastStatusDate { get; set; }
        public string FNAReason { get; set; }

        public string FHLStatusCode { get; set; }
        public string FHLStatusName { get; set; }
        public DateTime? FHLStatusDate { get; set; }

        public string FWBStatusCode { get; set; }
        public string FWBStatusName { get; set; }
        public DateTime? FWBStatusDate { get; set; }

        public string CargonautFHLStatusCode { get; set; }
        public string CargonautFHLStatusName { get; set; }
        public DateTime? CargonautFHLStatusDate { get; set; }

        public string CargonautFWBStatusCode { get; set; }
        public string CargonautFWBStatusName { get; set; }
        public DateTime? CargonautFWBStatusDate { get; set; }
        #endregion

        public string IssuingCarrierAgentId { get; set; }
        public string IncotermCode { get; set; }

        public bool AsAgreedFreight { get; set; }
        public bool AsAgreedOtherCharges { get; set; }
        public string AccountNumber { get; set; }
        public string MainCarriageFromCity { get; set; }
        public string MainCarriageFromCountryCode { get; set; }

        public string MainCarriageToCity { get; set; }
        public string MainCarriageToCountryCode { get; set; }

        public string CASSCode { get; set; }
        public string SpecialServicesTypeId { get; set; }
        public string SpecialServicesTypeName { get; set; }
        public bool ARInvoiceIssued { get; set; }
        public bool CreditNoteIssued { get; set; }

        public string ProductCode { get; set; }
        public int NumberOfInsidePackages { get; set; }
        public string NumberOfInsidePackagesDetails { get; set; }
        public string ManifestReason { get; set; }
        public string ManifestStatusCode { get; set; }
        public string AirlinePrefix { get; set; }

        public DateTime? OperationalCloseDate { get; set; }
        public DateTime? AccountingCloseDate { get; set; }
        public string CustomsDeclarationNumber { get; set; }
        public DateTime? OperationalDate { get; set; }
        public DateTime? CutoffDate { get; set; }
        public int NumberOfHouses { get; set; }

        public DateTime? FreightRelease { get; set; }
        public DateTime? TerminalAvailable { get; set; }
        public DateTime? Terminal2Available { get; set; }
        public string ISFNumber { get; set; }
        public DateTime? ISFDate { get; set; }
        public string ITNumber { get; set; }
        public DateTime? ITDate { get; set; }
        public DateTime? DocumentsClosingDate { get; set; }
        public string OBLTypeCode { get; set; }

        public string ENSNumber { get; set; }
        public DateTime? ENSDate { get; set; }
        public DateTime? RegistryDate { get; set; }
        public bool IsAssembly { get; set; }
        public DateTime? FirstOperationalCloseDate { get; set; }
        public DateTime? FirstAccountingCloseDate { get; set; }

        public string LastSharedEventId { get; set; }
        public string LastSharedEventName { get; set; }
        public string LastSharedEventLocation { get; set; }
        public string LastSharedEventNotes { get; set; }
        public DateTime? LastSharedEventDate { get; set; }

        public double? GrossWeightPerTon { get; set; }

        public string WarehouseLegWarehouseId { get; set; }
        public string WarehouseLegTerminalName { get; set; }
        public string WarehouseLegAddressId { get; set; }
        public string WarehouseLegTerminalCode { get; set; }
        public DateTime? WarehouseLegExpectedEntryDate { get; set; }
        public DateTime? WarehouseLegActualEntryDate { get; set; }
        public DateTime? WarehouseLegExpectedReleaseDate { get; set; }
        public DateTime? WarehouseLegActualReleaseDate { get; set; }
        public DateTime? WarehouseLegLastFreeDate { get; set; }
        public string WarehouseLegRemarks { get; set; }
        public string WarehouseLegReference { get; set; }
        public string WarehouseLeg2WarehouseId { get; set; }
        public string WarehouseLeg2TerminalName { get; set; }
        public string WarehouseLeg2AddressId { get; set; }
        public string WarehouseLeg2TerminalCode { get; set; }
        public DateTime? WarehouseLeg2ExpectedEntryDate { get; set; }
        public DateTime? WarehouseLeg2ActualEntryDate { get; set; }
        public DateTime? WarehouseLeg2ExpectedReleaseDate { get; set; }
        public DateTime? WarehouseLeg2ActualReleaseDate { get; set; }
        public string WarehouseLeg2Remarks { get; set; }
        public string WarehouseLeg2Reference { get; set; }
        #region followup fields

        public string FollowUpId { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string FollowUpNotes { get; set; }
        public string FollowUpTypeId { get; set; }
        public string FollowUpOwnerId { get; set; }
        public string FollowUpOwner { get; set; }
        public string FollowUpType { get; set; }
        #endregion

        public string INTTRASIError { get; set; }
        public DateTime? INTTRASIStatusDate { get; set; }
        public string INTTRASIStatusCode { get; set; }
        public string INTTRASIStatusName { get; set; }

        public string LastFinalDestination { get; set; }
        public DateTime? FirstPickupETD { get; set; }
        public DateTime? FirstPickupETA { get; set; }
        public DateTime? INTTRALastStatusDate { get; set; }
        public DateTime? ContainerLastStatusDate { get; set; }

        public string From { get; set; }
        public string To { get; set; }
        public string Origin { get; set; }
        public string DeclarationNumber { get; set; }
        public DateTime? DeclarationDate { get; set; }
        public string ARInvoices { get; set; }
        public double? NotInvoicedReceivablesAmount { get; set; }

        public DateTime? PreForwardingETD { get; set; }
        public DateTime? PreForwardingATD { get; set; }
        public DateTime? PreForwardingETA { get; set; }
        public DateTime? PreForwardingATA { get; set; }
        public DateTime? OnForwardingETD { get; set; }
        public DateTime? OnForwardingATD { get; set; }
        public DateTime? OnForwardingETA { get; set; }
        public DateTime? OnForwardingATA { get; set; }
        public bool IsStandalonePickupDelivery { get; set; }
        public string ParentShipmentNumber { get; set; }
        public string ParentShipmentType { get; set; }
        public string ParentShipmentDirectionId { get; set; }
        public bool IsHTSMissing { get; set; }
        public DateTime? PlannedCargoReadyDate { get; set; }
        public DateTime? ApprovedCargoReadyDate { get; set; }
        public string HandlerUserId { get; set; }
        public string HandlerUserName { get; set; }
        public string ShipperNotExporterReference1 { get; set; }
        public string ShipperNotExporterReference2 { get; set; }
        public bool HasUnassignedData { get; set; }
        public string OperationalStatusId { get; set; }
        public string OperationalStatusName { get; set; }
        public string DestinationWarehouseId { get; set; }
        public string DestinationWarehouseName { get; set; }
    }
}
