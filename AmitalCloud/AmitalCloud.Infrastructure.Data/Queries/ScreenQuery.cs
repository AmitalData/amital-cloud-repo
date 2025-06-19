using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class ScreenQuery
    {
        private readonly int tenant;
        private readonly IAmitalCloudContext context;
        private readonly Repository<Screen> repository;

        public ScreenQuery(int tenant)
        {
            this.tenant = tenant;
            context = AmitalCloudContext.GetContext(tenant);
            repository = new Repository<Screen>(context);
        }

        public List<ScreenPM> GetScreenPMsByTenant()
        {
            List<ScreenPM> screens;
            List<ScreenPM> zeroscreens;
            List<ScreenPM> currentscreens;

            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                zeroscreens = repository.GetMultiFromCache("GetScreen0", a => a.Tenant == 0, "ObjectTable", a => new ScreenPM(a)
                {
                    ObjectTableName = a.ObjectTable.Name,
                    UserTenant = tenant,
                });

                Dictionary<string, ScreenModification> screensDictionary = new Dictionary<string, ScreenModification>();
                screensDictionary = new Repository<ScreenModification>(context).GetMulti(te => te.Tenant == tenant).ToDictionary(dic => dic.ScreenId, dic => dic);

                foreach (ScreenPM screen in zeroscreens)
                {
                    if (screensDictionary.Keys.Contains(screen.Id))
                    {
                        ScreenModification mod = screensDictionary[screen.Id];
                        if (mod != null)
                        {
                            screen.NumberOfRows = mod.NumberOfRows;
                            screen.NumberOfColumns = mod.NumberOfColumns;
                        }
                    }
                }
            }

            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                currentscreens = repository.GetMultiFromCache($"screens{tenant}", a => a.Tenant == tenant, "ObjectTable", a => new ScreenPM(a)
                {
                    ObjectTableName = a.ObjectTable.Name,
                    UserTenant = tenant,
                });
            }
            screens = zeroscreens.Concat(currentscreens).ToList();

            return screens;
        }
    }
}
