using AmitalCloud.Infrastructure.Domain.DataContracts;
using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class QueryTreeFilterCollection
    {
        public List<QueryFilterItem> QueryFilterItems { get; } = new List<QueryFilterItem>();
        public QueryTreeFilterCollection(QueryTreeFilterContext queryTreeFilterContext)
        {
            if (queryTreeFilterContext?.QueryFilterItem != null)
            {
                BuildCollection(queryTreeFilterContext.QueryFilterItem);
            }
        }

        private void BuildCollection(QueryFilterItem queryFilterItem)
        {
            if (queryFilterItem.QueryFilterItems == null || !queryFilterItem.QueryFilterItems.Any())
            {
                QueryFilterItems.Add(queryFilterItem);
                return;
            }

            queryFilterItem.QueryFilterItems.ForEach(x => BuildCollection(x));
        }

        public QueryTreeFilterIterator CreateIterator() => new QueryTreeFilterIterator(QueryFilterItems);
        public QueryTreeFilterIterator CreateIterator(List<QueryFilterItem> collections) => new QueryTreeFilterIterator(collections);
    }
}
