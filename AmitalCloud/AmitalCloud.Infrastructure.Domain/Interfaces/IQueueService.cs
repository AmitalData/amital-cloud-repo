using System;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IQueueService
    {
        void InitializeQueue(string queueCode, int tenant);
        void Send(Dictionary<string, string> messageValues, int tenant, TimeSpan? delayTime = null, string CustomerId = null, string BatchNumber = null, DateTime? NextRunDate = null);
        //QueueResponse Receive();
        QueueResponse Receive(TimeSpan? serverWaitTime = null);

        QueueResponse ReceiveJournal(TimeSpan? serverWaitTime = null);
        void Complete();
        void Delay(TimeSpan delayTime);
        void Return();
        void CompleteAsFailed();

    }

    public partial class QueueResponse
    {

        //public bool HasError { get; set; }
        //public string ErrorMessage { get; set; }
        public string MessageId { get; set; }
        public int RetryNumber { get; set; }
        public int Tenant { get; set; }
        public IDictionary<string, string> MessageValues { get; set; }
    }

    public partial class QueueResponse
    {
        public DateTime? MessageCreatedServerTime { get; set; }
    }

}
