using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class PartnerEntityQueryFilterExpression : IQueryTreeFilterExpression
    {
        private QueryTreeFilterContext queryTreeFilterContext;
        public void Interpret(QueryTreeFilterContext queryTreeFilterContext)
        {
            queryTreeFilterContext.IsInterpreterFinished = true;

            this.queryTreeFilterContext = queryTreeFilterContext;
            QueryTreeFilterIterator queryTreeFilterIterator = CreateIterator();
            if (!queryTreeFilterIterator.Any()) return;
            queryTreeFilterContext.ParentEntity = GetParentEntity();
            if (queryTreeFilterContext.ParentEntity == null) return;
            while (queryTreeFilterIterator.HasNext())
            {
                Handle(queryTreeFilterIterator.Next());
            }
        }

        private object GetParentEntity()
        {
            object parentEntity = GetContextParentEntity();
            if (parentEntity != null) return parentEntity;
            if (string.IsNullOrEmpty(queryTreeFilterContext.ParentObjectTableName) || string.IsNullOrEmpty(queryTreeFilterContext.ParentEntityId)) return null;
            try
            {
                return InjectionUtil.Instance.GetEntityByObjectTableNameAndEntityId(queryTreeFilterContext.ParentObjectTableName, queryTreeFilterContext.ParentEntityId, queryTreeFilterContext.Tenant);
            }
            catch
            {
                return null;
            }
        }

        private object GetContextParentEntity()
        {
            try
            {
                if (queryTreeFilterContext.ParentEntity != null) return JsonConvert.DeserializeObject(queryTreeFilterContext.ParentEntity.ToString());
                return null;
            }
            catch
            {
                return queryTreeFilterContext.ParentEntity;
            }
        }

        private void Handle(QueryFilterItem queryFilterItem)
        {
            queryFilterItem.FieldValue = Validation(queryFilterItem);
            queryFilterItem.FieldName = "PartnerEntityField";
            queryFilterItem.Operator = nameof(PartnerEntity);
        }

        private bool Validation(QueryFilterItem queryFilterItem)
        {
            var entityFieldValue = GetEntityFieldValue(queryFilterItem);

            switch (queryFilterItem.Operator.Replace("Field", ""))
            {
                case "LessThan": return AssertLessThan(queryFilterItem, entityFieldValue);
                case "LessThanOrEqual": return AssertLessThanOrEqual(queryFilterItem, entityFieldValue);
                case "GreaterThanOrEqual": return AssertGreaterOrEqual(queryFilterItem, entityFieldValue);
                case "LargerThan": 
                case "GreaterThan": return AssertLargerThan(queryFilterItem, entityFieldValue);
                case "Contains": return AssertContains(queryFilterItem, entityFieldValue);
                case "NotContains": return AssertNotContains(queryFilterItem, entityFieldValue);
                case "Equal": return AssertEqual(queryFilterItem, entityFieldValue);
                case "NotEqual": return AssertNotEquals(queryFilterItem, entityFieldValue);
                case "IsEmpty": return IsEmpty(entityFieldValue);
                case "IsNotEmpty": return IsNotEmpty(entityFieldValue);
                default: throw new InvalidOperationException("Operator not supported.");
            }
        }

        private bool IsEmpty(string entityFieldValue) => string.IsNullOrEmpty(entityFieldValue);
        private bool IsNotEmpty(string entityFieldValue) => !string.IsNullOrEmpty(entityFieldValue);
        private bool AssertContains(QueryFilterItem queryFilterItem, string entityFieldValue) => entityFieldValue.Contains(GetQueryFilterItemFieldValue(queryFilterItem));
        private bool AssertNotContains(QueryFilterItem queryFilterItem, string entityFieldValue) => !entityFieldValue.Contains(GetQueryFilterItemFieldValue(queryFilterItem));
        private bool AssertEndsWith(QueryFilterItem queryFilterItem, string entityFieldValue) => entityFieldValue.EndsWith(GetQueryFilterItemFieldValue(queryFilterItem));
        private bool AssertStartsWith(QueryFilterItem queryFilterItem, string entityFieldValue) => entityFieldValue.StartsWith(GetQueryFilterItemFieldValue(queryFilterItem));
        private bool AssertLargerThan(QueryFilterItem queryFilterItem, string entityFieldValue) => entityFieldValue.CompareTo(GetQueryFilterItemFieldValue(queryFilterItem)) > 0;
        private bool AssertGreaterOrEqual(QueryFilterItem queryFilterItem, string entityFieldValue) => entityFieldValue.CompareTo(GetQueryFilterItemFieldValue(queryFilterItem)) >= 0;
        private bool AssertLessThanOrEqual(QueryFilterItem queryFilterItem, string entityFieldValue) => !(entityFieldValue.CompareTo(GetQueryFilterItemFieldValue(queryFilterItem)) == 1);
        private bool AssertLessThan(QueryFilterItem queryFilterItem, string entityFieldValue) => entityFieldValue.CompareTo(GetQueryFilterItemFieldValue(queryFilterItem)) < 0;
        private bool AssertNotEquals(QueryFilterItem queryFilterItem, string entityFieldValue) => entityFieldValue != GetQueryFilterItemFieldValue(queryFilterItem);
        private bool AssertEqual(QueryFilterItem queryFilterItem, string entityFieldValue) => entityFieldValue == GetQueryFilterItemFieldValue(queryFilterItem);

        private string GetQueryFilterItemFieldValue(QueryFilterItem queryFilterItem) => ConvertFieldValueToString(queryFilterItem.FieldDataType, queryFilterItem.FieldValue, queryFilterItem.IsCustomField);

        private string ConvertFieldValueToString(string dataTypeCode , object fieldValue , bool isCustomField)
        {
            if (fieldValue == null) return "";
            if (isCustomField) return  fieldValue != null ? fieldValue.ToString() : "";
            string result  = FieldValueResolver.GetFieldStringValue(new ObjectField() { DataTypeCode = dataTypeCode }, fieldValue);
            if ((dataTypeCode == "DateTime" || dataTypeCode == "Date") && !string.IsNullOrEmpty(result?.ToString()) && result?.ToString().Length >= 9)
            {
                return result.ToString().Remove(8);
            }
            return result;
        }

        private QueryTreeFilterIterator CreateIterator()
        {
            var iterator = new QueryTreeFilterCollection(queryTreeFilterContext).CreateIterator();
            var collection = iterator.Collection
                .Where(d => !string.IsNullOrEmpty(d.FieldName) &&
                d.FieldName.Split('.')[0] == queryTreeFilterContext.ParentObjectTableName)
                .ToList();
            iterator.SetCollection(collection);
            return iterator;
        }

        private string GetFieldName(string field)
        {
            if (string.IsNullOrEmpty(field)) return null;
            var fieldNames = field.ToString().Split('.');
            if (fieldNames.Length == 0) return null;
            return fieldNames[fieldNames.Length-1];
        }

        private string GetEntityFieldValue(QueryFilterItem queryFilterItem)
        {
            if (queryTreeFilterContext.ParentEntity == null) return "";
            
            object value = QueryTreeFilterFieldValueResolver.Get(queryTreeFilterContext.ParentEntity, GetFieldName(queryFilterItem.FieldName), queryFilterItem.FieldDataType);

            return ConvertFieldValueToString(queryFilterItem.FieldDataType, value, queryFilterItem.IsCustomField);
        }
    }
}
