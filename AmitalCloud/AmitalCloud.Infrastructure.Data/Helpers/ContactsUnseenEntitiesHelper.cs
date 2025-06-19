using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Services;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class ContactsUnseenEntitiesHelper
    {
        public static void AddUnseenEntityRecord(string traceEventId, int tenant, IUnitOfWork uow)
        {
            Tenant tenantpm = new Repository<Tenant>(uow).GetAll(tenant, true).Where(a => a.Id == tenant).FirstOrDefault();
            if (tenantpm.IsMobileActivated)
            {
                IQueueService queueservice = new DbQueueService();
                queueservice.InitializeQueue("contactunseenentityqueue", tenant);
                queueservice.Send(new Dictionary<string, string>() { { "Tenant", tenant.ToString() }, { "TenantName", tenantpm.Company }, { "TraceEventId", traceEventId }, { "SourceEventDate", DateTime.UtcNow.ToString() } }, tenant, null, null, null, null);
            }
        }
    }
}
