using AmitalCloud.Infrastructure.Domain.DataContracts;
using System.Text.Json;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class JavaScriptSerializerExpression: IQueryTreeFilterExpression
    {
        public void Interpret(QueryTreeFilterContext queryTreeFilterContext)
        {
            if (!string.IsNullOrWhiteSpace(queryTreeFilterContext?.AdditionalTreeFilter))
            {
                queryTreeFilterContext.QueryFilterItem = JsonSerializer.Deserialize<QueryFilterItem>(queryTreeFilterContext.AdditionalTreeFilter);
            }
            else
            {
                queryTreeFilterContext.QueryFilterItem = null;
            }
        }
    }
}
