using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Linq;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;
using Microsoft.Extensions.Logging.Abstractions;

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
            var entity = (from a in repository.GetMulti(a => a.ContactId == contactId && a.TenantId == tenant)
                    select a).FirstOrDefault();

            var config = new MapperConfiguration(cfg => cfg.AddProfile(new ContactTenantDataMapping()), new NullLoggerFactory());
            var mapper = config.CreateMapper();
            return mapper.Map<ContactTenantPM>(entity);
        }
    }
}
