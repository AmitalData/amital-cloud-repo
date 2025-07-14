using AmitalCloud.Infrastructure.Data.EntityDataMappings;
using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;



namespace AmitalCloud.Infrastructure.Application.EntityQueryServices
{
    public partial class FeatureQueryService
    {


        public HashSet<RoleFeature> GetUserAllowedFeatures(UserPM user, int tenant)
        {
            var repository = new Repository<ContactTenantRole>(tenant);

            var allRoles = repository.GetMulti(
                predicate: ct => ct.ContactTenantId == user.Id &&
                                 ct.Tenant == tenant &&
                                 ct.Role != null &&
                                 !ct.Role.Inactive,

                select: ct => ct.Role,

                ct => ct.Role,
                ct => ct.Role.RoleFeatures
            )
            .Select(f => f)
            .Distinct();
           

            var customParentIds = allRoles
                .Where(r => r.IsCustomRole && !string.IsNullOrEmpty(r.ParentRoleId))
                .Select(r => r.ParentRoleId)
                .Distinct()
                .ToHashSet();

            var roleFeatures = allRoles
                .Where(r => !customParentIds.Contains(r.Id))
                .SelectMany(r => r.RoleFeatures)
                .Distinct()
                .ToHashSet();



            return roleFeatures;


        }

 

        public HashSet<FeaturePM> GetAllowedFeaturesForRoles(HashSet<RoleFeature> allRoleFeatures, HashSet<string> allowedPackages, int tenant)
        {
            var _roleRepo = new Repository<Role>(tenant);
            var _featureRepo = new Repository<Feature>(tenant);
            var _packageFeatureRepo = new Repository<PackageFeature>(tenant);

            var allFeatures = _featureRepo
                .GetMulti(f => f.Tenant == 0 || f.Tenant == tenant);

            var config = new MapperConfiguration(cfg => cfg.AddProfile(new FeatureDataMapping()), new NullLoggerFactory());
            var mapper = config.CreateMapper();
            var allFeaturesPM =  mapper.Map<List<FeaturePM>>(allFeatures);
            var allFeaturesDict = allFeaturesPM.ToDictionary(f => f.FeatureUniqeCode, f => f);

            var roleFeatureQuery = new RoleFeatureQuery(tenant);

            var roleFeatureDict = allRoleFeatures
                .GroupBy(rf => rf.FeatureUniqeCode)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            var allowedPackageFeatures = _packageFeatureRepo
                .GetMulti(pf => allowedPackages.Contains(pf.PackageCode) && (pf.Tenant == tenant || pf.Tenant == 0))
                .ToHashSet();

            var packageFeatureMap = allowedPackageFeatures
                .GroupBy(pf => pf.FeatureUniqeCode)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault().PackageCode);

            var allowedFeatures = new HashSet<FeaturePM>();
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

            if (!feature.Packagable)
            {
                feature.Exists = true;
                return feature;
            }

            if (packageMap.TryGetValue(feature.FeatureUniqeCode, out var pkg))
            {
                feature.Exists = true;
                feature.PackageCode = pkg;
            }
            else
            {
                feature.Exists = false;
            }

            return feature;
        }



        private HashSet<FeaturePM> ApplyFeatureToggles(HashSet<FeaturePM> features, int tenant)
        {
          var  _featureToggleRepo = new Repository<FeatureToggle>(tenant);  

            var toggleCodes = features
                .Where(f => !string.IsNullOrEmpty(f.ToggleCode))
                .Select(f => f.ToggleCode)
                .Distinct()
                .ToHashSet();

            if (toggleCodes.Count == 0) return features;

            var toggles = _featureToggleRepo
                .GetMulti(t => toggleCodes.Contains(t.ToggleCode) && !t.Inactive)
                .ToHashSet();

            var toggleMap = toggles
                .ToLookup(t => t.ToggleCode)
                .ToDictionary(g => g.Key, g => g.ToList());

            return features
                .Where(f =>
                    string.IsNullOrEmpty(f.ToggleCode) ||
                    (toggleMap.TryGetValue(f.ToggleCode, out var toggleList) &&
                     toggleList.Any(t =>
                         t.TenantNumber == tenant ||
                         (tenant >= t.FromTenantNumber && tenant <= t.ToTenantNumber))))
                .ToHashSet();
        }




        public HashSet<FeaturePM> GetAllowedFeaturesForLoggedUser(string email, int tenant)
        {
            try
            {
                var contactQuery = new ContactQuery(tenant);
                var userQuery = new UserQuery(tenant);
                var featureService = new FeatureQueryService(tenant);

                var userId = contactQuery.GetContactByEmailOnly(email, tenant)?.Id;
                if (string.IsNullOrEmpty(userId))
                    return new HashSet<FeaturePM>();

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
                return new HashSet<FeaturePM>();
            }
        }

    }
}
