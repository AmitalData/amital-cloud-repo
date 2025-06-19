using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;

using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class CustomerRepository : Repository<Customer>
    {
        IAmitalCloudContext currentContext;

        public CustomerRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }

        public CustomerRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant)) { }

        public CustomerRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        public int GetCustomerCount(int tenant)
        {
            return context.Customers
                          .Include("Card")
                          .Include("SalesmanUser")
                          .Include("SalesmanUser.Contact")
                          .Include("AccountManagerUser.Contact")
                          .Include("Card.SharedLogisticsInvitationStatus")
                          .Include("Rank")
                          .Count(a => a.Tenant == tenant && a.IsCustomer);
        }

        public IQueryable<Customer> GetCustomers(int tenant)
        {
            return (from record in context.Customers
                                          .Include("Card")
                                          .Include("SalesmanUser")
                                          .Include("SalesmanUser.Contact")
                                          .Include("AccountManagerUser.Contact")
                                          .Include("Card.SharedLogisticsInvitationStatus")
                                          .Include("Rank")
                                          .Include("Collector.Contact")
                                          .Include("Classifier.Contact")
                    where record.Tenant == tenant
                    select record);
        }

        public Customer GetSingleCustomerByCode(string code, int tenant, bool getFromCache)
        {
            if (!string.IsNullOrEmpty(code))
            {
                string entityName = "Customer" + code.Trim() + tenant;
                Customer entity;
                if (getFromCache)
                {
                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        entity = context.Customers
                                        .Include("Card")
                                        .Include("SalesmanUser")
                                        .Include("SalesmanUser.Contact")
                                        .Include("AccountManagerUser.Contact")
                                        .Include("Card.SharedLogisticsInvitationStatus")
                                        .Include("Rank")
                                        .Include("Collector.Contact")
                                        .Include("Classifier.Contact")
                                        .FirstOrDefault(a => a.Tenant == tenant
                                                             && a.Card.Code == code.Trim());

                        if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        entity = (Customer)CacheManager.CacheWrapper.Get(entityName);
                    }
                }
                else
                {
                    entity = context.Customers
                                    .Include("Card")
                                    .Include("SalesmanUser")
                                    .Include("SalesmanUser.Contact")
                                    .Include("AccountManagerUser.Contact")
                                    .Include("Card.SharedLogisticsInvitationStatus")
                                    .Include("Rank")
                                    .Include("Collector.Contact")
                                    .Include("Classifier.Contact")
                                    .FirstOrDefault(record => record.Card.Code == code.Trim()
                                                              && record.Tenant == tenant);
                }

                return entity;
            }

            return null;
        }

        public Customer GetSingleCustomerByCodeForHybrid(string code, int tenant, bool getFromCache)
        {
            if (!string.IsNullOrEmpty(code))
            {
                string entityName = "Customer" + code.Trim() + tenant;
                Customer entity;

                if (getFromCache)
                {
                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        entity = context.Customers
                                        .Include("Card")
                                        .FirstOrDefault(a => a.Tenant == tenant
                                                             && a.Card.Code == code.Trim());

                        if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        entity = (Customer)CacheManager.CacheWrapper.Get(entityName);
                    }
                }
                else
                {
                    entity = context.Customers
                                    .Include("Card")
                                    .FirstOrDefault(record => record.Card.Code == code.Trim()
                                                              && record.Tenant == tenant);
                }

                return entity;
            }

            return null;
        }

        public Customer GetSingleCustomerByVat(string vat, int tenant, bool getFromCache)
        {
            if (!string.IsNullOrEmpty(vat))
            {
                string entityName = "Customer" + vat + tenant;
                Customer entity;

                if (getFromCache)
                {
                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        entity = context.Customers
                                        .Include("Card")
                                        .Include("SalesmanUser")
                                        .Include("SalesmanUser.Contact")
                                        .Include("AccountManagerUser.Contact")
                                        .Include("Card.SharedLogisticsInvitationStatus")
                                        .Include("Rank")
                                        .Include("Collector.Contact")
                                        .Include("Classifier.Contact")
                                        .FirstOrDefault(a => a.Tenant == tenant && a.Card.VatNumber == vat);

                        if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        entity = (Customer)CacheManager.CacheWrapper.Get(entityName);
                    }
                }
                else
                {
                    entity = context.Customers
                                    .Include("Card")
                                    .Include("SalesmanUser")
                                    .Include("SalesmanUser.Contact")
                                    .Include("AccountManagerUser.Contact")
                                    .Include("Card.SharedLogisticsInvitationStatus")
                                    .Include("Rank")
                                    .FirstOrDefault(record => record.Card.VatNumber == vat
                                                              && record.Tenant == tenant);
                }

                return entity;
            }

            return null;
        }

        public List<Customer> GetCustomersByVat(string vat, int tenant)
        {
            var result = new List<Customer>();

            if (!string.IsNullOrEmpty(vat))
            {
                result = context.Customers
                                .Include("Card")
                                .Where(a => a.Tenant == tenant
                                            && a.Card.VatNumber == vat
                                            && a.Card.InActive == false)
                                .ToList();
            }

            return result;
        }

        public Customer GetSingleCustomerByVatForHybrid(string vat, int tenant, bool getFromCache)
        {
            if (!string.IsNullOrEmpty(vat))
            {
                string entityName = "Customer" + vat + tenant;
                Customer entity;
                if (getFromCache)
                {

                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        entity = context.Customers
                                        .Include("Card")
                                        .FirstOrDefault(a => a.Tenant == tenant && a.Card.VatNumber == vat && a.Card.InActive == false);

                        if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        entity = (Customer)CacheManager.CacheWrapper.Get(entityName);
                    }
                }
                else
                {
                    entity = context.Customers
                                    .Include("Card")
                                    .FirstOrDefault(record => record.Card.VatNumber == vat
                                                              && record.Tenant == tenant
                                                              && record.Card.InActive == false);
                }

                return entity;
            }

            return null;
        }

        public Customer GetSingleCustomer(string id, int tenant, bool getFromCache = false)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string entityName = "Customer" + id + tenant;
                Customer entity;
                if (getFromCache)
                {
                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        entity = context.Customers
                                        .Include("Card")
                                        .Include("SalesmanUser")
                                        .Include("SalesmanUser.Contact")
                                        .Include("AccountManagerUser.Contact")
                                        .Include("Card.SharedLogisticsInvitationStatus")
                                        .Include("Rank")
                                        .Include("Collector.Contact")
                                        .Include("Classifier.Contact")
                                        .Include("Region")
                                        .Include("CustomerTeam")
                                        .FirstOrDefault(a => a.Tenant == tenant && a.Id == id);

                        if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        entity = (Customer)CacheManager.CacheWrapper.Get(entityName);
                    }
                }
                else
                {
                    entity = context.Customers
                                    .Include("Card")
                                    .Include("SalesmanUser")
                                    .Include("SalesmanUser.Contact")
                                    .Include("AccountManagerUser.Contact")
                                    .Include("Card.SharedLogisticsInvitationStatus")
                                    .Include("Rank")
                                    .Include("Collector.Contact")
                                    .Include("Classifier.Contact")
                                    .FirstOrDefault(record => record.Id == id && record.Tenant == tenant);
                }

                return entity;
            }

            return null;
        }

        public Customer GetSingleCustomerByVatAndStatusForHybrid(string vat, string statusCode, int tenant, bool getFromCache)
        {
            if (!string.IsNullOrEmpty(vat))
            {
                string entityName = "Customer" + vat + tenant;
                Customer entity;
                if (getFromCache)
                {
                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        entity = context.Customers
                                        .Include("Card")
                                        .FirstOrDefault(a => a.Tenant == tenant
                                                             && a.CustomerStatusCode == statusCode
                                                             && a.Card.VatNumber == vat
                                                             && a.Card.InActive == false);

                        if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        entity = (Customer)CacheManager.CacheWrapper.Get(entityName);
                    }
                }
                else
                {
                    entity = context.Customers
                                    .Include("Card")
                                    .FirstOrDefault(record => record.Card.VatNumber == vat
                                                              && record.Tenant == tenant
                                                              && record.CustomerStatusCode == statusCode
                                                              && record.Card.InActive == false);
                }

                return entity;
            }

            return null;
        }

        public Customer GetSingleCustomerWithCardOnly(string id, int tenant, bool getFromCache = false)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string entityName = "Customer_Card" + id + tenant;
                Customer entity;

                if (getFromCache)
                {
                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        entity = context.Customers
                                        .Include("Card")
                                        .FirstOrDefault(a => a.Tenant == tenant
                                                                && a.Id == id);

                        if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        entity = (Customer)CacheManager.CacheWrapper.Get(entityName);
                    }
                }
                else
                {
                    entity = context.Customers
                                    .Include("Card")
                                    .FirstOrDefault(a => a.Id == id
                                                         && a.Tenant == tenant);
                }

                return entity;
            }

            return null;
        }
        public List<Customer> GetCustomersByCardsIds(List<string> cardsIds, int tenant)
        {
            return context.Customers.Where(a => a.Tenant == tenant && cardsIds.Contains(a.Id)).ToList();
        }
        public IAmitalCloudContext context { get { return currentContext; } }
        public DateTime? ComputeLastInteractionDate(Customer customer, DateTime? oldestDate)
        {
            if (customer.LastInteractionDate != null)
            {
                if (customer.LastQuoteDate != null && customer.LastQuoteDate > oldestDate)
                {
                    oldestDate = customer.LastQuoteDate;
                }

                if (customer.LastOpportunityDate != null && customer.LastOpportunityDate > oldestDate)
                {
                    oldestDate = customer.LastOpportunityDate;
                }

                if (customer.LastCallDate != null && customer.LastCallDate > oldestDate)
                {
                    oldestDate = customer.LastCallDate;
                }

                if (customer.LastMeetingDate != null && customer.LastMeetingDate > oldestDate)
                {
                    oldestDate = customer.LastMeetingDate;
                }
            }

            return oldestDate;
        }

        public Customer GetFirstSingleByName(string name, int tenant)
        {
            return context.Customers
                          .Include("Card")
                          .FirstOrDefault(c => c.Card.EnglishName == name
                                               && c.Tenant == tenant);
        }

        public bool IsCustomerExist(string Id, int tenant)
        {
            return GetSingleCustomerWithCardOnly(Id, tenant, true) != null ? true : false;
        }
        public string GetSalesManByGLAccountId(string glAccountId, int tenant, string excludeCustomerId = null)
        {
            // Find all card IDs matching the given GLAccountId
            var cardIds = (
                from card in context.Cards
                where card.GLAccountId == glAccountId
                select card.Id
            );

            // Select SalesmanId from customers matching the card IDs and other conditions
            var SalesmanUser = (
                from customer in context.Customers
                where cardIds.Contains(customer.Id) && customer.Tenant == tenant && customer.SalesmanUserId != null && (excludeCustomerId == null || customer.Id != excludeCustomerId)
                select customer.SalesmanUser
            );
            var salesman = SalesmanUser.FirstOrDefault();
            if (salesman != null)
            {
                return salesman.Id;
            }
            else
            {
                return null;
            }
        }
        public string GetCollectorByGLAccount(string glAccountId, int tenant, string excludeCustomerId = null)
        {
            // Find all card IDs matching the given GLAccountId
            var cardIds = (
                from card in context.Cards
                where card.GLAccountId == glAccountId
                select card.Id
            );

            // Select SalesmanId from customers matching the card IDs and other conditions
            var Collector = (
                from customer in context.Customers
                where cardIds.Contains(customer.Id) && customer.Tenant == tenant && customer.CollectorId != null && (excludeCustomerId == null || customer.Id != excludeCustomerId)
                select customer.Collector
            );
            var colletor = Collector.FirstOrDefault();
            if (colletor != null)
            {
                return colletor.Id;
            }
            else
            {
                return null;
            }
        }
    }

}
