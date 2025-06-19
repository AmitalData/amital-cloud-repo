using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;

namespace AmitalCloud.Infrastructure.Data.DataMapping
{
    public class DocumentsFilingMetaDataValueMapping
    {
        public static void MapEntity(DocumentsFilingMetaDataValuePM entityPM, DocumentsFilingMetaDataValue entityPOCO, bool isNewState)
        {
            if (isNewState)
            {
                entityPOCO.Id = entityPM.Id;
                entityPOCO.Tenant = entityPM.Tenant;
                entityPOCO.DocumentsFilingId = entityPM.DocumentsFilingId;
            }

            entityPOCO.DocumentsMetaDataTypeId = entityPM.DocumentsMetaDataTypeId;
            entityPOCO.MetaDataValue = entityPM.MetaDataValue;


        }
    }

}
