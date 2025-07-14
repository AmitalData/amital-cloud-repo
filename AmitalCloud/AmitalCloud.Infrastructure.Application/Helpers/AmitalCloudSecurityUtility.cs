using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Security;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Transactions;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AmitalCloud.Infrastructure.Application.Helpers
{
    public class AmitalCloudSecurityUtility
    {
        [ThreadStatic]
        public static bool IsWorkerRoleCall = false;
        public static string GetAuthenticatedUser()
        {
            if (IsWorkerRoleCall && !string.IsNullOrEmpty(AuthenticationUtil.AuthenticatedUserEmail)) //for calling the excel export data from WR 
            {
                return AuthenticationUtil.AuthenticatedUserEmail;
            }
            if (IsWorkerRoleCall && HttpContextHelper.HttpContext == null) //for calling the excel export data from WR 
            {
                ILoggedContactUtil util = new LoggedContactUtil();
                var resolver = new LoggedContactResolver(util);
                var loggedContact = resolver.GetLoggedContact(0);
                return loggedContact?.Email;
            }
            if (HttpContextHelper.HttpContext != null)
            {
                if (!string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
                {
                    return HttpContextHelper.User.Identity.Name;
                }
                else
                {
                    throw new AutenticationException("Sorry! this user is not authorized!");
                }
            }
            throw new AutenticationException("Sorry! this user is not authorized!");
        }
        static ContactInformation contactinfo;
        public static void CheckContactFeature(string objectTableName, string featureCode, int tenant, string overrideEmail = null)
        {
            if (IsWorkerRoleCall && HttpContextHelper.HttpContext == null) //for calling the excel export data from WR 
            {
                return;
            }
            bool exists = false;
            if (HttpContextHelper.HttpContext != null && string.IsNullOrWhiteSpace(overrideEmail))
            {
                overrideEmail = HttpContextHelper.User.Identity.Name;
            }
            if (!string.IsNullOrEmpty(overrideEmail))
            {
                string email = overrideEmail;//HttpContextHelper.User.Identity.Name;
                contactinfo = GetContactInformation(email, tenant);
                if (contactinfo != null)
                {
                    if (contactinfo.IsApi)
                    {
                        exists = true;
                    }
                    else
                    {
                        ObjectTablePM objectTable = ObjectTableQuery.GetObjectTableByCode(objectTableName, tenant);
                        if (objectTable != null)
                        {
                            foreach (string myRoleId in contactinfo.RolesIds)
                            {
                                Dictionary<string, FeaturePM> features = GetFeaturesForRole(myRoleId, contactinfo.PackagesCodes, tenant);
                                if (features.Keys.Contains(featureCode + objectTable.Id))
                                {
                                    FeaturePM feature = features[featureCode + objectTable.Id];
                                    if (feature != null)
                                    {
                                        exists = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!exists)
            {
                bool showLocal = contactinfo != null ? (!contactinfo.DontShowLocalLabels) : false;
                string objectTableLocalName = TextCodesTranslator.TranslateText(objectTableName, tenant, showLocal);
                string error = TextCodesTranslator.TranslateText("Accounting.General.O.YouDontHavePermission", tenant, showLocal);
                throw new AutenticationException(error + " " + objectTableLocalName + ". Please contact your administrator.");
            }
        }
        private static Dictionary<string, FeaturePM> GetFeaturesForRole(string roleId, List<string> allowedPackages, int tenant, bool forceAPIFeaturesCheck = false)
        {
            // Vladi find Cache problem= can't get the features from cache by allowedPackages
            //string key = $"GetFeaturesForRole({roleId}, {tenant})"; 
            //Dictionary<string, FeaturePM> features = (Dictionary<string, FeaturePM>)CacheManager.CacheWrapper.Get(key); 
            //if (features == null || forceAPIFeaturesCheck)
            //{
            FeatureQuery featuresQuery = new FeatureQuery(tenant);
            List<FeaturePM> fet = featuresQuery.GetAllowedFeaturesForRole(roleId, allowedPackages, tenant);
            return fet.ToDictionary(d => d.Code + d.ObjectTableId, d => d);
            //    CacheManager.CacheWrapper.Insert(key, features, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
            //}
            //return features;
        }
        public static bool CheckFeature(string objectTableName, string featureCode, int tenant)
        {
            bool exists = false;
            string email = null;
            if (!string.IsNullOrEmpty(HttpContextHelper.User?.Identity.Name))
            {
                email = HttpContextHelper.User.Identity.Name;
            }
            else if (AuthenticationUtil.AuthenticatedUserEmail != null)
            {
                email = AuthenticationUtil.AuthenticatedUserEmail;
            }
            else if (AuthenticationUtil.AuthenticatedUserEmail != null)
            {
                email = AuthenticationUtil.AuthenticatedUserEmail;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                email = AuthenticationUtil.ResolveLoggingUserId(tenant);
            }
            contactinfo = GetContactInformation(email, tenant);
            if (contactinfo != null)
            {
                ObjectTablePM objectTable = ObjectTableQuery.GetObjectTableByCode(objectTableName, tenant);
                if (objectTable != null)
                {
                    foreach (string myRoleId in contactinfo.RolesIds)
                    {
                        Dictionary<string, FeaturePM> features = GetFeaturesForRole(myRoleId, contactinfo.PackagesCodes, tenant);
                        if (features.Keys.Contains(featureCode + objectTable.Id))
                        {
                            FeaturePM feature = features[featureCode + objectTable.Id];
                            if (feature != null)
                            {
                                exists = true;
                            }
                        }
                    }
                }
            }
            if (!exists)
            {
                return false;
            }
            else return true;
        }
        public static ContactInformation GetContactInformation(string email, int tenant, bool forceAPIFeaturesCheck = false)
        {
            int loggedTenant = tenant;
            ContactInformation myContactInfo = null;
            //string key = email + "_" + tenant + "_info";
            if (tenant == 0)
            {
                List<string> allPackages = GetAllPackagesCodes(email, loggedTenant, false);
                myContactInfo = new ContactInformation()
                {
                    ContactEmail = email,
                    IsAmitalAdmin = true,
                    PackagesCodes = allPackages,
                };
            }
            else
            {
                string token = null;
                IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                AuthenticationTokenRepository tokenRep = new AuthenticationTokenRepository();
                AuthenticationToken authToken = null;
                if (HttpContextHelper.HttpContext != null)
                {
                    token = HttpContextHelper.Request.Headers["Token"];
                }
                if (!string.IsNullOrEmpty(token))
                {
                    authToken = AuthenticationTokenRepository.GetSingleTokenFromCache(token);
                }
                if (authToken == null || !authToken.APIToken || forceAPIFeaturesCheck)
                {
                    Contact contact = GetSingleContactByEmail(tenant, email, context);
                    if (contact != null)
                    {
                        bool isCustomerCare = false;
                        if (contact.Tenant == 0 && tenant != 0)
                        {
                            User zeroUser = GetUserByMail(email, tenant, context);
                            if (zeroUser != null)
                            {
                                isCustomerCare = true;
                                if (zeroUser.IsDistributor)
                                {
                                    using (TransactionScope scope2 = TransactionFactory.GetNewTransaction())
                                    {
                                        bool isDistributorToCurrentTenant = CheckDistributor(tenant, context, zeroUser);
                                        if (!isDistributorToCurrentTenant)
                                        {
                                            return null;
                                        }
                                        scope2.Complete();
                                    }
                                }
                            }
                        }
                        bool IsAmitalAdmin = false;
                        if (tenant != 0)
                        {
                            User user = GetUserByMail(email, AmitalCloudSettings.AmitalCRMTenantNumber, context);
                            if (user != null)
                            {
                                tenant = AmitalCloudSettings.AmitalCRMTenantNumber;
                                IsAmitalAdmin = true;
                            }
                        }
                        RoleQuery roleQuery = new RoleQuery(tenant);
                        List<RolePM> allRoles = roleQuery.GetRolesForContact(contact.Id, contact.Tenant).ToList();
                        List<string> allRolesIds = allRoles.Select(s => s.Id).ToList();
                        List<RolePM> allCustomRoles = allRoles.Where(d => d.IsCustomRole == true).ToList();
                        foreach (RolePM item in allCustomRoles)
                        {
                            if (allRolesIds.Contains(item.ParentRoleId))
                            {
                                allRolesIds.Remove(item.ParentRoleId);
                            }
                        }
                        List<string> allPackages = GetAllPackagesCodes(email, loggedTenant, isCustomerCare);
                        myContactInfo = new ContactInformation()
                        {
                            Tenant = contact.Tenant,
                            ContactEmail = contact.Email,
                            IsAmitalAdmin = IsAmitalAdmin,
                            RolesIds = allRolesIds,
                            PackagesCodes = allPackages,
                            DontShowLocalLabels = contact.DontShowLocalLabels
                        };
                    }
                }
                else
                {
                    if (authToken != null)
                    {
                        myContactInfo = new ContactInformation()
                        {
                            Tenant = authToken.Tenant,
                            ContactEmail = authToken.Email,
                            IsApi = authToken.APIToken,
                        };
                    }
                }
            }
            return myContactInfo;
        }

        private static bool CheckDistributor(int tenant, IAmitalCloudContext context, User zeroUser)
        {
            return new Repository<TenantManagement>(context).GetMulti(a => a.Id == tenant && a.DistributorCode == zeroUser.DistributorCode).Any();
        }

        private static User GetUserByMail(string email, int tenant, IAmitalCloudContext context)
        {
            return new Repository<User>(context).GetMulti(d => d.Contact.Email == email && d.Tenant == tenant).FirstOrDefault();
        }
        public static ContactInfo GetContactInfo(string email, int tenant, bool forceAPIFeaturesCheck = false)
        {
            string cacheKey = $"info_({email}_{tenant})";
            ContactInfo myContactInfo = (ContactInfo)CacheManager.CacheWrapper.Get(cacheKey);
            if (myContactInfo == null || forceAPIFeaturesCheck)
            {
                if (tenant == 0)
                {
                    myContactInfo = new ContactInfo()
                    {
                        ContactEmail = email,
                        IsAmitalAdmin = true,
                        PackagesCodes = GetAllPackagesCodes(email, tenant, false),
                    };
                }
                else
                {
                    string token = null;
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    AuthenticationTokenRepository tokenRep = new AuthenticationTokenRepository();
                    AuthenticationToken authToken = null;
                    if (HttpContextHelper.HttpContext != null)
                    {
                        token = HttpContextHelper.Request.Headers["Token"];
                    }
                    if (!string.IsNullOrEmpty(token))
                    {
                        authToken = AuthenticationTokenRepository.GetSingleTokenFromCache(token);
                    }
                    if (authToken == null || !authToken.APIToken || forceAPIFeaturesCheck)
                    {
                        Contact contact = GetSingleContactByEmail(tenant, email, context);
                        if (contact != null)
                        {
                            bool isCustomerCare = false;
                            if (contact.Tenant == 0 && tenant != 0)
                            {
                                User zeroUser = GetUserByMail(email, 0, context);
                                if (zeroUser != null)
                                {
                                    isCustomerCare = true;
                                    if (zeroUser.IsDistributor)
                                    {
                                        using (TransactionScope scope2 = TransactionFactory.GetNewTransaction())
                                        {
                                            bool isDistributorToCurrentTenant = CheckDistributor(tenant, context, zeroUser);
                                            if (!isDistributorToCurrentTenant)
                                            {
                                                return null;
                                            }
                                            scope2.Complete();
                                        }
                                    }
                                }
                            }
                            bool IsAmitalAdmin = false;
                            if (tenant != 0)
                            {
                                User user = GetUserByMail(email, AmitalCloudSettings.AmitalCRMTenantNumber, context);
                                if (user != null)
                                {
                                    tenant = AmitalCloudSettings.AmitalCRMTenantNumber;
                                    IsAmitalAdmin = true;
                                }
                            }
                            RoleQuery roleQuery = new RoleQuery(tenant);
                            List<RolePM> allRoles = roleQuery.GetRolesForContact(contact.Id, contact.Tenant).ToList();
                            List<string> allRolesIds = allRoles.Select(s => s.Id).ToList();
                            List<RolePM> allCustomRoles = allRoles.Where(d => d.IsCustomRole == true).ToList();
                            foreach (RolePM item in allCustomRoles)
                            {
                                if (allRolesIds.Contains(item.ParentRoleId))
                                {
                                    allRolesIds.Remove(item.ParentRoleId);
                                }
                            }
                            myContactInfo = new ContactInfo()
                            {
                                Tenant = contact.Tenant,
                                ContactEmail = contact.Email,
                                IsAmitalAdmin = IsAmitalAdmin,
                                RolesIds = allRolesIds,
                                PackagesCodes = GetAllPackagesCodes(email, tenant, isCustomerCare),
                            };
                        }
                    }
                    else
                    {
                        if (authToken != null)
                        {
                            myContactInfo = new ContactInfo()
                            {
                                Tenant = authToken.Tenant,
                                ContactEmail = authToken.Email,
                                IsApi = authToken.APIToken,
                            };
                        }
                    }
                }

                if (myContactInfo != null)
                {
                    CacheManager.CacheWrapper.Insert(cacheKey, myContactInfo, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                }
            }
            return myContactInfo;
        }
        public static bool CheckFeatureAccessLevelPermission(string objectTableName, string featureCode, string entityUserId, string entityBusinessUnitId, int tenant)
        {
            bool isAllowed = false;
            if (tenant == 0)
            {
                isAllowed = true;
            }
            else if (!string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
            {
                string email = HttpContextHelper.User.Identity.Name;
                ContactInfo myContactInfo = GetContactInfo(email, tenant);
                if (myContactInfo != null)
                {
                    if (myContactInfo.IsAmitalAdmin)// || myContactInfo.IsApi)
                    {
                        isAllowed = true;
                    }
                    else
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        ObjectTablePM objectTable = ObjectTableQuery.GetObjectTableByCode(objectTableName, tenant);
                        Feature myFeature = new Repository<Feature>(context).GetMulti(a => a.ObjectTableId == objectTable.Id && a.Code == featureCode
                            && (a.Tenant == tenant || a.Tenant == 0)).FirstOrDefault();   //.GetSingleFeatureByCode(objectTable.Id, featureCode, tenant);
                        if (myFeature != null)
                        {
                            RoleFeatureRepository roleFeatureRepository = new RoleFeatureRepository(tenant);
                            if (myFeature.IsBusinessUnitEnabled)
                            {
                                List<RoleFeature> myFeatureRoles = new List<RoleFeature>();
                                foreach (string myRoleId in myContactInfo.RolesIds)
                                {
                                    RoleFeature myRoleFeature = roleFeatureRepository.GetBusinessUnitFilterRoleFeature(myRoleId, myFeature.Id, tenant);
                                    if (myRoleFeature != null)
                                    {
                                        myFeatureRoles.Add(myRoleFeature);
                                    }
                                }
                                if (myFeatureRoles.Count > 0)
                                {
                                    if (myFeatureRoles.Where(d => d.FeatureAccessLevelCode == "OR").Any())
                                    {
                                        isAllowed = true;
                                    }

                                    else
                                    {
                                        Contact contact = GetSingleContactByEmail(tenant, email, context);
                                        User logedUser = new Repository<User>(context).GetMulti(record => record.Id == contact.Id && record.Tenant == tenant).FirstOrDefault();   // .GetSingleUser(contact.Id, tenant, false);
                                        if (myFeatureRoles.Where(d => d.FeatureAccessLevelCode == "US").Any())
                                        {
                                            if (logedUser.Id == entityUserId)
                                            {
                                                isAllowed = true;
                                            }
                                        }
                                        else if (myFeatureRoles.Where(d => d.FeatureAccessLevelCode == "BU").Any())
                                        {
                                            if (logedUser.BusinessUnitId == entityBusinessUnitId)
                                            {
                                                isAllowed = true;
                                            }
                                        }
                                        else if (myFeatureRoles.Where(d => d.FeatureAccessLevelCode == "PR").Any())
                                        {
                                            if (entityBusinessUnitId.StartsWith(logedUser.BusinessUnitId))
                                            {
                                                isAllowed = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (string myRoleId in myContactInfo.RolesIds)
                                {
                                    RoleFeature myRoleFeature = roleFeatureRepository.GetBusinessUnitFilterRoleFeature(myRoleId, myFeature.Id, myFeature.Tenant);
                                    if (myRoleFeature != null)
                                    {
                                        isAllowed = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (!isAllowed)
                {
                    throw new Exception("Sorry! you have no permission to do this operation on " + objectTableName + ". Please contact your administrator.");
                }
            }
            return isAllowed;
        }

        private static Contact GetSingleContactByEmail(int tenant, string email, IAmitalCloudContext context)
        {
            List<Contact> entities = new Repository<Contact>(context).GetMulti(a => a.Email == email && (a.Tenant == tenant || a.Tenant == 0));

            Contact contact = entities.Where(a => a.Tenant == tenant).FirstOrDefault() ?? entities.Where(a => a.Tenant == 0).FirstOrDefault();

            return contact;
        }

        private static List<string> GetAllPackagesCodes(string email, int tenant, bool isCustomerCare)
        {
            List<string> myResult = new List<string>();
            string myPackageCode = null;
            bool isMultiPackage = false;
            bool isAddingAddOns = false;
            List<string> allPackagesCodes_AD = new List<string>();
            List<string> allPackagesCodes_PK = new List<string>();
            List<string> allPackagesCodes_BS = new List<string>();
            IAmitalCloudContext myCommonContext = AmitalCloudContext.GetContext(tenant);
            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                #region
                IGlobalContext globalContext = GlobalContext.GetContext();
                TenantManagement myTenantManagement = (from d in globalContext.TenantManagements where d.Id == tenant select d).FirstOrDefault();

                if (myTenantManagement != null)
                {
                    isMultiPackage = myTenantManagement.IsMultiPackage;

                    allPackagesCodes_AD = (from d in globalContext.TenantAddOns
                                           where d.Tenant == tenant
                                           group d by d.PackageCode into g
                                           select g.Key).ToList();
                    if (isMultiPackage)
                    {
                        allPackagesCodes_PK = (from d in globalContext.TenantManagementLicenses
                                               where d.Tenant == tenant
                                               group d by d.PackageCode into g
                                               select g.Key).ToList();
                    }
                    else
                    {
                        myPackageCode = myTenantManagement.PackageCode;
                        if (!string.IsNullOrEmpty(myTenantManagement.TemporalPackageCode) && myTenantManagement.TemporalStartDate != null && myTenantManagement.TemporalEndDate != null)
                        {
                            if (myTenantManagement.TemporalStartDate.Value.Date <= DateTime.Now.Date && DateTime.Now.Date <= myTenantManagement.TemporalEndDate.Value.Date)
                            {
                                myPackageCode = myTenantManagement.TemporalPackageCode;
                            }
                        }
                    }
                }
                scope.Complete();
                #endregion
            }
            if (isMultiPackage)
            {
                if (isCustomerCare)
                {
                    isAddingAddOns = true;
                }
                else
                {
                    isAddingAddOns = true;
                    string loggedUserId = null;
                    if (loggedUserId == null)
                    {
                        User user = new Repository<User>(myCommonContext).GetMulti(a => a.Contact.Email == email && a.Tenant == tenant).FirstOrDefault(); //.GetSingleUserByEmail(email, tenant, true);
                        if (user != null)
                        {
                            loggedUserId = user.Id;
                        }
                    }
                    List<string> allUserLicenses = (from a in myCommonContext.UserLicenses
                                                    where a.Tenant == tenant && a.UserId == loggedUserId
                                                    group a by a.PackageCode into g
                                                    select g.Key).ToList();
                    allPackagesCodes_PK = (from a in allPackagesCodes_PK
                                           where allUserLicenses.Contains(a)
                                           select a).ToList();
                }
                List<string> allConnectedCodes_PK = (from a in myCommonContext.PackageConnectedPackages
                                                     where allPackagesCodes_PK.Contains(a.PackageCode)
                                                     group a by a.ConnectedPackageCode into g
                                                     select g.Key).ToList();
                foreach (string itemCode in allConnectedCodes_PK)
                {
                    if (!allPackagesCodes_BS.Contains(itemCode))
                    {
                        allPackagesCodes_BS.Add(itemCode);
                    }
                }
            }
            else
            {
                isAddingAddOns = true;
                Package myPackage = (from a in myCommonContext.Packages where a.Code == myPackageCode select a).FirstOrDefault();
                if (myPackage != null)
                {
                    if (myPackage.FeaturePackageTypeCode == "BS")
                    {
                        if (!allPackagesCodes_BS.Contains(myPackageCode))
                        {
                            allPackagesCodes_BS.Add(myPackageCode);
                        }
                    }
                    else
                    {
                        var allCodes = (from a in myCommonContext.PackageConnectedPackages
                                        where a.PackageCode == myPackageCode
                                        group a by a.ConnectedPackageCode into g
                                        select g.Key).ToList();
                        foreach (string itemCode in allCodes)
                        {
                            if (!allPackagesCodes_BS.Contains(itemCode))
                            {
                                allPackagesCodes_BS.Add(itemCode);
                            }
                        }
                    }
                }
            }
            if (isAddingAddOns)
            {
                if (allPackagesCodes_AD.Count > 0)
                {
                    List<string> allConnectedCodes_AD = (from a in myCommonContext.PackageConnectedPackages
                                                         where allPackagesCodes_AD.Contains(a.PackageCode)
                                                         group a by a.ConnectedPackageCode into g
                                                         select g.Key).ToList();
                    foreach (string itemCode in allConnectedCodes_AD)
                    {
                        if (!allPackagesCodes_BS.Contains(itemCode))
                        {
                            allPackagesCodes_BS.Add(itemCode);
                        }
                    }
                }
            }
            myResult = allPackagesCodes_BS;
            return myResult;
        }
        public static string GetAuthenticatedUser(int tenant)
        {
            string email = !string.IsNullOrEmpty(HttpContextHelper.User?.Identity?.Name) ? HttpContextHelper.User.Identity.Name : "system@tenant" + tenant.ToString() + ".com";
            return email;
        }
        public static string GetAuthenticatedWorkWebUser()
        {
            if (HttpContextHelper.HttpContext != null)
            {
                if (!string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
                {
                    return HttpContextHelper.User.Identity.Name;
                }
                else
                {
                    throw new AutenticationException("Sorry! this user is not authorized!");
                }
            }
            else if (!string.IsNullOrEmpty(AuthenticationUtil.AuthenticatedUserEmail))
            {
                return AuthenticationUtil.AuthenticatedUserEmail;
            }
            throw new AutenticationException("Sorry! this user is not authorized!");
        }
        public static bool CheckSharedContactAuthentication(int tenant, string partnerId)
        {
            if (tenant != 0)
            {
                bool exists = false;
                if (!string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    string email = HttpContextHelper.User.Identity.Name;
                    Contact contact = GetSingleContactByEmail(tenant, email, context);
                    if (contact != null)
                    {
                        CardContact cardContact = context.CardContacts.Where(d => d.ContactId == contact.Id && d.CardId == partnerId).FirstOrDefault();
                        if (cardContact != null)
                        {
                            exists = true;
                        }
                    }
                }
                if (!exists)
                {
                    throw new AutenticationException("Sorry! you are not authorized to read data!");
                }
                return exists;
            }
            return true;
        }
        public static int AuthenticateTenant(int? entityTenant = null, string mode = null, string objectTableName = null)
        {
            try
            {
                string? token = HttpContextHelper.Request.Headers["Token"];
                if (string.IsNullOrEmpty(token))
                {
                    throw new AutenticationException("missing token");
                }

                AuthenticationToken authToken = AuthenticationTokenRepository.GetSingleTokenFromCache(token);
                AuthenticationOnTenant(authToken.Tenant);

                if (!string.IsNullOrEmpty(mode) && !string.IsNullOrEmpty(objectTableName))
                {
                    CheckContactFeature(objectTableName, mode, authToken.Tenant);
                }
                if (entityTenant != null)
                {
                    AuthenticationOnEntityTenant((int)entityTenant, authToken.Tenant);
                }
                return authToken.Tenant;
            }
            catch (AutenticationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NetCommonHelper.Logger.DevLog.Instance.WriteError($"Failed to authenticate tenant/token: {ex.Message}");
                throw new AutenticationException("Not authorized!");
            }
        }
        public static void AuthenticationOnTenant(int tenant)
        {
            if (HttpContextHelper.HttpContext != null)
            {
                string email = HttpContextHelper.User.Identity.Name;
                if (HttpContextHelper.HttpContext.Items != null)
                {
                    CheckHttpContextCurrentItems();
                }
                ContactInfo contactinfo = GetContactInfo(email, tenant);
                if (contactinfo == null || string.IsNullOrEmpty(email))
                {
                    throw new AutenticationException("Sorry! this user is not authorized!");
                }
                if (contactinfo.IsApi && (contactinfo.Tenant != tenant) && contactinfo.Tenant != 0)
                {
                    throw new AutenticationException("Sorry! this user is not authorized!");
                }
                var url = getLoggedDomain();



                if (HttpContextHelper.Request != null)
                {
                    string mobileVersion = HttpContextHelper.Request.Headers["MobileVersion"];
                    string Platform = HttpContextHelper.Request.Headers["Platform"];
                    if (!string.IsNullOrEmpty(mobileVersion))
                    {
                        double version = 0;
                        if (double.TryParse(mobileVersion, out version))
                        {
                            string mobileVersionError = "";
                            if (Platform == "IOS")
                            {
                                if (version < AmitalCloudSettings.IOSSharedAppMinimumVersion)
                                {
                                    mobileVersionError = "Your application version is out-of-date. Please upgrade your application to the latest version";
                                }
                            }
                            else
                            {
                                if (version < AmitalCloudSettings.AndroidSharedAppMinimumVersion)
                                {
                                    mobileVersionError = "Your application version is out-of-date. Please upgrade your application to the latest version";
                                }
                            }
                            if (!string.IsNullOrEmpty(mobileVersionError))
                            {
                                if (!string.IsNullOrEmpty(HttpContextHelper.Response.Headers["MobileVersionError"]))
                                {
                                    HttpContextHelper.Response.Headers["MobileVersionError"] = mobileVersionError;
                                }
                                else HttpContextHelper.Response.Headers.Add("MobileVersionError", mobileVersionError);
                            }
                        }
                    }
                    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                    {
                        if (new GlobalDBRepository(ConfigurationHelper.Conf).GetGlobalDBByTenant(tenant).IsBlocking)
                        {
                            if (!string.IsNullOrEmpty(HttpContextHelper.Response.Headers["MobileUpgrading"]))
                            {
                                HttpContextHelper.Response.Headers["MobileUpgrading"] = "Unifreight mobile is being updated, please try again later . Sorry for the inconvenience";
                            }
                            else HttpContextHelper.Response.Headers.Add("MobileUpgrading", "Unifreight mobile is being updated, please try again later . Sorry for the inconvenience");
                        }
                        scope.Complete();
                    }
                }
            }
        }
        private static void CheckHttpContextCurrentItems()
        {
            CheckSessionExpiration();
            CheckAPICredintialExpiration();
        }
        private static void CheckSessionExpiration()
        {
            if (!HttpContextHelper.HttpContext.Items.ContainsKey("Session")) return;
            string sessionItem = HttpContextHelper.HttpContext.Items["Session"] as string;
            if (sessionItem == "SessionExpiration")
            {
                throw new Exception("Sorry! this user is not authorized! due to session expiration");
            }
        }
        private static void CheckAPICredintialExpiration()
        {
            if (!HttpContextHelper.HttpContext.Items.ContainsKey("APICredintial")) return;
            string aPICredintialItem = HttpContextHelper.HttpContext.Items["APICredintial"] as string;
            if (aPICredintialItem == "APICredintialExpired")
            {
                throw new Exception("Sorry! this user is not authorized! due to api credintial expiration");
            }
        }
        public static bool CheckTableContactFeature(string objectTableName, string featureCode, int tenant)
        {
            if (IsWorkerRoleCall && HttpContextHelper.HttpContext == null) //for calling the excel export data from WR 
            {
                return true;
            }

            bool exists = false;

            if (tenant == 0)
            {
                exists = true;
            }

            else if (!string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
            {
                string email = HttpContextHelper.User.Identity.Name;

                ContactInfo myContactInfo = GetContactInfo(email, tenant);

                if (myContactInfo != null)
                {
                    if (myContactInfo.IsAmitalAdmin)
                    {
                        exists = true;
                    }

                    else
                    {
                        ObjectTablePM objectTable = ObjectTableQuery.GetObjectTableByCode(objectTableName, tenant);

                        foreach (string roleid in myContactInfo.RolesIds)
                        {
                            Dictionary<string, FeaturePM> features = GetFeaturesForRole(roleid, myContactInfo.PackagesCodes, tenant);
                            if (features.Keys.Contains(featureCode + objectTable.Id))
                            {
                                FeaturePM feature = features[featureCode + objectTable.Id];
                                if (feature != null)
                                {
                                    exists = true;
                                }
                            }
                        }
                    }
                }
            }

            return exists;
        }
        public static void AuthenticationOnEntityTenant(int entityTenant, int authTokenTenant)
        {
            if (entityTenant != authTokenTenant)
                throw new AutenticationException("Sorry! you have no permission to do this operation on Tenant:" + entityTenant + ". Please contact your administrator.");
        }

        public static string getLoggedDomain()
        {
            var isAppServiceENV = Environment.GetEnvironmentVariable("IsAppService") == "true";
            bool isAppService = ConfigurationHelper.GetValue("IsAppService") == "true";

            HttpContext context = HttpContextHelper.HttpContext;
            string host = (isAppServiceENV || isAppService) && !string.IsNullOrEmpty(context.Request.Headers["X-ORIGINAL-HOST"]) ? context.Request.Headers["X-ORIGINAL-HOST"] : context.Request.Host.Host;
            return host;
        }

        public static (string user, string? ip) GetAuditInfo()
        {
            string user;
            try { user = GetAuthenticatedUser(); }
            catch { user = "UnKnown"; }

            string? ip = "";
            try
            {
                var request = HttpContextHelper.Request;
                if (request != null)
                {
                    ip = request.Headers["X-Real-IP"].ToString() ?? HttpContextHelper.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                }
            }
            catch { ip = "UnKnown"; }

            return (user, ip);
        }
    }
}
