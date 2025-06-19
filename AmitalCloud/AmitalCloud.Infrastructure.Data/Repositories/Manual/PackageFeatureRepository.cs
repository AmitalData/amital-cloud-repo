using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class PackageFeatureRepository : Repository<PackageFeature>
    {
        IAmitalCloudContext currentContext;

        public PackageFeatureRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }
        public PackageFeatureRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public PackageFeature GetSinglePackageFeature(string id)
        {
            return GetSingle(a => a.Id == id);
        }
        public PackageFeature GetSinglePackageFeatureByPackageAndFeature(string packageCode, string featureId, int tenant)
        {
            return GetSingle(a => a.PackageCode == packageCode && a.FeatureId == featureId && a.Tenant == tenant);
        }
        public PackageFeature GetSinglePackageFeatureByPackageAndFeatureUCode(string packageCode, string featureUniqeCode, int tenant)
        {
            return GetSingle(a => a.PackageCode == packageCode && a.FeatureUniqeCode == featureUniqeCode && a.Tenant == tenant);
        }
        public IQueryable<PackageFeature> GetPackageFeaturesByTenant(int tenant)
        {
            return (from a in context.PackageFeatures
                    where a.Tenant == tenant
                    select a);
        }
        public void DeleteAllPackageFeaturesByPackage(string packageCode)
        {
            List<PackageFeature> packageFeatures = (from a in context.PackageFeatures
                                                    where a.PackageCode == packageCode
                                                    select a).ToList();
            foreach (PackageFeature packageFeature in packageFeatures)
            {
                this.Delete(packageFeature);
            }
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }
    }
}
