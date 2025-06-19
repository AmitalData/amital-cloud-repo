using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace AmitalCloud.Infrastructure.Data.Services
{
    public class CustomChildEntityService
    {
        private CustomChildEntityArgs customChildEntityArgs;
        private string parentObjectTableId = string.Empty;
        //private IAmitalCloudContext amitalCloudContext;
        public CustomChildEntityService(CustomChildEntityArgs customChildEntityArgs)
        {
            this.customChildEntityArgs = customChildEntityArgs;
            parentObjectTableId = customChildEntityArgs.ParentObjectTableId;
            if (!string.IsNullOrEmpty(customChildEntityArgs.ParentObjectTableName))
            {
                parentObjectTableId = new ObjectTableQuery(customChildEntityArgs.Tenant).GetObjectTableIdByName(customChildEntityArgs.ParentObjectTableName);
            }
            //amitalCloudContext = AmitalCloudContext.GetContext(customChildEntityArgs.Tenant);
        }
        public void Update()
        {
            if (string.IsNullOrEmpty(parentObjectTableId)) return;
            List<CustomChildEntity> customChildEntities = GetCustomChildEntities(this.customChildEntityArgs.ParentEntity);
            if (customChildEntities.Count() == 0) return;
            foreach (CustomChildEntity customChildEntity in customChildEntities)
            {
                UpdateCustomChildEntity(customChildEntity);
            }
        }
        public void Set()
        {
            if (string.IsNullOrEmpty(parentObjectTableId)) return;
            var property = customChildEntityArgs.ParentEntity.GetType().GetProperty("CustomChildEntities", BindingFlags.Public | BindingFlags.Instance);
            if (property == null) return;
            property.SetValue(customChildEntityArgs.ParentEntity, BuildCustomChildEntities(), null);
        }
        public List<CustomChildEntity> BuildCustomChildEntities()
        {
            List<CustomChildEntity> customChildEntities = new List<CustomChildEntity>();
            var childObjectTables = new ObjectTableQuery(customChildEntityArgs.Tenant).GetObjectPMsByTenant(customChildEntityArgs.Tenant).Where(d => d.IsCustom && d.ParentObjectTableId == parentObjectTableId).ToList();
            List<CustomChildObjectPM> customChildObjects = new CustomChildObjectQuery(customChildEntityArgs.Tenant).GetByParentEntityIdAndParentObjectId(customChildEntityArgs.ParentEntityId, parentObjectTableId, customChildEntityArgs.Tenant);

            foreach (ObjectTablePM objectTablePM in childObjectTables)
            {
                customChildEntities.Add(new CustomChildEntity()
                {
                    Name = objectTablePM.Name,
                    Values = customChildObjects.Where(d => d.ObjectTableId == objectTablePM.Id).ToList(),
                });
            }
            return customChildEntities;
        }
        private List<CustomChildObjectPM> GetCustomChildObjects(string objectTableId)
        {
            return new List<CustomChildObjectPM>();
        }
        private void UpdateCustomChildEntity(CustomChildEntity customChildEntity)
        {
            if (customChildEntity.Values == null || customChildEntity.Values.Count() == 0) return;
            CustomChildObjectService customChildObjectService = new CustomChildObjectService(customChildEntityArgs.Tenant);
            customChildObjectService.Updates(customChildEntity.Values);
        }
        private List<CustomChildEntity> GetCustomChildEntities(object entity)
        {
            List<CustomChildEntity> customChildEntities = new List<CustomChildEntity>();
            PropertyInfo propertyInfo = entity.GetType().GetProperty("CustomChildEntities");
            if (propertyInfo == null) return customChildEntities;
            object propertyValue = propertyInfo.GetValue(entity, null);
            if (propertyValue != null && propertyValue.GetType() == typeof(List<CustomChildEntity>))
            {
                customChildEntities = (propertyValue as List<CustomChildEntity>);
            }
            return customChildEntities;
        }
    }
    public class CustomChildEntityArgs
    {
        public int Tenant { get; set; }
        public string ParentEntityId { get; set; }
        public string ParentObjectTableName { get; set; }
        public string ParentObjectTableId { get; set; }
        public object ParentEntity { get; set; }
    }
}
