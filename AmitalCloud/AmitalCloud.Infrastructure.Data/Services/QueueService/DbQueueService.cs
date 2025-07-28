using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Transactions;
using System.Web;
using AmitalCloud.Infrastructure.Model.Enums;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public partial class DbQueueService : IQueueService
    {
        protected int Tenant { get; set; }
        protected string QueueCode { get; set; }
        protected string CurrentMessageId { get; set; }
        private const int messageBodyLength = 2000;
        [ThreadStatic] public static int? MessageID = null;
        public DbQueueService()
        {

        }

        public DbQueueService(string queueCode, int tenant)
        {
            this.InitializeQueue(queueCode, tenant);
        }


        public void InitializeQueue(string queueCode, int tenant)
        {
            if (!String.IsNullOrWhiteSpace(AmitalCloudSettings.DebugKey))
            {
                queueCode += @"\" + AmitalCloudSettings.DebugKey;
            }
            else
            {

            }
            this.Tenant = tenant;
            this.QueueCode = queueCode;
            using (var uow = new UnitOfWork(tenant))
            {
                //IRepository<IAmitalCloudContext,QueueMessage, string> messagesRepository = new Repository<QueueMessage, string>(uow);
                //IRepository<IAmitalCloudContext,QueueMessageMoreDetails, string> messagesMoreDetailsRepository = new Repository<QueueMessageMoreDetails, string>(uow);
                IRepository<QueueDefinition> queueDefRep = new Repository<QueueDefinition>(uow);
                QueueDefinition queueDefinition = GetQueueDefFromCache(queueCode, queueDefRep);
                queueDefinition = queueDefinition ?? queueDefRep.GetMulti(d => d.Code == queueCode).FirstOrDefault();//ensure
                if (queueDefinition == null)
                {
                    queueDefinition = new QueueDefinition() { Code = queueCode, Name = queueCode };
                    queueDefRep.Insert(queueDefinition);
                    uow.Save();
                    string key = $"GetQueueDefFromCache({queueCode})";
                }
            }

        }

        private static QueueDefinition GetQueueDefFromCache(string queueCode, IRepository<QueueDefinition> queueDefRep)
        {
            string key = $"GetQueueDefFromCache({queueCode})";
            var def = CacheManager.GetOrInsertNewObject<QueueDefinition>(key, () =>
            {
                return queueDefRep.GetMulti(d => d.Code == queueCode).FirstOrDefault();
            }, donotCacheNull: true);
            return def;
        }

        public void Send(Dictionary<string, string> messageValues, int tenant, TimeSpan? delayTime = null, string CustomerId = null, string BatchNumber = null, DateTime? NextRunDate = null)
        {
            SendReturnId(messageValues, tenant, delayTime, CustomerId, BatchNumber, NextRunDate);
        }
        protected int? SendReturnId(Dictionary<string, string> messageValues,
            int tenant, TimeSpan? delayTime = null, string CustomerId = null, string BatchNumber = null, DateTime? NextRunDate = null//,int tenantPriority = 89
            , QueueSendModel queueSendModel = null)
        {
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                return SendReturnIdCustoms(messageValues,
            tenant, delayTime, CustomerId, BatchNumber, NextRunDate
            , queueSendModel);
            }


            int tenantPriority = queueSendModel?.TenantPriority ?? 89;
            if (tenantPriority < 1)
            {
                tenantPriority = 89;
            }
            else if (tenantPriority > 99)
            {
                tenantPriority = 89;
            }
            int? queueMessageId = null;
            if (AmitalCloudSettings.IsCostomsDeploy) //ITZIK + YARON 
            {
                if (this.QueueCode == "ImportersShipmentDocumentsQueue")
                {
                    // i was try to avoid in Up Code - but Failed -
                    // due that unfourtuntlly - i finally Eliminated here !!
                    return null;
                }
            }
            if (delayTime != null)
            {
                LogMessagingUtil.Instance.AppendLine($"Delay  {this.QueueCode} Queue {delayTime.GetValueOrDefault()}");
            }
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                string messageBody = DictionaryJsonConverter.FromDictionaryToJson(messageValues);
                string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
                string bodyHashCode = MD5HashUtil.GenerateHashForString(messageBody);

                QueueResponse response = new QueueResponse();
                DataTable tblQueue = new DataTable();//
                int delaySeconds = 0;
                string CId = "";
                string BNo = "";
                if (delayTime != null)
                {
                    delaySeconds = (int)delayTime.Value.TotalSeconds;
                }
                else
                {
                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {
                        if (NextRunDate.HasValue)
                        {
                            if (DateTime.UtcNow > NextRunDate)
                            {
                                delaySeconds = 0;

                            }
                            else
                            {
                                var ts = NextRunDate.Value.Subtract(DateTime.UtcNow);
                                delaySeconds = (int)ts.TotalSeconds;
                            }
                            NextRunDate = null;
                        }
                    }

                }
                if (CustomerId != null)
                {
                    CId = CustomerId;
                }
                if (BatchNumber != null)
                {
                    BNo = BatchNumber;
                }
                if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                {
                    if (NextRunDate.HasValue)
                    {
                        throw new Exception("Opps  in oracle use by delayInSec Params ");
                    }
                    using (OracleConnection cn = new OracleConnection(strConnString))
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = cn;
                        cmd.CommandText =
                            DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_Enqueue", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                        cmd.CommandType = CommandType.StoredProcedure;


                        OracleParameter queueCodePar = new OracleParameter("QueueDefinitionCode", OracleDbType.Varchar2, 255);
                        OracleParameter msgBodyPar = new OracleParameter("v_MessageBody", OracleDbType.Varchar2, 1000);
                        OracleParameter tenantPar = new OracleParameter("Tenant", OracleDbType.Int32);
                        OracleParameter delayPar = new OracleParameter("v_DelaySeconds", OracleDbType.Int32);
                        OracleParameter customerId = new OracleParameter("v_CustomerId", OracleDbType.Varchar2, 15);
                        OracleParameter batchNumber = new OracleParameter("v_BatchNumber", OracleDbType.Varchar2, 15);
                        OracleParameter hashCodePar = new OracleParameter("v_HashCode", OracleDbType.NVarchar2, 1000);
                        //OracleParameter NextRunDateTime = new OracleParameter("NextRunDate", OracleDbType.Date);

                        OracleParameter queueMessageIdPar = new OracleParameter("v_QueueMessageId", OracleDbType.Int32);
                        queueMessageIdPar.Direction = ParameterDirection.Output;

                        OracleParameter watingStatusPar = new OracleParameter("v_WatingStatus", OracleDbType.Int32);
                        watingStatusPar.Direction = ParameterDirection.Input;

                        OracleParameter tenantPriPar = new OracleParameter("p_TenantPriority ", OracleDbType.Int32);
                        tenantPriPar.Direction = ParameterDirection.Input;

                        OracleParameter InterfaceTypeCodePar = new OracleParameter("p_InterfaceTypeCode", OracleDbType.Varchar2, 32);
                        InterfaceTypeCodePar.Direction = ParameterDirection.Input;

                        OracleParameter UseRabbitMQPar = new OracleParameter("p_UseRabbitMQ", OracleDbType.Int32);
                        UseRabbitMQPar.Direction = ParameterDirection.Input;


                        OracleParameter QueueCodeRabbitPar = new OracleParameter("p_QueueCodeRabbit", OracleDbType.Varchar2, 256);
                        QueueCodeRabbitPar.Direction = ParameterDirection.Input;

                        OracleParameter entityCodePar = new OracleParameter("p_EntityCode", OracleDbType.Varchar2, 40);
                        entityCodePar.Direction = ParameterDirection.Input;

                        OracleParameter entityIdPar = new OracleParameter("p_EntityId", OracleDbType.Varchar2, 40);
                        entityIdPar.Direction = ParameterDirection.Input;


                        queueCodePar.Direction = ParameterDirection.Input;
                        msgBodyPar.Direction = ParameterDirection.Input;
                        tenantPar.Direction = ParameterDirection.Input;
                        delayPar.Direction = ParameterDirection.Input;
                        customerId.Direction = ParameterDirection.Input;
                        batchNumber.Direction = ParameterDirection.Input;
                        hashCodePar.Direction = ParameterDirection.Input;
                        //NextRunDateTime.Direction = ParameterDirection.Input;

                        queueCodePar.Value = this.QueueCode;
                        msgBodyPar.Value = messageBody;
                        tenantPar.Value = tenant;
                        delayPar.Value = delaySeconds;
                        customerId.Value = CId;
                        batchNumber.Value = BNo;
                        watingStatusPar.Value = WorkerNameService.GetWorkerWaitingStatusForSending(tenant);
                        //NextRunDateTime.Value = NextRunDate;
                        hashCodePar.Value = bodyHashCode;
                        tenantPriPar.Value = tenantPriority;
                        InterfaceTypeCodePar.Value = queueSendModel?.InterfaceTypeCode;
                        if (queueSendModel != null && queueSendModel.UseRabbitMQ)
                        {
                            UseRabbitMQPar.Value = 1;
                        }
                        else
                        {
                            UseRabbitMQPar.Value = 0;
                        }

                        string myQueueCodeRabbit = RabbitQueueCodeService.GetRabbitQueueCode(this.QueueCode, queueSendModel?.QueueGroupCodeRabbit);
                        QueueCodeRabbitPar.Value = myQueueCodeRabbit.ToLower();
                        entityCodePar.Value = queueSendModel?.EntityCode;
                        entityIdPar.Value = queueSendModel?.EntityId;


                        cmd.Parameters.Add(queueCodePar);
                        cmd.Parameters.Add(msgBodyPar);
                        cmd.Parameters.Add(tenantPar);
                        cmd.Parameters.Add(delayPar);
                        cmd.Parameters.Add(customerId);
                        cmd.Parameters.Add(batchNumber);
                        cmd.Parameters.Add(hashCodePar);
                        cmd.Parameters.Add(watingStatusPar);
                        cmd.Parameters.Add(tenantPriPar);

                        cmd.Parameters.Add(InterfaceTypeCodePar);
                        cmd.Parameters.Add(UseRabbitMQPar);
                        cmd.Parameters.Add(QueueCodeRabbitPar);
                        cmd.Parameters.Add(entityCodePar);
                        cmd.Parameters.Add(entityIdPar);


                        cmd.Parameters.Add(queueMessageIdPar);




                        //cmd.Parameters.Add(NextRunDateTime);

                        try
                        {
                            cn.Open();
                            var output = cmd.ExecuteNonQuery();
                            cn.Close();
                            var v_QueueMessageId = cmd.Parameters["v_QueueMessageId"].Value;
                            if (v_QueueMessageId != null)
                            {
                                string sQueueMessageId = v_QueueMessageId.ToString();
                                if (!String.IsNullOrWhiteSpace(sQueueMessageId))
                                {
                                    queueMessageId = sQueueMessageId.ChangeValue<int>();
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Exception: {0}", ex.ToString());
                            throw;
                        }

                        cn.Close();
                    }


                }
                else
                {
                    using (SqlConnection cn = new SqlConnection(strConnString))
                    {
                        SqlCommand cmd = new SqlCommand("[dbo].[Queue_Enqueue]", cn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter queueCodePar = new SqlParameter("@V_QueueDefinitionCode", SqlDbType.VarChar, 255);
                        SqlParameter msgBodyPar = new SqlParameter("@V_MessageBody", SqlDbType.VarChar, messageBodyLength);
                        SqlParameter tenantPar = new SqlParameter("@V_Tenant", SqlDbType.Int);
                        SqlParameter delayPar = new SqlParameter("@V_DelaySeconds", SqlDbType.Int);
                        SqlParameter customerId = new SqlParameter("@V_CustomerId", SqlDbType.VarChar, 15);
                        SqlParameter batchNumber = new SqlParameter("@V_BatchNumber", SqlDbType.VarChar, 15);
                        //  SqlParameter NextRunDateTime = new SqlParameter("@NextRunDTime", SqlDbType.DateTime);
                        SqlParameter hashCodePar = new SqlParameter("@V_HashCode", SqlDbType.NVarChar, 1000);
                        SqlParameter watingStatusPar = new SqlParameter("@V_WatingStatus", SqlDbType.Int);
                        SqlParameter messageIdPar = new SqlParameter("@V_QUEUEMESSAGEID", SqlDbType.BigInt);
                        SqlParameter tenantpriorityPar = new SqlParameter("@P_TENANTPRIORITY", SqlDbType.Int);
                        SqlParameter interfaceTypeCodePar = new SqlParameter("@P_INTERFACETYPECODE", SqlDbType.VarChar, 255);
                        SqlParameter useRabbitMQPar = new SqlParameter("@P_USERABBITMQ", SqlDbType.Bit);
                        SqlParameter queueCodeRabbitPar = new SqlParameter("@P_QUEUECODERABBIT", SqlDbType.VarChar, 255);
                        SqlParameter entityCodePar = new SqlParameter("@P_ENTITYCODE", SqlDbType.VarChar, 255);
                        SqlParameter entityIPar = new SqlParameter("@P_ENTITYID", SqlDbType.VarChar, 255);


                        queueCodePar.Direction = ParameterDirection.Input;
                        msgBodyPar.Direction = ParameterDirection.Input;
                        tenantPar.Direction = ParameterDirection.Input;
                        delayPar.Direction = ParameterDirection.Input;
                        customerId.Direction = ParameterDirection.Input;
                        batchNumber.Direction = ParameterDirection.Input;
                        //   NextRunDateTime.Direction = ParameterDirection.Input;
                        hashCodePar.Direction = ParameterDirection.Input;
                        tenantpriorityPar.Direction = ParameterDirection.Input;
                        watingStatusPar.Direction = ParameterDirection.Input;
                        interfaceTypeCodePar.Direction = ParameterDirection.Input;
                        useRabbitMQPar.Direction = ParameterDirection.Input;
                        queueCodeRabbitPar.Direction = ParameterDirection.Input;
                        entityCodePar.Direction = ParameterDirection.Input;
                        entityIPar.Direction = ParameterDirection.Input;

                        messageIdPar.Direction = ParameterDirection.Output;

                        queueCodePar.Value = this.QueueCode;
                        msgBodyPar.Value = messageBody;
                        tenantPar.Value = tenant;
                        delayPar.Value = delaySeconds;
                        customerId.Value = CId;
                        batchNumber.Value = BNo;
                        //    NextRunDateTime.Value = NextRunDate;
                        hashCodePar.Value = bodyHashCode;
                        watingStatusPar.Value = WorkerNameService.GetWorkerWaitingStatusForSending(tenant);
                        tenantpriorityPar.Value = tenantPriority;
                        interfaceTypeCodePar.Value = queueSendModel?.InterfaceTypeCode != null ? queueSendModel?.InterfaceTypeCode : "";

                        if (queueSendModel != null && queueSendModel.UseRabbitMQ)
                        {
                            useRabbitMQPar.Value = 1;
                        }
                        else
                        {
                            useRabbitMQPar.Value = 0;
                        }

                        string myQueueCodeRabbit = RabbitQueueCodeService.GetRabbitQueueCode(this.QueueCode, queueSendModel?.QueueGroupCodeRabbit);
                        queueCodeRabbitPar.Value = myQueueCodeRabbit.ToLower();

                        entityCodePar.Value = queueSendModel?.EntityCode;
                        entityIPar.Value = queueSendModel?.EntityId;


                        cmd.Parameters.Add(queueCodePar);
                        cmd.Parameters.Add(msgBodyPar);
                        cmd.Parameters.Add(tenantPar);
                        cmd.Parameters.Add(delayPar);
                        cmd.Parameters.Add(customerId);
                        cmd.Parameters.Add(batchNumber);
                        //   cmd.Parameters.Add(NextRunDateTime);
                        cmd.Parameters.Add(hashCodePar);
                        cmd.Parameters.Add(watingStatusPar);
                        cmd.Parameters.Add(messageIdPar);
                        cmd.Parameters.Add(tenantpriorityPar);
                        cmd.Parameters.Add(interfaceTypeCodePar);
                        cmd.Parameters.Add(useRabbitMQPar);
                        cmd.Parameters.Add(queueCodeRabbitPar);
                        cmd.Parameters.Add(entityCodePar);
                        cmd.Parameters.Add(entityIPar);

                        cn.Open();
                        var output = cmd.ExecuteNonQuery();
                        cn.Close();

                        var v_QueueMessageId = cmd.Parameters["@V_QUEUEMESSAGEID"].Value;
                        if (v_QueueMessageId != null)
                        {
                            string sQueueMessageId = v_QueueMessageId.ToString();
                            if (!String.IsNullOrWhiteSpace(sQueueMessageId))
                            {
                                queueMessageId = sQueueMessageId.ChangeValue<int>();
                                AddQueueDetailsToRequestHeaders(messageBody, sQueueMessageId);
                            }
                        }

                    }
                }

                scope.Complete();
            }



            return queueMessageId;
        }


        private static void AddQueueDetailsToRequestHeaders(string messageBody, string sQueueMessageId)
        {
            if (HttpContextHelper.Request != null)
            {
                if (string.IsNullOrEmpty(HttpContextHelper.Response.Headers["SentQueueMessages"]))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>
                    {
                        { sQueueMessageId, messageBody }
                    };
                    string addedQueues = dictionary.FromDictionaryToJson();
                    HttpContextHelper.Response.Headers.Add("SentQueueMessages", addedQueues);

                }
                else
                {
                    string openedQueues = HttpContextHelper.Response.Headers["SentQueueMessages"];
                    Dictionary<string, string> dictionary = openedQueues.FromJsonToDictionary();
                    dictionary.Add(sQueueMessageId, messageBody);
                    string addedQueues = dictionary.FromDictionaryToJson();
                    HttpContextHelper.Response.Headers["SentQueueMessages"] = addedQueues;
                }
            }
        }

        public QueueResponse ReceiveJournal(TimeSpan? serverWaitTime = null)
        {
            if (serverWaitTime == null) { serverWaitTime = TimeSpan.FromSeconds(5); }

            long messageId = -1;

            string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
            QueueResponse response = new QueueResponse();
            if (string.IsNullOrEmpty(this.CurrentMessageId))
            {
                DataTable tblQueue = new DataTable();

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {

                        using (OracleConnection cn = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_Peek_Jouranl_Approval", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                            cmd.CommandType = CommandType.StoredProcedure;

                            OracleParameter messageIdPar = new OracleParameter("v_MessageId", OracleDbType.Int32);
                            OracleParameter nextRunDelayInSecPar = new OracleParameter("v_NextRunDelayInSec", OracleDbType.Int32);

                            OracleParameter queueCodePar = new OracleParameter("v_QueueDefinitionCode", OracleDbType.Varchar2, 255);
                            OracleParameter messageBodyPar = new OracleParameter("v_MessageBody", OracleDbType.Varchar2, 1000);
                            OracleParameter retryNumberPar = new OracleParameter("v_RetryNumber", OracleDbType.Int32);
                            OracleParameter messageCreatedServerTimePar = new OracleParameter("v_MessageCreatedServerTime", OracleDbType.Date);
                            OracleParameter watingStatusPar = new OracleParameter("v_WatingStatus", OracleDbType.Int32);

                            queueCodePar.Direction = ParameterDirection.Input;
                            nextRunDelayInSecPar.Direction = ParameterDirection.Input;

                            messageIdPar.Direction = ParameterDirection.Output;
                            messageBodyPar.Direction = ParameterDirection.Output;
                            retryNumberPar.Direction = ParameterDirection.Output;
                            messageCreatedServerTimePar.Direction = ParameterDirection.Output;
                            watingStatusPar.Direction = ParameterDirection.Input;

                            watingStatusPar.Value = WorkerNameService.GetWorkerWaitingStatusForReceiving(this.Tenant);

                            queueCodePar.Value = QueueCode;
                            nextRunDelayInSecPar.Value = serverWaitTime.Value.Milliseconds;
                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(messageBodyPar);
                            cmd.Parameters.Add(retryNumberPar);
                            cmd.Parameters.Add(messageCreatedServerTimePar);
                            cmd.Parameters.Add(queueCodePar);
                            cmd.Parameters.Add(nextRunDelayInSecPar);
                            cmd.Parameters.Add(watingStatusPar);



                            try
                            {
                                cn.Open();
                                var output = cmd.ExecuteNonQuery();
                                cn.Close();

                                object messageOb = cmd.Parameters["v_MessageId"].Value;
                                if (messageOb != null)
                                {

                                    if (long.TryParse(cmd.Parameters["v_MessageId"].Value.ToString(), out messageId))
                                    {
                                        this.CurrentMessageId = response.MessageId = messageId.ToString();
                                        response.RetryNumber = Convert.ToInt32(cmd.Parameters["v_RetryNumber"].Value);
                                        response.MessageCreatedServerTime = (DateTime)cmd.Parameters["v_MessageCreatedServerTime"].Value;
                                        string messageBody = cmd.Parameters["v_MessageBody"].Value as string;
                                        if (!string.IsNullOrEmpty(messageBody))
                                        {
                                            Dictionary<string, string> messageValues = DictionaryJsonConverter.FromJsonToDictionary(messageBody);
                                            response.MessageValues = messageValues;
                                        }

                                        RunDebuggerBreak();
                                    }


                                }

                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                throw;
                            }

                            cn.Close();
                        }


                    }
                    else
                    {
                        using (SqlConnection cn = new SqlConnection(strConnString))
                        {
                            SqlCommand cmd = new SqlCommand("[dbo].[Queue_Peek_Jouranl_Approval]", cn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter messageIdPar = new SqlParameter("@MessageId", SqlDbType.BigInt);
                            SqlParameter queueCodePar = new SqlParameter("@QueueDefinitionCode", SqlDbType.NVarChar, 255);
                            SqlParameter messageBodyPar = new SqlParameter("@MessageBody", SqlDbType.VarChar, messageBodyLength);
                            SqlParameter retryNumberPar = new SqlParameter("@RetryNumber", SqlDbType.Int);
                            SqlParameter watingStatusPar = new SqlParameter("@WatingStatus", SqlDbType.Int);
                            SqlParameter tenantPar = new SqlParameter("@Tenant", SqlDbType.Int);

                            messageIdPar.Direction = ParameterDirection.Output;
                            messageBodyPar.Direction = ParameterDirection.Output;
                            queueCodePar.Direction = ParameterDirection.Input;
                            retryNumberPar.Direction = ParameterDirection.Output;
                            watingStatusPar.Direction = ParameterDirection.Input;
                            tenantPar.Direction = ParameterDirection.Output;

                            queueCodePar.Value = QueueCode;
                            watingStatusPar.Value = WorkerNameService.GetWorkerWaitingStatusForReceiving(this.Tenant);

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(messageBodyPar);
                            cmd.Parameters.Add(retryNumberPar);
                            cmd.Parameters.Add(queueCodePar);
                            cmd.Parameters.Add(watingStatusPar);
                            cmd.Parameters.Add(tenantPar);

                            cn.Open();
                            var output = cmd.ExecuteNonQuery();
                            cn.Close();

                            object messageOb = cmd.Parameters["@MessageId"].Value;
                            if (messageOb != null)
                            {

                                if (long.TryParse(cmd.Parameters["@MessageId"].Value.ToString(), out messageId))
                                {
                                    try
                                    {
                                        MessageID = (int)messageId;
                                    }
                                    catch
                                    {

                                    }
                                    this.CurrentMessageId = response.MessageId = messageId.ToString();
                                    response.RetryNumber = (int)cmd.Parameters["@RetryNumber"].Value;
                                    response.Tenant = (int)cmd.Parameters["@Tenant"].Value;

                                    string messageBody = cmd.Parameters["@MessageBody"].Value as string;
                                    if (!string.IsNullOrEmpty(messageBody))
                                    {
                                        Dictionary<string, string> messageValues = DictionaryJsonConverter.FromJsonToDictionary(messageBody);
                                        response.MessageValues = messageValues;
                                    }

                                    RunDebuggerBreak();
                                }


                            }

                        }
                    }
                    scope.Complete();
                }


            }

            if (string.IsNullOrEmpty(response.MessageId))
            {
                Thread.Sleep(serverWaitTime.Value);
            }

            return response;
        }

        public void FreeTenants()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {

                string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
                using (SqlConnection cn = new SqlConnection(strConnString))
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[SetTenantIdleProcedure]", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();

                }
                scope.Complete();
            }
        }

        public QueueResponse Receive(TimeSpan? serverWaitTime = null)
        {

            return ReceiveDetail(serverWaitTime, suppressSleep: false);
        }
        public QueueResponse ReceiveDetail(TimeSpan? serverWaitTime, bool suppressSleep)
        {

            //
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                return ReceiveCustoms(((int)(serverWaitTime ?? TimeSpan.FromSeconds(60)).TotalSeconds));
            }

            if (serverWaitTime == null) { serverWaitTime = TimeSpan.FromSeconds(5); }

            long messageId = -1;

            string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
            QueueResponse response = new QueueResponse();
            if (string.IsNullOrEmpty(this.CurrentMessageId))
            {
                DataTable tblQueue = new DataTable();

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {

                        using (OracleConnection cn = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_Peek", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                            cmd.CommandType = CommandType.StoredProcedure;

                            OracleParameter messageIdPar = new OracleParameter("v_MessageId", OracleDbType.Int32);
                            OracleParameter nextRunDelayInSecPar = new OracleParameter("v_NextRunDelayInSec", OracleDbType.Int32);

                            OracleParameter queueCodePar = new OracleParameter("v_QueueDefinitionCode", OracleDbType.Varchar2, 255);
                            OracleParameter messageBodyPar = new OracleParameter("v_MessageBody", OracleDbType.Varchar2, 1000);
                            OracleParameter retryNumberPar = new OracleParameter("v_RetryNumber", OracleDbType.Int32);
                            OracleParameter messageCreatedServerTimePar = new OracleParameter("v_MessageCreatedServerTime", OracleDbType.Date);
                            OracleParameter watingStatusPar = new OracleParameter("v_WatingStatus", OracleDbType.Int32);

                            queueCodePar.Direction = ParameterDirection.Input;
                            nextRunDelayInSecPar.Direction = ParameterDirection.Input;

                            messageIdPar.Direction = ParameterDirection.Output;
                            messageBodyPar.Direction = ParameterDirection.Output;
                            retryNumberPar.Direction = ParameterDirection.Output;
                            messageCreatedServerTimePar.Direction = ParameterDirection.Output;
                            watingStatusPar.Direction = ParameterDirection.Input;

                            watingStatusPar.Value = WorkerNameService.GetWorkerWaitingStatusForReceiving(this.Tenant);

                            queueCodePar.Value = QueueCode;
                            //nextRunDelayInSecPar.Value = serverWaitTime.Value.Milliseconds;
                            nextRunDelayInSecPar.Value = serverWaitTime.Value.TotalSeconds;
                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(messageBodyPar);
                            cmd.Parameters.Add(retryNumberPar);
                            cmd.Parameters.Add(messageCreatedServerTimePar);
                            cmd.Parameters.Add(queueCodePar);
                            cmd.Parameters.Add(nextRunDelayInSecPar);
                            cmd.Parameters.Add(watingStatusPar);



                            try
                            {
                                cn.Open();
                                var output = cmd.ExecuteNonQuery();
                                cn.Close();

                                object messageOb = cmd.Parameters["v_MessageId"].Value;
                                if (messageOb != null)
                                {

                                    if (long.TryParse(cmd.Parameters["v_MessageId"].Value.ToString(), out messageId))
                                    {
                                        this.CurrentMessageId = response.MessageId = messageId.ToString();
                                        response.RetryNumber = Convert.ToInt32(cmd.Parameters["v_RetryNumber"].Value);
                                        response.MessageCreatedServerTime = (DateTime)cmd.Parameters["v_MessageCreatedServerTime"].Value;
                                        string messageBody = cmd.Parameters["v_MessageBody"].Value as string;
                                        if (!string.IsNullOrEmpty(messageBody))
                                        {
                                            Dictionary<string, string> messageValues = DictionaryJsonConverter.FromJsonToDictionary(messageBody);
                                            response.MessageValues = messageValues;
                                        }

                                        RunDebuggerBreak();
                                    }


                                }

                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                throw;
                            }

                            cn.Close();
                        }


                    }
                    else
                    {
                        using (SqlConnection cn = new SqlConnection(strConnString))
                        {
                            SqlCommand cmd = new SqlCommand("[dbo].[Queue_Peek]", cn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter messageIdPar = new SqlParameter("@V_MessageId", SqlDbType.BigInt);
                            SqlParameter queueCodePar = new SqlParameter("@V_QueueDefinitionCode", SqlDbType.NVarChar, 255);
                            SqlParameter messageBodyPar = new SqlParameter("@V_MessageBody", SqlDbType.VarChar, messageBodyLength);
                            SqlParameter retryNumberPar = new SqlParameter("@V_RetryNumber", SqlDbType.Int);
                            SqlParameter watingStatusPar = new SqlParameter("@V_WatingStatus", SqlDbType.Int);
                            SqlParameter messageCreatedServerTimepar = new SqlParameter("@V_MESSAGECREATEDSERVERTIME", SqlDbType.DateTime);
                            SqlParameter nextRunDelayInSecPar = new SqlParameter("@V_NEXTRUNDELAYINSEC", SqlDbType.Int);

                            messageIdPar.Direction = ParameterDirection.Output;
                            messageBodyPar.Direction = ParameterDirection.Output;
                            queueCodePar.Direction = ParameterDirection.Input;
                            retryNumberPar.Direction = ParameterDirection.Output;
                            messageCreatedServerTimepar.Direction = ParameterDirection.Output;

                            watingStatusPar.Direction = ParameterDirection.Input;
                            nextRunDelayInSecPar.Direction = ParameterDirection.Input;

                            queueCodePar.Value = QueueCode;
                            watingStatusPar.Value = WorkerNameService.GetWorkerWaitingStatusForReceiving(this.Tenant);
                            nextRunDelayInSecPar.Value = serverWaitTime.Value.Milliseconds;

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(messageBodyPar);
                            cmd.Parameters.Add(retryNumberPar);
                            cmd.Parameters.Add(queueCodePar);
                            cmd.Parameters.Add(watingStatusPar);
                            cmd.Parameters.Add(messageCreatedServerTimepar);
                            cmd.Parameters.Add(nextRunDelayInSecPar);

                            cn.Open();
                            var output = cmd.ExecuteNonQuery();
                            cn.Close();

                            object messageOb = cmd.Parameters["@V_MessageId"].Value;
                            if (messageOb != null)
                            {

                                if (long.TryParse(cmd.Parameters["@V_MessageId"].Value.ToString(), out messageId))
                                {
                                    try
                                    {
                                        MessageID = (int)messageId;
                                    }
                                    catch
                                    {

                                    }
                                    this.CurrentMessageId = response.MessageId = messageId.ToString();
                                    response.RetryNumber = (int)cmd.Parameters["@V_RetryNumber"].Value;
                                    response.MessageCreatedServerTime = (DateTime)cmd.Parameters["@V_MESSAGECREATEDSERVERTIME"].Value;

                                    string messageBody = cmd.Parameters["@V_MessageBody"].Value as string;
                                    if (!string.IsNullOrEmpty(messageBody))
                                    {
                                        Dictionary<string, string> messageValues = DictionaryJsonConverter.FromJsonToDictionary(messageBody);
                                        response.MessageValues = messageValues;
                                    }

                                    RunDebuggerBreak();
                                }


                            }

                        }
                    }

                    scope.Complete();
                }


            }

            if (!suppressSleep && string.IsNullOrEmpty(response.MessageId))
            {
                Thread.Sleep(serverWaitTime.Value);
            }

            return response;
        }

        private static void RunDebuggerBreak()
        {
            if (Debugger.IsAttached && AmitalCloudSettings.RunWorkerRoleAutomaticBreakPoint)
                Debugger.Break();
        }

        public QueueResponse Receive()
        {
            int nextRunDelayInSec = 60;
            return Receive(nextRunDelayInSec);
        }
        public QueueResponse Receive(int nextRunDelayInSec = 60, TimeSpan? serverWaitTime = null)
        {
            //
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                return ReceiveCustoms(nextRunDelayInSec);
            }
            if (serverWaitTime == null) { serverWaitTime = TimeSpan.FromSeconds(5); }
            long messageId = -1;

            string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
            QueueResponse response = new QueueResponse();
            if (string.IsNullOrEmpty(this.CurrentMessageId))
            {
                DataTable tblQueue = new DataTable();

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {

                        using (OracleConnection cn = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_Peek", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                            cmd.CommandType = CommandType.StoredProcedure;

                            OracleParameter messageIdPar = new OracleParameter("v_MessageId", OracleDbType.Int32);
                            OracleParameter nextRunDelayInSecPar = new OracleParameter("v_NextRunDelayInSec", OracleDbType.Int32);

                            OracleParameter queueCodePar = new OracleParameter("v_QueueDefinitionCode", OracleDbType.Varchar2, 255);
                            OracleParameter messageBodyPar = new OracleParameter("v_MessageBody", OracleDbType.Varchar2, 1000);
                            OracleParameter retryNumberPar = new OracleParameter("v_RetryNumber", OracleDbType.Int32);
                            OracleParameter messageCreatedServerTimePar = new OracleParameter("v_MessageCreatedServerTime", OracleDbType.Date);
                            OracleParameter watingStatusPar = new OracleParameter("v_WatingStatus", OracleDbType.Int32);

                            queueCodePar.Direction = ParameterDirection.Input;
                            nextRunDelayInSecPar.Direction = ParameterDirection.Input;

                            messageIdPar.Direction = ParameterDirection.Output;
                            messageBodyPar.Direction = ParameterDirection.Output;
                            retryNumberPar.Direction = ParameterDirection.Output;
                            messageCreatedServerTimePar.Direction = ParameterDirection.Output;
                            watingStatusPar.Direction = ParameterDirection.Input;
                            watingStatusPar.Value = WorkerNameService.GetWorkerWaitingStatusForReceiving(this.Tenant);

                            queueCodePar.Value = QueueCode;
                            nextRunDelayInSecPar.Value = nextRunDelayInSec;
                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(messageBodyPar);
                            cmd.Parameters.Add(retryNumberPar);
                            cmd.Parameters.Add(messageCreatedServerTimePar);
                            cmd.Parameters.Add(queueCodePar);
                            cmd.Parameters.Add(nextRunDelayInSecPar);
                            cmd.Parameters.Add(watingStatusPar);



                            try
                            {
                                cn.Open();
                                var output = cmd.ExecuteNonQuery();
                                cn.Close();

                                object messageOb = cmd.Parameters["v_MessageId"].Value;
                                if (messageOb != null)
                                {

                                    if (long.TryParse(cmd.Parameters["v_MessageId"].Value.ToString(), out messageId))
                                    {
                                        this.CurrentMessageId = response.MessageId = messageId.ToString();
                                        response.RetryNumber = Convert.ToInt32(cmd.Parameters["v_RetryNumber"].Value);
                                        response.MessageCreatedServerTime = (DateTime)cmd.Parameters["v_MessageCreatedServerTime"].Value;
                                        string messageBody = cmd.Parameters["v_MessageBody"].Value as string;
                                        if (!string.IsNullOrEmpty(messageBody))
                                        {
                                            Dictionary<string, string> messageValues = DictionaryJsonConverter.FromJsonToDictionary(messageBody);
                                            response.MessageValues = messageValues;
                                        }

                                        RunDebuggerBreak();
                                    }


                                }

                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                throw;
                            }

                            cn.Close();
                        }


                    }
                    else
                    {
                        using (SqlConnection cn = new SqlConnection(strConnString))
                        {
                            SqlCommand cmd = new SqlCommand("[dbo].[Queue_Peek]", cn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter messageIdPar = new SqlParameter("@V_MessageId", SqlDbType.BigInt);
                            SqlParameter queueCodePar = new SqlParameter("@V_QueueDefinitionCode", SqlDbType.NVarChar, 255);
                            SqlParameter messageBodyPar = new SqlParameter("@V_MessageBody", SqlDbType.VarChar, messageBodyLength);
                            SqlParameter retryNumberPar = new SqlParameter("@V_RetryNumber", SqlDbType.Int);
                            SqlParameter watingStatusPar = new SqlParameter("@V_WatingStatus", SqlDbType.Int);
                            SqlParameter messageCreatedServerTimepar = new SqlParameter("@V_MESSAGECREATEDSERVERTIME", SqlDbType.DateTime);
                            SqlParameter nextRunDelayInSecPar = new SqlParameter("@V_NEXTRUNDELAYINSEC", SqlDbType.Int);

                            messageIdPar.Direction = ParameterDirection.Output;
                            messageBodyPar.Direction = ParameterDirection.Output;
                            queueCodePar.Direction = ParameterDirection.Input;
                            retryNumberPar.Direction = ParameterDirection.Output;
                            watingStatusPar.Direction = ParameterDirection.Input;
                            messageCreatedServerTimepar.Direction = ParameterDirection.Output;
                            nextRunDelayInSecPar.Direction = ParameterDirection.Input;

                            watingStatusPar.Value = WorkerNameService.GetWorkerWaitingStatusForReceiving(this.Tenant);
                            queueCodePar.Value = QueueCode;
                            nextRunDelayInSecPar.Value = serverWaitTime.Value.Milliseconds;

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(messageBodyPar);
                            cmd.Parameters.Add(retryNumberPar);
                            cmd.Parameters.Add(queueCodePar);
                            cmd.Parameters.Add(watingStatusPar);
                            cmd.Parameters.Add(messageCreatedServerTimepar);
                            cmd.Parameters.Add(nextRunDelayInSecPar);

                            cn.Open();
                            var output = cmd.ExecuteNonQuery();
                            cn.Close();

                            object messageOb = cmd.Parameters["@V_MessageId"].Value;
                            if (messageOb != null)
                            {

                                if (long.TryParse(cmd.Parameters["@V_MessageId"].Value.ToString(), out messageId))
                                {
                                    this.CurrentMessageId = response.MessageId = messageId.ToString();
                                    try
                                    {
                                        MessageID = (int)messageId;
                                    }
                                    catch
                                    {

                                    }
                                    response.RetryNumber = (int)cmd.Parameters["@V_RetryNumber"].Value;
                                    response.MessageCreatedServerTime = (DateTime)cmd.Parameters["@V_MESSAGECREATEDSERVERTIME"].Value;

                                    string messageBody = cmd.Parameters["@v_MessageBody"].Value as string;
                                    if (!string.IsNullOrEmpty(messageBody))
                                    {
                                        Dictionary<string, string> messageValues = DictionaryJsonConverter.FromJsonToDictionary(messageBody);
                                        response.MessageValues = messageValues;
                                    }

                                    RunDebuggerBreak();
                                }
                            }



                        }
                    }

                    scope.Complete();
                }


            }

            if (string.IsNullOrEmpty(response.MessageId))
            {
                Thread.Sleep(serverWaitTime.Value);
            }

            return response;
        }

        public List<QueueResponse> Receive_new(int nextRunDelayInSec = 60, TimeSpan? serverWaitTime = null, int? selectCount = null)
        {
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                return ReceiveCustoms_new(nextRunDelayInSec, selectCount);
            }
            if (serverWaitTime == null) { serverWaitTime = TimeSpan.FromSeconds(5); }
            long messageId = -1;

            string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
            List<QueueResponse> responseList = new List<QueueResponse>();
            if (string.IsNullOrEmpty(this.CurrentMessageId))
            {
                DataTable tblQueue = new DataTable();

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {
                        using (OracleConnection DBConnection = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand(DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_Peek_List", AmitalCloudDBSchema.AMITAL_MAIN, DBConnection.ConnectionString), DBConnection);
                            try
                            {
                                DBConnection.Open();
                                cmd.CommandType = CommandType.StoredProcedure;
                                var nextRunDelayInSecPar = cmd.CreateParameter(); nextRunDelayInSecPar.ParameterName = "v_NextRunDelayInSec"; nextRunDelayInSecPar.DbType = DbType.Double;
                                var queueCodePar = cmd.CreateParameter(); queueCodePar.ParameterName = "v_QueueDefinitionCode"; queueCodePar.DbType = DbType.String; queueCodePar.Size = 255;
                                var watingStatusPar = cmd.CreateParameter(); watingStatusPar.ParameterName = "v_WatingStatus"; watingStatusPar.DbType = DbType.Double;
                                var selectCountPar = cmd.CreateParameter(); selectCountPar.ParameterName = "v_SelectCount"; selectCountPar.DbType = DbType.Double;
                                OracleParameter vQueueMessages = new OracleParameter("cursor_", OracleDbType.RefCursor, 18);

                                queueCodePar.Direction = ParameterDirection.Input;
                                nextRunDelayInSecPar.Direction = ParameterDirection.Input;
                                selectCountPar.Direction = ParameterDirection.Input;
                                vQueueMessages.Direction = ParameterDirection.Output;
                                watingStatusPar.Direction = ParameterDirection.Input;

                                var num = System.Configuration.ConfigurationManager.AppSettings.Get("CustomDbQueueNewReceiveSelectCount");
                                if (!string.IsNullOrEmpty(num))
                                {
                                    selectCountPar.Value = int.Parse(num);
                                }
                                else
                                {
                                    selectCountPar.Value = 10;
                                }
                                watingStatusPar.Value = WorkerNameService.GetWorkerWaitingStatusForReceiving(this.Tenant);
                                queueCodePar.Value = QueueCode;
                                nextRunDelayInSecPar.Value = nextRunDelayInSec;

                                cmd.Parameters.Add(vQueueMessages);
                                cmd.Parameters.Add(queueCodePar);
                                cmd.Parameters.Add(nextRunDelayInSecPar);
                                cmd.Parameters.Add(watingStatusPar);
                                cmd.Parameters.Add(selectCountPar);

                                var output = cmd.ExecuteNonQuery();
                                using (OracleDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var response = new QueueResponse()
                                        {
                                            MessageId = reader.GetString(0),
                                            RetryNumber = reader.GetInt32(2),
                                            MessageCreatedServerTime = reader.GetDateTime(3),
                                        };
                                        if (!string.IsNullOrEmpty(reader.GetString(1).ToString()))
                                        {
                                            Dictionary<string, string> messageValues = DictionaryJsonConverter.FromJsonToDictionary((reader.GetString(1).ToString()));
                                            response.MessageValues = messageValues;
                                        }
                                        responseList.Add(response);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                throw;
                            }
                            finally
                            {
                                cmd.Connection.Close();
                            }
                        }
                    }
                    else
                    {
                        //sql will be in the future
                    }

                    scope.Complete();
                }
            }

            return responseList;
        }


        public void Delay(TimeSpan delayTime)
        {
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                DelayCustoms(delayTime);
                return;
            }

            if (!string.IsNullOrEmpty(this.CurrentMessageId))
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    int seconds = (int)delayTime.TotalSeconds;
                    string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
                    QueueResponse response = new QueueResponse();
                    DataTable tblQueue = new DataTable();

                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {

                        using (OracleConnection cn = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_DelayMessage", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                            cmd.CommandType = CommandType.StoredProcedure;


                            OracleParameter messageIdPar = new OracleParameter("MessageId", OracleDbType.Int32, 18);
                            OracleParameter delayPar = new OracleParameter("v_DelaySeconds", OracleDbType.Int32);

                            messageIdPar.Direction = ParameterDirection.Input;
                            delayPar.Direction = ParameterDirection.Input;

                            messageIdPar.Value = this.CurrentMessageId;
                            delayPar.Value = seconds;

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(delayPar);

                            try
                            {
                                cn.Open();
                                var output = cmd.ExecuteNonQuery();
                                cn.Close();
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                throw;
                            }

                            cn.Close();
                        }


                    }
                    else
                    {
                        using (SqlConnection cn = new SqlConnection(strConnString))
                        {
                            SqlCommand cmd = new SqlCommand("[dbo].[Queue_DelayMessage]", cn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter messageIdPar = new SqlParameter("@MessageId", SqlDbType.BigInt);
                            SqlParameter delayPar = new SqlParameter("@v_DelaySeconds", SqlDbType.Int);


                            messageIdPar.Direction = ParameterDirection.Input;
                            delayPar.Direction = ParameterDirection.Input;

                            messageIdPar.Value = this.CurrentMessageId;
                            delayPar.Value = seconds;

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(delayPar);

                            cn.Open();
                            var output = cmd.ExecuteNonQuery();
                            cn.Close();

                            this.CurrentMessageId = null;

                        }
                    }

                    scope.Complete();
                }
            }

        }

        public void Return()
        {
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                throw new Exception("Queue_ReturnMessage not in use  in CostomsDeploy");
            }

            if (!string.IsNullOrEmpty(this.CurrentMessageId))
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
                    QueueResponse response = new QueueResponse();
                    DataTable tblQueue = new DataTable();
                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {

                        using (OracleConnection cn = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_ReturnMessage", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                            cmd.CommandType = CommandType.StoredProcedure;


                            OracleParameter messageIdPar = new OracleParameter("MessageId", OracleDbType.Int32, 18);
                            messageIdPar.Direction = ParameterDirection.Input;
                            messageIdPar.Value = this.CurrentMessageId;

                            cmd.Parameters.Add(messageIdPar);

                            try
                            {
                                cn.Open();
                                var output = cmd.ExecuteNonQuery();
                                cn.Close();
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                throw;
                            }

                            cn.Close();
                        }


                    }
                    else
                    {
                        using (SqlConnection cn = new SqlConnection(strConnString))
                        {
                            SqlCommand cmd = new SqlCommand("[dbo].[Queue_ReturnMessage]", cn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter messageIdPar = new SqlParameter("@MessageId", SqlDbType.BigInt);
                            messageIdPar.Direction = ParameterDirection.Input;
                            messageIdPar.Value = this.CurrentMessageId;

                            cmd.Parameters.Add(messageIdPar);

                            cn.Open();
                            var output = cmd.ExecuteNonQuery();
                            cn.Close();
                            this.CurrentMessageId = null;
                        }
                    }
                    scope.Complete();
                }
            }

        }
        /// <summary>
        /// Have TransactionScope Wrapper 
        /// </summary>
        public void Complete()
        {
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                CompleteCustoms(false);
                return;
            }
            if (!string.IsNullOrEmpty(this.CurrentMessageId))
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
                    QueueResponse response = new QueueResponse();
                    DataTable tblQueue = new DataTable();
                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {

                        using (OracleConnection cn = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_SetStatus", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                            cmd.CommandType = CommandType.StoredProcedure;


                            OracleParameter messageIdPar = new OracleParameter("MessageId", OracleDbType.Int32, 18);
                            OracleParameter statusPar = new OracleParameter("Statud", OracleDbType.Int32);

                            messageIdPar.Direction = ParameterDirection.Input;
                            statusPar.Direction = ParameterDirection.Input;

                            messageIdPar.Value = this.CurrentMessageId;
                            statusPar.Value = 1;

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(statusPar);

                            try
                            {
                                cn.Open();
                                var output = cmd.ExecuteNonQuery();
                                cn.Close();
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                throw;
                            }

                            cn.Close();
                        }


                    }
                    else
                    {
                        using (SqlConnection cn = new SqlConnection(strConnString))
                        {
                            SqlCommand cmd = new SqlCommand("[dbo].[Queue_SetStatus]", cn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter messageIdPar = new SqlParameter("@V_MessageId", SqlDbType.BigInt);
                            SqlParameter statusPar = new SqlParameter("@V_Statud", SqlDbType.Int);


                            messageIdPar.Direction = ParameterDirection.Input;
                            statusPar.Direction = ParameterDirection.Input;

                            messageIdPar.Value = this.CurrentMessageId;
                            statusPar.Value = 1;

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(statusPar);

                            cn.Open();
                            var output = cmd.ExecuteNonQuery();
                            cn.Close();

                            this.CurrentMessageId = null;

                        }
                    }

                    scope.Complete();
                }
            }
        }

        public void CompleteAsFailedParam(string queueId)
        {
            this.CurrentMessageId = queueId;
            this.CompleteAsFailed();
        }
        public void CompleteAsFailed()
        {
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                CompleteCustoms(true);
                return;
            }
            if (!string.IsNullOrEmpty(this.CurrentMessageId))
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
                    QueueResponse response = new QueueResponse();
                    DataTable tblQueue = new DataTable();
                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {

                        using (OracleConnection cn = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_SetStatus", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                            cmd.CommandType = CommandType.StoredProcedure;


                            OracleParameter messageIdPar = new OracleParameter("MessageId", OracleDbType.Int32, 18);
                            OracleParameter statusPar = new OracleParameter("Statud", OracleDbType.Int32);

                            messageIdPar.Direction = ParameterDirection.Input;
                            statusPar.Direction = ParameterDirection.Input;

                            messageIdPar.Value = this.CurrentMessageId;
                            statusPar.Value = -1;

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(statusPar);

                            try
                            {
                                cn.Open();
                                var output = cmd.ExecuteNonQuery();
                                cn.Close();
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                throw;
                            }

                            cn.Close();
                        }


                    }
                    else
                    {
                        using (SqlConnection cn = new SqlConnection(strConnString))
                        {
                            SqlCommand cmd = new SqlCommand("[dbo].[Queue_SetStatus]", cn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter messageIdPar = new SqlParameter("@V_MessageId", SqlDbType.BigInt);
                            SqlParameter statusPar = new SqlParameter("@V_Statud", SqlDbType.Int);


                            messageIdPar.Direction = ParameterDirection.Input;
                            statusPar.Direction = ParameterDirection.Input;

                            messageIdPar.Value = this.CurrentMessageId;
                            statusPar.Value = -1;

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(statusPar);

                            cn.Open();
                            var output = cmd.ExecuteNonQuery();
                            cn.Close();

                            this.CurrentMessageId = null;

                        }
                    }

                    scope.Complete();
                }
            }
        }

        public void Complete(string messageId)
        {
            this.CurrentMessageId = messageId;
            this.Complete();
        }

        public void DelayAndReturnBackToQueue(TimeSpan delayTime, string myMessageId)
        {
            if (!string.IsNullOrEmpty(myMessageId))
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    int seconds = (int)delayTime.TotalSeconds;
                    string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
                    QueueResponse response = new QueueResponse();
                    DataTable tblQueue = new DataTable();

                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {

                        using (OracleConnection cn = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Q_DelayMsgandChangeStatusTo0", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                            cmd.CommandType = CommandType.StoredProcedure;


                            OracleParameter messageIdPar = new OracleParameter("MessageId", OracleDbType.Int32);
                            OracleParameter delayPar = new OracleParameter("v_DelaySeconds", OracleDbType.Int32);

                            messageIdPar.Direction = ParameterDirection.Input;
                            delayPar.Direction = ParameterDirection.Input;

                            messageIdPar.Value = myMessageId;
                            delayPar.Value = seconds;

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(delayPar);

                            try
                            {
                                cn.Open();
                                var output = cmd.ExecuteNonQuery();
                                cn.Close();
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                throw;
                            }

                            cn.Close();
                        }


                    }
                    else
                    {
                        using (SqlConnection cn = new SqlConnection(strConnString))
                        {
                            SqlCommand cmd = new SqlCommand("[dbo].[Queue_DelayMessageandChangeStatusTozero]", cn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter messageIdPar = new SqlParameter("@V_MessageId", SqlDbType.BigInt);
                            SqlParameter delayPar = new SqlParameter("@V_DelaySeconds", SqlDbType.Int);


                            messageIdPar.Direction = ParameterDirection.Input;
                            delayPar.Direction = ParameterDirection.Input;

                            messageIdPar.Value = myMessageId;
                            delayPar.Value = seconds;

                            cmd.Parameters.Add(messageIdPar);
                            cmd.Parameters.Add(delayPar);

                            cn.Open();
                            var output = cmd.ExecuteNonQuery();
                            cn.Close();

                            this.CurrentMessageId = null;

                        }
                    }

                    scope.Complete();
                }
            }

        }



    }
}
