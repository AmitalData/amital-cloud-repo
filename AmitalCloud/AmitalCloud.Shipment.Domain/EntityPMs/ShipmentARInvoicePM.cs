using AmitalCloud.Infrastructure.Domain.BaseClasses;
using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    public partial class ShipmentARInvoicePM : BaseEntityPM
    {
        [Key]
        public string Id { get; set; }
        public string ShipmentId { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceTypeCode { get; set; }
        public string InvoiceTypeName { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public bool IsAutoCredit { get; set; }
        public bool IsCancelled { get; set; }
        public string InvoiceCurrencyId { get; set; }
        public string InvoiceCurrencyCode { get; set; }
        public string InvoiceLocalCurrencyCode { get; set; }
        public double? AmountDue { get; set; }
        public double? AmountInLocalCurrency { get; set; }
        public double? AmountInProfitCurrency { get; set; }
        public double? AmountInInvoiceCurrency { get; set; }
        public bool IsConstituentInvoice { get; set; }
        public bool IsConsolidationInvoice { get; set; }
        public string ConsolidationInvoiceId { get; set; }
        public string ConsolidationInvoiceNumber { get; set; }
        public string ReportUrl { get; set; }
        public bool IsDigitalDueDateColorRed { get; set; }
    }
}