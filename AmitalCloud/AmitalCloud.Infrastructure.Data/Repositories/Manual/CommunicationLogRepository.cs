using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class CommunicationLogRepository : Repository<CommunicationLog>, IRepository<CommunicationLog>
    {
        IAmitalCloudContext currentContext;


        public CommunicationLogRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }

        public CommunicationLogRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public CommunicationLog GetSingleCommunicationLogInProccess(string entityId, int tenant, string to, string correlationID)
        {
            DateTime dateTime = DateTime.Now.AddDays(-1);// 
            var q = (from a in context.CommunicationLogs
                     where a.To == to && a.EntityId == entityId && a.CommunicationStatusTypeCode == "W" && a.CorrelationID == correlationID
                     select a);
            q = q.Where(r => r.CreateDate > dateTime);//bad solution - need time !!!
            var log = q.FirstOrDefault();
            return log;

        }
        public CommunicationLog GetSingleCommunicationLogInProccess(string entityId, int tenant, List<string> subjects)
        {
            DateTime dateTime = DateTime.Now.AddDays(-3);// 
            var q = (from a in context.CommunicationLogs
                     where a.EntityId == entityId && a.CommunicationStatusTypeCode == "W" && subjects.Contains(a.Subject)
                     select a);
            q = q.Where(r => r.CreateDate > dateTime);//bad solution - need time !!!
            var log = q.FirstOrDefault();
            //if (log != null)
            //{
            //    log.CommunicationStatusTypeCode = "D";
            //    this.Update(log);

            //    this.SubmitChanges();
            //}
            return log;

        }

        public CommunicationLog GetSingleCommunicationByCorrelationID(int tenant, string correlationID)
        {
            bool lastDay = true;
            var q = (from a in context.CommunicationLogs
                     where a.CorrelationID == correlationID
                     select a);
            if (lastDay)//bad solution - need time !!!
            {
                DateTime dateTime = DateTime.Now.AddDays(-1);// 
                q = q.Where(r => r.CreateDate > dateTime);
            }
            var log = q.FirstOrDefault();
            return log;

        }
        public CommunicationLog GetSingleCommunicationLog(string id, int tenant)
        {
            var q = (from a in context.CommunicationLogs.Include("CommunicationLogType").Include("CommunicationStatusType").Include("CreatedByUser").Include("CreatedByUser.Contact").Include("ObjectTable").Include("InternalDocument").Include("ExternalDocument").Include("Document").Include("CurrentTenant")
                     where a.Id == id
                     select a);
            var log = q.FirstOrDefault();
            return log;

            return (from a in context.CommunicationLogs.Include("CommunicationLogType").Include("CommunicationStatusType").Include("CreatedByUser").Include("CreatedByUser.Contact").Include("ObjectTable").Include("InternalDocument").Include("ExternalDocument").Include("Document").Include("CurrentTenant")
                    where a.Id == id
                    select a).FirstOrDefault();

        }

        public int GetEmailCommunicationLogCountForTenantInLasthour(int tenant)
        {

            DateTime datetime = TenantServerConfigration.GetCurrentDateTime(tenant).AddHours(-1);
            return (from a in context.CommunicationLogs
                    where a.Tenant == tenant && a.CreateDate > datetime && a.CommunicationLogTypeCode == "E"
                    select a).Count();
        }



        public CommunicationLog GetSingleCommunicationLog(string id, int tenant, DateTime? createDate)
        {
            return (from a in context.CommunicationLogs.Include("CommunicationLogType").Include("CommunicationStatusType").Include("CreatedByUser").Include("CreatedByUser.Contact").Include("ObjectTable").Include("InternalDocument").Include("ExternalDocument").Include("Document").Include("CurrentTenant")
                    where a.Id == id && a.Tenant == tenant && a.CreateDate == createDate
                    select a).FirstOrDefault();
        }

        public IQueryable<CommunicationLog> GetCommunicationLogs(int tenant)
        {
            if (tenant != 0)
            {
                return (from a in context.CommunicationLogs.Include("Document").Include("CreatedByUser").Include("CommunicationLogType").Include("CommunicationStatusType").Include("CreatedByUser.Contact").Include("ObjectTable").Include("CurrentTenant")
                        where a.Tenant == tenant
                        select a);
            }
            else
            {
                // for now we consider that there is only one database ,in future this code must consider multi database
                return (from a in context.CommunicationLogs.Include("Document").Include("CreatedByUser").Include("CommunicationLogType").Include("CommunicationStatusType").Include("CreatedByUser.Contact").Include("ObjectTable").Include("CurrentTenant")
                        select a);
                // for now it should get all Record // Zaki asked // 14-6-2016
                //where a.CommunicationLogTypeCode != "Q"
            }
        }
        public IQueryable<CommunicationLog> GetCommunicationLogsByTenant(int tenant)
        {
            if (tenant != 0)
            {
                return (from a in context.CommunicationLogs.Include("Document").Include("CreatedByUser").Include("CommunicationLogType").Include("CommunicationStatusType").Include("CreatedByUser.Contact").Include("ObjectTable").Include("CurrentTenant")
                        where a.Tenant == tenant
                        select a);
            }
            else
            {
                // for now we consider that there is only one database ,in future this code must consider multi database
                return (from a in context.CommunicationLogs.Include("Document").Include("CreatedByUser").Include("CommunicationLogType").Include("CommunicationStatusType").Include("CreatedByUser.Contact").Include("ObjectTable").Include("CurrentTenant")

                        select a);
                //where a.CommunicationLogTypeCode != "Q"
            }
        }

        public bool CommunicationOutGoingLogInProgress(string entityId, string objectTableId, int tenant, string subject = null)
        {
            var q = (from a in context.CommunicationLogs
                     where a.EntityId == entityId && a.ObjectTableId == objectTableId
                     && a.Tenant == tenant && a.InOut == "O" && a.CommunicationStatusTypeCode == "W"
                     select a);
            if (!string.IsNullOrWhiteSpace(subject))
            {
                q = q.Where(r => r.Subject == subject);
            }
            return q.Any();
        }


        public CommunicationLog GetSpecificCommunicationLogForEntity(string entityId, int tenant, DateTime date)
        {
            CommunicationLog log = (from a in context.CommunicationLogs.Include("Document").Include("CreatedByUser").Include("CommunicationLogType").Include("CommunicationStatusType").Include("CreatedByUser.Contact").Include("ObjectTable").Include("CurrentTenant")
                                    where a.EntityId == entityId && a.Tenant == tenant && a.CreateDate > date && a.InOut == "I"
                                    select a).FirstOrDefault();

            return log;
        }


        public CommunicationLog GetCommunicationLogByEntityIdAndQueueNameAndSubject(string entityId, string queueName, string subject, int tenant)
        {
            return (from a in context.CommunicationLogs.Include("Document")
                    where a.EntityId == entityId && a.QueueName == queueName && a.Tenant == tenant && a.Subject == subject
                    select a).OrderByDescending(d => d.CreateDate).FirstOrDefault();

        }
        public CommunicationLog GetCommunicationLogByEntityIdAndSubject(string entityId, string subject, int tenant)
        {
            return (from a in context.CommunicationLogs
                    where a.EntityId == entityId && a.Tenant == tenant && a.Subject == subject
                    select a).OrderByDescending(d => d.CreateDate).FirstOrDefault();

        }

        public List<CommunicationLog> GetShareManifestCommunicationLogByEntityIdAndQueueNameAndSubject(string entityId, string queueName, string subject, string subject2)
        {
            return (from a in context.CommunicationLogs.Include("Document")
                    where a.EntityId == entityId && a.QueueName == queueName && (a.Subject == subject || a.Subject == subject2)
                    select a).OrderByDescending(d => d.CreateDate).ToList();

        }

        public void Add(CommunicationLog entity)
        {
            entity.SearchFields = entity.BCC + "," + entity.CC + "," + entity.To + "," + entity.From + "," + entity.EntityReference + "," + entity.Subject;
            context.CommunicationLogs.Add(entity);
        }

        public void Remove(CommunicationLog entity)
        {
            context.CommunicationLogs.Attach(entity);
            context.CommunicationLogs.Remove(entity);
        }

        public void Update(CommunicationLog entity)
        {
            try
            {
                entity.SearchFields = entity.BCC + "," + entity.CC + "," + entity.To + "," + entity.From + "," + entity.EntityReference + "," + entity.Subject;
                entity.SearchFields = StringHelper.TruncateLongString(entity.SearchFields, 1000);
                entity.ExceptionMessage = StringHelper.TruncateLongString(entity.ExceptionMessage, 4000);
                entity.Logs = StringHelper.TruncateLongString(entity.Logs, 4000);
                entity.EmailDeliveryError = StringHelper.TruncateLongString(entity.EmailDeliveryError, 4000);

                context.CommunicationLogs.Attach(entity);

            }
            catch
            { }

            context.SetAsModified(entity);
        }

        public List<CommunicationLog> All()
        {
            return context.CommunicationLogs.ToList();
        }

        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }

        public bool IsCommunicationLogExsit(string xmlId, int tenant)
        {
            var communicationLog = (from a in context.CommunicationLogs
                                    where a.UniqueNumber == xmlId
                                    && a.Tenant == tenant
                                    select a);

            return communicationLog.Any();
        }
    }
}
