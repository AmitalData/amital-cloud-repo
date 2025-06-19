using AmitalCloud.Infrastructure.Domain.BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class APInvoiceMultipleShipmentPM : BaseEntityPM
    {
        [Key]
        public string ShipmentId { get; set; }
        public string APInvoiceId { get; set; }
        public string House { get; set; }
        public string Master { get; set; }
        public string LongMaster { get; set; }
        public string ShipmentNumber { get; set; }
        public string PartnerType { get; set; }
        public string PartnerName { get; set; }
        public string ShipmentLevelCode { get; set; }
        public string MainCarriageCarrierName { get; set; }
        public double? SubTotalInLocalCurrency { get; set; }
        public double? SubTotalInInvoiceCurrency { get; set; }
        public double? ExpectedAmount { get; set; }
        public double? AccountedAmount { get; set; }
        public double? OpenAmount { get; set; }
        public double? TotalAmount { get; set; }
        public double? TotalVATAmount { get; set; }
        public int IndexOrder { get; set; }
        public DateTime? OperationalDate { get; set; }
        public string MainCarriageOrigin { get; set; }
        public string MainCarriageFinalDestination { get; set; }
        public double? ChargeableWeight { get; set; }
        public string Currency { get; set; }
        public double? TotalReceivables { get; set; }
        public double? Profit { get; set; }
        public double? GrossWeightInKG { get; set; }
        public double? VolumeinCBM { get; set; }
        public double? TotalAmountinLocalCurrency { get; set; }
        private List<string> totalVatsList;
        public List<string> TotalVatsList
        {
            get
            {
                if (totalVatsList == null)
                {
                    totalVatsList = new List<string>();
                }
                return totalVatsList;
            }
            set
            {
                totalVatsList = value;
            }
        }
    }
}
