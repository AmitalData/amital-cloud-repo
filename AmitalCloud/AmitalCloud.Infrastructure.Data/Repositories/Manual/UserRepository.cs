using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;

using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class UserRepository : Repository<User>, IRepository<User>
    {
        IAmitalCloudContext amitalCloudContext;

        public UserRepository(IUnitOfWork uow) : base(uow)
        {
        }
        public UserRepository(IAmitalCloudContext context) : base(context)
        {
            amitalCloudContext = context;
        }
        public UserRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public int GetUsersCount(int tenant)
        {
            return (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                    where record.Tenant == tenant && record.Contact.UserType == "R"
                    select record).Count();
        }

        public IQueryable<User> GetUsers(int tenant)
        {
            return (from record in context.Users.Include("Contact").Include("Department").Include("Branch") where record.Tenant == tenant && record.Contact.UserType == "R" select record);
        }

        public IQueryable<User> GetUsersTwoFactorAuthenticationEnabled(int tenant)
        {
            return (from record in context.Users where record.Tenant == tenant && record.IsTwoFactorAuthenticationEnabled select record);
        }

        public bool CheckUsersTwoFactorAuthenticationEnabled(int tenant)
        {
            return (from record in context.Users where record.Tenant == tenant && record.IsTwoFactorAuthenticationEnabled select record).Any();
        }

        public User GetSingleUserById(string id)
        {
            return context.Users.FirstOrDefault(x => x.Id == id);
        }

        public User GetSingleUser(string id, int tenant)
        {
            string entityName = "User" + id + tenant;
            User entity;


            if (CacheManager.CacheWrapper.Get(entityName) == null)
            {
                var x = context.Users.FirstOrDefault();
                entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                          where record.Id == id && record.Tenant == tenant
                          select record).FirstOrDefault();

                if (CacheManager.CacheWrapper.Get(entityName) == null)
                {
                    if (entity != null)
                    {
                        CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }
                }
            }

            else
            {
                entity = (User)CacheManager.CacheWrapper.Get(entityName);
            }




            return entity;
        }

        public User GetSingleUser(string id, int tenant, bool getFromCache)
        {

            string entityName = "User" + id + tenant;
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                entityName = "IsCostomsDeployUser" + id + tenant;
                var res = CacheManager.GetOrInsertNewObject<User>(entityName, () =>
                {
                    return (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                            where record.Id == id && record.Tenant == tenant
                            select record).FirstOrDefault();
                });
                return res;
            }
            User entity;
            if (getFromCache)
            {

                if (CacheManager.CacheWrapper.Get(entityName) == null)
                {
                    entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                              where record.Id == id && record.Tenant == tenant
                              select record).FirstOrDefault();

                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        if (entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                }

                else
                {
                    entity = (User)CacheManager.CacheWrapper.Get(entityName);
                }



            }
            else
            {
                entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                          where record.Id == id && record.Tenant == tenant
                          select record).FirstOrDefault();
            }
            return entity;
        }

        public User GetSingleUserByCode(string code, int tenant, bool getFromCache)
        {
            string entityName = "User" + code + tenant;
            User entity;
            if (getFromCache)
            {

                if (CacheManager.CacheWrapper.Get(entityName) == null)
                {
                    entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                              where record.Code == code && record.Tenant == tenant
                              select record).FirstOrDefault();

                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        if (entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                }

                else
                {
                    entity = (User)CacheManager.CacheWrapper.Get(entityName);
                }



            }

            else
            {
                entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit") where record.Code == code && record.Tenant == tenant select record).FirstOrDefault();
            }

            return entity;
        }



        public User GetSingleUserByEmail(string email, int tenant, bool getFromCache = false)
        {
            User entity;
            if (getFromCache)
            {
                string cacheKey = $"User_{email}_{tenant}";
                entity = (User)CacheManager.CacheWrapper.Get(cacheKey);
                if (entity == null)
                {
                    entity = GetUserFromDB(email, tenant);
                    if (entity != null)
                    {
                        if (CacheManager.CacheWrapper.Get(cacheKey) == null)
                        {
                            CacheManager.CacheWrapper.Insert(cacheKey, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                }
            }
            else
            {
                entity = GetUserFromDB(email, tenant);
            }
            return entity;
        }

        private User GetUserFromDB(string email, int tenant)
        {
            User entity = (from record in context.Users.Include("Contact")
                           where (record.Contact.Email == email && record.Contact.Tenant == tenant) && record.Tenant == tenant
                           select record).FirstOrDefault();
            if (entity == null && tenant != 0)
            {
                entity = (from record in context.Users.Include("Contact")
                          where (record.Contact.Email == email && record.Contact.Tenant == 0) && record.Tenant == 0
                          select record).FirstOrDefault();
            }
            return entity;
        }

        public User GetSingleUserByCodeOrEmailForTenant(string code, string email, int tenant, bool getFromCache)
        {

            email = email.ToLower();
            string entityName = "User" + (string.IsNullOrEmpty(code) ? email : code) + tenant;
            User entity;
            if (getFromCache)
            {

                if (CacheManager.CacheWrapper.Get(entityName) == null)
                {
                    entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                              where ((record.Code == code && !string.IsNullOrEmpty(record.Code)) || (record.Contact.Tenant == tenant && record.Contact.Email == email && !string.IsNullOrEmpty(record.Contact.Email))) && record.Tenant == tenant
                              select record).FirstOrDefault();



                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        if (entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                }
                else
                {
                    entity = (User)CacheManager.CacheWrapper.Get(entityName);
                    // HttpContext.Current.Cache.Insert(EntityNameValue, Entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                }



            }
            else
            {
                entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                          where ((record.Code == code && !string.IsNullOrEmpty(record.Code)) || (record.Contact.Tenant == tenant && record.Contact.Email == email && !string.IsNullOrEmpty(record.Contact.Email))) && record.Tenant == tenant
                          select record).FirstOrDefault();



            }
            return entity;
        }

        public User GetSingleUserByCodeOrEmail(string code, string email, int tenant, bool getFromCache)
        {
            email = email.ToLower();
            string entityName = "User" + (string.IsNullOrEmpty(code) ? email : code) + tenant;
            User entity;
            if (getFromCache)
            {
                if (CacheManager.CacheWrapper.Get(entityName) == null)
                {
                    entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                              where ((record.Code == code && !string.IsNullOrEmpty(record.Code)) || (record.Contact.Tenant == tenant && record.Contact.Email == email && !string.IsNullOrEmpty(record.Contact.Email))) && record.Tenant == tenant
                              select record).FirstOrDefault();

                    if (entity == null && tenant != 0)
                    {
                        entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                                  where ((record.Code == code && !string.IsNullOrEmpty(record.Code)) || (record.Contact.Email == email && !string.IsNullOrEmpty(record.Contact.Email))) && record.Tenant == 0
                                  select record).FirstOrDefault();
                    }

                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        if (entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                }
                else
                {
                    entity = (User)CacheManager.CacheWrapper.Get(entityName);
                    // HttpContext.Current.Cache.Insert(EntityNameValue, Entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                }



            }
            else
            {
                entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                          where ((record.Code == code && !string.IsNullOrEmpty(record.Code)) || (record.Contact.Email == email && !string.IsNullOrEmpty(record.Contact.Email))) && record.Tenant == tenant
                          select record).FirstOrDefault();

                if (entity == null && tenant != 0)
                {
                    entity = (from record in context.Users.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("BusinessUnit")
                              where ((record.Code == code && !string.IsNullOrEmpty(record.Code)) || (record.Contact.Email == email && !string.IsNullOrEmpty(record.Contact.Email))) && record.Tenant == 0
                              select record).FirstOrDefault();
                }

            }
            return entity;
        }

        public bool DoesUserExist(string email, int tenant)
        {
            email = email.ToLower();
            return (from a in context.Users
                    where a.Contact.Email == email && a.Tenant == tenant
                    select a).Any();
        }
        public IAmitalCloudContext context
        {
            get { return amitalCloudContext; }
        }
        public List<int> GetPersonTenantList(string personID)
        {
            var userList = context.Users.Where(usr => usr.PersonalId == personID);

            //return context.Users.Where(usr => usr.PersonId == personID); 
            var list = new List<int>();
            list = userList.Select(rec => rec.Tenant).ToList();
            //if ("049028392" == personID)
            //{
            //    list.Add(208);
            //    list.Add(92);
            //}
            if ("049028392" == personID)
            {
                list.Add(208);
                list.Add(92);
            }
            return list;
        }

        public bool IsUserIdExist(string personalId, int tenant)
        {
            return (from a in context.Users
                    where a.PersonalId == personalId && a.Tenant == tenant
                    select a).Any();
        }
        public string GetUserIdByPersonalId(string personalId, int tenant)
        {
            var poco = (from a in context.Users
                        where a.PersonalId == personalId && a.Tenant == tenant
                        select a).FirstOrDefault();
            if (poco != null)
            {
                return poco.Id;
            }
            else
            {
                return null;
            }
        }

        public List<User> GetUsersListFromIdList(List<string> ids, int tenant)
        {
            List<User> users = new List<User>();

            if (ids.Count > 0)
            {
                users = (from a in context.Users
                         where a.Tenant == tenant && ids.Contains(a.Id)
                         select a).ToList();
            }

            return users;
        }


        public bool IsContactIdExist(string Id, int tenant)
        {
            return (from a in context.Users
                    where a.Id == Id && a.Tenant == tenant
                    select a).Any();
        }

        public List<User> GetUsersFromTenantsList(List<int> tenantIds)
        {
            List<User> users = new List<User>();

            if (tenantIds.Count > 0)
            {
                users = (from a in context.Users.Include("Contact")
                         where tenantIds.Contains(a.Tenant)
                         select a).ToList();
            }

            return users;
        }

        public User GetSingleUserByDocumentFilingInbox(List<string> emails)
        {
            User myUser = new User();
            if (emails.Count > 0)
            {
                myUser = (from a in context.Users
                          where a.DocumentFilingInbox != null && emails.Contains(a.DocumentFilingInbox.Trim())
                          select a
                          ).FirstOrDefault();
            }
            return myUser;
        }

        public bool IsDocumentFilingInboxExist(string documentFiling)
        {
            return (from a in context.Users
                    where a.DocumentFilingInbox == documentFiling
                    select a).Any();
        }

    }
}
