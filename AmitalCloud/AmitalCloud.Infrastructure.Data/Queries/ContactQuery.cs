using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;
namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class ContactQuery
    {
        IRepository<Contact> repository;
        IAmitalCloudContext context;

        public ContactQuery(int tenant)
        {
            context = AmitalCloudContext.GetContext(tenant);
            repository = new Repository<Contact>(context);
        }
        #region GetSingle ContactPM
        private ContactPM GetContactPMFromCache(int tenant, string cacheKey, Expression<Func<Contact, bool>> predicate)
        {
            ContactPM entity;
            if (HttpContextHelper.HttpContext != null)
            {
                entity = (ContactPM)CacheManager.CacheWrapper.Get(cacheKey);
                if (entity == null)
                {
                    entity = GetEntityPMWithPassword(predicate);
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
                entity = GetEntityPMWithPassword(predicate);
            }

            return entity;
        }
        private void GetContactPassword(ContactPM instance)
        {
            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                IGlobalContext globalContext = GlobalContext.GetContext();
                ContactPassword contactPassword = globalContext.ContactPasswords.Where(cn => cn.Email == instance.Email.ToLower()).FirstOrDefault();
                GlobalContact globalContact = globalContext.GlobalContacts.Where(cn => cn.Email == instance.Email.ToLower() && cn.GlobalTenantId == instance.Tenant).FirstOrDefault();
                if (globalContact != null)
                {
                    //instance.IsUser = globalContact.IsUser;
                }
                if (contactPassword != null)
                {
                    //instance.Password = contactPassword.Password;
                    //instance.IsLocked = contactPassword.IsLocked;
                    //instance.MustChangePassword = contactPassword.MustChangePassword;
                    //instance.NumberOfRetries = contactPassword.NumberOfRetries;
                }
                scope.Complete();
            }
        }
        private ContactPM GetEntityPMWithPassword(Expression<Func<Contact, bool>> predicate)
        {
            Contact entity = context.Contacts.Where(predicate).FirstOrDefault();
            if (entity != null)
            {
                var config = new MapperConfiguration(cfg => cfg.AddProfile(new ContactDataMapping()));
                var mapper = config.CreateMapper();
                ContactPM contactPM = mapper.Map<ContactPM>(entity);
                GetContactPassword(contactPM);
                return contactPM;
            }
            else
            {
                return null;
            }
        }
        public ContactPM GetSingleContact(string email, int tenant) => GetEntityPMWithPassword(a => a.Email == email.ToLower() && a.Tenant == tenant);
        public ContactPM GetContactByEmailOnly(string email, int tenant)
        {
            ContactPM contact = GetContactPMFromCache(tenant, $"ContactPM_({email}_{tenant})".ToLower(), a => a.Email == email && a.InActive == false && a.Tenant == tenant);
            if (contact == null)
            {
                contact = GetContactPMFromCache(tenant, $"ContactPM_({email}_0)".ToLower(), a => a.Email == email && a.InActive == false && a.Tenant == 0);
            }
            return contact;
        }
        public ContactPM GetSingleByEmail(string email, int tenant) => GetSingleContact(email, tenant);
        #endregion GetSingle ContactPM  
        public IQueryable<ContactList> GetContactListsByListIds(List<string> contactIds, int tenant)
        => (from a in context.Contacts
            where contactIds.Contains(a.Id) && a.Tenant == tenant
            select new ContactList(a));

    }
}