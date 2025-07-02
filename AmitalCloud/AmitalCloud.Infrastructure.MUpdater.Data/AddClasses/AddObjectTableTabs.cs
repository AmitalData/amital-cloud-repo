using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.MUpdater.Data.AddClasses
{
    public class AddObjectTableTabs
    {
        public static void AddObjectTableTab(ObjectTableTabPM objectTableTabDetails,Repository<ObjectTableTab> objectTableTabRepository,Dictionary<string, ObjectTableTab> tenantObjectTableTab, int contextTenant = 0)
        {

            if (tenantObjectTableTab.Keys.Contains(objectTableTabDetails.Code))
            {
                ObjectTableTab objectTableTab = tenantObjectTableTab[objectTableTabDetails.Code];
                objectTableTab.ControlPath = objectTableTabDetails.ControlPath;
                objectTableTab.IndexOrder = objectTableTabDetails.IndexOrder;
                objectTableTab.ObjectTableId = objectTableTabDetails.ObjectTableId;
                objectTableTab.TabNameTextCodeId = objectTableTabDetails.TabNameTextCodeId;
                objectTableTab.TabNameTextCodeCode = objectTableTabDetails.TabNameTextCodeCode;
                objectTableTab.FeatureId = objectTableTabDetails.FeatureId;
                objectTableTab.HtmlComponentName = objectTableTabDetails.HtmlComponentName;
                objectTableTab.HtmlComponentUrl = objectTableTabDetails.HtmlComponentUrl;
                objectTableTab.FeatureUniqeCode = objectTableTabDetails.FeatureUniqeCode;
				objectTableTab.IsLocked = objectTableTabDetails.IsLocked;


				objectTableTabRepository.Update(objectTableTab);
            }
            else
            {
                ObjectTableTab newObjectTableTab = new ObjectTableTab()
                {
                    ObjectTableId = objectTableTabDetails.ObjectTableId,
                    IndexOrder = objectTableTabDetails.IndexOrder,
                    ControlPath = objectTableTabDetails.ControlPath,
                    Code = objectTableTabDetails.Code,
                    Id = IdCounter.GetNumber("ObjectTableTab", contextTenant).ToString(),
                    TabNameTextCodeId = objectTableTabDetails.TabNameTextCodeId,
                    TabNameTextCodeCode = objectTableTabDetails.TabNameTextCodeCode,
                    Tenant = objectTableTabDetails.Tenant,
                    FeatureId = objectTableTabDetails.FeatureId,
                    HtmlComponentName = objectTableTabDetails.HtmlComponentName,
                    HtmlComponentUrl = objectTableTabDetails.HtmlComponentUrl,
                    FeatureUniqeCode = objectTableTabDetails.FeatureUniqeCode,
                    IsLocked = objectTableTabDetails.IsLocked

                };
 
                objectTableTabRepository.Insert(newObjectTableTab);
            }

        }
    }
}