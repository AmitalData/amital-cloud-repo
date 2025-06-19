using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Domain.EntityPMs
{
    public class SharedLogisticContactPM : BaseClasses.BaseEntityPM
    {
        [Key]
        public string Id { get; set; }
        public string CardId { get; set; }
        public string ContactId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool InternetAccess { get; set; }
        public string EnglishName { get; set; }
        public string Position { get; set; }
        public string BusinessPhone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsCargoTrackingInvitation { get; set; }
        public string TemplateId { get; set; }
        public bool IsDigitalPortal { get; set; }
        public string HTMLTemplate { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
    }
}