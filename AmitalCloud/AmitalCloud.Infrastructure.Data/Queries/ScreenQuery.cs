using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
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
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new ScreenDataMapping()), new NullLoggerFactory());
            var mapper = config.CreateMapper();

            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                var zeroscreensPoco = repository.GetMultiFromCache("GetScreen0", a => a.Tenant == 0, "ObjectTable", a => a);
                zeroscreens = mapper.Map<List<ScreenPM>>(zeroscreensPoco);

                Dictionary<string, ScreenModification> screensDictionary = new Dictionary<string, ScreenModification>();
                screensDictionary = new Repository<ScreenModification>(context).GetMulti(te => te.Tenant == tenant).ToDictionary(dic => dic.ScreenId, dic => dic);

                foreach (ScreenPM screen in zeroscreens)
                {
                    screen.UserTenant = tenant;
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
                var currentscreensPoco = repository.GetMultiFromCache($"screens{tenant}", a => a.Tenant == tenant, "ObjectTable", a => a);
                currentscreens = mapper.Map<List<ScreenPM>>(currentscreensPoco);
            }
            screens = zeroscreens.Concat(currentscreens).ToList();

            return screens;
        }
    }
}
