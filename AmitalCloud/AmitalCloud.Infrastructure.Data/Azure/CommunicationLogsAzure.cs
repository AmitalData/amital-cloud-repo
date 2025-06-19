
namespace AmitalCloud.Infrastructure.Data.Azure
{
    public class CommunicationLogsAzure
    {
        public string Id { get; set; }
        public int Tenant { get; set; }

        public string CreatedByUserId { get; set; }

        public string CreatedByUserName { get; set; }

        public DateTime LocalCreateDateTime { get; set; }

        public DateTime GMTCreateDateTime { get; set; }

        public string Subject { get; set; }

        public DateTime SendDateTime { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string AttachedList { get; set; }

        public string BodyDocumentId { get; set; }

        public string StatusCode { get; set; } // Waiting,InProgress,Sent,Fail

        public bool IsBodyHtml { get; set; }


    }
}