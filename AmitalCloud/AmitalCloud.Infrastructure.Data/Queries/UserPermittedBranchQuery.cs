using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class UserPermittedBranchQuery
    {
        UserPermittedBranchRepository repository;



        public UserPermittedBranchQuery(int tenant) : this(new UserPermittedBranchRepository(tenant))
        {
        }

        public UserPermittedBranchQuery(UserPermittedBranchRepository UserPermittedBranchRepository)
        {
            repository = UserPermittedBranchRepository;
        }

        public UserPermittedBranchPM GetSinglePM(string id, int tenant)
        {
            UserPermittedBranchPM UserPermittedBranch = (from a in repository.context.UserPermittedBranches
                                                         where a.Id == id && a.Tenant == tenant
                                                         select new UserPermittedBranchPM()
                                                         {
                                                             Id = a.Id,
                                                             Tenant = a.Tenant,
                                                             BranchId = a.BranchId,
                                                             UserId = a.UserId,
                                                         }).FirstOrDefault();
            return UserPermittedBranch;
        }

        public IQueryable<UserPermittedBranchPM> GetUserPermittedBranchPMsByTenant(int tenant)
        {
            IQueryable<UserPermittedBranchPM> UserPermittedBranchs = from a in repository.context.UserPermittedBranches
                                                                     where a.Tenant == tenant
                                                                     select new UserPermittedBranchPM()
                                                                     {
                                                                         Id = a.Id,
                                                                         Tenant = a.Tenant,
                                                                         BranchId = a.BranchId,
                                                                         UserId = a.UserId,
                                                                     };
            return UserPermittedBranchs;
        }

        public IQueryable<UserPermittedBranchPM> GetContactFromUserPermittedBranchPMsByUserId(string id, int tenant)
        {
            IQueryable<UserPermittedBranchPM> UserPermittedBranchs = (from a in repository.context.UserPermittedBranches.Include("Contact")
                                                                      where a.UserId == id
                                                                      select new UserPermittedBranchPM()
                                                                      {
                                                                          Id = a.Id,
                                                                          Tenant = a.Tenant,
                                                                          BranchId = a.BranchId,
                                                                          UserId = a.UserId,
                                                                      });
            return UserPermittedBranchs;
        }
    }
}