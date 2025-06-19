using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class CardContactAdditionalServiceQuery
    {
        IRepository<CardContactAdditionalService> repository;

        public CardContactAdditionalServiceQuery(int tenant)
        {
            repository = new Repository<CardContactAdditionalService>(tenant);
        }
        public CardContactAdditionalServiceQuery(IRepository<CardContactAdditionalService> repository)
        {
            this.repository = repository;
        }
        public CardContactAdditionalServicePM GetSinglePM(string id, int tenant)
            => repository.GetMulti(a => a.Tenant == tenant && a.Id == id, a => new CardContactAdditionalServicePM(a)
            {
                //AdditionalServiceName = a.AdditionalService != null ? a.AdditionalService.Name : null,
            }, "AdditionalService").FirstOrDefault();
        public List<CardContactAdditionalServicePM> GetCardContactAdditionalServicePMsByCardContactId(string CardContactId, int tenant)
            => repository.GetMulti(a => a.Tenant == tenant && a.CardContactId == CardContactId, a => new CardContactAdditionalServicePM(a)
            {
                //AdditionalServiceName = a.AdditionalService != null ? a.AdditionalService.Name : null,
            }, "AdditionalService").ToList();
    }
}
