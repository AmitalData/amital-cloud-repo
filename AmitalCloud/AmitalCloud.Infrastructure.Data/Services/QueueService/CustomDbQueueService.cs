using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Data.Services
{


    public class CustomDbQueueService : DbQueueService
    {

        CustomDbQueueModel CustomDbQueueParams;
        public CustomDbQueueService(string queueCode, int tenant,
            int lockDurationInMin = 2,
            int maxDeliveryCount = 50,
            int timeOutInHour = 24)
            : base(queueCode, tenant)
        {
            CustomDbQueueParams = new CustomDbQueueModel();
            CustomDbQueueParams.QueueCode = queueCode;
            CustomDbQueueParams.Tenant = tenant;
            CustomDbQueueParams.LockDuration = TimeSpan.FromMinutes(lockDurationInMin);
            CustomDbQueueParams.MaxDeliveryCount = maxDeliveryCount;
            CustomDbQueueParams.TimeOutInHour = timeOutInHour;
        }
        public CustomDbQueueService(string queueCode, int tenant,
           CustomDBQueueMessage customDBQueueMessage,
           int lockDurationInMin = 2,
           int maxDeliveryCount = 50,
           int timeOutInHour = 24)
           : base(queueCode, tenant)
        {
            CustomDbQueueParams = new CustomDbQueueModel();
            CustomDbQueueParams.QueueCode = queueCode;
            CustomDbQueueParams.Tenant = tenant;
            CustomDbQueueParams.LockDuration = TimeSpan.FromMinutes(lockDurationInMin);
            CustomDbQueueParams.MaxDeliveryCount = maxDeliveryCount;
            CustomDbQueueParams.TimeOutInHour = timeOutInHour;
            CurrentCustomQueueResponse = customDBQueueMessage;
            base.CurrentMessageId = CurrentCustomQueueResponse.MessageId;
        }
        public CustomDbQueueService(CustomDbQueueModel CustomDbQueueParams, CustomDBQueueMessage customDBQueueMessage)
            : base(CustomDbQueueParams.QueueCode, CustomDbQueueParams.Tenant)
        {
            // TODO: Complete member initialization
            this.CustomDbQueueParams = CustomDbQueueParams;
            CurrentCustomQueueResponse = customDBQueueMessage;
            base.CurrentMessageId = CurrentCustomQueueResponse.MessageId;
        }
        new private QueueResponse Receive() { throw new NotImplementedException(); }
        new private QueueResponse Receive(TimeSpan serverWaitTime) { throw new NotImplementedException(); }
        new private void Complete(string messageId) { throw new NotImplementedException(); }
        new public List<CustomDBQueueMessage> Receive_new(int? nextRunDelayInSec = null, int? selectCount = null)
        {
            CurrentCustomQueueResponse = null;
            nextRunDelayInSec = nextRunDelayInSec ?? (int)(CustomDbQueueParams.LockDuration.TotalSeconds);
            base.CurrentMessageId = null;
            var q = base.Receive_new(nextRunDelayInSec.Value, selectCount: selectCount);
            if (q == null || q.Count == 0)
            {
                return null;
            }
            List<CustomDBQueueMessage> CurrentCustomQueueResponseList = new List<CustomDBQueueMessage>();
            foreach (var item in q)
            {
                var currentCustomQueueResponse = new CustomDBQueueMessage(item, CustomDbQueueParams);
                currentCustomQueueResponse.QueueStatus = QueueStatusEnum.Received;
                LogMessagingUtil.Instance.AppendLine("CustomDbQueueService:Receive:DbQueueName=" + CustomDbQueueParams.QueueCode + ":QMId=" + base.CurrentMessageId);
                CurrentCustomQueueResponseList.Add(currentCustomQueueResponse);
            }
            return CurrentCustomQueueResponseList;
        }



        new public CustomDBQueueMessage Receive(int? nextRunDelayInSec = null)
        {


            //var r= new DualRepository()
            if (CurrentCustomQueueResponse != null && !String.IsNullOrWhiteSpace(CurrentCustomQueueResponse.MessageId) && CurrentCustomQueueResponse.QueueStatus == QueueStatusEnum.Received)
            {
                if (CustomDbQueueParams.MaxDeliveryCount < CurrentCustomQueueResponse.Retries)
                {
                    this.CompleteAsFailed();
                }
                else
                {
                    if (
                    DateTime.Now
                   // TO DO  get oracle SysDate 
                   .Subtract(CurrentCustomQueueResponse.MessageCreatedServerTime.Value) > TimeSpan.FromHours(CustomDbQueueParams.TimeOutInHour))
                    {
                        this.CompleteAsFailed();
                    }
                }
            }
            CurrentCustomQueueResponse = null;
            nextRunDelayInSec = nextRunDelayInSec ?? (int)(CustomDbQueueParams.LockDuration.TotalSeconds);
            base.CurrentMessageId = null;
            var q = base.Receive(nextRunDelayInSec.Value);
            if (q == null)
            {
                return null;
            }
            CurrentCustomQueueResponse = new CustomDBQueueMessage(q, CustomDbQueueParams);
            CurrentCustomQueueResponse.QueueStatus = QueueStatusEnum.Received;
            LogMessagingUtil.Instance.AppendLine("CustomDbQueueService:Receive:DbQueueName=" + CustomDbQueueParams.QueueCode + ":QMId=" + base.CurrentMessageId);
            return CurrentCustomQueueResponse;
        }


        new private void Send(Dictionary<string, string> messageValues, TimeSpan? delayTime = null, string CustomerId = null, string BatchNumber = null, DateTime? NextRunDate = null)
        { throw new NotImplementedException(); }

        public int? Send(Dictionary<string, string> messageValues, int tenant, TimeSpan? delayTime, /*int tenantPriority, */
            QueueSendModel queueSendModel = null)
        {
            //int tenantPriority=8;
            //LogMessagingUtil.Instance.AppendLine($"SendCommunicationLogMessageToQueue(${queueName},UseRabbitMQ={UseRabbitMQ})");
            var queueId = base.SendReturnId(messageValues, tenant, delayTime, null, null, null, /*tenantPriority,*/ queueSendModel);
            LogMessagingUtil.Instance.AppendLine("CustomDbQueueService:CreateNew:DbQueueName=" + CustomDbQueueParams.QueueCode + ":QMId=" + queueId + ":UseRabbitMQ=" + queueSendModel?.UseRabbitMQ);
            return queueId;
        }

        public void SafeComplete()
        {
            this.Complete();
            CurrentCustomQueueResponse.QueueStatus = QueueStatusEnum.Complete;
            LogMessagingUtil.Instance.AppendLine("CustomDbQueueService:SafeComplete:DbQueueName=" + CustomDbQueueParams.QueueCode + "QMId=" + base.CurrentMessageId);

        }
        public bool SafeAbandon()
        {
            bool safcomplete = false;
            if (CurrentCustomQueueResponse.Retries > 10)
            {
                this.SafeComplete();
                safcomplete = true;
            }
            else
            {
                if (CurrentCustomQueueResponse.MessageCreatedServerTime.HasValue)
                {
                    if (DateTime.UtcNow.Subtract(CurrentCustomQueueResponse.MessageCreatedServerTime.GetValueOrDefault())
                        > TimeSpan.FromHours(12))
                    {
                        this.SafeComplete();
                        safcomplete = true;
                    }
                }
            }
            if (!safcomplete)
            {
                if (CurrentCustomQueueResponse.Retries < 8)
                {
                    this.Delay(TimeSpan.FromMinutes(1));
                }
                else
                {
                    this.Delay(TimeSpan.FromMinutes(10));
                }

            }

            //else if (CurrentCustomQueueResponse.Retries > 10)
            //{
            //    this.Delay(TimeSpan.FromMinutes(60));
            //}
            //else if (CurrentCustomQueueResponse.Retries > 5)
            //{
            //    this.Delay(TimeSpan.FromMinutes(10));
            //}
            //this.SafeComplete();

            CurrentCustomQueueResponse.QueueStatus = QueueStatusEnum.DeadLetter;
            LogMessagingUtil.Instance.AppendLine("CustomDbQueueService:SafeAbandon:DbQueueName=" + CustomDbQueueParams.QueueCode + "QMId=" + base.CurrentMessageId);
            //this.Return();
            return safcomplete;
        }
        public CustomDBQueueMessage CurrentCustomQueueResponse { get; set; }

        //public CustomDBQueueMessage GetRabbitMQPseudoByMessageId(long messageId)
        //{
        //    var messagesRepository = new QueueMessageRepository(Tenant);

        //    var q = messagesRepository.GetSingleQueueMessage(messageId.ToString());
        //    if (q?.Id == null)
        //    {
        //        return null;
        //    }
        //    Dictionary<string, string> messageValues = DictionaryJsonConverter.FromJsonToDictionary(q.MessageBody);
        //    ;
        //    QueueResponse myQueueResponse = new QueueResponse()
        //    {
        //        MessageId = messageId.ToString(),
        //        RetryNumber = q.RetryNumber,
        //        MessageValues = messageValues

        //    };
        //    CurrentCustomQueueResponse = new CustomDBQueueMessage(myQueueResponse, CustomDbQueueParams);
        //    CurrentCustomQueueResponse.QueueStatus = QueueStatusEnum.Received;
        //    LogMessagingUtil.Instance.AppendLine("Rabbit:CustomDbQueueService:Receive:DbQueueName=" + CustomDbQueueParams.QueueCode + ":QMId=" + base.CurrentMessageId);
        //    return CurrentCustomQueueResponse;
        //}


        //public static void SendCommunicationLogMessageToQueue(string queueName, string communicationLogId, int tenant, bool UseRabbitMQ, int? tenantPriority = null)
        //{
        //    try
        //    {

        //        SendCommunicationLogMessageToQueue(queueName, 
        //            new Dictionary<string, string>() { { "CommunicationLogId", communicationLogId }, { "Tenant", tenant.ToString() } }, tenant, 
        //            UseRabbitMQ,
        //            "CommunicationLog".ToLower(),
        //            communicationLogId
        //            ,tenantPriority 
        //            );
        //        //IQueueService queueservice = new DbQueueService();
        //        //queueservice.InitializeQueue(queueName, 0);
        //        //queueservice.Send(new Dictionary<string, string>() { { "CommunicationLogId", communicationLogId }, { "Tenant", tenant.ToString() } }, tenant);

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.HandleException(ex, DateTime.Now, 0, null, $"SendCommunicationLogMessageToQueue{queueName}", null, null);
        //    }
        //}

        public static bool IsFeatureOnRABBITMQ_Communication()
        {
            int Tenant = 1;
            //INSERT INTO "TOGGLES" (CODE, NAME, SEARCHFIELDS) VALUES ('MQC', 'RABBITMQ Communication', 'MQC,RABBITMQ Communication')
            //INSERT INTO "FEATURETOGGLES"(ID, TENANT, CREATEDATE, CREATEDBYUSERID, UPDATEDATE, UPDATEDBYUSERID, SEARCHFIELDS, TENANTNUMBER, INACTIVE, TOGGLECODE)
            //                    VALUES('MQC', '1', TO_TIMESTAMP('2022-03-01 14:19:28.729000000', 'YYYY-MM-DD HH24:MI:SS.FF'), '1-9', TO_TIMESTAMP('2022-03-01 14:19:46.456000000', 'YYYY-MM-DD HH24:MI:SS.FF'), '1-9', 'MQC', '1', '0', 'MQC')

            ///Bug 75132: העלאת מסמך ללא קישור - מסמך נשלח למכס מס' פעמים
            return Helpers.FeatureToggleHelper.HasFeatureToggle("MQC", Tenant);
        }

        static List<string> _SupportedRabbitMQList = new List<string>() {
            SBQueueNames.SendWEBAPIMessage2MamanQ.ToString() ,
            SBQueueNames.AnalyzeQueueMQ.ToString() ,
            SBQueueNames.SendDataToExternalServicesBQ.ToString() ,
        };
        public static List<string> SupportedRabbitMQList { get { return _SupportedRabbitMQList; } }

        public static void SendCommunicationLogMessageToQueue(string queueName, Dictionary<string, string> messageValues, int tenant,
            bool UseRabbitMQ,
            string entityCode, string entityId
, int? tenantPriority)
        {
            try
            {



                UseRabbitMQ = IsFeatureOnRABBITMQ_Communication() && UseRabbitMQ && SupportedRabbitMQList.Contains(queueName);
                //LogMessagingUtil.Instance.AppendLine($"SendCommunicationLogMessageToQueue(${queueName},UseRabbitMQ={UseRabbitMQ})");


                var customDbQueueService = new CustomDbQueueService(queueName, tenant);
                customDbQueueService.Send(messageValues, tenant, null,
                    new QueueSendModel()
                    {
                        UseRabbitMQ = UseRabbitMQ,
                        EntityCode = entityCode,
                        EntityId = entityId,

                        TenantPriority = tenantPriority ?? 89,


                    });
                //var queueservice = new DbQueueService();
                //queueservice.InitializeQueue(queueName, 0);
                //queueservice.Send(messageValues, tenant);

            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, DateTime.Now, 0, null, $"SendCommunicationLogMessageToQueue{queueName}", null, null);
            }
        }
    }
    public class CustomDbQueueModel
    {

        public TimeSpan LockDuration { get; set; }

        public int MaxDeliveryCount { get; set; }

        public int TimeOutInHour { get; set; }

        public string QueueCode { get; set; }

        public int Tenant { get; set; }
    }
}
