using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Web.Helpers.TreeFilterQuery
{
    public class QueryTreeFilterFieldValueResolver
    {
        public static object Get(object parentEntity, string fieldName, string fieldDataType)
        {
            if (parentEntity is JObject)
            {
                return GetFromDynamicEntity(parentEntity, fieldName, fieldDataType);
            }
            else
            {
                return GetFromEntity(parentEntity, fieldName, fieldDataType);
            }
        }

        public static object GetFromDynamicEntity(object parentEntity, string fieldName, string fieldDataType)
        {
            dynamic dynamicParentEntity = parentEntity;
            object value = dynamicParentEntity[fieldName];
            if (value == null) return null;

            if (value is JObject)
            {
                return GetCustomFieldValue(value, fieldDataType);
            }

            if (string.IsNullOrEmpty(value.ToString())) return null;

            return FieldValueResolver.GetFieldDataValue(new ObjectField { DataTypeCode = fieldDataType }, value.ToString());
        }

        private static object GetCustomFieldValue(object value, string fieldDataType)
        {
            dynamic customFieldValue = value;

            if (string.IsNullOrWhiteSpace(customFieldValue["value"]?.ToString()))
                return string.Empty;

            CustomFieldClass customFilterClass = new CustomFieldClass();
            return customFilterClass.SetFieldDataType(fieldDataType, customFieldValue["value"]);
        }
        public static object GetFromEntity(object parentEntity, string fieldName, string fieldDataType)
        {
            if (parentEntity == null || string.IsNullOrEmpty(fieldName)) return null;

            PropertyInfo propertyInfo = GetProperty(parentEntity, fieldName);
            if (propertyInfo == null) return "";
            object value = propertyInfo.GetValue(parentEntity, null);

            if (value == null) return "";

            if (!(value is CustomFieldClass))
            {
                return value;
            }

            return new CustomFieldClass().SetFieldDataType(fieldDataType, (value as CustomFieldClass).Value);
        }

        private static PropertyInfo GetProperty(object entity, string fieldName)
        {
            Type type = entity.GetType();
            return type.GetProperty(fieldName);
        }
    }
}
