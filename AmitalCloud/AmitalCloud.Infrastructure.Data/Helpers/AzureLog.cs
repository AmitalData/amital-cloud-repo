using AmitalCloud.Infrastructure.Data.Azure;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using Azure.Storage.Blobs;
using System.Text;
using System.Transactions;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public static class AzureLog
    {
        private static readonly NLog.Logger NLogger = NLog.LogManager.GetLogger("AmitalLogger");

        private static void InitNlogConfig()
        {
            try
            {
                if (NLog.LogManager.Configuration == null)
                    NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NLog.config"));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

        }

        public static void SaveLogsInStorage(string log, string type, DateTime clientDate, string message, string stackTrace, int tenant, string userId, string userName, string IP, Exception cachedException = null)
        {
            BlobContainerClient blobContainer = null;
            BlobClient blobfile = null;

            if (type == "E")
            {
                AddLogRecord(log, tenant, stackTrace, clientDate, userId, userName, IP, cachedException);
            }

            if (AmitalCloudSettings.UsingAzure && AmitalCloudSettings.IsLogEnabled)
            {
                try
                {
                    StringBuilder stringbuilder = new StringBuilder();
                    DateTime now = DateTime.Now;
                    string newlog = "DateTime : " + now.ToString() + " : " + Environment.NewLine + log + Environment.NewLine;
                    blobContainer = StorageAcountDetails.GetCurrentContainer(tenant);
                    blobContainer.CreateIfNotExists();

                    switch (type)
                    {
                        case "VD":
                            {
                                blobfile = blobContainer.GetBlobClient("arinvoicevoid.txt");

                                if (BlobExtensions.Exists(blobfile))
                                {
                                    //Read data from errorlogs
                                    using (Stream blbstrRead = blobfile.OpenRead())
                                    {
                                        if (blbstrRead != null)
                                        {
                                            byte[] previousErrordata = new byte[blbstrRead.Length];
                                            blbstrRead.Read(previousErrordata, 0, previousErrordata.Length);

                                            Encoding encoding = new UTF8Encoding();
                                            stringbuilder.Append(encoding.GetString(previousErrordata));
                                        }
                                    }
                                }

                                //write data to errorlogs
                                using (Stream blbstr = blobfile.OpenWrite(overwrite: true))
                                {

                                    Encoding encoding = new UTF8Encoding();
                                    stringbuilder.AppendLine(newlog);
                                    byte[] errordata = encoding.GetBytes(stringbuilder.ToString());
                                    blbstr.Write(errordata, 0, errordata.Length);
                                }

                                break;
                            }

                        case "E": // Errors Log
                                  // AddLogRecord(log, tenant, stackTrace, clientDate, userId, userName, IP);
                                  //blobfile = blobContainer.GetBlobClient("errorslog.txt");
                            break;

                        case "P": // Performance Log
                            blobfile = blobContainer.GetBlobClient("performancelog.txt");

                            if (BlobExtensions.Exists(blobfile))
                            {
                                //Read data from errorlogs
                                using (Stream blbstrRead = blobfile.OpenRead())
                                {
                                    if (blbstrRead != null)
                                    {
                                        byte[] previousErrordata = new byte[blbstrRead.Length];
                                        blbstrRead.Read(previousErrordata, 0, previousErrordata.Length);

                                        Encoding encoding = new UTF8Encoding();
                                        stringbuilder.Append(encoding.GetString(previousErrordata));
                                    }
                                }
                            }

                            //write data to errorlogs
                            using (Stream blbstr = blobfile.OpenWrite(overwrite: true))
                            {

                                Encoding encoding = new UTF8Encoding();
                                stringbuilder.AppendLine(newlog);
                                byte[] errordata = encoding.GetBytes(stringbuilder.ToString());
                                blbstr.Write(errordata, 0, errordata.Length);
                            }


                            break;


                        case "L": // Performance Log
                            blobfile = blobContainer.GetBlobClient("log.txt");

                            if (BlobExtensions.Exists(blobfile))
                            {
                                //Read data from errorlogs
                                using (Stream blbstrRead = blobfile.OpenRead())
                                {
                                    if (blbstrRead != null)
                                    {
                                        byte[] previousErrordata = new byte[blbstrRead.Length];
                                        blbstrRead.Read(previousErrordata, 0, previousErrordata.Length);

                                        Encoding encoding = new UTF8Encoding();
                                        stringbuilder.Append(encoding.GetString(previousErrordata));
                                    }
                                }
                            }

                            //write data to errorlogs
                            using (Stream blbstr = blobfile.OpenWrite(overwrite: true))
                            {

                                Encoding encoding = new UTF8Encoding();
                                stringbuilder.AppendLine(newlog);
                                byte[] errordata = encoding.GetBytes(stringbuilder.ToString());
                                blbstr.Write(errordata, 0, errordata.Length);
                            }


                            break;

                        default:
                            AddLogRecord(log, tenant, stackTrace, clientDate, userId, userName, IP, cachedException);
                            break;
                    }
                }
                catch { }
            }
        }


        static void AddLogRecord(string exception, int tenant, string stackTrace, DateTime clientDate, string userId, string userName, string IP, Exception cachedException)
        {

            InitNlogConfig();
            NLogger.Error(cachedException, "Tenant {0}, clientDate {1}, userID {2}, userName {3}, IP {4}, stackTrace {5}, exception:{6}",
                tenant, clientDate, userId, userName, IP, stackTrace, exception);

            if (!string.IsNullOrEmpty(exception) && exception.Contains("Sorry! you have no permission to do this operation"))
                return;
            string myStackTrace = GetStackTrace(exception);
            if (!string.IsNullOrEmpty(myStackTrace))
            {
                stackTrace = myStackTrace;
            }
            if (stackTrace?.Length > 7000)
            {
                stackTrace = stackTrace.Substring(0, 6999);
            }
            var better1st7000ThenNothing = true;//itzik 
            if (better1st7000ThenNothing)
            {

                exception = exception ?? "";
                if (cachedException != null && cachedException.Source == "EntityFramework" && exception.Length > 7000)//Islam: take the start and the end if it is a db exception.
                {
                    exception = exception.Substring(0, 3500) + exception.Substring(exception.Length - 3500, 3500);
                    //exception = HandleExceptionLength(exception);
                }
                else
                {
                    exception = exception.Substring(0, Math.Min(7000, exception.Length));
                }
                if (AmitalCloudSettings.IsCostomsDeploy)
                {
                    exception = exception.Substring(0, Math.Min(2000, exception.Length));
                }
            }



            if (String.IsNullOrWhiteSpace(userName))
            {
                userName = userId;
            }
            if (String.IsNullOrWhiteSpace(userName) && AmitalCloudSettings.GetUserNameInject != null)
            {
                userName = AmitalCloudSettings.GetUserNameInject(tenant);
            }
            if (String.IsNullOrWhiteSpace(userName))
            {
                userName = "UnKnown";//Ismust
            }
            if (!string.IsNullOrEmpty(userName))
            {
                userName = TruncateLongString(userName, 99);
            }

            using (var uow = new UnitOfWork<SystemLogContext>(tenant))
            {
                IRepository<ErrorLog> errorLogRrp = new Repository<ErrorLog>(uow);
                ErrorLog errorLog = new ErrorLog()
                {
                    Id = Guid.NewGuid().ToString(),
                    StackTrace = stackTrace,
                    Exception = exception,
                    Tenant = tenant,
                    UserName = userName,
                    LogDate = DateTime.Now,
                    Tier = "Server",
                    ClientDate = clientDate,
                    SearchFields = "Server" + "," + userName + "," + exception + "," + tenant,
                    IP = IP,
                };
                uow.CreateTransactionScope(TransactionScopeOption.RequiresNew);
                errorLogRrp.Insert(errorLog);
                uow.Save();
                uow.Commit();
                try// mohammad : i didn't understand this code but it keeps throwing an exception
                {
                    var curr = AmitalCloudDomainScope.GetCurrent<ExceptionInErrorLog>();
                    if (curr != null)
                    {
                        curr.ErrorlogId = errorLog.Id;
                        curr.Message = exception;
                    }
                }
                catch
                {
                }
            }
        }

        private static string GetStackTrace(string exception)
        {
            string StackTrace = "";
            string[] myException = exception.Split('~');

            if (myException.Length == 3)
            {
                //string Header = myException[0];
                //string Body = myException[1]; 
                StackTrace = myException[2];
            }
            return StackTrace;
        }

        public static void SaveWarmingLogsInStorage(string log, string type, int tenant)
        {
            BlobContainerClient blobContainer = null;
            BlobClient blobfile = null;

            if (AmitalCloudSettings.UsingAzure && AmitalCloudSettings.IsLogEnabled)
            {
                StringBuilder stringbuilder = new StringBuilder();
                DateTime now = DateTime.Now;
                string newlog = "DateTime : " + now.ToString() + " : " + Environment.NewLine + log + Environment.NewLine;
                blobContainer = StorageAcountDetails.GetCurrentContainer("warminglogs");
                blobContainer.CreateIfNotExists();
                string machineInfo = (!string.IsNullOrEmpty(Environment.MachineName) ? Environment.MachineName : "").ToLower();
                machineInfo = machineInfo + "_" + DateTime.Now.Date.ToShortDateString().ToLower();
                switch (type)
                {

                    case "E": // Errors Log

                        blobfile = blobContainer.GetBlobClient("warmingerrors_" + machineInfo + ".txt");
                        break;

                    case "M": // Performance Log
                        blobfile = blobContainer.GetBlobClient("warminglogs_" + machineInfo + ".txt");
                        break;

                    default:
                        blobfile = blobContainer.GetBlobClient("warminglogs_" + machineInfo + ".txt");
                        // blobfile = blobContainer.GetBlobClient("errorslog.txt");
                        break;


                }

                try
                {
                    if (BlobExtensions.Exists(blobfile))
                    {
                        //Read data from errorlogs
                        using (Stream blbstrRead = blobfile.OpenRead())
                        {
                            if (blbstrRead != null)
                            {
                                byte[] previousErrordata = new byte[blbstrRead.Length];
                                blbstrRead.Read(previousErrordata, 0, previousErrordata.Length);

                                Encoding encoding = new UTF8Encoding();
                                stringbuilder.Append(encoding.GetString(previousErrordata));
                            }
                        }
                    }
                    //write data to errorlogs
                    using (Stream blbstr = blobfile.OpenWrite(overwrite: true))
                    {
                        Encoding encoding = new UTF8Encoding();
                        stringbuilder.AppendLine(newlog);
                        byte[] errordata = encoding.GetBytes(stringbuilder.ToString());
                        blbstr.Write(errordata, 0, errordata.Length);
                    }
                }

                catch (Exception e)
                {
                }
            }
        }

        public static string TruncateLongString(string str, int maxLength)
        {
            if (!string.IsNullOrEmpty(str))

                return str.Substring(0, Math.Min(str.Length, maxLength));

            else
                return str;
        }
    }
}