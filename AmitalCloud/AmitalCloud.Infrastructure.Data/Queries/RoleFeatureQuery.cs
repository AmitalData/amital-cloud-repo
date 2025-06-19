using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using System.Collections.Generic;
using System.Linq;


namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class RoleFeatureQuery
    {
        RoleFeatureRepository repository;



        public RoleFeatureQuery(int tenant)
        {
            repository = new RoleFeatureRepository(tenant);
        }

        public RoleFeatureQuery(RoleFeatureRepository repository)
        {
            this.repository = repository;
        }

        public RoleFeaturePM GetRoleFeatureByRoleAndFeature(string roleId, string featureId, int tenant)
        {
            return (from a in repository.context.RoleFeatures
                    where a.RoleId == roleId && a.FeatureId == featureId && a.Tenant == tenant
                    select new RoleFeaturePM()
                    {
                        FeatureId = a.FeatureId,
                        Id = a.Id,
                        Tenant = a.Tenant,
                        RoleId = a.RoleId,
                        FeatureAccessLevelCode = a.FeatureAccessLevelCode,
                        FeatureUniqeCode = a.FeatureUniqeCode
                    }).FirstOrDefault();
        }

        public IQueryable<RoleFeaturePM> GetRoleFeaturePMsByTenant(int tenant)
        {
            return (from a in repository.context.RoleFeatures
                    where a.Tenant == tenant
                    select new RoleFeaturePM(a)
                    );
        }

        public List<RoleFeaturePM> GetRoleFeaturesForRole(string roleId, int tenant)
        {
            return (from a in repository.context.RoleFeatures
                    where a.RoleId == roleId && a.Tenant == tenant
                    select new RoleFeaturePM(a)).ToList();
        }
        public List<RoleFeature> GetRoleFeaturesForRoleFromCache(string roleId, int tenant)
        {
            string entityName = "RoleFeature" + roleId + tenant;
            List<RoleFeature> entity;
            bool getFromCache = true;

            if (getFromCache)
            {

                if (CacheManager.CacheWrapper.Get(entityName) == null)
                {
                    entity = (from a in repository.context.RoleFeatures
                              where (a.Tenant == tenant || a.Tenant == 0) && a.RoleId == roleId
                              select a).ToList();
                    if (entity != null)
                    {
                        CacheManager.CacheWrapper.Insert(entityName, entity);
                    }
                }
                else
                {
                    entity = (List<RoleFeature>)CacheManager.CacheWrapper.Get(entityName);
                }


            }
            else
            {
                entity = (from a in repository.context.RoleFeatures
                          where (a.Tenant == tenant || a.Tenant == 0) && a.RoleId == roleId
                          select a).ToList();

            }
            return entity;

        }
    }
}