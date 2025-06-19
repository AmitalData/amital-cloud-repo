using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class GeneralLockRepository : Repository<GeneralLock>
    {
        IAmitalCloudContext currentContext;


        public GeneralLockRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }


        public GeneralLockRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }

        public GeneralLock GetSingleGeneralLock(string generalKey, int tenant)
        {


            var poco = (from a in context.GeneralLocks
                        where a.GeneralKey == generalKey && a.Tenant == tenant
                        select a).FirstOrDefault();


            return poco;
        }
        public GeneralLock GetSingleGeneralLockNOWAIT(string generalKey, int tenant)
        {
            throw new NotImplementedException();
            //return GetListNOWAITWhere(rec => rec.GeneralKey == generalKey && rec.Tenant == tenant).FirstOrDefault(); ;
        }
        public void FastDelete(string generalKey, int tenant)
        {
            DeleteWhere(rec => rec.GeneralKey == generalKey && rec.Tenant == tenant);
        }
        public void FastDeleteIfCreated15MinOld(string generalKey, int tenant)
        {
            DateTime createdAtb4_15min = TenantServerConfigration.GetCurrentDateTime(tenant).AddMinutes(-15);
            DeleteWhere(rec => rec.GeneralKey == generalKey && rec.Tenant == tenant
            && rec.CreatedAt < createdAtb4_15min
            );
        }
        public IQueryable<GeneralLock> GetGeneralLocks()
        {
            return context.GeneralLocks;
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }





    }

}
