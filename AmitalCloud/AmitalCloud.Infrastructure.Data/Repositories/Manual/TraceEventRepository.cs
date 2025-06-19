using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;

using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class TraceEventRepository : Repository<TraceEvent>, IRepository<TraceEvent>
    {
        IAmitalCloudContext amitalCloudContext;

        public TraceEventRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public TraceEventRepository(IAmitalCloudContext context) : base(context)
        {
            amitalCloudContext = context;
        }
        public TraceEventRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public TraceEvent GetSingleTraceEvent(string id)
        {
            return (from a in context.TraceEvents.Include("EventType").Include("EventType.EntityStatus")
                    where a.Id == id
                    select a).FirstOrDefault();
        }

        public TraceEvent GetSingleTraceEventByExternalId(string externalId, int tenant)
        {
            return (from a in context.TraceEvents.Include("EventType").Include("EventType.EntityStatus")
                    where a.ExternalId == externalId && a.Tenant == tenant
                    select a).FirstOrDefault();
        }

        public TraceEvent GetSingleTraceEventByEntityId(string entityId, string eventTypeId, int tenant)
        {
            TraceEvent traceevent = (from a in context.TraceEvents.Include("EventType").Include("EventType.EntityStatus")
                                     where a.EntityId == entityId && a.Tenant == tenant && a.EventTypeId == eventTypeId && !a.Deleted
                                     select a).FirstOrDefault();
            return traceevent;
        }

        public TraceEvent GetSingleTraceEventByExternalIdandEventTypeCode(string externalId, string eventTypeCode, int tenant)
        {
            return (from a in context.TraceEvents.Include("EventType").Include("EventType.EntityStatus")
                    where a.ExternalId == externalId && a.EventType.Code.ToLower() == eventTypeCode.ToLower() && a.Tenant == tenant
                    select a).FirstOrDefault();
        }

        public IQueryable<TraceEvent> GetTraceEvents(int tenant, string entityId, string objectTableId)
        {
            IQueryable<TraceEvent> iQueryable = from a in context.TraceEvents.Include("EventType").Include("EventType.EntityStatus")
                                                where a.Tenant == tenant
                                                && a.EntityId == entityId
                                                && a.ObjectTableId == objectTableId
                                                select a;

            return iQueryable;
        }

        public IQueryable<TraceEvent> GetEntityStatusTraceEvents(int tenant, string entityId, string objectTableId)
        {
            return (from a in context.TraceEvents.Include("EventType").Include("EventType.EntityStatus")
                    where a.Tenant == tenant
                    && a.EntityId == entityId
                    && a.ObjectTableId == objectTableId
                    && a.EventType.EntityStatusId != null
                    && a.Deleted == false
                    select a).OrderBy(a => a.EventDateTime);
        }

        public TraceEvent GetLatestEntityStatusTraceEvent(int tenant, string entityId, string objectTableId)
        {
            return (from a in context.TraceEvents.Include("EventType").Include("EventType.EntityStatus")
                    where a.Tenant == tenant
                    && a.EntityId == entityId
                    && a.ObjectTableId == objectTableId
                    && a.EventType.EntityStatusId != null
                    && a.Deleted == false
                    select a).OrderByDescending(a => a.EventDateTime).FirstOrDefault();
        }

        public IQueryable<TraceEvent> GetAllTraceEventsByEventType(string entityId, string eventTypeId, int tenant)
        {
            IQueryable<TraceEvent> iQueryable = from a in context.TraceEvents
                                                where a.Tenant == tenant
                                                && a.EntityId == entityId
                                                && a.EventTypeId == eventTypeId
                                                select a;

            return iQueryable;
        }

        public IAmitalCloudContext context
        {
            get { return amitalCloudContext; }
        }



        public TraceEvent GetLastExceptionTraceEventByShipmentId(string entityId, int tenant)
        {
            TraceEvent traceevent = (from a in context.TraceEvents.Include("EventType").Include("EventType.EntityStatus")
                                     where a.EntityId == entityId && a.Tenant == tenant && a.EventType.Code == "EXCE"
                                     select a).OrderByDescending(d => d.LogDateTime).FirstOrDefault();
            return traceevent;
        }

        public TraceEvent GetLastExceptionTraceEventByContainerId(string entityId, string eventCode, int tenant)
        {
            TraceEvent traceevent = (from a in context.TraceEvents.Include("EventType").Include("EventType.EntityStatus")
                                     where a.EntityId == entityId && a.Tenant == tenant && a.EventType.Code == eventCode
                                     select a).OrderByDescending(d => d.LogDateTime).FirstOrDefault();
            return traceevent;
        }

        public bool IsTraceEventExistByEntityIdAndEventCode(string entityId, string eventCode, int tenant)
        {
            var isTraceEventExist = (from a in context.TraceEvents.Include("EventType").Include("EventType.EntityStatus")
                                     where a.EntityId == entityId && a.Tenant == tenant && a.EventType.Code == eventCode
                                     select a).Any();
            return isTraceEventExist;
        }
    }
}
