using System;
using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class ObjectTableRuleQuery
    {
        private readonly int tenant;
        private readonly IAmitalCloudContext context;
        private readonly Repository<ObjectTableRule> repository;

        public ObjectTableRuleQuery(int tenant)
        {
            this.tenant = tenant;
            context = AmitalCloudContext.GetContext(tenant);
            repository = new Repository<ObjectTableRule>(context);
        }

        public List<ObjectTableRulePM> GetObjectTableRulePMsByTenant()
        {
            string pmslistName = "objecttablerulepmstenant" + tenant;
            List<ObjectTableRulePM> allRules = new List<ObjectTableRulePM>();

            if (CacheManager.CacheWrapper.Get(pmslistName) == null)
            {
                Repository<RuleConditionField> ruleConditionFieldRepository = new Repository<RuleConditionField>(context);

                List<ObjectTableRulePM> loggedTenantobjectTableRulePMs = GetObjectTableRules(tenant, ruleConditionFieldRepository);

                if (tenant != 0)
                {
                    List<ObjectTableRulePM> zeroTenantobjectTableRulePMs = GetObjectTableRules(0, ruleConditionFieldRepository);

                    foreach (ObjectTableRulePM rule in loggedTenantobjectTableRulePMs)
                    {
                        rule.IsCreatedFromSystemRule = zeroTenantobjectTableRulePMs.Any(r => r.RuleCode == rule.RuleCode);
                        allRules.Add(rule);
                    }

                    foreach (ObjectTableRulePM rule in zeroTenantobjectTableRulePMs)
                    {
                        if (!allRules.Any(r => r.RuleCode == rule.RuleCode))
                        {
                            allRules.Add(rule);
                        }
                    }
                }
                else
                {
                    allRules = loggedTenantobjectTableRulePMs;
                }
               
                CacheManager.CacheWrapper.Insert(pmslistName, allRules, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
            }
            else
            {
                allRules = (List<ObjectTableRulePM>)CacheManager.CacheWrapper.Get(pmslistName);
            }

            return allRules;
        }

        private List<ObjectTableRulePM> GetObjectTableRules(int tenant, IRepository<RuleConditionField> ruleConditionFieldRepository)
        {
            List<ObjectTableRulePM> objectTableRulePMs = repository.GetMultiFromCache($"GetObjectTableRules{tenant}", a => a.Tenant == tenant, "RuleType", a => new ObjectTableRulePM(a)
            {
                RuleTypeName = a.RuleType.Name,
            });

            foreach (ObjectTableRulePM rule in objectTableRulePMs)
            {
                rule.RuleConditionFields = ruleConditionFieldRepository.GetMultiFromCache($"GetObjectTableRulesRuleConditionFields{tenant}-{rule.Id}", a => (a.Tenant == rule.Tenant) && a.ObjectTableRuleId == rule.Id, "ObjectField", a => new RuleConditionFieldPM(a)
                {
                    ObjectFieldName = a.ObjectField.FieldName,
                });
            }
            return objectTableRulePMs;
        }

    }
}