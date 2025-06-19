//using Microsoft.WindowsAzure.Storage.Queue;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AmitalCloud.Infrastructure.Data.Azure
//{
//    public class EmailSendingService
//    {
//        //public string InsertCommunicationLog(int tenant, string createdByUserId, string createdByUserName, DateTime localCreateDateTime, string from, string to, string subject, string bodyDocumentId, bool isBodyHtml)
//        //{

//        //    CommunicationLogsAzure communicationLog = new CommunicationLogsAzure()
//        //    {
//        //        PartitionKey = DateTime.Now.Date.Day + "," + DateTime.Now.Date.Month + "," + DateTime.Now.Date.Year, //"1",
//        //        RowKey = Guid.NewGuid().ToString(),
//        //        Id = Guid.NewGuid().ToString(),
//        //        Tenant = tenant,
//        //        CreatedByUserId = createdByUserId,
//        //        CreatedByUserName = createdByUserName,
//        //        From = from,
//        //        To = to,
//        //        Subject = subject,
//        //        StatusCode = "W",
//        //        GMTCreateDateTime = DateTime.Now,
//        //        SendDateTime = DateTime.Now,
//        //        LocalCreateDateTime = localCreateDateTime,
//        //        BodyDocumentId = bodyDocumentId,
//        //        IsBodyHtml = isBodyHtml,

//        //    };


//        //    CommunicationLogsAzureContext commLogcontext = new CommunicationLogsAzureContext();

//        //    commLogcontext.Add(communicationLog);
//        //    //commLogcontext.SaveChanges();

//        //    var storageaccount = StorageAcountDetails.StorageAccount;
//        //    var queueclient = storageaccount.CreateCloudQueueClient();

//        //    var queue = queueclient.GetQueueReference("communicationlogqueue");
//        //    queue.CreateIfNotExists();

//        //    var message = new CloudQueueMessage(communicationLog.Id + "," + communicationLog.Tenant);
//        //    queue.AddMessage(message);

//        //    return communicationLog.Id;
//        //}
//    }
//}
