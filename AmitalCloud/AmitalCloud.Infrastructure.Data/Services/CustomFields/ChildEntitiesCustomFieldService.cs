using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public class ChildEntitiesCustomFieldService
    {
        private string objectTableId = string.Empty;
        private string entityId = string.Empty;
        private string childObjectTableId = string.Empty;
        private int tenant;
        private List<ChildEntitiesCustomField> childEntitiesCustomFields = null;
        private CustomFieldResolver customFieldResolver = null;
        private List<ObjectField> customObjectFields = null;
        private IRepository<ChildEntitiesCustomField> childEntitiesCustomFieldRepository;
        private string childObjectTableName = string.Empty;
        private string deleteChangeSetOp = "Delete";
        private string noneChangeSetOp = "None";
        private List<object> deletedChildEntities = null;
        private List<object> modificationChildEntities = null;
        private List<object> childEntities = null;
        private string childEntityId = string.Empty;
        private void Initialize(ChildEntitiesCustomFieldArgs childEntitiesCustomFieldArgs)
        {
            childEntities = childEntitiesCustomFieldArgs.ChildEntities;
            objectTableId = ObjectTableRepository.GetObjectTableByName(childEntitiesCustomFieldArgs.ObjectTableName);
            childObjectTableId = ObjectTableRepository.GetObjectTableByName(childEntitiesCustomFieldArgs.ChildObjectTableName);
            entityId = childEntitiesCustomFieldArgs.EntityId;
            childEntityId = childEntitiesCustomFieldArgs.ChildEntityId;
            tenant = childEntitiesCustomFieldArgs.Tenant;
            childObjectTableName = childEntitiesCustomFieldArgs.ChildObjectTableName;
            customFieldResolver = new CustomFieldResolver(tenant);
            childEntitiesCustomFieldRepository = new Repository<ChildEntitiesCustomField>(tenant);
            customObjectFields = GetCustomObjectFields();
            childEntitiesCustomFields = GetChildEntitiesCustomFields();
        }
        public void Set(ChildEntitiesCustomFieldArgs childEntitiesCustomFieldArgs)
        {
        }
        public void Update(ChildEntitiesCustomFieldArgs childEntitiesCustomFieldArgs)
        {
        }
        private void RemoveUnusedChildEntitiesCustomFields()
        {
            deletedChildEntities = GetDeletedChildEntities();
            if (deletedChildEntities == null || deletedChildEntities.Count() == 0) return;
            foreach (object childEntity in deletedChildEntities)
            {
                RemoveChildEntitiesCustomField(childEntity);
            }
        }
        private void RemoveChildEntitiesCustomField(object childEntity)
        {
            var childEntityId = GetPropertyValue(childEntity, "Id").ToString();
            var childEntitiesCustomField = childEntitiesCustomFields.Where(d => d.ChildEntityId == childEntityId).FirstOrDefault();
            if (childEntitiesCustomField == null) return;
            childEntitiesCustomFieldRepository.Delete(childEntitiesCustomField);
        }
        private List<object> GetDeletedChildEntities()
        {
            deletedChildEntities = new List<object>();
            if (childEntities == null || childEntities.Count() == 0) return deletedChildEntities;
            foreach (object childEntity in childEntities)
            {
                AddDeletedChildEntity(childEntity);
            }
            return deletedChildEntities;
        }
        private void AddDeletedChildEntity(object childEntity)
        {
            if (GetChangeSetOpValue(childEntity) != deleteChangeSetOp) return;
            deletedChildEntities.Add(childEntity);
        }
        private void UpdateModificationChildEntitiesCustomField()
        {
            modificationChildEntities = GetModificationChildEntities();
            if ((modificationChildEntities == null || modificationChildEntities.Count() == 0) || customObjectFields.Count() == 0) return;
            foreach (object childEntity in modificationChildEntities)
            {
                UpdateCustomFieldsValue(childEntity);
            }
        }
        private void UpdateCustomFieldsValue(object childEntity)
        {
            ChildEntitiesCustomField childEntitiesCustomField = GetChildEntitiesCustomField(childEntity);
            foreach (ObjectField customObjectField in customObjectFields)
            {
                SetPropertyValue(childEntitiesCustomField, customObjectField.FieldName, (GetPropertyValue(childEntity, customObjectField.FieldName) as CustomFieldClass)?.Value);
            }

            if (IsNewEntity(childEntitiesCustomField))
            {
                childEntitiesCustomFieldRepository.Insert(childEntitiesCustomField);
            }
            else childEntitiesCustomFieldRepository.Update(childEntitiesCustomField);
        }
        private List<object> GetModificationChildEntities()
        {
            modificationChildEntities = new List<object>();
            if (childEntities == null || childEntities.Count() == 0) return modificationChildEntities;
            foreach (object childEntity in childEntities)
            {
                AddModificationChildEntity(childEntity);
            }
            return modificationChildEntities;
        }
        private void AddModificationChildEntity(object childEntity)
        {
            var changeSetOpValue = GetChangeSetOpValue(childEntity);
            if (changeSetOpValue == deleteChangeSetOp || changeSetOpValue == noneChangeSetOp) return;
            modificationChildEntities.Add(childEntity);
        }
        private string GetChangeSetOpValue(object childEntity)
        {
            var changeSetOp = GetPropertyValue(childEntity, "ChangeSetOp");
            if (changeSetOp == null) return null;

            return changeSetOp.ToString();
        }
        private void SetCustomFieldValues(ObjectField customObjectField)
        {
            foreach (object childEntity in childEntities)
            {
                SetCustomFieldEntityValues(customObjectField, childEntity);
            }
        }
        private void SetCustomFieldEntityValues(ObjectField customObjectField, object childEntity)
        {
            if (childEntity == null) return;
            ChildEntitiesCustomField childEntitiesCustomField = GetChildEntitiesCustomField(childEntity);
            if (childEntitiesCustomField == null) return;
            SetPropertyValue(childEntity, customObjectField.FieldName, GetCustomFieldValue(childEntitiesCustomField, customObjectField));
        }
        private bool IsNewEntity(ChildEntitiesCustomField childEntitiesCustomField)
        {
            return !(childEntitiesCustomFields.Where(d => d.ChildEntityId == childEntitiesCustomField.ChildEntityId).Any());
        }
        private ChildEntitiesCustomField GetChildEntitiesCustomField(object childEntity)
        {
            string childEntityId = GetPropertyValue(childEntity, "Id").ToString();
            var childEntitiesCustomField = childEntitiesCustomFields.Where(d => d.ChildEntityId == childEntityId).FirstOrDefault();
            if (childEntitiesCustomField != null) return childEntitiesCustomField;
            return new ChildEntitiesCustomField()
            {
                Id = IdCounter.GetNumber("ChildEntitiesCustomField", tenant).ToString(),
                Tenant = tenant,
                ObjectTableId = objectTableId,
                ChildEntityId = childEntityId,
                EntityId = entityId,
                ChildObjectTableId = childObjectTableId,
            };
        }

        private object GetCustomFieldValue(object entity, ObjectField objectField)
        {
            var propertyValue = GetPropertyValue(entity, objectField.FieldName);
            return new CustomFieldClass(objectField.FieldName, childObjectTableName, propertyValue != null ? propertyValue.ToString() : "");

        }

        private List<ObjectField> GetCustomObjectFields()
        {
            return ObjectFieldRepository.GetCustomObjectFieldsByObjectTableName(childObjectTableName, tenant).ToList();
        }

        private List<ChildEntitiesCustomField> GetChildEntitiesCustomFields()
        {
            if (!string.IsNullOrEmpty(childEntityId))
            {
                return childEntitiesCustomFieldRepository.GetMulti(d => d.ChildEntityId == childEntityId && d.ObjectTableId == objectTableId && d.ChildObjectTableId == childObjectTableId && d.Tenant == tenant).ToList();
            }
            return childEntitiesCustomFieldRepository.GetMulti(d => d.EntityId == entityId && d.ObjectTableId == objectTableId && d.ChildObjectTableId == childObjectTableId && d.Tenant == tenant).ToList();
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

