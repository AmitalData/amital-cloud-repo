using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class CardContactProductQuery
    {
        IRepository<CardContactProduct> repository;
        IAmitalCloudContext context;

        public CardContactProductQuery(int tenant)
        {
            context = AmitalCloudContext.GetContext(tenant);
            repository = new Repository<CardContactProduct>(context);
        }

        public CardContactProductQuery(IRepository<CardContactProduct> CardContactProductRepository)
        {
            repository = CardContactProductRepository;
        }

        public CardContactProductPM GetSinglePM(string id, int tenant)
        {
            CardContactProductPM entity = (from a in context.CardContactProducts.Include("ProductType")
                                           where a.Tenant == tenant && a.Id == id
                                           select new CardContactProductPM(a)
                                           {
                                               //ProductTypeName = a.ProductType != null ? a.ProductType.Name : null,
                                           }).FirstOrDefault();

            return entity;
        }

        public List<CardContactProductPM> GetCardContactProductPMsByCardContactId(string CardContactId, int tenant)
        {
            List<CardContactProductPM> result =
                (from a in context.CardContactProducts.Include("ProductType")
                 where a.Tenant == tenant && a.CardContactId == CardContactId
                 select new CardContactProductPM(a)
                 {
                     //ProductTypeName = a.ProductType != null ? a.ProductType.Name : null,
                 }).ToList();

            return result;
        }
    }
}
