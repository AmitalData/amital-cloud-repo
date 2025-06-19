using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Domain.EntityPOCOs
{
    public class LastRate
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string ForeignCurrencyId { get; set; }
        public string ForeignCurrencyCode { get; set; }
        public string ForeignCurrencyName { get; set; }
        public string BaseCurrencyId { get; set; }
        public string BaseCurrencyCode { get; set; }
        public DateTime? ValueDate { get; set; }
        public double? Rate { get; set; }
        public int? Unit { get; set; }
        public DateTime? LogDateTime { get; set; }
        public int? HistoryCount { get; set; }
        public string UpdatedByUderId { get; set; }
        public string UpdatedByUserName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
