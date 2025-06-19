using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Transactions;
using System.Web;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class ObjectTableRuleFieldRepository : Repository<ObjectTableRuleField>
    {

        IAmitalCloudContext amitalCloudContext;

        public ObjectTableRuleFieldRepository(IAmitalCloudContext context) : base(context)
        {
            amitalCloudContext = context;

        }
        public ObjectTableRuleFieldRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public IQueryable<ObjectTableRuleField> GetObjectTableRuleFields(int tenant)
        {
            return (from record in context.ObjectTableRuleFields where record.Tenant == tenant select record);
        }
        public ObjectTableRuleField GetSingleObjectTableRuleField(string id, int tenant)
        {
            return (from record in context.ObjectTableRuleFields where record.Id == id && record.Tenant == tenant select record).FirstOrDefault();
        }
        public IQueryable<ObjectTableRuleField> GetRuleFieldsByRuleId(string ruleId, int tenant)
        {
            return (from record in context.ObjectTableRuleFields where record.ObjectTableRuleId == ruleId && record.Tenant == tenant select record);
        }
        public static List<ObjectTableRuleField> GetTenantRuleFields(int tenant)
        {
            List<ObjectTableRuleField> zeroquery;
            List<ObjectTableRuleField> currentquery;
            List<ObjectTableRuleField> query = new List<ObjectTableRuleField>();
            string rulesFieldsListName = "RuleFields" + tenant;
            if (HttpContextHelper.HttpContext != null)
            {
                if (CacheManager.CacheWrapper.Get(rulesFieldsListName) == null)
                {
                    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        currentquery = (from a in context.ObjectTableRuleFields.Include("ObjectField").Include("ObjectTableRule")
                                        where (a.Tenant == tenant)
                                        select a).ToList();
                    }
                    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        zeroquery = (from a in context.ObjectTableRuleFields.Include("ObjectField").Include("ObjectTableRule")
                                     where (a.Tenant == 0)
                                     select a).ToList();
                    }
                    query = zeroquery.Concat(currentquery).ToList();
                    CacheManager.CacheWrapper.Insert(rulesFieldsListName, query, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                }
                else
                {
                    query = (List<ObjectTableRuleField>)CacheManager.CacheWrapper.Get(rulesFieldsListName);
                }
            }
            else
            {
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    currentquery = (from a in context.ObjectTableRuleFields.Include("ObjectField").Include("ObjectTableRule")
                                    where (a.Tenant == tenant)
                                    select a).ToList();
                }
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    zeroquery = (from a in context.ObjectTableRuleFields.Include("ObjectField").Include("ObjectTableRule")
                                 where (a.Tenant == 0)
                                 select a).ToList();
                }
                query = zeroquery.Concat(currentquery).ToList();
            }
            return query;
        }
        public IAmitalCloudContext context
        {
            get { return amitalCloudContext; }
        }
    }
}
