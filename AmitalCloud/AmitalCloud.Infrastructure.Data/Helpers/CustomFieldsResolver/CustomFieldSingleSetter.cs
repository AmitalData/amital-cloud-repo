using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmitalCloud.Infrastructure.Model.Interfaces;


namespace AmitalCloud.Infrastructure.Data.Helpers.CustomFieldsResolver
{
    public class CustomFieldSingleSetter
    {
        public static void SetFieldValue(CustomFieldResolverArgs customFieldResolverArgs)
        {
            List<ObjectField> customObjectFields = ObjectFieldRepository.GetCustomObjectFieldsByObjectTableName(customFieldResolverArgs.ObjectTableName, customFieldResolverArgs.Tenant).ToList();
            ObjectField customObjectField = customObjectFields.FirstOrDefault(f => f.Code.Replace(" ", "") == customFieldResolverArgs.FieldCode);
            if (customObjectField == null) return;

            PropertyInfo propInfo = customFieldResolverArgs.EntityPM.GetType().GetProperty(customObjectField.FieldName);
            if (propInfo == null) return;

            CustomFieldClass customFilterClass = new CustomFieldClass();
            string customFieldValue = customFilterClass.SetFieldDataType(customObjectField.DataTypeCode, customFieldResolverArgs.FieldValue);
            customFieldValue = ResolveCustomFieldValue(customObjectField, customFieldValue, customFieldResolverArgs.Tenant);
            propInfo.SetValue(customFieldResolverArgs.EntityPM, new CustomFieldClass(customObjectField.FieldName, customFieldResolverArgs.ObjectTableName, customFieldValue));
        }

        private static string ResolveCustomFieldValue(ObjectField objectField, string fieldValue, int tenant)
        {
            //For now we handle picklist type
            if (objectField.DataTypeCode == "PickList")
            {
                return HandleCustomPickListField(objectField, fieldValue, tenant);
            }
            return fieldValue;
        }

        private static string HandleCustomPickListField(ObjectField objectField, string fieldValue, int tenant)
        {
            if (fieldValue == null) return "";
            IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
            CustomPickList picklist = new Repository<CustomPickList>(context).GetMulti(a => a.Code == objectField.CustomPickListCode && a.Value == fieldValue && a.Tenant == tenant).FirstOrDefault();
            if (picklist != null) return picklist.Id;
            return fieldValue;
        }
    }
}
