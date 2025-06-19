using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers.CustomFieldsResolver;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;


namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class CustomFieldResolver
    {
        public Object lockMe = new Object();
        public Dictionary<string, object> definedObjects = new Dictionary<string, object>();
        public CultureInfo en = new CultureInfo("en-US");
        public bool UsingParallelMechanisim;
        public List<CustomPickList> customPickLists;
        public List<LookUpFieldDataStorage> LookUpFieldsDataStorage;
        public CustomFieldResolver(int tenant)
        {
            IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
            customPickLists = new Repository<CustomPickList>(context).GetMulti(a => a.Tenant == tenant).ToList();
            LookUpFieldsDataStorage = new List<LookUpFieldDataStorage>();
        }
        public void SetFieldValue(CustomFieldResolverArgs customFieldResolverArgs)
        {
            CustomFieldSingleSetter.SetFieldValue(customFieldResolverArgs);
        }

        public void SetCustomFieldsValues(string objectTableName, int tenant, List<object> listQuery)
        {

            CustomFieldMultiSetter.SetCustomFieldsValues(objectTableName, tenant, listQuery);
        }

        public void SetDataProviderCustomFieldsValues(string objectTableName, int tenant, Object entity, Object provider, string propertyIdientifier = null)
        {
            CustomFieldMultiSetter.SetDataProviderCustomFieldsValues(new CustomFieldDataProviderSetterValueArgs
            {
                objectTableName = objectTableName,
                tenant = tenant,
                entity = entity,
                provider = provider,
                propertyIdientifier = propertyIdientifier
            });
        }

        public string GetFieldValue(object entity, ObjectField objectField, int tenant, bool externalAPICall = false)
        {
            return CustomFieldGetter.Get(new CustomFieldGetterArgs
            {
                entity = entity,
                objectField = objectField,
                tenant = tenant,
                customFieldResolver = this,
                externalAPICall = externalAPICall,
            });
        }

        public string GetFieldValue2(object value, ObjectField objectField, int tenant)
        {
            return CustomFieldGetter.Get2(value, objectField, tenant, this);
        }
    }

    public class CustomFieldResolverArgs
    {
        public string ObjectTableName { get; set; }
        public object EntityPM { get; set; }
        public string FieldCode { get; set; }
        public string FieldValue { get; set; }
        public int Tenant { get; set; }
    }

    public class LookUpFieldValueGetterArgs
    {
        public ObjectField ObjectField { get; set; }
        public int Tenant { get; set; }
        public bool ExternalAPICall { get; set; }
        public object Value { get; set; }
        public string InsideTypePath { get; set; }
        public Type InsideEntityType { get; set; }
    }

    public class LookUpFieldDataStorage
    {
        public string FieldValue { get; set; }
        public object Value { get; set; }
        public string LookUpTableId { get; set; }
        public int Tenant { get; set; }
    }
}
