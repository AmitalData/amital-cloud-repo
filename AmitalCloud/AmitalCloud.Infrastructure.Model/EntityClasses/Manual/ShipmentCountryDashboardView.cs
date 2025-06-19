using System;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentCountryDashboardView
    {
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string ShipmentNumber { get; set; }
        public string DirectionId { get; set; }
        public string TransportModeId { get; set; }
        public string CountryForStatisticsId { get; set; }
        public string CountryForStatisticsCode { get; set; }
        public string CountryForStatisticsName { get; set; }
        public double? ChargeableWeightInKG { get; set; }
        public double? GrossWeightInKG { get; set; }
        public string CustomerId { get; set; }

        public DateTime CreateDateTime { get; set; }
        public DateTime OperationalDate { get; set; }

        public string ShipmentLevelCode { get; set; }

        public double? ProfitInLocalCurrency { get; set; }

        public double? ProfitInProfitCurrency { get; set; }

        public double? OpenReceivablesInLocalCurrency { get; set; }
        public double? OpenReceivablesInProfitCurrency { get; set; }
        public string BranchId { get; set; }
        public bool IsCancelled { get; set; }
    }
}
