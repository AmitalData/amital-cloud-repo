using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
    public class ShipmentAdditionalCloudData
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string DeclarationXmlData { get; set; }
        public bool IsImporterApprovalRequried { get; set; }
        public string ApprovedByUserName { get; set; }
        public DateTime? ApproveDateTime { get; set; }
        public string VersionApproved { get; set; }
        public string DenyReason { get; set; }
        public string ShipmentAddtionalDataXML { get; set; }
        public Shipment Shipment { get; set; }
        public bool SendUpdatesToAgentEnabled { get; set; }
        public bool DocsSentToAgent { get; set; }

        public bool IsPaymentRequired { get; set; }
        public string PaymentRequestXML { get; set; }
        public DateTime? PaymentDateTime { get; set; }
        public string DeclarationWCOXml { get; set; }

        public bool IsUserIDNumberRequired { get; set; }
        public DateTime? UserIdNumberUpdateDate { get; set; }
        public string UserIdNumberXMLData { get; set; }
        public string UserIdNumber { get; set; }
        public string DocumentsApprovedByUserName { get; set; }
        public DateTime? DocumentInspection { get; set; }
        public DateTime? GatepassDocumentsReady { get; set; }
        public DateTime? GoodsClassification { get; set; }

        public DateTime? PaymentRequestDateTime { get; set; }
        public DateTime? DenyDate { get; set; }
        public DateTime? InvoiceIssuedDate { get; set; }
        public bool UserAcceptSaveID { get; set; }

        /*
                 IsUserIDNumberRequired (Bit)
        UserIdNumberUpdateDate (Date)
        UserIdNumberXMLData (VCMax)
        UserIdNumber (VC35)
         */

    }
}
