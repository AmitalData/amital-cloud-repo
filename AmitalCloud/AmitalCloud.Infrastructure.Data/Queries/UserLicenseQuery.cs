using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class UserLicenseQuery
    {
        IRepository<UserLicense> repository;
        IAmitalCloudContext context;

        public UserLicenseQuery(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public UserLicenseQuery(IAmitalCloudContext context) : this(new Repository<UserLicense>(context))
        {
            this.context = context;
        }
        public UserLicenseQuery(IRepository<UserLicense> myRepository)
        {
            repository = myRepository;
            if (context == null)
            {
                //context = repository.Context
            }
        }
        public IQueryable<UserLicensePM> GetUserLicensesByTenant(int tenant)
        {
            IQueryable<UserLicensePM> myResult = (from a in context.UserLicenses
                                                  where a.Tenant == tenant
                                                  select new UserLicensePM()
                                                  {
                                                      Id = a.Id,
                                                      Tenant = a.Tenant,
                                                      UserId = a.UserId,
                                                      PackageCode = a.PackageCode,
                                                  });
            return myResult;
        }
    }
}
