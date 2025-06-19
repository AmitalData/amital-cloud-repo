using System.Linq.Expressions;

namespace AmitalCloud.Infrastructure.Web.Helpers
{
    public class ExpressionValidation
    {
        public bool ExecuteExpression(string ruleCondition, Dictionary<string, object> conditionFieldsDic)
        {
            var parameter = Expression.Parameter(typeof(Dictionary<string, object>), "fields");

            var propertyAccess = Expression.Property(parameter, "Item", Expression.Constant(ruleCondition));
            var comparison = Expression.GreaterThan(propertyAccess, Expression.Constant(18));

            var lambda = Expression.Lambda<Func<Dictionary<string, object>, bool>>(comparison, parameter);
            var compiledExpression = lambda.Compile();

            return compiledExpression(conditionFieldsDic);
        }
    }
}
