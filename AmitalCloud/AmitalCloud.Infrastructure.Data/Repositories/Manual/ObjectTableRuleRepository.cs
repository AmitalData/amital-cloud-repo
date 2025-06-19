using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Transactions;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class ObjectTableRuleRepository : Repository<ObjectTableRule>
    {
        IAmitalCloudContext amitalCloudContext;
        public ObjectTableRuleRepository(IAmitalCloudContext context) : base(context)
        {
            amitalCloudContext = context;
        }
        public ObjectTableRuleRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public IQueryable<ObjectTableRule> GetObjectTableRules(int tenant)
        {
            return (from record in context.ObjectTableRules.Include("ObjectTable") where record.Tenant == tenant select record);
        }
        public ObjectTableRule GetSingleObjectTableRule(string id, int tenant)
        {
            ObjectTableRule rule = (from record in context.ObjectTableRules.Include("ObjectTable") where record.Id == id && record.Tenant == tenant select record).FirstOrDefault();
            return rule;
        }
        public ObjectTableRule GetSingleObjectTableRuleByCode(string code, int tenant)
        {
            ObjectTableRule rule = (from record in context.ObjectTableRules.Include("ObjectTable") where record.RuleCode == code && record.Tenant == tenant select record).FirstOrDefault();
            return rule;
        }
        public static List<ObjectTableRule> GetObjectTableRulesByTenant(int tenant)
        {
            List<ObjectTableRule> zeroObjectTableRules;
            List<ObjectTableRule> currentObjectTableRules = new List<ObjectTableRule>();
            List<ObjectTableRule> objectTableRules;
            string listName = "objecttablerulestenant" + tenant;
            List<ObjectTableRule> selectedRules = new List<ObjectTableRule>();
            if (CacheManager.CacheWrapper.Get(listName) == null)
            {
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    zeroObjectTableRules = (from a in context.ObjectTableRules.Include("ObjectTable").Include("RuleType")
                                            where a.Tenant == 0
                                            select a).ToList();
                }
                if (tenant != 0)
                {
                    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        currentObjectTableRules = (from a in context.ObjectTableRules.Include("ObjectTable").Include("RuleType")
                                                   where a.Tenant == tenant
                                                   select a).ToList();
                    }
                }
                objectTableRules = zeroObjectTableRules.Concat(currentObjectTableRules).ToList();
                foreach (ObjectTableRule rule in objectTableRules)
                {
                    ObjectTableRule existedRule = (from a in selectedRules
                                                   where a.RuleCode == rule.RuleCode
                                                   select a).FirstOrDefault();
                    if (existedRule != null)
                    {
                        if (existedRule.Tenant == 0 && rule.Tenant == tenant)
                        {
                            selectedRules.Remove(existedRule);
                            selectedRules.Add(rule);
                        }
                    }
                    else
                    {
                        selectedRules.Add(rule);
                    }
                }
                CacheManager.CacheWrapper.Insert(listName, selectedRules, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
            }
            else
            {
                selectedRules = (List<ObjectTableRule>)CacheManager.CacheWrapper.Get(listName);
            }
            return selectedRules;
        }
        public IAmitalCloudContext context
        {
            get { return amitalCloudContext; }
        }
    }
}