using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Linq;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class TreeFilterQueryService : ITreeFilterQueryService
    {
        public IQueryable<T> Apply<T>(IQueryable<T> queryable, TreeFilterQueryArgs treeFilterQueryArgs)
        {
            if (string.IsNullOrWhiteSpace(treeFilterQueryArgs.AdditionalTreeFilter) || treeFilterQueryArgs.AdditionalTreeFilter == "null" || treeFilterQueryArgs.AdditionalTreeFilter == "undefined") return queryable;
            treeFilterQueryArgs.Type = typeof(T);
            QueryFilterItem queryFilterItem = new QueryTreeFilterInterpreter(treeFilterQueryArgs).Run();
            if (IsQueryFilterEmpty(queryFilterItem)) return queryable;
            return queryable.Where(queryFilterItem.GetTreeExpression<T>());
        }

        private bool IsQueryFilterEmpty(QueryFilterItem queryFilterItem) => queryFilterItem?.QueryFilterItems == null || !queryFilterItem.QueryFilterItems.Any();
    }
}