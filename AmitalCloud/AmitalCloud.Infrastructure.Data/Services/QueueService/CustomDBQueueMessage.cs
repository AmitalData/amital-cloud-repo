using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public partial class CustomDBQueueMessage //: QueueResponse//Oracle Extention
    {
        //private QueueResponse q;
        private CustomDbQueueModel CustomDbQueueParams;

        public CustomDBQueueMessage(QueueResponse baseQueueResponse)
        {
            if (baseQueueResponse == null)
            {
                return;
            }

            MyQueueResponse = baseQueueResponse;

            this.MessageId = baseQueueResponse.MessageId;
            this.Retries = baseQueueResponse.RetryNumber;
            this.Properties = baseQueueResponse.MessageValues;
            this.MessageCreatedServerTime = baseQueueResponse.MessageCreatedServerTime;
        }

        public CustomDBQueueMessage(QueueResponse q, CustomDbQueueModel CustomDbQueueParams)
            : this(q)
        {
            // TODO: Complete member initialization

            this.CustomDbQueueParams = CustomDbQueueParams;
        }
        public QueueResponse MyQueueResponse { get; private set; }

        public DateTime? MessageCreatedServerTime { get; set; }
        public QueueStatusEnum QueueStatus { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public int Retries { get; set; }

        public string MessageId { get; set; }

        public bool SafeAbandon()
        {

            var myCustomDbQueueService = new CustomDbQueueService(CustomDbQueueParams, this);
            return myCustomDbQueueService.SafeAbandon();
        }
        public void SafeComplete()
        {
            var myCustomDbQueueService = new CustomDbQueueService(CustomDbQueueParams, this);
            myCustomDbQueueService.SafeComplete();
        }
    }

}
