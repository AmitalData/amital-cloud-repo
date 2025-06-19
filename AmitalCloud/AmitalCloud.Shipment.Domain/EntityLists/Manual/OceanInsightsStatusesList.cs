using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Shipment.Domain.EntityLists
{
    public class OceanInsightsStatusesList
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string OceanInsightsRequestId { get; set; }
        public string ContentDocumentId { get; set; }
        public string CommunicationLogId { get; set; }
        public DateTime CreateDate { get; set; }
        public string XML { get; set; }
     
    }
}
