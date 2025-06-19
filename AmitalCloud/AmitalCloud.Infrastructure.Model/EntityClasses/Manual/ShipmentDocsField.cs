using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentDocsField
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public bool IsPODReceived { get; set; }
        public DateTime? PODReceivedDate { get; set; }
        public bool IsCommercialInvoiceReceived { get; set; }
        public DateTime? CommercialInvoiceReceivedDate { get; set; }
        public bool IsPackingListReceived { get; set; }
        public DateTime? PackingListReceivedDate { get; set; }
        public bool IsBOLReceived { get; set; }
        public DateTime? BOLReceivedDate { get; set; }
        public bool IsMasterBOLReceived { get; set; }
        public DateTime? MasterBOLReceivedDate { get; set; }
        public bool IsArrivalNoticeReceived { get; set; }
        public DateTime? ArrivalNoticeReceivedDate { get; set; }

        //public Shipment Shipment { get; set; }
    }
}
