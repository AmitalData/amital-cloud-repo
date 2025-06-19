using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;

using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class UserLastLoginRepository : Repository<UserLastLogin>
    {
        IAmitalCloudContext currentContext;
        public UserLastLoginRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }
        public UserLastLoginRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public IQueryable<UserLastLogin> GetUserLastLogins(int tenant)
        {
            return (from record in context.UserLastLogins.Include("User") where record.Tenant == tenant select record);
        }
        public UserLastLogin GetSingleUserLastLogin(string id, int tenant, bool getFromCache)
        {
            return (from record in context.UserLastLogins.Include("User") where record.Id == id && record.Tenant == tenant select record).FirstOrDefault();
        }
        public IQueryable<UserLastLogin> GetUsersWorkspaceLastLogins(int tenant)
        {
            return (from d in context.UserLastLogins.Include("User").Include("User.Contact").Include("User.BusinessUnit")
                    where d.Tenant == tenant
                    select d);
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }
    }
}