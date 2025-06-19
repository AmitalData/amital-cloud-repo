using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AmitalCloud.Infrastructure.Data.Helpers.CustomFieldsResolver
{
    public class CustomFieldMultiSetter
    {
        const string resolveCustomFieldsValuesUsingParallelMechanisimFeatureToggleCode = "XUP";
        public static void SetCustomFieldsValues(string objectTableName, int tenant, List<object> entities)
        {
            if (FeatureToggleHelper.HasFeatureToggle(resolveCustomFieldsValuesUsingParallelMechanisimFeatureToggleCode, tenant))
            {
                SetCustomFieldsValuesUsingParallelMechanisim(objectTableName, tenant, entities);
                return;
            }
            SetCustomFieldsValuesWithoutUsingParallelMechanisim(objectTableName, tenant, entities);
        }

        private static void SetCustomFieldsValuesUsingParallelMechanisim(string objectTableName, int tenant, List<object> entities)
        {
            CustomFieldResolver customFieldResolver = new CustomFieldResolver(tenant);
            customFieldResolver.UsingParallelMechanisim = true;
            List<ObjectField> customFields = ObjectFieldRepository.GetCustomObjectFieldsByObjectTableName(objectTableName, tenant).ToList();

            string loggedUserEmail = AuthenticationUtil.AuthenticatedUserEmail;
            Parallel.ForEach(entities, entity =>
            {
                if (entity != null)
                {
                    Parallel.ForEach(customFields, field =>
                    {
                        SetCustomFieldValue(new CustomFieldMultiSetterValueArgs
                        {
                            tenant = tenant,
                            Entity = entity,
                            field = field,
                            customFieldResolver = customFieldResolver,
                            loggedUserEmail = loggedUserEmail,
                        });
                    });
                }
            });
        }

        private static void SetCustomFieldsValuesWithoutUsingParallelMechanisim(string objectTableName, int tenant, List<object> listQuery)
        {
            CustomFieldResolver customFieldResolver = new CustomFieldResolver(tenant);
            List<ObjectField> customFields = ObjectFieldRepository.GetCustomObjectFieldsByObjectTableName(objectTableName, tenant).ToList();
            string loggedUserEmail = AuthenticationUtil.AuthenticatedUserEmail;

            foreach (ObjectField field in customFields)
            {
                foreach (object list in listQuery)
                {
                    SetCustomFieldValue(new CustomFieldMultiSetterValueArgs
                    {
                        tenant = tenant,
                        Entity = list,
                        field = field,
                        customFieldResolver = customFieldResolver,
                        loggedUserEmail = loggedUserEmail,
                    });
                }
            }
        }

        private static void SetCustomFieldValue(CustomFieldMultiSetterValueArgs customFieldMultiSetterValueArgs)
        {
            if (customFieldMultiSetterValueArgs.Entity == null) return;
            AuthenticationUtil.AuthenticatedUserEmail = customFieldMultiSetterValueArgs.loggedUserEmail;
            int tenant = customFieldMultiSetterValueArgs.tenant;
            object list = customFieldMultiSetterValueArgs.Entity;
            ObjectField field = customFieldMultiSetterValueArgs.field;

            PropertyInfo propInfo = list.GetType().GetProperty(field.FieldName);
            object newValue = customFieldMultiSetterValueArgs.customFieldResolver.GetFieldValue(list, field, tenant);

            if (propInfo == null)
            {
                return;
            }

            propInfo.SetValue(list, newValue, null);
        }

        public static void SetDataProviderCustomFieldsValues(CustomFieldDataProviderSetterValueArgs customFieldDataProviderSetterValueArgs)
        {
            if (customFieldDataProviderSetterValueArgs.entity == null || customFieldDataProviderSetterValueArgs.provider == null) return;

            CustomFieldResolver customFieldResolver = new CustomFieldResolver(customFieldDataProviderSetterValueArgs.tenant);
            List<ObjectField> customFields = ObjectFieldRepository.GetCustomObjectFieldsByObjectTableName(customFieldDataProviderSetterValueArgs.objectTableName, customFieldDataProviderSetterValueArgs.tenant).ToList();
            customFieldDataProviderSetterValueArgs.loggedUserEmail = AuthenticationUtil.AuthenticatedUserEmail;
            customFieldDataProviderSetterValueArgs.customFieldResolver = customFieldResolver;

            if (FeatureToggleHelper.HasFeatureToggle(resolveCustomFieldsValuesUsingParallelMechanisimFeatureToggleCode, customFieldDataProviderSetterValueArgs.tenant))
            {
                Parallel.ForEach(customFields, field =>
                {
                    customFieldDataProviderSetterValueArgs.field = field;
                    SetDataProviderCustomFieldValue(customFieldDataProviderSetterValueArgs);
                });
                return;
            }

            foreach (ObjectField field in customFields)
            {
                customFieldDataProviderSetterValueArgs.field = field;
                SetDataProviderCustomFieldValue(customFieldDataProviderSetterValueArgs);
            }
        }

        private static void SetDataProviderCustomFieldValue(CustomFieldDataProviderSetterValueArgs customFieldDataProviderSetterValueArgs)
        {
            object entity = customFieldDataProviderSetterValueArgs.entity;
            ObjectField field = customFieldDataProviderSetterValueArgs.field;

            PropertyInfo propInfo = entity.GetType().GetProperty(field.FieldName);
            if (propInfo == null) return;

            string objectTableName = customFieldDataProviderSetterValueArgs.objectTableName;
            int tenant = customFieldDataProviderSetterValueArgs.tenant;
            object provider = customFieldDataProviderSetterValueArgs.provider;
            string propertyIdientifier = customFieldDataProviderSetterValueArgs.propertyIdientifier;
            string loggedUserEmail = customFieldDataProviderSetterValueArgs.loggedUserEmail;
            CustomFieldResolver customFieldResolver = customFieldDataProviderSetterValueArgs.customFieldResolver;

            string propertyName = (!string.IsNullOrEmpty(propertyIdientifier) ? propertyIdientifier : objectTableName) + field.FieldName;
            PropertyInfo providerPropInfo = provider.GetType().GetProperty(propertyName);
            if (providerPropInfo == null) return;

            AuthenticationUtil.AuthenticatedUserEmail = loggedUserEmail;
            object newValue = customFieldResolver.GetFieldValue(entity, field, tenant);
            providerPropInfo.SetValue(provider, newValue, null);
        }
    }

    public class CustomFieldMultiSetterValueArgs
    {
        public int tenant { get; set; }
        public object Entity { get; set; }
        public ObjectField field { get; set; }
        public CustomFieldResolver customFieldResolver { get; set; }
        public string loggedUserEmail { get; set; }
    }

    public class CustomFieldDataProviderSetterValueArgs
    {
        public string objectTableName { get; set; }
        public int tenant { get; set; }
        public object entity { get; set; }
        public object provider { get; set; }
        public string propertyIdientifier { get; set; }
        public ObjectField field { get; set; }
        public string loggedUserEmail { get; set; }
        public CustomFieldResolver customFieldResolver { get; set; }
    }
}
