using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using System;
using System.Linq;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class CustomFieldExpression : IQueryTreeFilterExpression
    {
        private readonly CustomFieldClass customFilterClass = new CustomFieldClass();
        public void Interpret(QueryTreeFilterContext queryTreeFilterContext)
        {
            QueryTreeFilterIterator iterator = CreateIterator(queryTreeFilterContext);
            if (!iterator.Any()) return;
            while (iterator.HasNext())
            {
                Handle(iterator.Next(), queryTreeFilterContext);
            }
        }

        private void Handle(QueryFilterItem filterItem, QueryTreeFilterContext queryTreeFilterContext)
        {
            if(!filterItem.IsCustomField || (string.IsNullOrEmpty(filterItem.Operator) && filterItem.Operator.Contains("Field"))) return;
            filterItem.FieldValue = GetCustomFieldStringValue(GetFieldValue(filterItem.FieldDataType, filterItem.FieldValue), filterItem.FieldDataType);
            filterItem.FieldValue2 = GetCustomFieldStringValue(GetFieldValue(filterItem.FieldDataType, filterItem.FieldValue2), filterItem.FieldDataType);
        }

        private object GetCustomFieldStringValue(object fieldValue ,string fieldDataType)
        {
            if (fieldValue == null) return null;
             string result = customFilterClass.SetFieldDataType(fieldDataType, fieldValue);

            if (string.Equals(result, "false", StringComparison.OrdinalIgnoreCase))
                result = null;
            return result;
        }

        public string GetFieldValue(string dataTypeCode, object fieldValue)
        {
            if (fieldValue == null || (dataTypeCode != "DateTime" && dataTypeCode != "Date")) return fieldValue != null ? fieldValue.ToString() : null;
            try
            {
                return FieldValueResolver.ConvertToDate(fieldValue.ToString()).ToString();
            }
            catch
            {
                return fieldValue?.ToString();
            }
        }

        private QueryTreeFilterIterator CreateIterator(QueryTreeFilterContext queryTreeFilterContext)
        {
            var iterator = new QueryTreeFilterCollection(queryTreeFilterContext).CreateIterator();
            var collection = iterator.Collection.Where(d => d.IsCustomField && (d.Operator == null || !d.Operator.Contains("Field"))).ToList();
            iterator.SetCollection(collection);
            return iterator;
        }
    }
}
