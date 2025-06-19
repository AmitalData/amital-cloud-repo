using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class UserLoginLogRepository : Repository<UserLoginLog>
    {
        IAmitalCloudContext currentContext;


        public UserLoginLogRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }
        public UserLoginLogRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public IQueryable<UserLoginLog> GetUserLoginLogs(int tenant)
        {
            return (from d in context.UserLoginLogs where d.Tenant == tenant select d);
        }
        public IQueryable<UserLoginLog> GetUserLoginHistory(string userId, int tenant)
        {
            return (from d in context.UserLoginLogs where d.UserId == userId && d.Tenant == tenant select d);
        }
        public UserLoginLog GetSingleUserLoginLog(string id, int tenant, bool getFromCache)
        {
            string cacheKey = $"UserLoginLog_({id}_{tenant})";
            UserLoginLog entity;
            if (getFromCache)
            {
                entity = (UserLoginLog)CacheManager.CacheWrapper.Get(cacheKey);
                if (entity == null)
                {
                    entity = (from record in context.UserLoginLogs where record.Id == id && record.Tenant == tenant select record).FirstOrDefault();
                    if (CacheManager.CacheWrapper.Get(cacheKey) == null)
                    {
                        if (entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(cacheKey, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                }
            }
            else
            {
                entity = (from record in context.UserLoginLogs where record.Id == id && record.Tenant == tenant select record).FirstOrDefault();
            }
            return entity;
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }
    }
}