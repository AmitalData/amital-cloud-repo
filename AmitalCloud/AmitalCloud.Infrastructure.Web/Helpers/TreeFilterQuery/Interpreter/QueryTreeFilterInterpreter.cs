using AmitalCloud.Infrastructure.Domain.DataContracts;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class QueryTreeFilterInterpreter
    {
        private readonly TreeFilterQueryArgs _treeFilterQueryArgs;
        public QueryTreeFilterInterpreter(TreeFilterQueryArgs treeFilterQueryArgs)
        {
            this._treeFilterQueryArgs = treeFilterQueryArgs;
        }

        public QueryFilterItem Run()
        {
            QueryTreeFilterContext queryTreeFilterContext = this.BuildContext(_treeFilterQueryArgs);
            List<IQueryTreeFilterExpression> expressions = BuildExpressions();
            foreach (IQueryTreeFilterExpression expression in expressions)
            {
                expression.Interpret(queryTreeFilterContext);
            }

            return queryTreeFilterContext.QueryFilterItem;
        }

        private List<IQueryTreeFilterExpression> BuildExpressions()
        {
            List<IQueryTreeFilterExpression> expressions = new List<IQueryTreeFilterExpression>();
            expressions.Add(new JavaScriptSerializerExpression());
            expressions.Add(new QueryTreeFilterValidateExpression());
            expressions.Add(new QueryTreeFilterResetValueExpression());
            expressions.Add(new CustomFieldExpression());
            expressions.Add(new DateGroupFilterExpression());
            expressions.Add(new PartnerEntityResolveFieldValueExpression());
            expressions.Add(new PartnerEntityQueryFilterExpression());
            return expressions;
        }

        private QueryTreeFilterContext BuildContext(TreeFilterQueryArgs treeFilterQueryArgs)
        {
            return new QueryTreeFilterContext()
            {

                AdditionalTreeFilter = treeFilterQueryArgs.AdditionalTreeFilter,
                ObjectTableName = treeFilterQueryArgs.ObjectTableName,
                ParentEntityId = treeFilterQueryArgs.ParentEntityId,
                ParentObjectTableName = treeFilterQueryArgs.ParentObjectTableName,
                Tenant = treeFilterQueryArgs.Tenant,
                Type = treeFilterQueryArgs.Type,
                ParentEntity = treeFilterQueryArgs.ParentEntity,
            };
        }
    }
}
