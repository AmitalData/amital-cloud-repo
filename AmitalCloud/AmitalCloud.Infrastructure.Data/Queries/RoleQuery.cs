using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class RoleQuery
    {
        IRepository<Role> repository;
        IAmitalCloudContext context;
        public RoleQuery(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }

        public RoleQuery(Repository<Role> roleRepository, int tenant)
        {
            repository = roleRepository;
        }

        public RoleQuery(IAmitalCloudContext amitalCloudContext) : this(new Repository<Role>(amitalCloudContext), amitalCloudContext.Tenant)
        {
            context = amitalCloudContext;
        }

        public List<RolePM> GetRolesForContact(string contactid, int tenant)
        {
            List<RolePM> roles = repository.GetMulti(a => (a.Tenant == tenant && contactid != null) || a.Tenant == 0).Select(a => new RolePM(a)).ToList();

            if (contactid != null)
            {
                Repository<ContactTenant> contactTenant = new Repository<ContactTenant>(context);
                ContactTenant contacttenant = contactTenant.GetSingle(a => a.ContactId == contactid && a.TenantId == tenant);
                if (contacttenant == null)
                {
                    throw new Exception($"contacttenant not exist in DB ({contactid})");
                }

                Repository<ContactTenantRole> contactTenantRoleRepo = new Repository<ContactTenantRole>(context);
                List<ContactTenantRole> contactTenantRoles = contactTenantRoleRepo.GetMulti(a => a.ContactTenantId == contacttenant.Id && a.Tenant == tenant);

                foreach (ContactTenantRole contacttenantrole in contactTenantRoles)
                {
                    RolePM role = (from a in roles
                                   where a.Id == contacttenantrole.RoleId
                                   select a).FirstOrDefault();
                }
            }
            return roles;
        }
        private List<RolePM> GetCustomRolesByIds(List<string> myRolesIds)
        {
            List<RolePM> myResult = new List<RolePM>();

            if (myRolesIds.Count > 0)
            {
                myResult = (from a in context.Roles
                            where myRolesIds.Contains(a.Id) && a.IsCustomRole == true
                            select new RolePM(a)).ToList();
            }

            return myResult;
        }
    }
}
