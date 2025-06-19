using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Infrastructure.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 
namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    public partial class ConsoleShipmentPM : BaseEntityPM
    {
        [Key]
        public string Id { get; set; }
        public bool IsLCL { get; set; }
        public bool IsFCL { get; set; }
        public string ShipmentNumber { get; set; }
        public string MasterShipmentDataId { get; set; }

        public string FHLStatusCode { get; set; }
        public string FHLStatusName { get; set; }
        public string CargonautFHLStatusCode { get; set; }
        public string CargonautFHLStatusName { get; set; }
        public string FNAReason { get; set; }

        public double? TEU { get; set; }
        public double? Volume { get; set; }
        public double? GrossWeight { get; set; }
        public double? ChargeableWeight { get; set; }
        public double? VolumetricWeight { get; set; }
        public double? GrossWeightInKG { get; set; }
        public double? GrossWeightPerTon { get; set; }
        public double? ValueOfGoods { get; set; }
        public double? FreightPayablesAmount { get; set; }
        public double? FreightReceivablesAmount { get; set; }

        public double? OAMTPayables_Local { get; set; }
        public double? ACCTPayables_Local { get; set; }
        public double? OAMTPayables_Profit { get; set; }
        public double? ACCTPayables_Profit { get; set; }

        public double? OAMTReceivables_Local { get; set; }
        public double? ACCTReceivables_Local { get; set; }
        public double? OAMTReceivables_Profit { get; set; }
        public double? ACCTReceivables_Profit { get; set; }

        public double? OAMTReceivables_Local_NoParent { get; set; }
        public double? ACCTReceivables_Local_NoParent { get; set; }
        public double? OAMTReceivables_Profit_NoParent { get; set; }
        public double? ACCTReceivables_Profit_NoParent { get; set; }

        public int? NumberOfPackages { get; set; }
        public int? NumberOfContainers { get; set; }

        public string House { get; set; }
        public string DescriptionOfGoods { get; set; }
        public string PreForwardingFromPortId { get; set; }
        public string PreForwardingToPortId { get; set; }
        public string OnForwardingFromPortId { get; set; }
        public string OnForwardingToPortId { get; set; }

        [Association("ConsoleShipmentContainerPackage", "Id", "ConsoleId")]
        public List<HouseContainerPackage> FCLDataList { get; set; }

        public ChangeSetOperation ChangeSetOp { get; set; }

        public double? GrossWeightPerStorageDays { get; set; }
    }

    public partial class HouseContainerPackage
    {
        [Key]
        public string Id { get; set; }
        public string ConsoleId { get; set; }
        public int? Quantity { get; set; }
    }
}