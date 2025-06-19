using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.Enums;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Transactions;
namespace AmitalCloud.Infrastructure.Data.Services
{
    public partial class DbQueueService
    {


        protected int? SendReturnIdCustoms(Dictionary<string, string> messageValues,
         int tenant, TimeSpan? delayTime = null, string CustomerId = null, string BatchNumber = null, DateTime? NextRunDate = null//,int tenantPriority = 89
         , QueueSendModel queueSendModel = null)
        {
            if (!AmitalCloudSettings.IsCostomsDeploy)
            {
                throw new Exception("SendReturnIdCustoms  use in AmitalCloudSettings.IsCostomsDeploy");
            }
            //if (NextRunDate.HasValue)
            //{
            //    throw new Exception("Opps  in oracle use by delayInSec Params ");
            //}
            if (NextRunDate.HasValue)
            {
                if (DateTime.UtcNow > NextRunDate)
                {
                    delayTime = null;

                }
                else
                {
                    delayTime = NextRunDate.Value.Subtract(DateTime.UtcNow);

                }
                NextRunDate = null;
            }


            int tenantPriority = GetTenantPriority(queueSendModel);
            int? queueMessageId = null;
            //if (AmitalCloudSettings.IsCostomsDeploy) //ITZIK + YARON 
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
                if (CustomerId != null)
                {
                    CId = CustomerId;
                }
                if (BatchNumber != null)
                {
                    BNo = BatchNumber;
                }
                //if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                {
                    using (var context = AmitalCloudContext.GetContext(this.Tenant) as DbContext)
                    {
                        var cmd = context.Database.GetDbConnection().CreateCommand();
                        if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                        {
                            cmd.CommandText =
                                DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_Enqueue", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);

                        }
                        else
                        {
                            cmd.CommandText = "[dbo].[Queue_Enqueue]";
                        }
                        cmd.CommandType = CommandType.StoredProcedure;


                        var queueCodePar = cmd.CreateParameter();
                        queueCodePar.ParameterName = "v_QueueDefinitionCode"; queueCodePar.DbType = DbType.String; //queueCodePar.Size = 255;
                        var msgBodyPar = cmd.CreateParameter();
                        msgBodyPar.ParameterName = "v_MessageBody"; msgBodyPar.DbType = DbType.String; //msgBodyPar.Size = 1000;

                        var tenantPar = cmd.CreateParameter();
                        tenantPar.ParameterName = "v_Tenant"; tenantPar.DbType = DbType.Double;

                        var delayPar = cmd.CreateParameter(); delayPar.ParameterName = "v_DelaySeconds"; delayPar.DbType = DbType.Double; ;
                        var customerId = cmd.CreateParameter(); customerId.ParameterName = "v_CustomerId"; customerId.DbType = DbType.String; //customerId.Size = 15;
                        var batchNumber = cmd.CreateParameter(); batchNumber.ParameterName = "v_BatchNumber"; batchNumber.DbType = DbType.String; //batchNumber.Size = 15;
                        var hashCodePar = cmd.CreateParameter(); hashCodePar.ParameterName = "v_HashCode"; hashCodePar.DbType = DbType.String; //hashCodePar.Size = 100;


                        var watingStatusPar = cmd.CreateParameter(); watingStatusPar.ParameterName = "v_WatingStatus"; watingStatusPar.DbType = DbType.Double;
                        watingStatusPar.Direction = ParameterDirection.Input;
                        var tenantPriPar = CreatetenantPri(cmd);
                        var InterfaceTypeCodePar = CreateInterfaceTypeCode(cmd, hashCodePar);
                        var UseRabbitMQPar = CreateUseRabbitMQ(cmd);

                        var QueueCodeRabbitPar = cmd.CreateParameter(); QueueCodeRabbitPar.ParameterName = "p_QueueCodeRabbit"; QueueCodeRabbitPar.DbType = DbType.String; //QueueCodeRabbitPar.Size = 256;
                        QueueCodeRabbitPar.Direction = ParameterDirection.Input;

                        var entityCodePar = cmd.CreateParameter(); entityCodePar.ParameterName = "p_EntityCode"; entityCodePar.DbType = DbType.String; ///QueueCodeRabbitPar.Size = 40;
                        entityCodePar.Direction = ParameterDirection.Input;

                        var entityIdPar = cmd.CreateParameter(); entityIdPar.ParameterName = "p_EntityId"; entityIdPar.DbType = DbType.String; ///entityIdPar.Size = 40;
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
                        InterfaceTypeCodePar.Value = "";// DBNull.Value;
                        if (!string.IsNullOrWhiteSpace(queueSendModel?.InterfaceTypeCode))
                        {
                            InterfaceTypeCodePar.Value = queueSendModel?.InterfaceTypeCode;
                        }


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

                        var queueMessageIdPar = cmd.CreateParameter(); queueMessageIdPar.ParameterName = "v_QueueMessageId"; queueMessageIdPar.DbType = DbType.Double;
                        queueMessageIdPar.Direction = ParameterDirection.Output;


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
                            cmd.Connection.Open();
                            var output = cmd.ExecuteNonQuery();

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
                        finally
                        {
                            cmd.Connection.Close();
                        }


                    }


                }


