using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPOCOs;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class RatesTableQuery
    {
        private readonly int tenant;
        private readonly IAmitalCloudContext context;
        private readonly Repository<RatesTable> repository;

        public RatesTableQuery(int tenant)
        {
            this.tenant = tenant;
            context = AmitalCloudContext.GetContext(tenant);
            repository = new Repository<RatesTable>(context);
        }

        public LastRate GetLastUpdateByCurrencyCode(string foreignCurrency, string tenantCurrencyId)
        {
            if (IsFullAccountingActivated())
            {
                Currency currency = new Repository<Currency>(context).GetSingle(a => a.Tenant == tenant && a.Code == foreignCurrency);
                if (currency != null)
                {
                    var lastRate = GetLastRecord(currency.Id, tenantCurrencyId);
                    return lastRate;
                }
            }
            return null;
        }

        private bool IsFullAccountingActivated() => new Repository<Tenant>(context).GetSingle(a => a.Id == tenant, a => a.AccountingActivated);

        private LastRate GetLastRecord(string foreignCurrencyId, string baseCurrencyId)
        {
            List<LastRate> myList = repository.GetMulti(r => r.Tenant == tenant && r.ForeignCurrencyId == foreignCurrencyId && r.BaseCurrencyId == baseCurrencyId, a => new LastRate()
            {
                Id = a.Id,
                Tenant = a.Tenant,
                ValueDate = a.ValueDate,
                Rate = a.Rate,
                ForeignCurrencyId = a.ForeignCurrency.Id,
                ForeignCurrencyCode = a.ForeignCurrency.Code,
                ForeignCurrencyName = a.ForeignCurrency.EnglishName,
                LogDateTime = a.LogDateTime,
                BaseCurrencyId = baseCurrencyId,
            }, "ForeignCurrency");

            LastRate lastRate = myList.OrderByDescending(r => r.LogDateTime).FirstOrDefault();
            if (lastRate != null)
            {
                lastRate.HistoryCount = myList.Count;
            }
            return lastRate;
        }
    }
}