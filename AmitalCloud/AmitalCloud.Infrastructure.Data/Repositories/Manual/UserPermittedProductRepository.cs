using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;

using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class UserPermittedProductRepository : Repository<UserPermittedProduct>
    {
        IAmitalCloudContext currentContext;
        public UserPermittedProductRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }

        public UserPermittedProductRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public IQueryable<UserPermittedProduct> GetUserPermittedProducts(int tenant)
        {
            return (from record in context.UserPermittedProducts.Include("User") where record.Tenant == tenant select record);
        }
        public IQueryable<UserPermittedProduct> GetUserPermittedProductesByUserId(string userId, int tenant)
        {
            return (from record in context.UserPermittedProducts.Include("User") where record.UserId == userId && record.Tenant == tenant select record);
        }
        public UserPermittedProduct GetSingleUserPermittedProduct(string id, int tenant)
        {
            return (from record in context.UserPermittedProducts.Include("User") where record.Id == id && record.Tenant == tenant select record).FirstOrDefault();
        }
        public UserPermittedProduct GetUserPermittedProductByUserAndProduct(string userId, string productTypeCode, int tenant)
        {
            return (from record in context.UserPermittedProducts.Include("User") where record.UserId == userId && record.Tenant == tenant && record.ProductTypeCode == productTypeCode select record).FirstOrDefault();
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }
    }
}