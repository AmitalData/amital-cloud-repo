using AmitalCloud.Infrastructure.Domain.DataContracts;
using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class QueryTreeFilterIterator
    {
        private List<QueryFilterItem> _collection;
        private int position = 0;

        public List<QueryFilterItem> Collection { get; }

        public QueryTreeFilterIterator(List<QueryFilterItem> collection)
        {
            this._collection = collection;
        }

        public void SetCollection(List<QueryFilterItem> collection)
        {
            this._collection = collection;
        }

        public bool Any() => _collection?.Any() == true;

        public QueryFilterItem Next() => _collection[position++];

        public bool HasNext() => position < _collection.Count && _collection[position] != null;
    }
}
