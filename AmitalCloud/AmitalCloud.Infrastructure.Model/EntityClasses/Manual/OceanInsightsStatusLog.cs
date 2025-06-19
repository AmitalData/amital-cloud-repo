using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class OceanInsightsStatusLog
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string OceanInsigntRequestId { get; set; }
        public string XML { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
