using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class CustomChildObjectQuery
    {
        private IRepository<CustomChildObject> repository;
        private List<ObjectTable> objectTables;
        private int tenant;
        private int numberOfCustomFields = 50;
        public CustomChildObjectQuery(int tenant)
        {
            repository = new Repository<CustomChildObject>(tenant);
            objectTables = new List<ObjectTable>();
            this.tenant = tenant;

        }

        public List<CustomChildObjectPM> GetByParentEntityIdAndParentObjectId(string parentEntityId, string parentObjectTableId, int tenant)
        {
            List<CustomChildObject> customChildObjects = repository.GetMulti(a => a.Tenant == tenant && a.ParentEntityId == parentEntityId && a.ParentObjectTableId == parentObjectTableId);
            List<CustomChildObjectPM> results = new List<CustomChildObjectPM>();
            foreach (CustomChildObject customChildObject in customChildObjects)
            {
                results.Add(MapCustomChildObjectToCustomChildObjectPM(customChildObject, new CustomChildObjectPM()));
            }
            return results;


        }

        private CustomChildObjectPM MapCustomChildObjectToCustomChildObjectPM(CustomChildObject customChildObject, CustomChildObjectPM customChildObjectPM)
        {
            customChildObjectPM.Id = customChildObject.Id;
            customChildObjectPM.CreateDate = customChildObject.CreateDate;
            customChildObjectPM.CreatedBy = customChildObject.CreatedBy;
            customChildObjectPM.UpdatedBy = customChildObject.UpdatedBy;
            customChildObjectPM.UpdateDate = customChildObject.UpdateDate;
            customChildObjectPM.ObjectTableId = customChildObject.ObjectTableId;
            customChildObjectPM.ParentEntityId = customChildObject.ParentEntityId;
            customChildObjectPM.ParentObjectTableId = customChildObject.ParentObjectTableId;
            customChildObjectPM.Tenant = customChildObject.Tenant;
            for (int i = 1; i <= numberOfCustomFields; i++)
            {
                MapCustomFieldValue(("Field" + i.ToString()), customChildObject, customChildObjectPM);
            }

            return customChildObjectPM;
        }

        private void MapCustomFieldValue(string fieldName, CustomChildObject customChildObject, CustomChildObjectPM customChildObjectPM)
        {
            object customFieldValue = GetFieldValueByName(customChildObject, fieldName);
            if (customFieldValue == null || string.IsNullOrEmpty(customFieldValue.ToString())) return;

            var objectTable = GetObjectTableById(customChildObject.ObjectTableId, customChildObject.Tenant, objectTables);
            if (objectTable == null) return;
            CustomFieldClass customFieldClass = new CustomFieldClass(fieldName, objectTable.Name, customFieldValue.ToString());
            SetFieldValue(customChildObjectPM, fieldName, customFieldClass);
        }



        private ObjectTable GetObjectTableById(string objectTableId, int tenant, List<ObjectTable> objectTables)
        {
            var objectTable = objectTables.Where(d => d.Id == objectTableId).FirstOrDefault();
            if (objectTable != null) return objectTable;
            objectTable = ObjectTableRepository.GetSingleObjectTableById(objectTableId, tenant);
            objectTables.Add(objectTable);
            return objectTable;


        }

        private object GetFieldValueByName(CustomChildObject customChildObject, string fieldName)
        {
            PropertyInfo property = customChildObject.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null) return null;
            return property.GetValue(customChildObject, null);
        }

        private void SetFieldValue(CustomChildObjectPM customChildObjectPM, string fieldName, object fieldValue)
        {
            PropertyInfo propertyInfo = customChildObjectPM.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null) return;
            propertyInfo.SetValue(customChildObjectPM, fieldValue, null);
        }


    }
}
