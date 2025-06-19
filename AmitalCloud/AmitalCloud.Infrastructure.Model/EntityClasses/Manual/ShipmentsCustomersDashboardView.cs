using System;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentsCustomersDashboardView
    {
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string ShipmentNumber { get; set; }
        public string BranchId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime OperationalDate { get; set; }
        public string TransportModeId { get; set; }
        public string DirectionId { get; set; }
        public bool IsCancelled { get; set; }
        public double? OpenReceivablesInLocalCurrency { get; set; }
        public double? AccountedReceivablesInLocalCurrency { get; set; }
        public double? ProfitInLocalCurrency { get; set; }
        public string CustomerId { get; set; }
        public double? ProfitInProfitCurrency { get; set; }
        public double? OpenReceivablesInProfitCurrency { get; set; }
        public double? AccountedReceivablesInProfitCurrency { get; set; }
        public string ShipmentLevelCode { get; set; }
        public double? ChargeableWeightInKG { get; set; }
        public double? GrossWeightInKG { get; set; }
        public string CustomerName { get; set; }
    }
}
