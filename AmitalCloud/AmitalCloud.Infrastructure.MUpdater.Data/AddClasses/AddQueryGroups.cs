using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityKeys;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.MUpdater.Data.AddClasses
{
    public class AddQueryGroups
    {
        public static QueryGroup AddQueryGroup(QueryGroupPM queryGroupDetails, Repository<QueryGroup> queryGroupRepository)
        {
            Dictionary<string, QueryGroup> tenantFixedAmounts = queryGroupRepository.GetQueryable().ToDictionary(d => d.Code, a => a);
            if (tenantFixedAmounts.Keys.Contains(queryGroupDetails.Code))
            {
                QueryGroup queryGroup = queryGroupRepository.GetSingle(a=> a.Code == queryGroupDetails.Code);
                queryGroup.Name = queryGroupDetails.Name;
                queryGroup.IndexOrder = queryGroupDetails.IndexOrder;
                queryGroupRepository.Update(queryGroup);
                return queryGroup;
            }
            else
            {
                QueryGroup newQueryGroup = new QueryGroup() { Code = queryGroupDetails.Code, Name = queryGroupDetails.Name, IndexOrder = queryGroupDetails.IndexOrder };
                queryGroupRepository.Insert(newQueryGroup);
                return newQueryGroup;
            }

        }

        public static QueryGroup AddQueryGroup(QueryGroupPM queryGroupDetails, Repository<QueryGroup> queryGroupRepository, Dictionary<string, QueryGroup> tenantQueryGroups)
        {
          
            if (tenantQueryGroups.Keys.Contains(queryGroupDetails.Code))
            {
                QueryGroup queryGroup = tenantQueryGroups[queryGroupDetails.Code];
                queryGroup.Name = queryGroupDetails.Name;
                queryGroup.IndexOrder = queryGroupDetails.IndexOrder;
                queryGroupRepository.Update(queryGroup);
                return queryGroup;
            }
            else
            {
                QueryGroup newQueryGroup = new QueryGroup() { Code = queryGroupDetails.Code, Name = queryGroupDetails.Name, IndexOrder = queryGroupDetails.IndexOrder };
                queryGroupRepository.Insert(newQueryGroup);
                tenantQueryGroups.Add(newQueryGroup.Code, newQueryGroup);
                return newQueryGroup;
            }

        }
       
    }
}