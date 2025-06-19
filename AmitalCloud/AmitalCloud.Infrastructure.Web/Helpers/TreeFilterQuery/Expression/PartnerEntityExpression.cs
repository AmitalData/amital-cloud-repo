using AmitalCloud.Infrastructure.Domain.DataContracts;
using System.Linq.Expressions;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class PartnerEntity
    {
        public Expression CreateExpression(Expression expression, QueryFilterItem queryFilterItem)
        {
            bool value = queryFilterItem.FieldValue is bool b && b;
            return value ? Expression.Equal(expression, expression) : Expression.NotEqual(expression, expression);
        }
    }
}
