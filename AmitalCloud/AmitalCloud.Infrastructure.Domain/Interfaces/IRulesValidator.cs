using AmitalCloud.Infrastructure.Model.EntityClasses;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IRulesValidator
    {
        void Initialize(int tenant);
        List<ObjectTableRuleField> ValidateAllRequiredFieldRules(object entity, string objectTableName, int tenant);
        bool ValidateEntityRules(object entity, string objectTableName, int tenant, ref string outputMessage);
        bool ApplyDuplicationRules(object entity, string objectTableName, int tenant, ref string outputMessage);
    }
}
