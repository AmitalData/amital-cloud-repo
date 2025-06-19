using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class PackageFeatureQuery
    {
        PackageFeatureRepository repository;



        public PackageFeatureQuery(int tenant)
        {
            repository = new PackageFeatureRepository(tenant);
        }

        public PackageFeatureQuery(PackageFeatureRepository repository)
        {
            this.repository = repository;
        }

        public PackageFeaturePM GetPackageFeatureByPackageAndFeature(string packageCode, string featureId, int tenant)
        {
            return (from a in repository.context.PackageFeatures
                    where a.PackageCode == packageCode && a.FeatureId == featureId && a.Tenant == tenant
                    select new PackageFeaturePM()
                    {
                        FeatureId = a.FeatureId,
                        Id = a.Id,
                        Tenant = a.Tenant,
                        PackageCode = a.PackageCode,
                        FeatureUniqeCode = a.FeatureUniqeCode
                    }).FirstOrDefault();
        }

        public IQueryable<PackageFeaturePM> GetPackageFeaturePMsByTenant(int tenant)
        {
            return (from a in repository.context.PackageFeatures
                    where a.Tenant == tenant
                    select new PackageFeaturePM()
                    {
                        FeatureId = a.FeatureId,
                        Id = a.Id,
                        Tenant = a.Tenant,
                        PackageCode = a.PackageCode,
                        FeatureUniqeCode = a.FeatureUniqeCode
                    });
        }

        public List<PackageFeaturePM> GetPackageFeaturesForPackage(string packageCode, int tenant)
        {
            return (from a in repository.context.PackageFeatures
                    where a.PackageCode == packageCode && a.Tenant == tenant
                    select new PackageFeaturePM()
                    {
                        FeatureId = a.FeatureId,
                        Id = a.Id,
                        Tenant = a.Tenant,
                        PackageCode = a.PackageCode,
                        FeatureUniqeCode = a.FeatureUniqeCode
                    }).ToList();
        }
    }
}