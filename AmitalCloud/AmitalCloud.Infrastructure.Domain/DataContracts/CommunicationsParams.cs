using System;
using System.Collections.Generic;
using System.Text;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class CommunicationsParams
    {

        public int Tenant { get; set; }

        public string InOut { get; set; }

        public string LoggingEntityId { get; set; }

        public string LoggingObjectTableId { get; set; }

        public string Subject { get; set; }

        public string LoggingUserId { get; set; }

        public string LoggingEntityReference { get; set; }

        public byte[] ByteData { get; set; }

        public string Status { get; set; }

        public string CommunicationLogTypeCode { get; set; }

        public string To { get; set; }

        public string From { get; set; }

        public string BCC { get; set; }

        public string CC { get; set; }

        public string FolderName { get; set; }

        private string xmlData;

        public string XMLData
        {
            get { return xmlData; }
            set
            {
                xmlData = value;
                //ITZIK hebrew/arbic not valid with ascii change to UTF8 !!!! ByteData = Encoding.ASCII.GetBytes(xmlData);
                ByteData = Encoding.UTF8.GetBytes(xmlData);
            }
        }

        public string Logs { get; set; }

        public string CorrelationID { get; set; }
        public DateTime? NextTryDateTime { get; set; }

        public DateTime CreateDateUTC { get; set; }

        public string QueueName { get; set; }
        public int Priority { get; set; }
        public string FileExtension { get; set; }

        public Dictionary<string, string> QueueParameters { get; set; }
        public string AdditionalFields { get; set; }
        public string ExceptionMessage { get; set; }
        public string UniqueNumber { get; set; }
        public bool? WasAnalyzed { get; set; }
    }

}
