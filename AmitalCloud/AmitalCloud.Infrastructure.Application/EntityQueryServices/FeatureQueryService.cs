using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;



namespace AmitalCloud.Infrastructure.Application.EntityQueryServices
{
    public partial class FeatureQueryService
    {


        public List<RoleFeature> GetUserAllowedFeatures(UserPM user, int tenant)
        {
            var repository = new Repository<ContactTenantRole>(tenant);

            var allRoles = repository.GetMulti(
                predicate: ct => ct.ContactTenantId == user.Id &&
                                 ct.Tenant == tenant  &&
                                 ct.Role != null &&
                                 !ct.Role.Inactive,

                select: ct => ct.Role,   

                ct => ct.Role,
                ct => ct.Role.RoleFeatures
            )
            .Select(f => f)
            .Distinct()
            .ToList();

            var customParentIds = allRoles
                .Where(r => r.IsCustomRole && !string.IsNullOrEmpty(r.ParentRoleId))
                .Select(r => r.ParentRoleId)
                .Distinct()
                .ToList();

            var roleFeatures = allRoles
                .Where(r => !customParentIds.Contains(r.Id))
                .SelectMany(r => r.RoleFeatures)
                .Distinct()
                .ToList();



            return roleFeatures;


        }

 

        public List<FeaturePM> GetAllowedFeaturesForRoles(List<RoleFeature> allRoleFeatures, List<string> allowedPackages, int tenant)
        {
            var _roleRepo = new Repository<Role>(tenant);
            var _featureRepo = new Repository<Feature>(tenant);
            var _packageFeatureRepo = new Repository<PackageFeature>(tenant);

            var allFeaturesDict = _featureRepo
                .GetMulti(f => f.Tenant == 0 || f.Tenant == tenant, f => new FeaturePM(f))
                .ToDictionary(f => f.FeatureUniqeCode, f => f);

            var roleFeatureQuery = new RoleFeatureQuery(tenant);

            var roleFeatureDict = allRoleFeatures
                .GroupBy(rf => rf.FeatureUniqeCode)
                .ToDictionary(g => g.Key, g => g.First());

            var allowedPackageFeatures = _packageFeatureRepo
                .GetMulti(pf => allowedPackages.Contains(pf.PackageCode) && (pf.Tenant == tenant || pf.Tenant == 0))
                .ToList();

            var packageFeatureMap = allowedPackageFeatures
                .GroupBy(pf => pf.FeatureUniqeCode)
                .ToDictionary(g => g.Key, g => g.First().PackageCode);

            var allowedFeatures = new List<FeaturePM>();
            var allowedFeatureIds = new HashSet<string>();

            foreach (var (featureCode, roleFeature) in roleFeatureDict)
            {
                if (!allFeaturesDict.TryGetValue(featureCode, out var feature)) continue;

                var mapped = MapFeatureAccess(feature, roleFeature, packageFeatureMap);
                if (!mapped.Exists || !allowedFeatureIds.Add(mapped.Id)) continue;

                allowedFeatures.Add(mapped);
            }

            return allowedFeatures;
        }

    

        private FeaturePM MapFeatureAccess(FeaturePM feature, RoleFeature roleFeature, Dictionary<string, string> packageMap)
        {
            feature.RoleId = roleFeature.RoleId;

            string pkg =string.Empty;

            feature.Exists = !feature.Packagable || packageMap.TryGetValue(feature.FeatureUniqeCode, out pkg);

            if (feature.Exists && feature.Packagable)
                feature.PackageCode = pkg;

            return feature;
        }


        private List<FeaturePM> ApplyFeatureToggles(List<FeaturePM> features, int tenant)
        {
          var  _featureToggleRepo = new Repository<FeatureToggle>(tenant);  

            var toggleCodes = features
                .Where(f => !string.IsNullOrEmpty(f.ToggleCode))
                .Select(f => f.ToggleCode)
                .Distinct()
                .ToList();

            if (toggleCodes.Count == 0) return features;

            var toggles = _featureToggleRepo
                .GetMulti(t => toggleCodes.Contains(t.ToggleCode) && !t.Inactive)
                .ToList();

            var toggleMap = toggles
                .GroupBy(t => t.ToggleCode)
                .ToDictionary(g => g.Key, g => g.ToList());

            return features
                .Where(f =>
                    string.IsNullOrEmpty(f.ToggleCode) ||
                    (toggleMap.TryGetValue(f.ToggleCode, out var toggleList) &&
                     toggleList.Any(t =>
                         t.TenantNumber == tenant ||
                         (tenant >= t.FromTenantNumber && tenant <= t.ToTenantNumber))))
                .ToList();
        }




        public List<FeaturePM> GetAllowedFeaturesForLoggedUser(string email, int tenant)
        {
            try
            {
                var contactQuery = new ContactQuery(tenant);
                var userQuery = new UserQuery(tenant);
                var featureService = new FeatureQueryService(tenant);

                var userId = contactQuery.GetContactByEmailOnly(email, tenant)?.Id;
                if (string.IsNullOrEmpty(userId))
                    return new List<FeaturePM>();

                var user = userQuery.GetSinglePM(userId, tenant);
                var roleIds = featureService.GetUserAllowedFeatures(user, tenant);

                var packageManager = new PackageManager
                {
                    Tenant = tenant,
                    LoggedUserId = userId,
                    IsCustomerCare = user.IsDistributor
                };

                new PackagesQueryService(packageManager).GetAllPackagesByTenant();

                var features = featureService.GetAllowedFeaturesForRoles(roleIds, packageManager.BasePackagesCodes, tenant);
                return featureService.ApplyFeatureToggles(features, tenant);
            }
            catch (Exception ex)
            {
                NetCommonHelper.Logger.DevLog.Instance.WriteError($"Error retrieving features for user {email} in tenant {tenant}: {ex.Message}");
                return new List<FeaturePM>();
            }
        }

    }
}
