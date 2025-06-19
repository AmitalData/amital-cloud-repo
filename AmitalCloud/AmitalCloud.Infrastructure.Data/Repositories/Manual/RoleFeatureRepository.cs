using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class RoleFeatureRepository : Repository<RoleFeature>
    {
        IAmitalCloudContext currentContext;

        public RoleFeatureRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }
        public RoleFeatureRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public RoleFeature GetSingleRoleFeature(string id)
        {
            return GetMulti(a => a.Id == id).FirstOrDefault();
        }
        public RoleFeature GetRoleFeatureByRoleAndFeature(string roleId, string featureId, int tenant)
        {
            return GetMulti(a => a.RoleId == roleId && a.FeatureId == featureId && a.Tenant == tenant).FirstOrDefault();
        }
        public RoleFeature GetRoleFeatureByRoleAndFeatureUCode(string roleId, string FeatureUniqeCode, int tenant)
        {
            return GetMulti(a => a.RoleId == roleId && a.FeatureUniqeCode == FeatureUniqeCode && a.Tenant == tenant).FirstOrDefault();
        }
        public RoleFeature GetBusinessUnitFilterRoleFeature(string myRoleId, string myFeatureId, int tenant)
        {
            RoleFeature myResult = null;
            Role myRole = (from d in context.Roles where d.Id == myRoleId select d).FirstOrDefault();
            if (myRole != null)
            {
                if (myRole.IsCustomRole)
                {
                    myResult = (from d in context.RoleFeatures
                                where
                                d.RoleId == myRoleId
                                && d.FeatureId == myFeatureId
                                && (d.Tenant == tenant || d.Tenant == 0)
                                select d).FirstOrDefault();
                    if (myResult != null)
                    {
                        if (myResult.IsDeleted)
                        {
                            myResult = null;
                        }
                    }
                    else
                    {
                        myResult = (from d in context.RoleFeatures
                                    where
                                    d.RoleId == myRole.ParentRoleId
                                    && d.FeatureId == myFeatureId
                                    && (d.Tenant == tenant || d.Tenant == 0)
                                    select d).FirstOrDefault();
                    }
                }
                else
                {
                    myResult = (from d in context.RoleFeatures
                                where
                                d.RoleId == myRoleId
                                && d.FeatureId == myFeatureId
                                && (d.Tenant == tenant || d.Tenant == 0)
                                select d).FirstOrDefault();
                }
            }
            return myResult;
        }
        public IQueryable<RoleFeature> GetRoleFeaturesByTenant(int tenant)
        {
            return (from a in context.RoleFeatures
                    where a.Tenant == tenant
                    select a);
        }
        public List<RoleFeature> GetRoleFeatureByRoleIds(List<string> myRolesIds, int tenant)
        {
            List<RoleFeature> myResult = new List<RoleFeature>();
            if (myRolesIds.Count > 0)
            {
                myResult = (from a in context.RoleFeatures
                            where myRolesIds.Contains(a.RoleId) && (a.Tenant == tenant || a.Tenant == 0)
                            select a).ToList();
            }
            return myResult;
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }
        public void DeleteAllRoleFeaturesByRole(string roleId)
        {
            List<RoleFeature> roleFeatures = (from a in context.RoleFeatures
                                              where a.RoleId == roleId
                                              select a).ToList();
            foreach (RoleFeature roleFeature in roleFeatures)
            {
                this.Delete(roleFeature);
            }
        }
    }
}
