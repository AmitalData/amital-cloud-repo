using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class MessagingStockDataView
    {
        [Key]
        public string Id { get; set; }
        public int TenantNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Amount { get; set; }
        public int? Remaining { get; set; }
        public bool IsCancelled { get; set; }
        public string Notes { get; set; }
        public string SearchFields { get; set; }
        public double? TotalPrice { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreatedByUserId { get; set; }
        public string UpdatedByUserId { get; set; }
        public string TenantName { get; set; }
        public string StockType { get; set; }
    }
}
