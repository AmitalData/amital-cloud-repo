using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AmitalCloud.Infrastructure.Model.Interfaces;
using System.Linq.Expressions;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;
using Microsoft.Extensions.Logging.Abstractions;
namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class FeatureQuery
    {
        IRepository<Feature> repository;
        IAmitalCloudContext context;
        public FeatureQuery(int tenant)
        {
            context = AmitalCloudContext.GetContext(tenant);
            repository = new Repository<Feature>(context);
        }
        public List<FeaturePM> GetAllowedFeaturesForRole(string myRoleId, List<string> allowedPackages, int tenant)
        {
            List<FeaturePM> myResult = new List<FeaturePM>();

            if (!string.IsNullOrEmpty(myRoleId))
            {
                Role myRole = (from d in context.Roles where d.Id == myRoleId select d).FirstOrDefault();

                List<FeaturePM> allFeatures = new List<FeaturePM>();
                List<RoleFeature> allRoleFeatures = new List<RoleFeature>();
                List<PackageFeature> allPackageFeatures = new List<PackageFeature>();

                #region allFeatures
                var allFeaturesPoco = repository.GetMulti(a => a.Tenant == 0 || a.Tenant == tenant).ToList();

                var config = new MapperConfiguration(cfg => cfg.AddProfile(new FeatureDataMapping()), new NullLoggerFactory());
                var mapper = config.CreateMapper();
                allFeatures = mapper.Map<List<FeaturePM>>(allFeaturesPoco);
                #endregion

                #region allRoleFeatures
                RoleFeatureQuery roleFeatureQuery = new RoleFeatureQuery(context.Tenant);

                if (myRole.IsCustomRole)
                {

                    allRoleFeatures = roleFeatureQuery.GetRoleFeaturesForRoleFromCache(myRole.ParentRoleId, tenant);

                    List<RoleFeature> allChildFeatures = roleFeatureQuery.GetRoleFeaturesForRoleFromCache(myRole.Id, tenant);


                    foreach (RoleFeature item in allChildFeatures)
                    {
                        RoleFeature myParentItem = allRoleFeatures.Where(d => d.FeatureUniqeCode == item.FeatureUniqeCode).FirstOrDefault();

                        if (item.IsDeleted)
                        {
                            if (myParentItem != null)
                            {
                                allRoleFeatures.Remove(myParentItem);
                            }
                        }

                        else
                        {
                            if (myParentItem == null)
                            {
                                allRoleFeatures.Add(item);
                            }
                        }
                    }
                }

                else
                {
                    allRoleFeatures = roleFeatureQuery.GetRoleFeaturesForRoleFromCache(myRole.Id, tenant);
                }
                #endregion

                #region allPackageFeatures
                allPackageFeatures = (from a in context.PackageFeatures where allowedPackages.Contains(a.PackageCode) && (a.Tenant == tenant || a.Tenant == 0) select a).ToList();
                #endregion

                List<FeaturePM> allAllowedPackageFeatures = new List<FeaturePM>();

                if (allPackageFeatures.Count > 0)
                {
                    foreach (PackageFeature item in allPackageFeatures)
                    {
                        FeaturePM myFeature = allFeatures.Where(f => f.FeatureUniqeCode == item.FeatureUniqeCode).FirstOrDefault();
                        if (myFeature != null)
                        {
                            if (!allAllowedPackageFeatures.Where(d => d.Id == myFeature.Id).Any())
                            {
                                myFeature.PackageCode = item.PackageCode;
                                allAllowedPackageFeatures.Add(myFeature);
                            }
                        }
                    }
                }

                if (allRoleFeatures.Count > 0)
                {
                    foreach (RoleFeature item in allRoleFeatures)
                    {
                        FeaturePM myFeature = allFeatures.Where(f => f.FeatureUniqeCode == item.FeatureUniqeCode).FirstOrDefault();
                        if (myFeature != null)
                        {
                            if (!myResult.Where(d => d.Id == myFeature.Id).Any())
                            {
                                myFeature.RoleId = item.RoleId;

                                if (myFeature.Packagable)
                                {
                                    FeaturePM myAllowedPackageFeature = allAllowedPackageFeatures.Where(d => d.Id == myFeature.Id).FirstOrDefault();
                                    if (myAllowedPackageFeature != null)
                                    {
                                        myFeature.Exists = true;
                                        myFeature.PackageCode = myAllowedPackageFeature.PackageCode;
                                        myResult.Add(myFeature);
                                    }
                                }
                                else
                                {
                                    myFeature.Exists = true;
                                    myResult.Add(myFeature);
                                }
                            }
                        }
                    }
                }

                List<FeaturePM> allModuleFeatures = (from a in allAllowedPackageFeatures
                                                     where a.FeatureTypeCode == "MODL"
                                                     select a).ToList();

                foreach (FeaturePM item in allModuleFeatures)
                {
                    if (!myResult.Where(d => d.Id == item.Id).Any())
                    {
                        //item.Exists = true;
                        myResult.Add(item);
                    }
                }
            }

            return myResult;
        }
  
        public LoggedUserFeatures GetAllowedFeaturesForLoggedUser(string loggedUserId, int tenant)
        {
            LoggedUserFeatures loggedUserFeatures;
            if (!SettingUtil.DeploymentStage.IsDBStage(SettingUtil.DeploymentStage.LogboxAndAccountingProduction))
            {
                string key = $"GetAllowedFeaturesForLoggedUser,{loggedUserId},{tenant}";
                loggedUserFeatures = CacheManager.GetOrInsertNewObject<LoggedUserFeatures>(key, () =>
                {
                    return GetAllowedFeaturesForLoggedUserBL(loggedUserId, tenant);
                });
            }
            else
            {
                loggedUserFeatures = GetAllowedFeaturesForLoggedUserBL(loggedUserId, tenant);
            }
            return loggedUserFeatures;
        }
        public LoggedUserFeatures GetAllowedFeaturesForLoggedUserBL(string loggedUserId, int tenant)
        {


 

            LoggedUserFeatures myResult = new LoggedUserFeatures();
            bool isDistributor = false;
            bool isCustomerCare = false;
            List<FeaturePM> allFeatures = new List<FeaturePM>();
            List<string> allowedPackages = new List<string>();
            ContactTenantQuery contactTenantQuery = new ContactTenantQuery(tenant);
            ContactTenantPM contactTenant = contactTenantQuery.GetContactTenantForUser(loggedUserId, tenant);
            if (contactTenant == null)
            {
                contactTenant = contactTenantQuery.GetContactTenantForUser(loggedUserId, 0);
                UserQuery userQuery = new UserQuery(0);
                UserPM user = userQuery.GetSinglePM(loggedUserId, 0);
                isDistributor = user.IsDistributor;
                isCustomerCare = !user.IsDistributor;
            }

            else if (contactTenant.TenantId == 0)
            {
                UserQuery userQuery = new UserQuery(0);
                UserPM user = userQuery.GetSinglePM(contactTenant.ContactId, 0);
                isDistributor = user.IsDistributor;
                isCustomerCare = !user.IsDistributor;
            }
            List<ContactTenantRolePM> contactTenantRoles = new List<ContactTenantRolePM>();
            ContactTenantRoleQuery contactTenantRoleQuery = new ContactTenantRoleQuery(tenant);
            if (isCustomerCare || isDistributor)
            {
                contactTenantRoles = contactTenantRoleQuery.GetContactTenantRolesForContactTenant(contactTenant.Id, 0);
            }
            else
            {
                contactTenantRoles = contactTenantRoleQuery.GetContactTenantRolesForContactTenant(contactTenant.Id, tenant);
            }
            if (contactTenantRoles.Count > 0)
            {
                List<string> allRolesIds = contactTenantRoles.Select(s => s.RoleId).ToList();
                List<Role> allCustomRoles = new Repository<Role>(context)
                    .GetMulti(a => allRolesIds.Contains(a.Id) && a.IsCustomRole == true);
                foreach (Role item in allCustomRoles)
                {
                    if (allRolesIds.Contains(item.ParentRoleId))
                    {
                        allRolesIds.Remove(item.ParentRoleId);
                    }
                }
                var isTenantZeroAccess = isCustomerCare || isDistributor ? true : false;
                allowedPackages = this.GetAllPackagesCodes(loggedUserId, tenant, isTenantZeroAccess);
                foreach (string myRoleId in allRolesIds)
                {
                    List<FeaturePM> myFeatures = this.GetAllowedFeaturesForRole(myRoleId, allowedPackages, tenant);
                    foreach (FeaturePM feature in myFeatures)
                    {
                        if (!allFeatures.Contains(feature))
                        {
                            allFeatures.Add(feature);
                        }
                    }
                }
            }
            myResult.Features = allFeatures;
            myResult.AllowedPackagesCodes = allowedPackages;
            return myResult;
        }
        private List<string> GetAllPackagesCodes(string loggedUserId, int tenant, bool isCustomerCare)
        {
            PackagesCodesManager iManager = new PackagesCodesManager(tenant, loggedUserId, isCustomerCare);
            return iManager.BasePackagesCodes;
        }
    }
    public class LoggedUserFeatures
    {
        public List<FeaturePM> Features { get; set; }
        public List<string> AllowedPackagesCodes { get; set; }
    }

    public class PackagesCodesManager
    {
        public int Tenant { get; set; }
        public string LoggedUserId { get; set; }
        public bool IsCustomerCare { get; set; }
        public string MainPackageCode { get; set; }
        public bool IsMultiPackage { get; set; }
        public bool MainAdditionalPackageApplied { get; set; }
        private bool IsUserAdditionalPackagesOnly { get; set; }
        public List<string> BasePackagesCodes { get; set; }
        public List<string> AdonsPackagesCodes { get; set; }
        public List<string> AdditionalPackagesCodes { get; set; }

        private TenantManagement tenantManagement;
        private IAmitalCloudContext iCommonContext;
        public PackagesCodesManager(int tenant, string loggedUserId, bool isCustomerCare)
        {
            this.Tenant = tenant;
            this.LoggedUserId = loggedUserId;
            this.IsCustomerCare = isCustomerCare;
            this.BasePackagesCodes = new List<string>();
            this.AdonsPackagesCodes = new List<string>();
            this.AdditionalPackagesCodes = new List<string>();
            this.iCommonContext = AmitalCloudContext.GetContext(this.Tenant);
            this.GetGlobalData();
            this.GetUserData();
            this.BuildPackages();
        }

        private void GetGlobalData()
        {
            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                IGlobalContext globalContext = GlobalContext.GetContext();

                this.tenantManagement = (from d in globalContext.TenantManagements where d.Id == this.Tenant select d).FirstOrDefault();

                this.AdonsPackagesCodes = (from d in globalContext.TenantAddOns
                                           where d.Tenant == this.Tenant
                                           group d by d.PackageCode into g
                                           select g.Key).ToList();

                this.AdditionalPackagesCodes = (from d in globalContext.TenantManagementLicenses
                                                where d.Tenant == this.Tenant
                                                group d by d.PackageCode into g
                                                select g.Key).ToList();
                this.GetTenantManagementData();

                scope.Complete();
            }
        }

        private void GetTenantManagementData()
        {
            if (this.tenantManagement != null)
            {
                this.Tenant = this.tenantManagement.Id;
                this.MainPackageCode = this.tenantManagement.PackageCode;
                this.IsMultiPackage = this.tenantManagement.IsMultiPackage;
                this.MainAdditionalPackageApplied = this.tenantManagement.MainAdditionalPackageApplied;

                if (!string.IsNullOrEmpty(this.tenantManagement.TemporalPackageCode) && this.tenantManagement.TemporalStartDate != null && this.tenantManagement.TemporalEndDate != null)
                {
                    if (this.tenantManagement.TemporalStartDate.Value.Date <= DateTime.Now.Date && DateTime.Now.Date <= this.tenantManagement.TemporalEndDate.Value.Date)
                    {
                        this.MainPackageCode = this.tenantManagement.TemporalPackageCode;
                    }
                }
            }
        }

        private void GetUserData()
        {
            User loggedUser = this.iCommonContext.Users.Where(d => d.Id == this.LoggedUserId).FirstOrDefault();
            if (loggedUser != null)
            {
                this.IsUserAdditionalPackagesOnly = loggedUser.AdditionalPackagesOnly;
            }
        }

        private void BuildPackages()
        {
            if (this.MainAdditionalPackageApplied)
            {
                this.BuildPackagesFromMainAdditional();
            }

            else
            {
                this.BuildPackagesFromSingleMulti();
            }

            this.FillBasePackages(this.GetConnectedPackages(this.AdonsPackagesCodes));
        }

        public void BuildPackagesFromMainAdditional()
        {
            List<string> allCodes = new List<string>();

            if (this.IsMultiPackage)
            {
                allCodes.AddRange(this.AdditionalPackagesCodes);

                if (!this.IsCustomerCare)
                {
                    allCodes = this.FilterPackagesUserLicenses(allCodes);
                }
            }

            if (!this.IsUserAdditionalPackagesOnly && !allCodes.Contains(this.MainPackageCode))
            {
                allCodes.Add(this.MainPackageCode);
            }

            this.FillBasePackages(this.GetConnectedPackages(allCodes));
        }

        public void BuildPackagesFromSingleMulti()
        {
            if (this.IsMultiPackage)
            {
                if (this.AdditionalPackagesCodes.Count > 0)
                {
                    if (!this.IsCustomerCare)
                    {
                        AdditionalPackagesCodes = this.FilterPackagesUserLicenses(AdditionalPackagesCodes);
                    }
                }

                this.FillBasePackages(this.GetConnectedPackages(this.AdditionalPackagesCodes));
            }

            else
            {
                Package myPackage = (from a in iCommonContext.Packages where a.Code == this.MainPackageCode select a).FirstOrDefault();
                if (myPackage != null)
                {
                    if (myPackage.FeaturePackageTypeCode == "BS")
                    {
                        if (!this.BasePackagesCodes.Contains(this.MainPackageCode))
                        {
                            this.BasePackagesCodes.Add(this.MainPackageCode);
                        }
                    }

                    else
                    {
                        this.FillBasePackages(this.GetConnectedPackages(new List<string>() { this.MainPackageCode }));
                    }
                }
            }
        }

        private List<string> FilterPackagesUserLicenses(List<string> iPackagesCodes)
        {
            List<string> allUserLicenses = new List<string>();

            if (string.IsNullOrEmpty(this.LoggedUserId))
            {
                allUserLicenses = (from a in iCommonContext.UserLicenses
                                   where a.Tenant == this.Tenant
                                   group a by a.PackageCode into g
                                   select g.Key).ToList();
            }

            else
            {
                allUserLicenses = (from a in iCommonContext.UserLicenses
                                   where a.Tenant == this.Tenant
                                   && a.UserId == this.LoggedUserId
                                   group a by a.PackageCode into g
                                   select g.Key).ToList();
            }

            iPackagesCodes = (from a in iPackagesCodes where allUserLicenses.Contains(a) select a).ToList();

            return iPackagesCodes;
        }

        private List<string> GetConnectedPackages(List<string> iPackagesCodes)
        {
            List<string> iConnectedCodes = new List<string>();

            if (iPackagesCodes.Count > 0)
            {
                iConnectedCodes = (from a in iCommonContext.PackageConnectedPackages
                                   where iPackagesCodes.Contains(a.PackageCode)
                                   group a by a.ConnectedPackageCode into g
                                   select g.Key).ToList();
            }

            return iConnectedCodes;
        }

        private void FillBasePackages(List<string> iPackagesCodes)
        {
            foreach (string itemCode in iPackagesCodes)
            {
                if (!this.BasePackagesCodes.Contains(itemCode))
                {
                    this.BasePackagesCodes.Add(itemCode);
                }
            }
        }
    }
}
