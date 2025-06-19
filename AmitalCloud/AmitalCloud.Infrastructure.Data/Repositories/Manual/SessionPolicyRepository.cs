using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class SessionPolicyRepository : Repository<SessionPolicy>
    {
        IGlobalContext currentContext;
        public SessionPolicyRepository(IGlobalContext context) : base(context) { currentContext = context; }
        public SessionPolicyRepository() : this(GlobalContext.GetContext()) { }
        public SessionPolicy GetSingleSessionPolicy()
        {
            SessionPolicy item = currentContext.SessionPolicies.FirstOrDefault();
            string entityName = "SessionPolicy";
            if (CacheManager.CacheWrapper != null)
            {
                if (CacheManager.CacheWrapper.Get(entityName) == null && item != null)
                {
                    CacheManager.CacheWrapper.Insert(entityName, item, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                }
                else
                {
                    item = (SessionPolicy)CacheManager.CacheWrapper.Get(entityName);
                }
            }
            return item;
        }
    }
}
