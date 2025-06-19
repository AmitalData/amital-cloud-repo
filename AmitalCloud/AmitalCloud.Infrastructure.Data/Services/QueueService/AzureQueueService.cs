using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Azure.Storage.Queues;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public class AzureQueueService : IQueueService
    {
        private int Tenant { get; set; }
        private string QueueCode { get; set; }
        private ServiceBusReceiver QueueClient { get; set; }
        private ServiceBusSender QueueSender { get; set; }
        private ServiceBusReceivedMessage CurrentMessage { get; set; }
        public void InitializeQueue(string queueCode, int tenant)
        {
            this.Tenant = tenant;
            this.QueueCode = queueCode;
            this.QueueClient = Communications.GetQueueClient(this.QueueCode);

            /*
            var client = new ServiceBusClient(connectionString);
            this.QueueClient = client.CreateReceiver(this.QueueCode);
            this.QueueSender = client.CreateSender(this.QueueCode);
            */
        }

        public void Send(Dictionary<string, string> messageValues, int tenant, TimeSpan? delayTime = null, string CustomerId = null, string BatchNumber = null, DateTime? NextRunDate = null)
        {
            using (TransactionScope scope = TransactionFactory.GetNewSerializableTransaction())
            {
                ServiceBusMessage message = new ServiceBusMessage();
                foreach (string key in messageValues.Keys)
                    message.ApplicationProperties[key] = messageValues[key];
                if (delayTime != null)
                {
                    message.ScheduledEnqueueTime = DateTime.UtcNow.Add(delayTime.Value);
                }

                this.QueueSender.SendMessageAsync(message).Wait();

                scope.Complete();
            }
        }

        public QueueResponse Receive(TimeSpan? serverWaitTime = null)
        {
            if (serverWaitTime == null) { serverWaitTime = TimeSpan.FromSeconds(5); }
            using (TransactionScope scope = TransactionFactory.GetNewSerializableTransaction())
            {
                QueueResponse response = new QueueResponse();
                if (this.CurrentMessage == null)
                {
                    ServiceBusReceivedMessage message = this.QueueClient.ReceiveMessageAsync(serverWaitTime.Value).Result;
                    if (message != null)
                    {
                        response.MessageId = message.MessageId;
                        response.MessageValues = message.ApplicationProperties.ToDictionary(pair => pair.Key, pair => (pair.Value != null ? pair.Value.ToString() : null));
                        response.RetryNumber = message.DeliveryCount;
                        this.CurrentMessage = message;
                    }
                }

                scope.Complete();

                return response;
            }
        }

        public QueueResponse ReceiveJournal(TimeSpan? serverWaitTime = null)
        {
            throw new NotImplementedException();
        }

        public void Complete()
        {
            using (TransactionScope scope = TransactionFactory.GetNewSerializableTransaction())
            {
                if (this.CurrentMessage != null)
                {
                    this.QueueClient.CompleteMessageAsync(this.CurrentMessage).Wait();
                    this.CurrentMessage = null;
                }

                scope.Complete();
            }
        }

        public void Delay(TimeSpan delayTime)
        {
            using (TransactionScope scope = TransactionFactory.GetNewSerializableTransaction())
            {
                if (this.CurrentMessage != null)
                {
                    this.QueueClient.AbandonMessageAsync(this.CurrentMessage).Wait();

                    ServiceBusMessage delayedMessage = new ServiceBusMessage(this.CurrentMessage);
                    delayedMessage.ScheduledEnqueueTime = DateTime.UtcNow.Add(delayTime);

                    this.QueueSender.SendMessageAsync(delayedMessage).Wait();
                    this.CurrentMessage = null;
                }
                scope.Complete();
            }
        }

        public void Return()
        {
            using (TransactionScope scope = TransactionFactory.GetNewSerializableTransaction())
            {
                if (this.CurrentMessage != null)
                {
                    this.QueueClient.AbandonMessageAsync(this.CurrentMessage).Wait();
                    this.CurrentMessage = null;
                }

                scope.Complete();
            }
        }


        public void CompleteAsFailed()
        {
            //throw new NotImplementedException();
        }

    }
}
