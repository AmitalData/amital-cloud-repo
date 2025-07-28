using AmitalCloud.Infrastructure.Data.Azure;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.DBHelpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Services;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Transactions;
using System.Xml;
using System.Xml.Serialization;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AmitalCloud.Infrastructure.Model.Enums;
using Azure.Storage.Queues;
using Azure.Messaging.ServiceBus;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class Communications
    {
        public static CommunicationLog GetCommunicationLog(int tenant, string requestCommunicationLogId)
        {

            //CommunicationLog communicationLog = null;
            var myContext = AmitalCloudContext.GetContext(tenant);
            CommunicationLogRepository communicationLogRep = new CommunicationLogRepository(myContext);

            var myCommLog = communicationLogRep.GetSingleCommunicationLog(requestCommunicationLogId, tenant);
            return myCommLog;
        }
        public static CommunicationLog GetCommunicationLogByCorrelationID(int tenant, string correlationID)
        {
            var myContext = AmitalCloudContext.GetContext(tenant);
            CommunicationLogRepository communicationLogRep = new CommunicationLogRepository(myContext);

            var myCommLog = communicationLogRep.GetSingleCommunicationByCorrelationID(tenant, correlationID);
            return myCommLog;
        }
        public static CommunicationLog GetSingleCommunicationLogInProccess(int tenant, string entityId, string to, string correlationID)
        {

            //CommunicationLog communicationLog = null;
            var myContext = AmitalCloudContext.GetContext(tenant);
            CommunicationLogRepository communicationLogRep = new CommunicationLogRepository(myContext);

            var myCommLog = communicationLogRep.GetSingleCommunicationLogInProccess(entityId, tenant, to, correlationID);
            return myCommLog;
        }
        public static CommunicationLog GetSingleCommunicationLogInProccess(int tenant, string entityId, List<string> subjects)
        {

            //CommunicationLog communicationLog = null;
            var myContext = AmitalCloudContext.GetContext(tenant);
            CommunicationLogRepository communicationLogRep = new CommunicationLogRepository(myContext);

            var myCommLog = communicationLogRep.GetSingleCommunicationLogInProccess(entityId, tenant, subjects);
            return myCommLog;
        }


        public static string GetData(CommunicationLog myCommLog)
        {
            string xmlfile;
            if (myCommLog == null)
            {
                throw new Exception("CommunicationAttachment is empty WaitingCommLog.Id=" + myCommLog.Id + ",Tenant=" + myCommLog.Tenant);
            }

            string filename;
            if (myCommLog.Document == null)
            {
                throw new Exception("CommunicationAttachment WaitingCommLog.Document== null WaitingCommLog.Id=" + myCommLog.Id + ",Tenant=" + myCommLog.Tenant);
            }

            filename = myCommLog.DocumentId + "." + myCommLog.Document.Extension;

            BlobFileInfo fileInfo = new BlobFileInfo()
            {
                FileName = myCommLog.Document.Id,
                FolderName = myCommLog.Document.Folder,
                Extension = myCommLog.Document.Extension,
                Tenant = myCommLog.Document.Tenant,
                FileSize = myCommLog.Document.FileSize,
            };
            //todo
            //IBlobService storageservice = ContainerAccessor.Container.Resolve(typeof(IBlobService), "StorageService", new ParameterOverride("", 1)) as IBlobService;
            //byte[] dataByte = storageservice.Read(fileInfo);
            byte[] dataByte = null;

            Encoding encoding = Encoding.UTF8;
            xmlfile = encoding.GetString(dataByte);


            //CloudBlobContainer blobContainer = StorageAcountDetails.GetCurrentContainer(myCommLog.Tenant);
            //var blobfile = blobContainer.GetBlockBlobReference(StorageAcountDetails.GetBlobNameByLocation(filename, myCommLog.Document.Folder));


            //using (MemoryStream memstream = new MemoryStream())
            //{
            //    blobfile.DownloadToStream(memstream);
            //    Encoding encoding = Encoding.UTF8;
            //    xmlfile = encoding.GetString(memstream.ToArray());
            //}
            return xmlfile;
        }
        public static void SetBolb(int tenant, string filename, string documentFolder, byte[] byteData)
        {
            //string filename = Guid.NewGuid().ToString() + ".xml" ;
            //var stopwatch = System.Diagnostics.Stopwatch.StartNew(); 
            //var blobContainer = StorageAcountDetails.GetCurrentContainer(tenant);
            //var blobfile = blobContainer.GetBlockBlobReference(StorageAcountDetails.GetBlobNameByLocation(filename, documentFolder));
            //blobfile.UploadFromStream(new MemoryStream(byteData));
            //using (Stream blobstream = blobfile.OpenWrite())
            //{


            //blobstream.Write(byteData, 0, (int)byteData.Length);
            //}
            //stopwatch.Stop();
            //LogMessagingUtil.Instance.AppendLine("SetBolb:" + filename + ":Took:" + stopwatch.Elapsed.ToString());


            //string filePath = "tenant" + tenant + "/" + StorageAcountDetails.GetBlobNameByLocation(filename, documentFolder);
            string[] fileparams = filename.Split('.');
            BlobFileInfo fileInfo = new BlobFileInfo()
            {
                FileName = fileparams[0],
                FolderName = documentFolder,
                Extension = fileparams[1],
                Tenant = tenant,
                FileSize = byteData.Length,

            };
            //todo
            //IBlobService storageservice = ContainerAccessor.Container.Resolve(typeof(IBlobService), "StorageService", new ParameterOverride("", 1)) as IBlobService;
            //storageservice.Write(byteData, fileInfo);

        }

        //public static string AddCommunicationLog(CommunicationsParams communicationParams)
        //{
        //    //if((communicationParams.QueueName != null && communicationParams.QueueName.StartsWith("externaltasksqueue")) || communicationParams.To == "Unifreight")
        //    //{
        //    //    CustomsSetting customsSettings = CustomsSettingRepository.GetSettingByTenantCache(communicationParams.Tenant);
        //    //    if (customsSettings.StandAlone)
        //    //        return null;
        //    //}

        //    CommunicationLog commLog;
        //    using (TransactionScope scope = TransactionFactory.GetTransaction())
        //    {
        //        int tenant = communicationParams.Tenant;
        //        IAmitalCloudContext commonContext = AmitalCloudContext.GetContext(tenant);
        //        UserRepository userRepository = new UserRepository(commonContext);
        //        CommunicationLogRepository communicationLogRepository = new CommunicationLogRepository(commonContext);
        //        DocumentRepository documentrepository = new DocumentRepository(commonContext);
        //        ObjectTableRepository objecttableRep = new ObjectTableRepository(communicationParams.Tenant);
        //        ObjectTable objectTable = null;// Itzik default (aksioma  there is communication that not conected to entity) 
        //        if (!String.IsNullOrWhiteSpace(communicationParams.LoggingObjectTableId))
        //        {

        //            objectTable = objecttableRep.GetSingleObjectTable(communicationParams.LoggingObjectTableId, 0, true);
        //        }
        //        else
        //        {
        //            objectTable = new ObjectTable();
        //        }
        //        Document document = new Document()
        //        {
        //            CreateDate = DateTime.Now,
        //            Extension = !string.IsNullOrEmpty(communicationParams.FileExtension) ? communicationParams.FileExtension : "xml",
        //            FileSize = communicationParams.ByteData.Length,
        //            Tenant = Convert.ToInt32(tenant),
        //            Id = IdCounter.GetNumber("Document", tenant),
        //            HasFile = true,
        //            Folder = communicationParams.FolderName,
        //        };
        //        documentrepository.Insert(document);
        //        documentrepository.SubmitChanges();
        //        User loggedUser = null;
        //        if (!string.IsNullOrEmpty(communicationParams.LoggingUserId))
        //        {
        //            loggedUser = userRepository.GetSingleUser(communicationParams.LoggingUserId, tenant);
        //        }
        //        if (loggedUser == null)
        //        {
        //            string systenEmail = "system@tenant" + tenant + ".com";
        //            User systemUser = userRepository.GetSingleUserByEmail(systenEmail, tenant, true);
        //            if (systemUser != null)
        //                communicationParams.LoggingUserId = systemUser.Id;
        //        }
        //        commLog = new CommunicationLog()
        //        {
        //            Id = IdCounter.GetNumber("CommunicationLog", tenant),
        //            LastStatusDate = TenantServerConfigration.GetCurrentDateTime(tenant),
        //            To = communicationParams.To,
        //            From = communicationParams.From,
        //            BCC = communicationParams.BCC,
        //            CC = communicationParams.CC,
        //            InOut = communicationParams.InOut,
        //            EntityId = communicationParams.LoggingEntityId,
        //            ObjectTableId = !string.IsNullOrEmpty(communicationParams.LoggingObjectTableId) ? communicationParams.LoggingObjectTableId : null,
        //            Subject = communicationParams.Subject,
        //            Tenant = tenant,
        //            CommunicationLogTypeCode = communicationParams.CommunicationLogTypeCode,
        //            CreateDate = TenantServerConfigration.GetCurrentDateTime(tenant),
        //            CommunicationStatusTypeCode = communicationParams.Status,
        //            CreatedByUserId = communicationParams.LoggingUserId,
        //            DocumentId = document.Id,
        //            EntityReference = communicationParams.LoggingEntityReference,
        //            SearchFields = communicationParams.To + ',' + communicationParams.From + ',' + communicationParams.BCC + ',' + communicationParams.CC + ',' + communicationParams.InOut + ',' + communicationParams.Subject + ',' + communicationParams.CommunicationLogTypeCode + ',' + communicationParams.Status + ',' + communicationParams.LoggingEntityReference + ',' + objectTable.Name,
        //            Logs = communicationParams.Logs,
        //            CorrelationID = communicationParams.CorrelationID,
        //            NextTryDateTime = communicationParams.NextTryDateTime,
        //            CreateDateUTC = DateTime.UtcNow,
        //            LastStatusDateUTC = DateTime.UtcNow,
        //            QueueName = communicationParams.QueueName,
        //            Priority = communicationParams.Priority,
        //            AdditionalFields = communicationParams.AdditionalFields,
        //            ExceptionMessage = communicationParams.ExceptionMessage,
        //            UniqueNumber = communicationParams.UniqueNumber,
        //            WasAnalyzed = communicationParams.WasAnalyzed,
        //        };
        //        communicationLogRepository.Add(commLog);
        //        communicationLogRepository.SubmitChanges();

        //        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        //        string filename = document.Id + "." + document.Extension;
        //        string filePath = "tenant" + communicationParams.Tenant + "/" + StorageAcountDetails.GetBlobNameByLocation(filename, document.Folder);
        //        BlobFileInfo fileInfo = new BlobFileInfo()
        //        {
        //            FileName = document.Id,
        //            FolderName = document.Folder,
        //            Extension = document.Extension,
        //            Tenant = document.Tenant,
        //            FileSize = communicationParams.ByteData.Length,

        //        };
        //        //todo
        //        //IBlobService storageservice = ContainerAccessor.Container.Resolve(typeof(IBlobService), "StorageService", new ParameterOverride("", 1)) as IBlobService;
        //        //storageservice.Write(communicationParams.ByteData, fileInfo);

        //        stopwatch.Stop();
        //        LogMessagingUtil.Instance.AppendLine("SetBolb:" + filePath + ":Took:" + stopwatch.Elapsed.ToString());


        //        if (!string.IsNullOrEmpty(communicationParams.QueueName))
        //        {
        //            try
        //            {
        //                Communications.UpdateCommunicationLogStatus(commLog.Id, tenant, null, commLog.CommunicationStatusTypeCode, "Before adding message to queue " + DateTime.Now.ToString(), null);
        //                SendCommunicationLogMessageToQueue(communicationParams.QueueName, commLog.Id, tenant, communicationParams.QueueParameters);
        //                Communications.UpdateCommunicationLogStatus(commLog.Id, tenant, null, commLog.CommunicationStatusTypeCode, "after adding message to queue " + DateTime.Now.ToString(), null);
        //            }
        //            catch (Exception ex)
        //            {
        //                string errorMessage = ex.Message;

        //                if (!string.IsNullOrEmpty(ex.StackTrace))
        //                {
        //                    errorMessage += Environment.NewLine + ex.StackTrace;
        //                }

        //                Communications.UpdateCommunicationLogStatus(commLog.Id, tenant, null, commLog.CommunicationStatusTypeCode, "Exception occured while adding message to queue " + DateTime.Now.ToString(), errorMessage);

        //                throw ex;
        //            }
        //        }
        //        scope.Complete();


        //    }
        //    return commLog.Id;
        //}

        public static byte[] SerializeData<T>(T dataObject)
        {
            MemoryStream memstream = new MemoryStream();

            XmlSerializer ser = new XmlSerializer(typeof(T));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "http://www.champ.aero/GCCS/CargoXML");
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "",
                OmitXmlDeclaration = true,
                NewLineChars = "",
                NewLineHandling = NewLineHandling.Replace,


            };

            XmlWriter writer = XmlTextWriter.Create(memstream, settings);
            writer.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n");
            ser.Serialize(writer, dataObject, ns);
            memstream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(memstream);
            string content = reader.ReadToEnd();
            content = content.Replace(" />", "/>");
            byte[] bytearray = Encoding.ASCII.GetBytes(content);

            return bytearray;
        }


        public static void SendCommunicationLogMessageToQueue(string queueName, string communicationLogId, int tenant, Dictionary<string, string> queueParameters = null,
            TimeSpan? delayTime = null,
            string InterfaceTypeCode = null

            )
        {


            try
            {
                if (!AmitalCloudSettings.IsCostomsDeploy && !SettingUtil.DeploymentStage.IsDBStage(SettingUtil.DeploymentStage.AmitalOracle))
                {

                    DbQueueService queueservice = new DbQueueService(queueName, tenant);
                    if (queueParameters != null)
                    {
                        if (!queueParameters.Keys.Contains("CommunicationLogId"))
                            queueParameters.Add("CommunicationLogId", communicationLogId);
                        queueservice.Send(queueParameters, tenant);

                    }
                    else
                    {
                        queueservice.Send(new Dictionary<string, string>() { { "CommunicationLogId", communicationLogId }, { "Tenant", tenant.ToString() } }, tenant);
                    }


                    //BrokeredMessage message = new BrokeredMessage();

                    //message.Properties["CommunicationLogId"] = communicationLogId;
                    // message.Properties["Tenant"] = tenant;

                    //QueueClient client = GetQueueClient(queueName);
                    //using (TransactionScope scope = TransactionFactory.GetNewSerializableTransaction())
                    //{
                    //    client.Send(message);
                    //    scope.Complete();
                    //}
                }
                else
                {




                    var UseRabbitMQ = CustomDbQueueService.IsFeatureOnRABBITMQ_Communication() && CustomDbQueueService.SupportedRabbitMQList.Contains(queueName);

                    var queueService = new CustomDbQueueService//();
                                                               //queueService.InitializeQueue
                    (queueName, 0);
                    var messageProperties = new Dictionary<string, string>();
                    messageProperties["CommunicationLogId"] = communicationLogId;
                    messageProperties["Tenant"] = tenant.ToString();
                    var queueId = queueService.Send(messageProperties, tenant, delayTime, new QueueSendModel()
                    {
                        TenantPriority = 7,
                        UseRabbitMQ = UseRabbitMQ,
                        EntityCode = "CommunicationLog".ToLower(),
                        EntityId = communicationLogId,
                        InterfaceTypeCode = InterfaceTypeCode
                    });

                    LogMessagingUtil.Instance.AppendLine($"SendCommunicationLogMessageToQueue({queueName}, {communicationLogId})=>QID={queueId} ");
                    ///throw new Exception("Queue is DbMode "); 
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, DateTime.Now, 0, null, "SendCommunicationLogMessageToQueue", null, null);
            }
        }


        public static ServiceBusReceiver GetQueueClient(string queuename)
        {
            queuename = AmitalCloudEntryPoint.GetQueueByEnviroment(queuename);

            //todo
            /*
            if (!StorageAcountDetails.NameSpaceManager.QueueExistsAsync(queuename))
            {
                QueueDescription queueDescription = new QueueDescription(queuename);
                queueDescription.MaxSizeInMegabytes = 5120;
                queueDescription.MaxDeliveryCount = 99999;
                queueDescription.LockDuration = new TimeSpan(0, 5, 0);

                //queueDescription.LockDuration
                //queueDescription.DefaultMessageTimeToLive = new TimeSpan(3, 1, 0);

                StorageAcountDetails.NameSpaceManager.CreateQueueAsync(queueDescription);
            }*/

            ServiceBusReceiver client = StorageAcountDetails.CreateServiceBusQueueClient(queuename, ServiceBusReceiveMode.PeekLock);

            return client;
        }


        public static void UpdateCommunicationLogStatus(string id, int tenant, object messageLockId, object statusTypeCode, object log, object exceptionMessage)
        {

            //usp_UpdateQueueCommunicationLog
            //                      @pCommunicationLogId  as  varchar(40),
            //@pTenant   as int,
            //@pCommunicationStatusTypeCode as varchar(4),
            //@pLog as  nvarchar(MAX),
            //@pExceptionMessage as  nvarchar(MAX),
            //@pMessageLockId  as  varchar(40)

            log = (log == null ? DBNull.Value : log);
            exceptionMessage = (exceptionMessage == null ? DBNull.Value : exceptionMessage);
            messageLockId = (messageLockId == null ? DBNull.Value : messageLockId);

            //DBNull.Value;
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                UpdateQueueCommunicationLogOracle(id, tenant, messageLockId, statusTypeCode, log, exceptionMessage);
            }
            else
            {
                UpdateQueueCommunicationLogMSSQL(id, tenant, messageLockId, statusTypeCode, log, exceptionMessage);
            }

        }
        static string GetConnection(int tenant)
        {
            GlobalDBRepository globalDbRep;
            GlobalDB currentDb;
            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                //GlobalDBRep = new GlobalDBRepository();
                currentDb = new GlobalDBRepository(ConfigurationHelper.Conf).GetGlobalDBByTenant(tenant);
            }
            string dbConnectionInfo = currentDb.DBConnection;

            return dbConnectionInfo;
        }
        private static void UpdateQueueCommunicationLogOracle(string id, int tenant, object messageLockId, object statusTypeCode, object log, object exceptionMessage)
        {




            string number = null;
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {

                using (DbConnection cn =
                    new OracleConnection(GetConnection(tenant))
                    //(AmitalCloudContext.GetContext(tenant) as DbContext).Database.Connection
                    )
                {

                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = cn as OracleConnection;
                    cmd.CommandText = DbContextBaseUtil.GetStoredProcedureName("usp_UpdateQueueCommunicationLo", AmitalCloudDBSchema.AMITAL_MAIN, cmd.Connection.ConnectionString);
                    cmd.CommandType = CommandType.StoredProcedure;


                    var pCommunicationLogId = new OracleParameter()
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2
                        //, Size = 40
                        ,
                        ParameterName = "v_pCommunicationLogId",
                        Value = id
                    };
                    cmd.Parameters.Add(pCommunicationLogId);

                    var pTenant = new OracleParameter()
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Int32,
                        ParameterName = "v_pTenant",
                        Value = tenant
                    };
                    cmd.Parameters.Add(pTenant);
                    var pCommunicationStatusTypeCode = new OracleParameter()
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        //Size = 4, 
                        ParameterName = "v_pCommunicationStatusTypeCode",
                        Value = statusTypeCode
                    };
                    cmd.Parameters.Add(pCommunicationStatusTypeCode);
                    var pLog = new OracleParameter()
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.NVarchar2,
                        //Size = -1,
                        ParameterName = "iv_pLog",
                        Value = log
                    };
                    cmd.Parameters.Add(pLog);
                    var pExceptionMessage = new OracleParameter()
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        //Size = -1,
                        ParameterName = "v_pExceptionMessage",
                        Value = exceptionMessage
                    };
                    cmd.Parameters.Add(pExceptionMessage);
                    var pMessageLockId = new OracleParameter()
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        //Size = 40,
                        ParameterName = "v_pMessageLockId",
                        Value = messageLockId
                    };

                    cmd.Parameters.Add(pMessageLockId);

                    try
                    {
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        //number = (string)cmd.Parameters["v_pLastNumber"].Value;

                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception: {0}", ex.ToString());
                        throw;
                    }

                    cn.Close();
                }


            }
        }
        private static void UpdateQueueCommunicationLogMSSQL(string id, int tenant, object messageLockId, object statusTypeCode, object log, object exceptionMessage)
        {
            System.Collections.Generic.List<StoredProcedureParam> paramList = new System.Collections.Generic.List<StoredProcedureParam>()
                        {
                            new StoredProcedureParam()   { Direction = ParameterDirection.Input, ParamDBType = SqlDbType.VarChar, ParamSize = 40, ParamName = "@pCommunicationLogId",Value = id },
                            new StoredProcedureParam()   { Direction = ParameterDirection.Input, ParamDBType = SqlDbType.Int, ParamName = "@pTenant",Value = tenant },
                            new StoredProcedureParam()   { Direction = ParameterDirection.Input, ParamDBType = SqlDbType.VarChar, ParamSize = 4, ParamName = "@pCommunicationStatusTypeCode",Value= statusTypeCode },
                            new StoredProcedureParam()   { Direction = ParameterDirection.Input, ParamDBType = SqlDbType.NVarChar, ParamSize = -1, ParamName = "@pLog",Value = log },
                            new StoredProcedureParam()   { Direction = ParameterDirection.Input, ParamDBType = SqlDbType.VarChar, ParamSize = -1, ParamName = "@pExceptionMessage",Value = exceptionMessage },
                            new StoredProcedureParam()   { Direction = ParameterDirection.Input, ParamDBType = SqlDbType.VarChar, ParamSize = 40, ParamName = "@pMessageLockId",Value = messageLockId }

                        };

            object value = ExecuteStoredProcedures.Execute("dbo.usp_UpdateQueueCommunicationLog", tenant, paramList);
        }


        //public static string AddEmailCommunicationLogQueue(EmailCommunicationParams communicationParams, int tenant)
        //{
        //    if (!string.IsNullOrEmpty(communicationParams.To) && communicationParams.To.Contains("system@tenant"))
        //    {
        //        return string.Empty;
        //    }
        //    IAmitalCloudContext commonContext = AmitalCloudContext.GetContext(tenant);
        //    DocumentRepository documentRepository = new DocumentRepository(commonContext);
        //    UserRepository userRepository = new UserRepository(commonContext);
        //    CommunicationLogRepository communicationLogRepository = new CommunicationLogRepository(commonContext);

        //    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        //    byte[] htmlByteData = enc.GetBytes(communicationParams.EmailBody);

        //    Document document = new Document()
        //    {
        //        CreateDate = TenantServerConfigration.GetCurrentDateTime(tenant),
        //        Extension = "html",
        //        FileSize = Convert.ToInt32(htmlByteData.Length),
        //        Tenant = Convert.ToInt32(tenant),
        //        Id = IdCounter.GetNumber("Document", tenant).ToString(),
        //        Folder = "emailsout",
        //    };

        //    documentRepository.Insert(document);
        //    documentRepository.SubmitChanges();

        //    string filePath = "tenant" + tenant.ToString() + "/" + StorageAcountDetails.GetBlobNameByLocation(document.Id + ".html", document.Folder);

        //    //todo
        //    //IBlobService storageservice = ContainerAccessor.Container.Resolve(typeof(IBlobService), "StorageService", new ParameterOverride("", 1)) as IBlobService;
        //    BlobFileInfo fileInfo = new BlobFileInfo()
        //    {
        //        FileName = document.Id,
        //        FolderName = document.Folder,
        //        Extension = document.Extension,
        //        Tenant = tenant,
        //        FileSize = htmlByteData.Length,

        //    };

        //    //storageservice.Write(htmlByteData, fileInfo);
        //    User loggedUser = null;
        //    if (!string.IsNullOrEmpty(communicationParams.LoggingUserId))
        //    {
        //        loggedUser = userRepository.GetSingleUser(communicationParams.LoggingUserId, tenant);
        //    }
        //    if (loggedUser == null)
        //    {
        //        string systenEmail = "system@tenant" + tenant + ".com";
        //        User systemUser = userRepository.GetSingleUserByEmail(systenEmail, tenant, false);
        //        if (systemUser != null)
        //            communicationParams.LoggingUserId = systemUser.Id;
        //    }
        //    CommunicationLog commLog = new CommunicationLog()
        //    {
        //        Id = IdCounter.GetNumber("CommunicationLog", tenant),
        //        Tenant = tenant,
        //        CreateDate = TenantServerConfigration.GetCurrentDateTime(tenant),
        //        To = communicationParams.To,
        //        From = communicationParams.From,
        //        BCC = communicationParams.BCC,
        //        CC = communicationParams.CC,
        //        LastStatusDate = TenantServerConfigration.GetCurrentDateTime(tenant),
        //        CommunicationLogTypeCode = "E",
        //        CommunicationStatusTypeCode = "W",
        //        InOut = "O",
        //        EntityId = communicationParams.LoggingEntityId,
        //        ObjectTableId = !string.IsNullOrEmpty(communicationParams.LoggingObjectTableId) ? communicationParams.LoggingObjectTableId : null,
        //        Subject = communicationParams.Subject,
        //        CreatedByUserId = communicationParams.LoggingUserId,
        //        DocumentId = document.Id,
        //        CreateDateUTC = DateTime.UtcNow,
        //        LastStatusDateUTC = DateTime.UtcNow,
        //        QueueName = "emailqueue",
        //        SearchFields = string.Join(",", new string[] { communicationParams.To, communicationParams.From, communicationParams.BCC, communicationParams.CC, communicationParams.Subject }),
        //        IsSecured = communicationParams.IsBodySecured,
        //    };

        //    communicationLogRepository.Add(commLog);
        //    communicationLogRepository.SubmitChanges();

        //    //IQueueService queueservice = QueueServiceManager.GetQueueService("emailqueue", 0);
        //    //Dictionary<string, string> message = new Dictionary<string, string>()
        //    //        {
        //    //            { "CommunicationLogId", commLog.Id},
        //    //            { "Tenant", commLog.Tenant.ToString() },
        //    //        };

        //    //queueservice.Send(message);

        //    DbQueueService queueservice = new DbQueueService("EmailQueue", tenant);
        //    queueservice.Send(new Dictionary<string, string>() { { "CommunicationLogId", commLog.Id }, { "Tenant", commLog.Tenant.ToString() } }, commLog.Tenant);
        //    return commLog.Id;
        //}

    }



    public class EmailCommunicationParams
    {

        public int Tenant { get; set; }

        public string LoggingEntityId { get; set; }

        public string LoggingObjectTableId { get; set; }

        public string Subject { get; set; }

        public string LoggingUserId { get; set; }

        public string EmailBody { get; set; }

        public string To { get; set; }

        public string From { get; set; }

        public string BCC { get; set; }

        public string CC { get; set; }

        public bool IsBodySecured { get; set; }

    }
}
