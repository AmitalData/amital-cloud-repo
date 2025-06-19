using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class ContactRepository : Repository<Contact>, IRepository<Contact>
    //, Simplog.Data.CommonDataModel.Repositories.IContactRepository
    {
        IAmitalCloudContext currentContext;



        public ContactRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }
        public ContactRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public ContactRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public IQueryable<Contact> GetContactsByEmail(string email)
        {
            return from a in context.Contacts
                   where a.Email == email.ToLower()
                   select a;
        }

        public IQueryable<Contact> GetActiveContacts(int tenant)
        {
            return (from record in context.Contacts where record.Tenant == tenant && record.UserType == "R" && !record.InActive select record);
        }

        public IQueryable<Contact> GetContacts(int tenant)
        {
            return (from record in context.Contacts where record.Tenant == tenant && record.UserType == "R" select record);
        }

        public IQueryable<Contact> GetAllContacts(int tenant)
        {
            return (from record in context.Contacts where record.Tenant == tenant && record.UserType == "R" select record);
        }

        public IQueryable<Contact> GetAllContactsThatHaveSignature()
        {
            return (from record in context.Contacts where record.Signature != null && record.UserType == "R" select record);
        }

        public IQueryable<Contact> GetContacts(List<string> allContactsId, int tenant)
        {
            IQueryable<Contact> myResult = null;

            if (allContactsId.Count > 0)
            {
                myResult = context.Contacts.Where(d => allContactsId.Contains(d.Id));
            }

            return myResult;
        }

        public Contact GetSingleContactForUpdate(string id, int tenant)
        {
            Contact entity = (from record in context.Contacts where record.Id == id && record.Tenant == tenant select record).FirstOrDefault();
            return entity;
        }

        public Contact GetSingleContactByExternalId(string externalId, int tenant)
        {


            Contact entity = (from a in context.Contacts
                              where a.Tenant == tenant && a.ExternalId == externalId
                              select a).FirstOrDefault();

            return entity;

        }

        public Contact GetSingleContactByExternalIdOrEmail(string externalId, string email, int tenant)
        {


            Contact entity = (from a in context.Contacts
                              where a.Tenant == tenant && (a.ExternalId == externalId || a.Email == email.ToLower())
                              select a).FirstOrDefault();

            return entity;

        }

        public Contact GetSingleContact(string id, int tenant)
        {


            Contact entity = (from a in context.Contacts
                              where a.Tenant == tenant && a.Id == id
                              select a).FirstOrDefault();

            if (entity == null)
            {
                entity = (from a in context.Contacts
                          where a.Tenant == 0 && a.Id == id
                          select a).FirstOrDefault();
            }

            return entity;

        }

        public Contact GetSingleContactByIdAndTenant(string id, int tenant, bool getFromCache)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string entityName = "Contact" + id + tenant;
                Contact entity;
                if (getFromCache)
                {

                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {

                        entity = (from a in context.Contacts
                                  where a.Tenant == tenant && a.Id == id
                                  select a).FirstOrDefault();


                        if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        entity = (Contact)CacheManager.CacheWrapper.Get(entityName);
                        // HttpContext.Current.Cache.Insert(EntityNameValue, Entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }


                }
                else
                {

                    entity = (from record in context.Contacts where record.Id == id && record.Tenant == tenant select record).FirstOrDefault();
                }
                return entity;
            }
            return null;

        }

        public static Contact GetSingleContact(string id, int tenant, bool getFromCache)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string entityName = "Contact" + id + tenant;
                Contact entity;


                if (getFromCache)
                {

                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        entity = (from a in context.Contacts
                                  where a.Tenant == tenant && a.Id == id
                                  select a).FirstOrDefault();


                        if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        entity = (Contact)CacheManager.CacheWrapper.Get(entityName);
                        // HttpContext.Current.Cache.Insert(EntityNameValue, Entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }


                }
                else
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    entity = (from record in context.Contacts where record.Id == id && record.Tenant == tenant select record).FirstOrDefault();
                }
                return entity;
            }
            return null;

        }

        public Contact GetSingleContactByEmailAndTenant(string email, int tenant)
        {
            UserRepository usersRepository = new UserRepository(tenant);
            Contact contact = (from a in context.Contacts
                               where a.Email == email.ToLower() && (a.Tenant == tenant || a.Tenant == 0)
                               select a).FirstOrDefault();


            return contact;
        }

        public Contact GetSingleContactByEmailSpecificTenant(string email, int tenant)
        {
            Contact contact = (from a in context.Contacts
                               where
                               a.Email != null
                               && a.Email.ToLower() == email.ToLower()
                               && a.Tenant == tenant
                               select a).FirstOrDefault();

            return contact;
        }

        //public Contact GetSingleContactByEmailForSecurity(string email, int tenant)
        //{
        //    UserRepository usersRepository = new UserRepository(tenant);
        //    Contact contact = (from a in context.Contacts
        //                       where a.Email == email && a.Tenant == tenant
        //                       select a).FirstOrDefault();

        //    if (contact == null)
        //    {
        //        contact = (from a in context.Contacts
        //                   where a.Email == email && a.Tenant == 0
        //                   select a).FirstOrDefault();
        //    }

        //    return contact;
        //}

        public Contact GetSingleContactByEmail(string email, int tenant, bool getFromCache = false)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.ToLower();
                string entityName = "Contact" + email + tenant;
                Contact entity;
                if (getFromCache)
                {

                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        entity = (from a in context.Contacts
                                  where a.Tenant == tenant && a.Email == email
                                  select a).FirstOrDefault();


                        if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }

                        else
                        {

                            if (entity == null)
                            {
                                entity = (from a in context.Contacts
                                          where a.Email == email && a.Tenant == 0
                                          select a).FirstOrDefault();
                            }
                        }
                        //}

                        //entity = (Contact)CacheManager.CacheWrapper.Get(entityName);


                    }
                    else
                    {
                        entity = (Contact)CacheManager.CacheWrapper.Get(entityName);

                    }





                }
                else
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    entity = (from record in context.Contacts where record.Email == email.ToLower() && record.Tenant == tenant select record).FirstOrDefault();

                    if (entity == null)
                    {
                        entity = (from a in context.Contacts
                                  where a.Email == email.ToLower() && a.Tenant == 0
                                  select a).FirstOrDefault();
                    }
                }

                return entity;
            }
            else
                return null;
        }

        public bool IsContactByEmailExists(string email, int tenant)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(email))
            {
                result = context.Contacts.Where(d => d.Tenant == tenant && d.Email == email.ToLower()).Any();
            }

            return result;
        }

        public List<Contact> GetContactsByEmailAndTenant(string email)
        {

            List<Contact> users = (from a in context.Contacts
                                   where a.Email == email.ToLower()
                                   select a).ToList();
            return users;

        }

        public bool CheckEmailAvailabilityForTenant(string email, int tenant)
        {
            if (!string.IsNullOrEmpty(email))
            {
                bool exists = (from a in context.Contacts
                               where (a.Email == email.ToLower() && a.Tenant == tenant)
                               select a).Any();
                return exists;
            }
            return false;

        }

        public bool CheckEmailAvailabilityForTenant0(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                bool exists = (from a in context.Contacts
                               where (a.Email == email.ToLower() && a.Tenant == 0)
                               select a).Any();
                return exists;
            }
            return false;

        }

        public IQueryable<CardContact> GetCardContactsForTenant(int tenant)
        {
            return from a in context.CardContacts
                   where a.Tenant == tenant
                   select a;
        }

        new public void Insert(Contact entity)
        {
            SetComputedKeyValue(entity);
            SetUpdateDate(entity);
            base.Insert(entity);
        }


        new public void Update(Contact entity)
        {
            SetComputedKeyValue(entity);
            SetUpdateDate(entity);
            base.Update(entity);
        }

        private void SetComputedKeyValue(Contact entity)
        {
            if (!string.IsNullOrEmpty(entity.Email))
                entity.ComputedKey = entity.Email;
            else
                entity.ComputedKey = entity.Id;

        }

        private void SetUpdateDate(Contact entity)
        {
            if (entity.UpdateDate == null)
            {
                entity.UpdateDate = TenantServerConfigration.GetCurrentDateTime(entity.Tenant);
            }
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }

        public string GetEmailContactByIdAndTenant(int tenant, string id)
        {
            string Email = "";
            Contact contact = (from a in context.Contacts
                               where a.Tenant == tenant && a.Id == id
                               select a).FirstOrDefault();
            if (contact != null)
            {
                Email = contact.Email;
            }

            return Email;
        }

        public List<EmailTypeClass> GetContactsListByEmailAndTenant(List<EmailTypeClass> emails, int tenant)
        {
            List<EmailTypeClass> contacts = new List<EmailTypeClass>();

            if (emails.Count > 0)
            {
                //List<string> conemails = emails.Select(e => e.Email).Distinct().ToList(); // remove duplicate emails 
                List<string> conemails = new List<string>();

                foreach (string email in emails.Select(e => e.Email))
                {
                    conemails.Add(Regex.Replace(email.ToLower(), @"\s+", ""));
                }

                contacts = (from a in context.Contacts
                            where a.Tenant == tenant && conemails.Contains(a.Email)
                            select new EmailTypeClass
                            {
                                Id = a.Id,
                                Email = a.Email,

                            }).Distinct().ToList();

                foreach (EmailTypeClass item in contacts)
                {
                    item.Type = emails.Where(e => e.Email == item.Email.ToLower()).Select(e => e.Type).FirstOrDefault();
                }
            }

            return contacts;
        }

        public Contact GetContactByUserTypeAndTenant(string userType, int tenant)
        {
            Contact entity = (from a in context.Contacts
                              where a.Tenant == tenant && a.UserType == userType && a.Email.Contains("system")
                              select a).FirstOrDefault();
            return entity;
        }

        public string GetContactIdByUserTypeAndTenant(string userType, int tenant)
        {
            string contactId = (from a in context.Contacts
                                where a.Tenant == tenant && a.UserType == userType && a.Email.Contains("system")
                                select a.Id).FirstOrDefault();
            return contactId;
        }




        public IQueryable<Contact> GetContactsByIds(List<string> trackedIds, int tenant)
        {
            IQueryable<Contact> contactlist = (from a in context.Contacts
                                               where trackedIds.Contains(a.Id)
                                               select a);
            return contactlist;
        }

        public string GetConactIdByemail(string email, int tenant)
        {
            return (from a in context.Contacts
                    where a.Email == email.ToLower() && a.Tenant == tenant
                    select a.Id).FirstOrDefault();
        }

        public string GetConactNameByemail(string email, int tenant)
        {
            return (from a in context.Contacts
                    where a.Email == email.ToLower() && a.Tenant == tenant
                    select a.EnglishName).FirstOrDefault();
        }

        public string GetConactEmail(string id)
        {
            return (from x in context.Contacts where x.Id == id select x.Email).FirstOrDefault();

        }


        public string GetContactNameById(string id, int tenant)
        {

            string contactName = (from a in context.Contacts
                                  where a.Tenant == tenant && a.Id == id
                                  select a.EnglishName).FirstOrDefault();

            if (string.IsNullOrEmpty(contactName))
            {
                contactName = (from a in context.Contacts
                               where a.Tenant == 0 && a.Id == id
                               select a.EnglishName).FirstOrDefault();
            }

            return contactName;

        }


        public IQueryable<Contact> GetContactListsByListids(List<string> contactIds, int tenant)
        {
            IQueryable<Contact> contactlist = (from a in context.Contacts
                                               where contactIds.Contains(a.Id)
                                               select a);
            return contactlist;
        }
        public string GetContactForAccountingByGLAccountIdExcludeOneCard(string glAccountId, int tenant, string excludeContactId, string includeContactId)
        {
            var contacts = (
                from contact in context.Contacts
                where contact.ContactForAccounting == true
                && (
                    from cardContact in context.CardContacts
                    where (
                        from card in context.Cards
                        where card.GLAccountId == glAccountId && card.Tenant == tenant
                        select card.Id
                    ).Contains(cardContact.CardId)
                    select cardContact.ContactId
                ).Contains(contact.Id)
                && (includeContactId == null || contact.Id.ToString() == includeContactId) // Include specific contact ID
                && (excludeContactId == null || contact.Id.ToString() != excludeContactId) // Exclude specific contact ID
                select contact
            ).FirstOrDefault();

            return contacts?.Id.ToString();
        }



    }
}
