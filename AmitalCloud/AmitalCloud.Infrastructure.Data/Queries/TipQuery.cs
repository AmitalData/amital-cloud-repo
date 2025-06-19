using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class TipQuery
    {
        private readonly Repository<Tip> repository;

        public TipQuery(int tenant)
        {
            repository = new Repository<Tip>(AmitalCloudContext.GetContext(tenant));
        }

        public List<Tip> GetTips()
        {
            return repository.GetQueryable().ToList();
        }
    }
}