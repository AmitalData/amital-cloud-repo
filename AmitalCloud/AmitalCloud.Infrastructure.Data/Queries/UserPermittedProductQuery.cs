using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class UserPermittedProductQuery
    {
        UserPermittedProductRepository repository;


        public UserPermittedProductQuery(int tenant) : this(new UserPermittedProductRepository(tenant))
        {
        }
        public UserPermittedProductQuery(UserPermittedProductRepository UserPermittedProductRepository)
        {
            repository = UserPermittedProductRepository;
        }

        public UserPermittedProductPM GetSinglePM(string id, int tenant)
        {
            UserPermittedProductPM UserPermittedProduct = (from a in repository.context.UserPermittedProducts
                                                           where a.Id == id && a.Tenant == tenant
                                                           select new UserPermittedProductPM()
                                                           {
                                                               Id = a.Id,
                                                               Tenant = a.Tenant,
                                                               ProductTypeCode = a.ProductTypeCode,
                                                               UserId = a.UserId,
                                                           }).FirstOrDefault();
            return UserPermittedProduct;
        }

        public IQueryable<UserPermittedProductPM> GetUserPermittedProductPMsByTenant(int tenant)
        {
            IQueryable<UserPermittedProductPM> UserPermittedProducts = from a in repository.context.UserPermittedProducts
                                                                       where a.Tenant == tenant
                                                                       select new UserPermittedProductPM()
                                                                       {
                                                                           Id = a.Id,
                                                                           Tenant = a.Tenant,
                                                                           ProductTypeCode = a.ProductTypeCode,
                                                                           UserId = a.UserId,
                                                                       };
            return UserPermittedProducts;
        }

        public IQueryable<UserPermittedProductPM> GetContactFromUserPermittedProductPMsByUserId(string id, int tenant)
        {
            IQueryable<UserPermittedProductPM> UserPermittedProducts = (from a in repository.context.UserPermittedProducts.Include("Contact")
                                                                        where a.UserId == id
                                                                        select new UserPermittedProductPM()
                                                                        {
                                                                            Id = a.Id,
                                                                            Tenant = a.Tenant,
                                                                            ProductTypeCode = a.ProductTypeCode,
                                                                            UserId = a.UserId,
                                                                        });
            return UserPermittedProducts;
        }
    }
}