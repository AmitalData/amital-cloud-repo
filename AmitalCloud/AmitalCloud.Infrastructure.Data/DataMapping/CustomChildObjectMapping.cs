using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.DataMapping
{
    public class CustomChildObjectMapping
    {
        private static int numberOfCustomFields = 50;

        public static void MapEntity(CustomChildObjectPM entityPM, CustomChildObject entityPOCO, bool isNewState)
        {
            if (isNewState)
            {
                entityPOCO.Id = entityPM.Id;
                entityPOCO.Tenant = entityPM.Tenant;
            }
            entityPOCO.CreateDate = entityPM.CreateDate;
            entityPOCO.CreatedBy = entityPM.CreatedBy;
            entityPOCO.UpdateDate = entityPM.UpdateDate;
            entityPOCO.UpdatedBy = entityPM.UpdatedBy;
            entityPOCO.ObjectTableId = entityPM.ObjectTableId;
            entityPOCO.ParentEntityId = entityPM.ParentEntityId;
            entityPOCO.ParentObjectTableId = entityPM.ParentObjectTableId;
            MapCustomFields(entityPM, entityPOCO);
        }

        private static void MapCustomFields(CustomChildObjectPM entityPM, CustomChildObject entityPOCO)
        {
            for (int i = 1; i <= numberOfCustomFields; i++)
            {
                MapCustomFieldValue(("Field" + i.ToString()), entityPM, entityPOCO);
            }

        }


        private static void MapCustomFieldValue(string fieldName, CustomChildObjectPM entityPM, CustomChildObject entityPOCO)
        {
            var customFieldValue = GetFieldValue(entityPM, fieldName);
            var fieldProperty = entityPOCO.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (fieldProperty == null) return;
            fieldProperty.SetValue(entityPOCO, customFieldValue, null);

        }


        private static object GetFieldValue(CustomChildObjectPM entityPM, string fieldName)
        {
            var fieldProperty = entityPM.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (fieldProperty == null) return null;

            var customFieldValue = fieldProperty.GetValue(entityPM, null);
            if (customFieldValue != null && customFieldValue.GetType() == typeof(CustomFieldClass))
            {
                CustomFieldClass c = customFieldValue as CustomFieldClass;
                customFieldValue = c.Value;
            }


            return customFieldValue;

        }
    }
}
