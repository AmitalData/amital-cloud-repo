using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class ContactTenantRoleQuery
    {
        IRepository<ContactTenantRole> repository;

        public ContactTenantRoleQuery(int tenant)
        {
            repository = new Repository<ContactTenantRole>(tenant);
        }
        public ContactTenantRoleQuery(IRepository<ContactTenantRole> contactTenantRoleRepository)
        {
            repository = contactTenantRoleRepository;
        }
        public ContactTenantRolePM GetSinglePM(string id, int tenant)
        {
            var contactTenantRole = (from a in repository.GetMulti(a =>
                                      a.Id == id && a.Tenant == tenant)
                                     select new ContactTenantRolePM()
                                     {
                                         ContactTenantId = a.ContactTenantId,
                                         Id = a.Id,
                                         RoleId = a.RoleId,
                                         Tenant = a.Tenant,
                                     }).FirstOrDefault();
            return contactTenantRole;
        }

        public List<ContactTenantRolePM> GetContactTenantRolesForContactTenant(string contactTenantId, int tenant)
        {
            var contactTenantRole = (from a in repository.GetMulti(a =>
                                                            a.ContactTenantId == contactTenantId && a.Tenant == tenant)
                                     select new ContactTenantRolePM()
                                     {
                                         ContactTenantId = a.ContactTenantId,
                                         Id = a.Id,
                                         RoleId = a.RoleId,
                                         Tenant = a.Tenant,
                                     });

            if (contactTenantRole == null)
            {
                return null;
            }
            return contactTenantRole.ToList();

        }
    }
}