                scope.Complete();
            }



            return queueMessageId;
        }

        private static System.Data.Common.DbParameter CreateUseRabbitMQ(System.Data.Common.DbCommand cmd)
        {
            var UseRabbitMQPar = cmd.CreateParameter();
            UseRabbitMQPar.ParameterName = "p_UseRabbitMQ";
            UseRabbitMQPar.DbType = DbType.Double;
            UseRabbitMQPar.Direction = ParameterDirection.Input;
            return UseRabbitMQPar;
        }

        private static System.Data.Common.DbParameter CreatetenantPri(System.Data.Common.DbCommand cmd)
        {
            var tenantPriPar = cmd.CreateParameter();
            tenantPriPar.ParameterName = "p_TenantPriority";
            tenantPriPar.DbType = DbType.Double;
            tenantPriPar.Direction = ParameterDirection.Input;
            return tenantPriPar;
        }

        private static System.Data.Common.DbParameter CreateInterfaceTypeCode(System.Data.Common.DbCommand cmd, System.Data.Common.DbParameter hashCodePar)
        {
            var InterfaceTypeCodePar = cmd.CreateParameter();
            InterfaceTypeCodePar.ParameterName = "p_InterfaceTypeCode";
            InterfaceTypeCodePar.DbType = DbType.String;
            hashCodePar.Size = 32;
            InterfaceTypeCodePar.Direction = ParameterDirection.Input;
            return InterfaceTypeCodePar;
        }


        private static int GetTenantPriority(QueueSendModel queueSendModel)
        {
            int tenantPriority = queueSendModel?.TenantPriority ?? 89;
            if (tenantPriority < 1)
            {
                tenantPriority = 89;
            }
            else if (tenantPriority > 99)
            {
                tenantPriority = 89;
            }

            return tenantPriority;
        }


        public QueueResponse ReceiveCustoms(int nextRunDelayInSec = 60)
        {
            if (!AmitalCloudSettings.IsCostomsDeploy)
            {
                throw new Exception("ReceiveCustoms  use in AmitalCloudSettings.IsCostomsDeploy");
            }

            //
            long messageId = -1;

            string strConnString = TenantServerConfigration.GetDbConnection(this.Tenant);
            QueueResponse response = new QueueResponse();
            if (string.IsNullOrEmpty(this.CurrentMessageId))
            {


                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                //using (TransactionScope scope = TransactionFactory.GetTransaction())
                {

                    using (var context = AmitalCloudContext.GetContext(this.Tenant) as DbContext)
                    {
                        var cmd = context.Database.GetDbConnection().CreateCommand();
                        if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                        {
                            cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_Peek", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                        }
                        else
                        {
                            cmd.CommandText = "[dbo].[Queue_Peek]";
                        }


                        cmd.CommandType = CommandType.StoredProcedure;

                        var messageIdPar = cmd.CreateParameter(); messageIdPar.ParameterName = "v_MessageId"; messageIdPar.DbType = DbType.Double;

                        var nextRunDelayInSecPar = cmd.CreateParameter(); nextRunDelayInSecPar.ParameterName = "v_NextRunDelayInSec"; nextRunDelayInSecPar.DbType = DbType.Double;
                        var queueCodePar = cmd.CreateParameter(); queueCodePar.ParameterName = "v_QueueDefinitionCode"; queueCodePar.DbType = DbType.String; queueCodePar.Size = 255;
                        var messageBodyPar = cmd.CreateParameter(); messageBodyPar.ParameterName = "v_MessageBody"; messageBodyPar.DbType = DbType.String; messageBodyPar.Size = 1000;
                        var retryNumberPar = cmd.CreateParameter(); retryNumberPar.ParameterName = "v_RetryNumber"; retryNumberPar.DbType = DbType.Double;
                        var messageCreatedServerTimePar = cmd.CreateParameter(); messageCreatedServerTimePar.ParameterName = "v_MessageCreatedServerTime"; messageCreatedServerTimePar.DbType = DbType.DateTime;
                        var watingStatusPar = cmd.CreateParameter(); watingStatusPar.ParameterName = "v_WatingStatus"; watingStatusPar.DbType = DbType.Double;



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
                            cmd.Connection.Open();
                            var output = cmd.ExecuteNonQuery();


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
                        finally
                        {
                            cmd.Connection.Close();
                        }


                    }


                    scope.Complete();
                }


            }

            //if (string.IsNullOrEmpty(response.MessageId))
            //{
            //    Thread.Sleep(serverWaitTime.Value);
            //}

            return response;
        }
        public List<QueueResponse> ReceiveCustoms_new(int nextRunDelayInSec = 60, int? selectCount = null)
        {
            if (!AmitalCloudSettings.IsCostomsDeploy)
            {
                throw new Exception("ReceiveCustoms  use in AmitalCloudSettings.IsCostomsDeploy");
            }
            //
            List<QueueResponse> responseList = new List<QueueResponse>();

            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            //using (TransactionScope scope = TransactionFactory.GetTransaction())
            //{
            using (var context = AmitalCloudContext.GetContext(this.Tenant) as DbContext)
            {
                if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                {
                    using (OracleConnection DBConnection = new OracleConnection(context.Database.GetDbConnection().ConnectionString))
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
                            OracleParameter vQueueMessages = new OracleParameter("l_cursor", OracleDbType.RefCursor, 18);

                            queueCodePar.Direction = ParameterDirection.Input;
                            nextRunDelayInSecPar.Direction = ParameterDirection.Input;
                            selectCountPar.Direction = ParameterDirection.Input;
                            vQueueMessages.Direction = ParameterDirection.Output;
                            watingStatusPar.Direction = ParameterDirection.Input;

                            if (selectCount == null || selectCount == 0)
                            {
                                selectCount = 10;
                            }
                            selectCountPar.Value = selectCount;
                            /*
                            // var num = System.Configuration.ConfigurationManager.AppSettings.Get("CustomDbQueueNewReceiveSelectCount");
                            if (!string.IsNullOrEmpty(selectCount))
                            {
                                selectCountPar.Value = int.Parse(num);
                            }
                            else
                            {
                                selectCountPar.Value = 10;
                            }*/

                            watingStatusPar.Value = WorkerNameService.GetWorkerWaitingStatusForReceiving(this.Tenant);
                            queueCodePar.Value = QueueCode;
                            nextRunDelayInSecPar.Value = nextRunDelayInSec;

                            cmd.Parameters.Add(vQueueMessages);
                            cmd.Parameters.Add(queueCodePar);
                            cmd.Parameters.Add(nextRunDelayInSecPar);
                            cmd.Parameters.Add(watingStatusPar);
                            cmd.Parameters.Add(selectCountPar);

                            //LogTime($"{QueueCode} start Execute proc");

                            var ou = cmd.ExecuteNonQuery();
                            //LogTime($"{QueueCode} end Execute proc");

                            if (vQueueMessages.Value != DBNull.Value)
                            {
                                // LogTime($"{QueueCode} start extract data returnd from proc");
                                using (OracleDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        if (reader["id"].ToString() == "0")
                                        {
                                            continue;
                                        }

                                        var response = new QueueResponse();
                                        response.MessageId = reader["id"].ToString();
                                        response.RetryNumber = Convert.ToInt32(reader["retrynumber"]);
                                        response.MessageCreatedServerTime = Convert.ToDateTime(reader["createdatetime"]);

                                        var messageBody = reader["messagebody"].ToString();

                                        if (!string.IsNullOrEmpty(messageBody))
                                        {
                                            Dictionary<string, string> messageValues = DictionaryJsonConverter.FromJsonToDictionary(messageBody);
                                            response.MessageValues = messageValues;
                                        }
                                        responseList.Add(response);
                                    }
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

                //scope.Complete();
            }
            // }

            return responseList;
        }



        public void CompleteCustoms(bool Failed)
        {
            if (string.IsNullOrEmpty(this.CurrentMessageId))
            {
                return;
            }
            if (!AmitalCloudSettings.IsCostomsDeploy)
            {
                throw new Exception("CompleteCustoms  use in AmitalCloudSettings.IsCostomsDeploy");
            }
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                using (var context = AmitalCloudContext.GetContext(this.Tenant) as DbContext)
                {
                    var cmd = context.Database.GetDbConnection().CreateCommand();
                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {
                        cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_SetStatus", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                    }
                    else
                    {
                        cmd.CommandText = "[dbo].[Queue_SetStatus]";
                    }


                    cmd.CommandType = CommandType.StoredProcedure;


                    //OracleParameter messageIdPar = new OracleParameter("MessageId", OracleDbType.Number, 18);
                    var messageIdPar = cmd.CreateParameter(); messageIdPar.ParameterName = "V_MESSAGEID"; messageIdPar.DbType = DbType.Double;
                    //OracleParameter statusPar = new OracleParameter("Statud", OracleDbType.Number);
                    var statusPar = cmd.CreateParameter(); statusPar.ParameterName = "V_STATUD"; statusPar.DbType = DbType.Double;

                    messageIdPar.Direction = ParameterDirection.Input;
                    statusPar.Direction = ParameterDirection.Input;

                    messageIdPar.Value = this.CurrentMessageId;
                    if (Failed)
                    {
                        statusPar.Value = -1;
                    }
                    else
                    {
                        statusPar.Value = 1;
                    }


                    cmd.Parameters.Add(messageIdPar);
                    cmd.Parameters.Add(statusPar);

                    try
                    {
                        cmd.Connection.Open();
                        var output = cmd.ExecuteNonQuery();

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
                scope.Complete();
            }
        }




        public void DelayCustoms(TimeSpan delayTime)
        {

            if (!AmitalCloudSettings.IsCostomsDeploy)
            {
                throw new Exception("DelayCustoms  use in AmitalCloudSettings.IsCostomsDeploy");
            }
            if (string.IsNullOrEmpty(this.CurrentMessageId))
            {
                return;
            }


            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                int seconds = (int)delayTime.TotalSeconds;

                using (var context = AmitalCloudContext.GetContext(this.Tenant) as DbContext)
                {
                    var cmd = context.Database.GetDbConnection().CreateCommand();
                    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                    {
                        cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("Queue_DelayMessage", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                    }
                    else
                    {
                        cmd.CommandText = "[dbo].[Queue_DelayMessage]";
                    }

                    cmd.CommandType = CommandType.StoredProcedure;


                    //OracleParameter messageIdPar = new OracleParameter("MessageId", OracleDbType.Number, 18);
                    var messageIdPar = cmd.CreateParameter(); messageIdPar.ParameterName = "V_MESSAGEID"; messageIdPar.DbType = DbType.Double;
                    //OracleParameter delayPar = new OracleParameter("DelaySeconds", OracleDbType.Number);
                    var delayPar = cmd.CreateParameter(); delayPar.ParameterName = "V_DELAYSECONDS"; delayPar.DbType = DbType.Double;

                    messageIdPar.Direction = ParameterDirection.Input;
                    delayPar.Direction = ParameterDirection.Input;

                    messageIdPar.Value = this.CurrentMessageId;
                    delayPar.Value = seconds;

                    cmd.Parameters.Add(messageIdPar);
                    cmd.Parameters.Add(delayPar);

                    try
                    {
                        cmd.Connection.Open();
                        var output = cmd.ExecuteNonQuery();

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

                scope.Complete();
            }



        }
    }

}


