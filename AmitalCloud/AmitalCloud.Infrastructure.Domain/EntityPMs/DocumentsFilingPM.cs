namespace AmitalCloud.Infrastructure.Domain.EntityPMs
{
    public partial class DocumentsFilingPM
    {

        public string InvoiceBillTo { get; set; }
        public bool FromCTool { get; set; }
        public bool IsApprovalRequired { get; set; }
        public bool IsFromDigital { get; set; }
        public bool IsUoloadedField { get; set; }
        public bool IsAttachment { get; set; }
        public bool IsHybrid { get; set; }
        public int FollowUpCount { get; set; }
        public bool HasFollowUp { get; set; }
        public string FollowUpId { get; set; }
    }
}
