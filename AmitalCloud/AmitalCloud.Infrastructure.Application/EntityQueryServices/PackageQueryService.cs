using AmitalCloud.Infrastructure.Application.CloseTables;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.Constants;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;


namespace AmitalCloud.Infrastructure.Application.EntityQueryServices
{
    public partial class PackagesQueryService
    {
        private readonly PackageManager _state;

        private readonly IRepository<TenantManagement> _tenantRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<PackageConnectedPackage> _connectedRepo;
        private readonly IRepository<UserLicense> _userLicenseRepo;
        private readonly IRepository<Package> _packageRepo;

 
        public PackagesQueryService(PackageManager state)
        {
            _state = state;

            _tenantRepo = new Repository<TenantManagement>(EnvironmentConstants.GlobalTenantId); 
            _userRepo = new Repository<User>(_state.Tenant);
            _connectedRepo = new Repository<PackageConnectedPackage>(_state.Tenant);
            _userLicenseRepo = new Repository<UserLicense>(_state.Tenant);
            _packageRepo = new Repository<Package>(_state.Tenant);

        }

        public void GetAllPackagesByTenant()
        {
            try
            {
            LoadTenantConfiguration();
            LoadUserPreferences();
            ResolveEffectivePackageAccess();
            AddConnectedPackages(_state.AddonsPackagesCodes);
            }
            
                 catch (Exception ex)
            {
 
                throw new ApplicationException("An error occurred while loading tenant packages.", ex);
             
            }
         
        }


        private void LoadTenantConfiguration()
        {
            try
            {
                NetCommonHelper.Logger.DevLog.Instance.WriteDebug($"[LoadTenantConfiguration] Starting to load tenant configuration for tenant: {_state.Tenant}");

                var tenant = _tenantRepo.GetSingle(
                    predicate: t => t.Id == _state.Tenant,
                    t => t.TenantAddOns,
                    t => t.TenantManagementLicenses);

                if (tenant == null)
                {
                    NetCommonHelper.Logger.DevLog.Instance.WriteDebug($"[LoadTenantConfiguration] Tenant not found for ID: {_state.Tenant}");
                    return;
                }

                _state.MainPackageCode = IsTemporalActive(tenant)
                    ? tenant.TemporalPackageCode
                    : tenant.PackageCode;

                NetCommonHelper.Logger.DevLog.Instance.WriteDebug($"[LoadTenantConfiguration] MainPackageCode set to: {_state.MainPackageCode}");

                _state.IsMultiPackage = tenant.IsMultiPackage;
                _state.MainAdditionalPackageApplied = tenant.MainAdditionalPackageApplied;

                _state.AddonsPackagesCodes = tenant.TenantAddOns
                    .Select(a => a.PackageCode)
                    .Distinct()
                    .ToList();

                _state.AdditionalPackagesCodes = tenant.TenantManagementLicenses
                    .Select(l => l.PackageCode)
                    .Distinct()
                    .ToList();

                NetCommonHelper.Logger.DevLog.Instance.WriteDebug($"[LoadTenantConfiguration] Loaded AddonsPackagesCodes: {string.Join(", ", _state.AddonsPackagesCodes)}");
                NetCommonHelper.Logger.DevLog.Instance.WriteDebug($"[LoadTenantConfiguration] Loaded AdditionalPackagesCodes: {string.Join(", ", _state.AdditionalPackagesCodes)}");
                NetCommonHelper.Logger.DevLog.Instance.WriteDebug($"[LoadTenantConfiguration] Configuration loaded successfully for tenant: {_state.Tenant}");
            }
            catch (Exception ex)
            {
                NetCommonHelper.Logger.DevLog.Instance.WriteFatal( ex, $"[LoadTenantConfiguration] Error loading tenant configuration for tenant: {_state.Tenant}. Exception: {ex.Message}");
                throw;
            }
        }


        private void LoadUserPreferences()
        {
            var user = _userRepo.GetSingle(u => u.Id == _state.LoggedUserId);
            _state.IsUserAdditionalOnly = user?.AdditionalPackagesOnly ?? false;
        }

        private void ResolveEffectivePackageAccess()
        {
            if (_state.MainAdditionalPackageApplied || _state.IsMultiPackage)
            {
                ResolveMultiplePackages(_state.MainAdditionalPackageApplied);
            }
            else
            {
                ResolveSinglePackage();
            }
        }

        private void ResolveMultiplePackages(bool includeMainPackage)
        {
            var packages = _state.IsCustomerCare
                ? _state.AdditionalPackagesCodes
                : FilterUserLicensed(_state.AdditionalPackagesCodes);

            if (includeMainPackage &&
                !_state.IsUserAdditionalOnly &&
                !packages.Contains(_state.MainPackageCode))
            {
                packages.Add(_state.MainPackageCode);
            }

            AddConnectedPackages(packages);
        }

        private void ResolveSinglePackage()
        { 
            var pkg = _packageRepo.GetSingle(p => p.Code == _state.MainPackageCode);
            if (pkg == null) return;

            if (pkg.FeaturePackageTypeCode == FeaturePackageTypeValues.Base)
            {
                _state.BasePackagesCodes.Add(pkg.Code);
            }
            else
            {
                AddConnectedPackages(new() { pkg.Code });
            }
        }

        private void AddConnectedPackages(List<string> source)
        {
            if (source == null || source.Count == 0) return;

            var connected = _connectedRepo
                .GetMulti(cp => source.Contains(cp.PackageCode))
                .Select(cp => cp.ConnectedPackageCode)
                .Distinct();

                _state.BasePackagesCodes.UnionWith(connected);
            
        }

        private List<string> FilterUserLicensed(List<string> packages)
        {
            if (_state.IsCustomerCare || string.IsNullOrEmpty(_state.LoggedUserId)) return packages;

            var userLicenses = _userLicenseRepo
                .GetMulti(l =>
                    l.Tenant == _state.Tenant &&
                    (string.IsNullOrEmpty(_state.LoggedUserId) || l.UserId == _state.LoggedUserId))
                .Select(l => l.PackageCode)
                .Distinct()
                .ToList();

            return packages.Intersect(userLicenses).ToList();
        }

        private bool IsTemporalActive(TenantManagement tenant) =>
            !string.IsNullOrEmpty(tenant.TemporalPackageCode) &&
            tenant.TemporalStartDate <= DateTime.Now &&
            tenant.TemporalEndDate >= DateTime.Now;
    }


}