using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public class EntityCustomFieldService
    {
        private string objectTableId = string.Empty;
        private int tenant;
        private List<CustomFieldsMainObject> customFieldsMainObjects = null;
        private List<ObjectField> customObjectFields = null;
        private IRepository<CustomFieldsMainObject> customFieldsMainObjectRepository;
        private string objectTableName = string.Empty;
        private string originalObjectTableName = string.Empty;
        private string type = string.Empty;
        private List<object> entities = null;
        private string keyName = "Id";
        public EntityCustomFieldService(EntityCustomFieldServiceArgs entityCustomFieldServiceArgs)
        {
            originalObjectTableName = entityCustomFieldServiceArgs.ObjectTableName;
            objectTableName = GetObjectTableName(entityCustomFieldServiceArgs.ObjectTableName);
            objectTableId = new ObjectTableRepository(entityCustomFieldServiceArgs.Tenant).GetObjectTableIdByName(objectTableName, entityCustomFieldServiceArgs.Tenant);
            tenant = entityCustomFieldServiceArgs.Tenant;
            entities = entityCustomFieldServiceArgs.Entities;
            type = entityCustomFieldServiceArgs.Type;
            customFieldsMainObjectRepository = new Repository<CustomFieldsMainObject>(tenant);
            customObjectFields = GetCustomObjectFields();
            customFieldsMainObjects = GetCustomFieldsMainObjects(entityCustomFieldServiceArgs.EntityId);
            if (!string.IsNullOrEmpty(entityCustomFieldServiceArgs.KeyName))
            {
                keyName = entityCustomFieldServiceArgs.KeyName;
            }
        }
        private static string GetObjectTableName(string objectTableName)
        {
            List<string> PartnersObjectTablesNames = GetPartnersObjectTablesNames();

            return PartnersObjectTablesNames.Contains(objectTableName) ? "Card" : objectTableName;
        }
        private static List<string> GetPartnersObjectTablesNames()
        {
            return new List<string>()
            {
                "AccountingPartner",
                "Agent",
                "Airline",
                "CustomAgent",
                "CustomsShipper",
                "Participant",
                "ShippingAgent",
                "ShippingLine",
                "Trucker",
                "Vendor",
                "Warehouse",
                "Customer",
            };
        }
        public void Update()
        {
            using (var uow = new UnitOfWork(tenant))
            {

                if (customObjectFields.Count() == 0 || entities == null || entities.Count() == 0) return;
                object entity = entities.FirstOrDefault();
                string entityId = GetPropertyValue(entity, keyName).ToString();
                CustomFieldsMainObject customFieldsMainObject = GetCustomFieldsMainObject(entityId);
                if (customFieldsMainObject == null) return;
                foreach (ObjectField customObjectField in customObjectFields)
                {
                    SetPropertyValue(customFieldsMainObject, customObjectField.FieldName, (GetPropertyValue(entity, customObjectField.FieldName) as CustomFieldClass)?.Value);
                }
                if (IsEntityExist(entityId))
                {
                    customFieldsMainObjectRepository.Update(customFieldsMainObject);
                }
                uow.Save();
            }
        }
        public void Set()
        {
            if (customObjectFields.Count() == 0 || entities == null || entities.Count() == 0) return;
            foreach (object entity in entities)
            {
                SetCustomFieldValue(entity);
            }
        }
        private void SetCustomFieldValue(object entity)
        {
            CustomFieldsMainObject customFieldsMainObject = GetCustomFieldsMainObject((GetPropertyValue(entity, keyName).ToString()), false);
            if (entity == null || customFieldsMainObject == null) return;
            foreach (ObjectField customObjectField in customObjectFields)
            {
                SetPropertyValue(entity, customObjectField.FieldName, (type == "List" ? GetPropertyValue(customFieldsMainObject, customObjectField.FieldName) : GetCustomFieldValue(customFieldsMainObject, customObjectField)));
            }
        }
        private object GetCustomFieldValue(object entity, ObjectField objectField)
        {
            var propertyValue = GetPropertyValue(entity, objectField.FieldName);
            return new CustomFieldClass(objectField.FieldName, originalObjectTableName, propertyValue != null ? propertyValue.ToString() : "");
        }
        private bool IsEntityExist(string entityId) => customFieldsMainObjects.Where(d => d.EntityId == entityId).Any();
        private CustomFieldsMainObject GetCustomFieldsMainObject(string entityId, bool isNew = true)
        {
            if (IsEntityExist(entityId)) return customFieldsMainObjects.Where(d => d.EntityId == entityId).FirstOrDefault();
            if (!isNew) return null;

            CustomFieldsMainObject customFieldsMainObject = new CustomFieldsMainObject()
            {
                Id = IdCounter.GetNumber("ChildEntitiesCustomField", tenant).ToString(),
                Tenant = tenant,
                ObjectTableId = objectTableId,
                EntityId = entityId,
            };
            customFieldsMainObjectRepository.Insert(customFieldsMainObject);
            return customFieldsMainObject;
        }
        private List<ObjectField> GetCustomObjectFields()
        {
            return ObjectFieldRepository.GetCustomObjectFieldsByObjectTableName(objectTableName, tenant).ToList();
        }
        private List<CustomFieldsMainObject> GetCustomFieldsMainObjects(string entityId)
        {
            if (!string.IsNullOrEmpty(entityId))
            {
                return customFieldsMainObjectRepository.GetMulti(d => d.EntityId == entityId && d.ObjectTableId == objectTableId && d.Tenant == tenant).ToList();                   //.GetCustomFieldsMainObjects(tenant).Where(d => d.EntityId == entityId && d.ObjectTableId == objectTableId).ToList();
            }
            return customFieldsMainObjectRepository.GetMulti(d => d.ObjectTableId == objectTableId && d.Tenant == tenant).ToList();                     //.GetCustomFieldsMainObjects(tenant).Where(d => d.ObjectTableId == objectTableId).ToList();
        }
        private void SetPropertyValue(object obj, string property, object value)
        {
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                prop.SetValue(obj, value, null);
            }
        }
        private object GetPropertyValue(object obj, string property)
        {
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                return prop.GetValue(obj);
            }
            return "";
        }
    }
}

