using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;

using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class EventTypeRepository : Repository<EventType>, IRepository<EventType>
    {

        IAmitalCloudContext currentContext;

        public EventTypeRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }


        public EventTypeRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public EventTypeRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }

        public IQueryable<EventType> GetEventTypes()
        {
            return context.EventTypes;
        }



        public IQueryable<EventType> GetEventTypes(int tenant)
        {
            IQueryable<EventType> eventTypes = from a in context.EventTypes
                                               where a.Tenant == tenant
                                               select a;
            return eventTypes;
        }

        public IQueryable<EventType> GetEventTypesByCodes(int tenant, string tableId, List<string> codes)
        {
            IQueryable<EventType> eventTypes = from a in context.EventTypes
                                               where codes.Contains(a.Code) && a.Tenant == tenant && a.ObjectTableId == tableId
                                               select a;
            return eventTypes;
        }

        public IQueryable<EventType> GetEventTypesByTenant(int tenant)
        {
            IQueryable<EventType> eventTypes = from a in context.EventTypes where a.Tenant == tenant select a;
            return eventTypes;
        }

        public IQueryable<EventType> GetEventTypesByTenantAndObjectTableId(int tenant, string tableId)
        {
            IQueryable<EventType> eventTypes = from a in context.EventTypes where a.Tenant == tenant && a.ObjectTableId == tableId select a;
            return eventTypes;
        }

        public EventType GetSingleEventType(string id, int tenant)
        {
            EventType entity = (from a in context.EventTypes where a.Tenant == tenant && a.Id == id select a).Include("EntityStatus").FirstOrDefault();
            return entity;
        }


        public EventType GetSingleEventTypeByCode(string code, int tenant)
        {
            string key = $"GetSingleEventTypeByCode({code}, {tenant})";
            return CacheManager.GetOrInsertNewObject<EventType>(key, () =>
            {
                return GetSingleEventTypeByCodeReal(code, tenant);
            });
        }
        EventType GetSingleEventTypeByCodeReal(string code, int tenant)
        {
            EventType entity = (from a in context.EventTypes where a.Tenant == tenant && a.Code == code select a).FirstOrDefault();
            return entity;
        }


        public string GetSingleEventTypeIdByCode(string code, int tenant)
        {
            string id = (from a in context.EventTypes where a.Tenant == tenant && a.Code == code select a.Id).FirstOrDefault();
            return id;
        }

        public string GetEventTypeNameById(string id, int tenant)
        {
            string name = (from a in context.EventTypes where a.Tenant == tenant && a.Id == id select a.FollowUpEnglishName).FirstOrDefault();
            return name;
        }

        public EventType GetSingleEventTypeByCodeAndObjectTableId(string code, string tableId, int tenant)
        {
            EventType entity = (from a in context.EventTypes where a.Tenant == tenant && a.Code == code && a.ObjectTableId == tableId select a).FirstOrDefault();
            return entity;
        }



        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }



        public string GetCustomFieldByEventTypeId(string id, int tenant)
        {
            return (from a in context.EventTypes
                    where a.Id == id && a.Tenant == tenant
                    select a.CustomField).FirstOrDefault();
        }


    }
}
