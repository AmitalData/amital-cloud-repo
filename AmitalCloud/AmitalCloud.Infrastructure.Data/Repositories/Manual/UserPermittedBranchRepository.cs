using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;

using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class UserPermittedBranchRepository : Repository<UserPermittedBranch>
    {
        IAmitalCloudContext currentContext;
        public UserPermittedBranchRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }

        public UserPermittedBranchRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public IQueryable<UserPermittedBranch> GetUserPermittedBranches(int tenant)
        {
            return (from record in context.UserPermittedBranches.Include("User") where record.Tenant == tenant select record);
        }
        public IQueryable<UserPermittedBranch> GetUserPermittedBranchesByUserId(string userId, int tenant)
        {
            return (from record in context.UserPermittedBranches.Include("User") where record.UserId == userId && record.Tenant == tenant select record);
        }
        public UserPermittedBranch GetSingleUserPermittedBranch(string id, int tenant)
        {
            return (from record in context.UserPermittedBranches.Include("User") where record.Id == id && record.Tenant == tenant select record).FirstOrDefault();
        }
        public UserPermittedBranch GetUserPermittedBranchByUserAndBranch(string userId, string branchId, int tenant)
        {
            return (from record in context.UserPermittedBranches.Include("User") where record.UserId == userId && record.Tenant == tenant && record.BranchId == branchId select record).FirstOrDefault();
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }
        public List<string> GetUserPermittedBranchesIdsByUserId(string userId, int tenant)
        {
            return GetMulti(record => record.UserId == userId && record.Tenant == tenant).Select(a => a.BranchId).ToList();
        }
    }
}