using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class ObjectTableTabQuery
    {
        private readonly int tenantZero = 0;
        private readonly int tenant;
        private readonly IAmitalCloudContext context;
        private readonly Repository<ObjectTableTab> repository;

        public ObjectTableTabQuery(int tenant)
        {
            this.tenant = tenant;
            context = AmitalCloudContext.GetContext(tenant);
            repository = new Repository<ObjectTableTab>(context);
        }

        public List<ObjectTableTabPM> GetObjectTableTabPMsByTenant()
        {
            List<ObjectTableTabPM> tenantZeroTabs = GetTenantZeroTabs();
            if (tenant == tenantZero) return tenantZeroTabs;

            GetEntityChangesFromModification(tenant, tenantZeroTabs);
            var tenantTabs = GetTenantTabs(tenant);

            return tenantZeroTabs.Concat(tenantTabs).ToList();
        }

        private List<ObjectTableTabPM> GetTenantZeroTabs()
        {
            return repository.GetMultiFromCache("GetTenantZeroTabs", a => a.Tenant == 0, "TabNameTextCode,ObjectTable", a => new ObjectTableTabPM(a)
            {
                Type = "Predefined",
                TabNameTextCodeDefaultText = a.TabNameTextCode.DefaultText,
                ObjectTableName = a.ObjectTable.Name,
            });
        }

        private void GetEntityChangesFromModification(int tenant, List<ObjectTableTabPM> tabs)
        {
            Repository<TabModification> tabModificationRepo = new Repository<TabModification>(context);
            Dictionary<string, TabModification> tabsModsDictionary = tabModificationRepo.GetMulti(a => a.Tenant == tenant)
                .Distinct().ToDictionary(dic => dic.TabCode, dic => dic);

            foreach (ObjectTableTabPM tab in tabs)
                MapTabFieldsFromModification(tabsModsDictionary, tab);
        }

        private static void MapTabFieldsFromModification(Dictionary<string, TabModification> tabsModsDictionary, ObjectTableTabPM tab)
        {
            if (!tabsModsDictionary.Keys.Contains(tab.Code))
                return;

            TabModification mod = tabsModsDictionary[tab.Code];

            if (mod == null)
                return;

            tab.Name = mod.Name;
            tab.IndexOrder = mod.IndexOrder;
        }

        public List<ObjectTableTabPM> GetTenantTabs(int tenant)
        {
            Repository<Screen> screenRepo = new Repository<Screen>(context);

            List<ObjectTableTabPM> tabs = (from tab in repository.GetQueryable().Include(a => a.ObjectTable).Include(a => a.TabNameTextCode)
                                           join screen in screenRepo.GetQueryable() on tab.ScreenCode equals screen.Code into screenJoin
                                           from screen in screenJoin.DefaultIfEmpty()
                                           where tab.Tenant == tenant
                                           select new { tab, screen }).AsEnumerable().Select(a => new ObjectTableTabPM(a.tab)
                                           {
                                               TabNameTextCodeDefaultText = a.tab.TabNameTextCode.DefaultText,
                                               Name = a.tab.TabNameTextCode.DefaultText,
                                               ObjectTableName = a.tab.ObjectTable.Name,
                                               ScreenName = a.screen == null ? null : a.screen.Name,
                                           }).ToList();           
            return tabs;
        }
    }
}