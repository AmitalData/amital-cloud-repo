using AmitalCloud.Infrastructure.Domain.DataContracts;
using System;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public interface IQueryTreeFilterExpression
    {
        void Interpret(QueryTreeFilterContext context);
    }

    public class QueryTreeFilterContext
    {
        public QueryFilterItem QueryFilterItem { get; set; }
        public string ParentObjectTableName { get; set; }
        public string ParentEntityId { get; set; }
        public string AdditionalTreeFilter { get; set; }
        public string ObjectTableName { get; set; }
        public object ParentEntity { get; set; }
        public int Tenant { get; set; }
        public bool IsInterpreterFinished { get; set; }
        public Type Type { get; set; }
    }
}
