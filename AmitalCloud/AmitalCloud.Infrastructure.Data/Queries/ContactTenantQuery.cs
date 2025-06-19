using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class ContactTenantQuery
    {
        IRepository<ContactTenant> repository;

        public ContactTenantQuery(int tenant)
        {
            repository = new Repository<ContactTenant>(tenant);
        }
        public ContactTenantQuery(IRepository<ContactTenant> contactTenantRepository)
        {
            repository = contactTenantRepository;
        }
        public ContactTenantPM GetContactTenantForUser(string contactId, int tenant)
        {
            return (from a in repository.GetMulti(a => a.ContactId == contactId && a.TenantId == tenant)
                    select new ContactTenantPM(a)).FirstOrDefault();

        }
        public ContactTenantPM GetSinglePM(string id, int tenant)
        {
            return (from a in repository.GetMulti(a => a.Id == id && a.TenantId == tenant)
                    select new ContactTenantPM(a)).FirstOrDefault();
        }
    }
}
