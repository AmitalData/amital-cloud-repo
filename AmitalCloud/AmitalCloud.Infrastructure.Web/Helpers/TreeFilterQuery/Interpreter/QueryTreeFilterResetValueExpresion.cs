using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class QueryTreeFilterResetValueExpression : IQueryTreeFilterExpression
    {
        private List<ObjectField> mainObjectFields;
        private List<ObjectField> partnerObjectFields;
        private QueryTreeFilterContext queryTreeFilterContext;

        public void Interpret(QueryTreeFilterContext queryTreeFilterContext)
        {
            this.queryTreeFilterContext = queryTreeFilterContext;

            var iterator = new QueryTreeFilterCollection(queryTreeFilterContext).CreateIterator();
            mainObjectFields = ObjectFieldRepository.GetObjectFieldsByObjectTableName(queryTreeFilterContext.ObjectTableName, queryTreeFilterContext.Tenant);
            if (iterator.Collection.Any(d => d.FieldName.Split('.')[0] == queryTreeFilterContext.ParentObjectTableName))
            {
                partnerObjectFields = ObjectFieldRepository.GetObjectFieldsByObjectTableName(queryTreeFilterContext.ParentObjectTableName, queryTreeFilterContext.Tenant);
            }

            while (iterator.HasNext())
            {
                Handel(iterator.Next());
            }
        }

        private void Handel(QueryFilterItem queryFilterItem)
        {
            ObjectField objectField = GetObjectField(queryFilterItem);
            if (queryFilterItem.IsAnalyticsMetadatas)
            {
                objectField = new ObjectField
                {
                    DataTypeCode = queryFilterItem.FieldDataType
                };
            }
            if (objectField == null) return;

            queryFilterItem.IsCustomField = objectField.IsCustom;
            queryFilterItem.FieldDataType = objectField.DataTypeCode;
            if (queryFilterItem.IsCustomField || (!string.IsNullOrEmpty(queryFilterItem.Operator) &&  queryFilterItem.Operator.Contains("Field"))) return;
            if (queryFilterItem.FieldValue != null && string.IsNullOrWhiteSpace(queryFilterItem.FieldValue.ToString())) queryFilterItem.FieldValue = null;
            if (queryFilterItem.FieldValue2 != null && string.IsNullOrWhiteSpace(queryFilterItem.FieldValue2.ToString())) queryFilterItem.FieldValue2 = null;

            queryFilterItem.FieldValue = FieldValueResolver.GetFieldDataValue(objectField, GetFieldValue(objectField.DataTypeCode , queryFilterItem.FieldValue));
            queryFilterItem.FieldValue2 = FieldValueResolver.GetFieldDataValue(objectField, GetFieldValue(objectField.DataTypeCode, queryFilterItem.FieldValue2));
        }

        public string GetFieldValue(string dataTypeCode , object fieldValue)
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

        private ObjectField GetObjectField(QueryFilterItem queryFilterItem)
        {
            string tableName = !string.IsNullOrEmpty(queryFilterItem?.FieldName) && queryFilterItem.FieldName.Split('.')[0] == queryTreeFilterContext.ParentObjectTableName ? queryTreeFilterContext.ParentObjectTableName : queryTreeFilterContext.ObjectTableName;
            string fieldName = GetFieldName(queryFilterItem);
            if(tableName == queryTreeFilterContext.ParentObjectTableName)
            {
                return partnerObjectFields?.FirstOrDefault(f => f.FieldName == fieldName);
            }

            return mainObjectFields?.FirstOrDefault(f => f.FieldName == fieldName);
        }

        private string GetFieldName(QueryFilterItem queryFilterItem)
        {
            if (string.IsNullOrEmpty(queryFilterItem.FieldName)) return "";
            var fieldNames = queryFilterItem.FieldName.Split('.');
            if (fieldNames.Length == 0) return null;
            return fieldNames.LastOrDefault();
        }
    }
}
