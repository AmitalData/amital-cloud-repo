using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentAnalytic
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string ShipmentNumber { get; set; }
        public string BranchId { get; set; }
        public string IncotermId { get; set; }
        public string SalesmanUserId { get; set; }
        public string AccountManagerUserId { get; set; }
        public string ShipmentTypeId { get; set; }
        public string TransportModeId { get; set; }
        public string DirectionId { get; set; }
        public string ShipmentLevelCode { get; set; }
        public string ShipperId { get; set; }
        public string ConsigneeId { get; set; }
        public string AgentId { get; set; }
        public string CustomerId { get; set; }
        public bool IsOperationalClosed { get; set; }
        public bool IsAccountingClosed { get; set; }
        public double? GrossWeightInKG { get; set; }
        public double? ChargeableWeightInKG { get; set; }
        public double? VolumeInCBM { get; set; }
        public double? TEU { get; set; }
        public string StatusId { get; set; }
        public bool IsCancelled { get; set; }
        public double? OpenReceivablesInLocalCurrency { get; set; }
        public double? AccountedReceivablesInLocalCurrency { get; set; }
        public double ProfitInLocalCurrency { get; set; }
        public double? ProfitInProfitCurrency { get; set; }
        public double? OpenReceivablesInProfitCurrency { get; set; }
        public double? AccountedReceivablesInProfitCurrency { get; set; }
        public string MainCarriageFromPortId { get; set; }
        public string MainCarriageToPortId { get; set; }
        public double? OpenPayablesInLocalCurrency { get; set; }
        public double? AccountedPayablesInLocalCurrency { get; set; }
        public double? OpenPayablesInProfitCurrency { get; set; }
        public double? AccountedPayablesInProfitCurrency { get; set; }
        public bool ARInvoiceIssued { get; set; }
        public string ShipmentSubTypeId { get; set; }
        public string MainCarriageCarrierId { get; set; }
        public int? NumberOfPackages { get; set; }
        public int? NumberOfContainers { get; set; }
        public string FromCountryId { get; set; }
        public string ToCountryId { get; set; }
        public DateTime? OperationalDate { get; set; }
        public string ProductCode { get; set; }
    }
}
