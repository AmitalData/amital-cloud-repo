using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentFinancial
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }

        public double OpenReceivablesInLocalCurrency { get; set; }
        public double AccountedReceivablesInLocalCurrency { get; set; }
        public double? OpenReceivablesInProfitCurrency { get; set; }
        public double? AccountedReceivablesInProfitCurrency { get; set; }

        public double OpenPayablesInLocalCurrency { get; set; }
        public double AccountedPayablesInLocalCurrency { get; set; }
        public double? OpenPayablesInProfitCurrency { get; set; }
        public double? AccountedPayablesInProfitCurrency { get; set; }

        public double ProfitInLocalCurrency { get; set; }
        public double? EstimateProfitInLocalCurrency { get; set; }

        public double? ProfitInProfitCurrency { get; set; }
        public double? EstimateProfitInProfitCurrency { get; set; }


    }
}