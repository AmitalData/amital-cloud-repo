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
    public class RuleConditionFieldRepository : Repository<RuleConditionField>
    {
        IAmitalCloudContext amitalCloudContext;
        public RuleConditionFieldRepository(IAmitalCloudContext context) : base(context)
        {
            amitalCloudContext = context;
        }

        public RuleConditionFieldRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public RuleConditionField GetSingleRuleConditionField(string id, int tenant)
        {
            return (from a in context.RuleConditionFields.Include("ObjectField")
                    where a.Id == id && a.Tenant == tenant
                    select a).FirstOrDefault();
        }
        public IQueryable<RuleConditionField> GetRuleConditionFieldsByTenant(int tenant)
        {
            IQueryable<RuleConditionField> ruleConditionFields = from a in context.RuleConditionFields.Include("ObjectField")
                                                                 where a.Tenant == tenant
                                                                 select a;
            return ruleConditionFields;
        }
        public IQueryable<RuleConditionField> GetRuleConditionFieldsByRuleId(string ruleId, int tenant)
        {
            IQueryable<RuleConditionField> ruleConditionFields = from a in context.RuleConditionFields.Include("ObjectField")
                                                                 where a.Tenant == tenant && a.ObjectTableRuleId == ruleId
                                                                 select a;
            return ruleConditionFields;
        }
        public static List<RuleConditionField> GetObjectRuleConditionFieldsByTenant(int tenant)
        {
            string listName = "ruleconditionfieldstenant" + tenant;
            List<RuleConditionField> zeroRuleConditionField;
            List<RuleConditionField> currentRuleConditionField = new List<RuleConditionField>();
            List<RuleConditionField> ruleConditionField;
            List<RuleConditionField> selectedFields = new List<RuleConditionField>();
            if (CacheManager.CacheWrapper.Get(listName) == null)
            {
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    zeroRuleConditionField = (from a in context.RuleConditionFields.Include("ObjectField")
                                              where a.Tenant == 0
                                              select a).ToList();
                }
                if (tenant != 0)
                {
                    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        currentRuleConditionField = (from a in context.RuleConditionFields.Include("ObjectField")
                                                     where a.Tenant == tenant
                                                     select a).ToList();
                    }
                }
                ruleConditionField = zeroRuleConditionField.Concat(currentRuleConditionField).ToList();
                foreach (RuleConditionField field in ruleConditionField)
                {
                    RuleConditionField existedRuleField = (from a in selectedFields
                                                           where a.ObjectFieldCode == field.ObjectFieldCode && a.ObjectTableRuleId == field.ObjectTableRuleId
                                                           select a).FirstOrDefault();
                    if (existedRuleField != null)
                    {
                        if (existedRuleField.Tenant == 0 && existedRuleField.Tenant == tenant)
                        {
                            selectedFields.Remove(existedRuleField);
                            selectedFields.Add(field);
                        }
                    }
                    else
                    {
                        selectedFields.Add(field);
                    }
                }
                CacheManager.CacheWrapper.Insert(listName, selectedFields, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
            }
            else
            {
                selectedFields = (List<RuleConditionField>)CacheManager.CacheWrapper.Get(listName);
            }
            return selectedFields;
        }
        public IAmitalCloudContext context
        {
            get { return amitalCloudContext; }
        }
    }
}
