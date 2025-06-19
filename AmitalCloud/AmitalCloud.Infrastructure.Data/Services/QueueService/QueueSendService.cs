using AmitalCloud.Infrastructure.Domain.DataContracts;
using System;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public class QueueSendService
    {
        private string _CorrelationId;

        string _SBQueueName;
        private QueueSendModel _QueueSendModel;


        public QueueSendService(string SBQueueName, string correlationId, QueueSendModel queueSendModel)
        {
            // TODO: Complete member initialization
            this._SBQueueName = SBQueueName;
            this._CorrelationId = correlationId;
            _QueueSendModel = queueSendModel;

        }
    }
    public class QueueSendModel
    {
        public QueueSendModel()
        {

        }
        //public int ProcessState { get; set; }



        public int Tenant { get; set; }

        public string InterfaceTypeCode { get; set; }

        //public bool DebugMode { get; set; }

        public TimeSpan? Delay { get; set; }

        string _DcaAnalyzeAggregateKey;

        public int? TenantPriority { get; set; }

        public string DcaAnalyzeAggregateKey
        {
            get { return _DcaAnalyzeAggregateKey; }
            set { _DcaAnalyzeAggregateKey = value; }
        }

        public bool UseRabbitMQ { get; set; }
        public string QueueGroupCodeRabbit { get; set; }
        public string EntityCode { get; set; }
        public string EntityId { get; set; }

    }
    public class RabbitQueueCodeService
    {
        public static string GetRabbitQueueCode(string QueueDefinitionCode, string QueueGroupCodeRabbit)
        {
            string env = GetEnv();
            string myQueueCodeRabbit = QueueDefinitionCode;// $"AN_{env}_{this.QueueCode}";
            if (!string.IsNullOrEmpty(QueueGroupCodeRabbit))
            {
                myQueueCodeRabbit = $"{myQueueCodeRabbit}_{QueueGroupCodeRabbit}";
            }
            myQueueCodeRabbit += "_" + env;
            return myQueueCodeRabbit.ToLower();
        }

        private static string GetEnv()
        {
            var uri = new Uri(AmitalCloudSettings.AmitalURL);
            var branchEnv = uri.LocalPath.Trim(@"\"[0]).Trim(@"/"[0]);
            return branchEnv;
        }
    }

}
