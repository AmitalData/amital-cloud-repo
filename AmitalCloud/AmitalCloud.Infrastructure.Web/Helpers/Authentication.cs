using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.EntityUpdateServices;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Data.Security;
using AmitalCloud.Infrastructure.Data.Services;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Web.DataContracts;
using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers.MixPanel;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using AmitalCloud.Infrastructure.Model.Interfaces;
using UAParser;
using Microsoft.Extensions.Caching.Memory;

namespace AmitalCloud.Infrastructure.Application.Helpers
{
    public class Authentication
    {
        readonly private int tenant;
        readonly private IGlobalContext globalContext;
        private IAmitalCloudContext amitalCloudContext;
        readonly private GlobalContactQueryService globalContactQueryService;
        readonly private ContactPasswordQueryService contactPasswordQueryService;
        readonly private TenantManagementQueryService tenantManagementQueryService;
        readonly private IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;

        public Authentication(int tenant, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            this.tenant = tenant;
            globalContext = GlobalContext.GetContext();
            amitalCloudContext = AmitalCloudContext.GetContext(tenant);
            globalContactQueryService = new GlobalContactQueryService(globalContext);
            contactPasswordQueryService = new ContactPasswordQueryService(globalContext);
            tenantManagementQueryService = new TenantManagementQueryService(globalContext);
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        public UserData AuthenticateUser(LoginParameters loginParameters)
        {
            TenantManagmentPrivateLabelsPM? privatelabel = null;
            var url = AmitalCloudSecurityUtility.getLoggedDomain();

            url = url.Split(':')[0];

            if (!loginParameters.IsFromPLSignApp && !url.Contains("system.logbox.co.il") && !url.Contains("cloud.amital.co.il"))
            {
                TenantManagmentPrivateLabelsQueryService tenantManagmentPrivateLabelsQueryService = new TenantManagmentPrivateLabelsQueryService(globalContext);
                privatelabel = tenantManagmentPrivateLabelsQueryService.GetMulti(a => a.PrivateLabelUrl == url && a.InActive == false).FirstOrDefault();
            }
            DateTime DateBeforePostUserValidation = DateTime.Now;
            string email = loginParameters.Email.Trim();
            string password = loginParameters.Password;
            UserData data = new UserData();
            List<CompanyLogin> loginsList = new List<CompanyLogin>();
            ContactPasswordPM? contactPassword = null;

            CardQueryService cardQueryService = new CardQueryService(amitalCloudContext);
            TenantQueryService tenantQueryService = new TenantQueryService(amitalCloudContext);
            UserQueryService userQueryService = new UserQueryService(amitalCloudContext);
            GlobalTenantQueryService globalTenantQueryService = new GlobalTenantQueryService(globalContext);

            data = CheckCaptchaState(loginParameters);
            if (!data.HasError)
            {
                data = CheckUserState(email.ToLower(), password, ref contactPassword, loginParameters.ByToken, loginParameters.ClientType);
            }

            if (!data.HasError)
            {
                bool customerCare = false;
                bool distributor = false;
                UserPM? amitalUser = null;
                GlobalContactPM? zeroContact = globalContactQueryService.GetMulti(d => d.GlobalTenantId == 0 && d.Email == email && d.InActive == false).FirstOrDefault();

                if (zeroContact != null)
                {
                    amitalUser = userQueryService.GetSingle(zeroContact.Id, false, false);
                    if (amitalUser?.Tenant == 0)
                    {
                        distributor = amitalUser.IsDistributor;
                        customerCare = !amitalUser.IsDistributor;
                    }
                }

                Expression<Func<GlobalContact, bool>> predicateGlobalContact;

                if (customerCare || distributor)
                {
                    Expression<Func<TenantManagement, bool>> predicate;
                    if (distributor && amitalUser != null)
                    {
                        predicate = a => a.DistributorCode == amitalUser.DistributorCode && a.Id != 0 && a.IsDistributorSupportEnabled;
                    }
                    else
                    {
                        predicate = a => a.IsSystemSupportEnabled || a.Id == 0;
                    }
                    List<int> tenantManagementIds = tenantManagementQueryService.GetMulti(predicate, a => a.Id);
                    List<GlobalTenantPM> globalTenants = globalTenantQueryService.GetMulti(t => tenantManagementIds.Contains(t.Id) && t.IsActive);
                    globalTenants.ForEach(globalTenant => loginsList.Add(CreateCompanyLogin(zeroContact.Email, $"{globalTenant.CompanyName} ({globalTenant.Id})", true, globalTenant.Id, null, null, zeroContact.Id, true, false, globalTenant.PrivateLabelId)));

                    predicateGlobalContact = m => m.Email == email && m.GlobalTenant.IsActive == true && m.InActive == false && m.InternetAccess == true;
                }
                else
                {
                    predicateGlobalContact = m => m.Email == email && m.GlobalTenant.IsActive == true && m.InActive == false && (m.IsUser == true || m.InternetAccess == true);
                }

                List<GlobalContactPM> contacts = globalContactQueryService.GetMulti(predicateGlobalContact);
                foreach (GlobalContactPM contact in contacts)
                {
                    GlobalTenantPM globalTenant = globalTenantQueryService.GetSingle(contact.GlobalTenantId, false, true);

                    if (contact.IsUser && !customerCare && !distributor &&
                        ((!loginParameters.IsCargoTracking || !loginParameters.IsCustomsBook || (loginParameters.IsCustomsBook && FeatureToggleHelper.HasFeatureToggle("LCB", contact.GlobalTenantId)))
                            || (loginParameters.IsCargoTracking && FeatureToggleHelper.HasFeatureToggle("LCT", contact.GlobalTenantId))))
                    {
                        bool manageLicencesPerUser = tenantManagementQueryService.GetSingle(contact.GlobalTenantId, false, true)?.ManageLicencesPerUser ?? false;
                        bool licensed = !manageLicencesPerUser || (manageLicencesPerUser && (userQueryService.GetSingle(contact.Id, false, false)?.LicencedUser ?? false));

                        loginsList.Add(CreateCompanyLogin(contact.Email, $"{globalTenant.CompanyName} ({contact.GlobalTenantId})", isUser: true, contact.GlobalTenantId, null, null, contact.Id, licensedUser: licensed, contact.InternetAccess, globalTenant.PrivateLabelId));
                    }

                    if (contact.InternetAccess)
                    {
                        TenantPM currentTenant = tenantQueryService.GetSingle(contact.GlobalTenantId, false, true);

                        if ((loginParameters.IsMobileLogin && currentTenant.IsMobileActivated)
                            || (!loginParameters.IsMobileLogin && (currentTenant.IsWebAccessActivated || currentTenant.IsCargoTrackWebAccessActivated || currentTenant.IsDigitalPortalAccessActivated)))
                        {
                            CardContactQueryService cardContactQueryService = new CardContactQueryService(amitalCloudContext);
                            List<CardContactPM> cardcontactsList = cardContactQueryService.GetMulti(c => c.ContactId == contact.Id && c.InternetAccess == true && c.Tenant == contact.GlobalTenantId);

                            cardcontactsList.ForEach(cardContact =>
                            {
                                CardPM? card = cardQueryService.GetMulti(c => c.Id == cardContact.CardId && c.Tenant == cardContact.Tenant).FirstOrDefault();

                                loginsList.Add(CreateCompanyLogin(contact.Email, $"{globalTenant.CompanyName}-{card?.EnglishName} ({contact.GlobalTenantId})", isUser: false, contact.GlobalTenantId, card.Id, card.PartnerTypeId, contact.Id, licensedUser: false, contact.InternetAccess, globalTenant.PrivateLabelId, card.EnglishName));
                            });
                        }
                    }
                }

                List<CompanyLogin> unliscened = loginsList.Where(s => s.LicensedUser == false && s.IsUser == true).ToList();

                loginsList = loginsList.Where(s => s.LicensedUser == true || s.IsUser == false).OrderBy(c => c.CompanyName).ToList();

                if (!loginParameters.IsFromPLSignApp)
                {
                    List<string> logboxAccessiblePrivateLabelTenantsIds = GetLogboxAccessiblePrivateLabelTenantsIds(url);
                    MapHasLogboxAccessPrivateLabelTenants(loginsList, logboxAccessiblePrivateLabelTenantsIds);
                }

                if (loginsList.Count == 1)
                {
                    CompanyLogin companyAccess = loginsList.First();

                    bool isValidPrivateLabel = privatelabel != null && companyAccess.PrivateLabelId == privatelabel.Id;
                    bool isInvalidAccess = (privatelabel == null && !string.IsNullOrEmpty(companyAccess.PrivateLabelId) &&
                                            !companyAccess.HasLogboxAccess && !loginParameters.IsFromPLSignApp) ||
                                           (privatelabel != null && string.IsNullOrEmpty(companyAccess.PrivateLabelId));

                    if (isValidPrivateLabel || !isInvalidAccess)
                    {
                        data = LoginUser(new LoginParameters()
                        {
                            Email = email,
                            Password = password,
                            IsUser = companyAccess.IsUser,
                            CardId = companyAccess.CardId,
                            CardType = companyAccess.CardType,
                            ByToken = loginParameters.ByToken,
                            IsMobileLogin = loginParameters.IsMobileLogin,
                            GetToken = loginParameters.GetToken,
                            IsAngularLogin = loginParameters.IsAngularLogin,
                            InternalLoginValidationCall = true,
                            ClientType = loginParameters.ClientType
                        }, companyAccess.Tenant);
                    }
                    else
                    {
                        data = new UserData()
                        {
                            UserName = email,
                            CompanyLogins = new List<CompanyLogin>(),
                            HasError = true,
                            ContactsCount = 0,
                            InActive = true,
                            Unlicensed = unliscened.Count() > 0,
                            PrivateLablehasZeroTenant = true
                        };
                    }

                    data.CompanyLogins = loginsList;
                    data.ContactsCount = 1;
                }
                else
                {
                    var temp = new List<CompanyLogin>();

                    if (loginParameters.IsFromPLSignApp)
                    {
                        temp = loginsList.Where(x => x.PrivateLabelId != null).ToList();
                    }
                    else if (privatelabel != null)
                    {
                        temp = loginsList.Where(a => a.PrivateLabelId == privatelabel.Id).ToList();
                    }
                    else
                    {
                        temp = loginsList.Where(a => a.PrivateLabelId == null || a.HasLogboxAccess).ToList();
                    }

                    if (temp.Count == 1)
                    {
                        CompanyLogin companyAccess = temp.First();
                        data = LoginUser(new LoginParameters()
                        {
                            Email = email,
                            Password = password,
                            IsUser = companyAccess.IsUser,
                            CardId = companyAccess.CardId,
                            CardType = companyAccess.CardType,
                            ByToken = loginParameters.ByToken,
                            IsMobileLogin = loginParameters.IsMobileLogin,
                            GetToken = loginParameters.GetToken,
                            IsAngularLogin = loginParameters.IsAngularLogin,
                            InternalLoginValidationCall = true,
                            ClientType = loginParameters.ClientType
                        }, companyAccess.Tenant);
                        data.CompanyLogins = temp;
                        data.ContactsCount = 1;
                    }
                    else
                    {
                        data = new UserData()
                        {
                            UserName = email,
                            CompanyLogins = temp,
                            HasError = temp.Count() == 0,
                            ContactsCount = temp.Count(),
                            InActive = privatelabel == null && temp.Count() == 0,
                            Unlicensed = unliscened.Count() > 0,
                            PrivateLablehasZeroTenant = privatelabel != null && temp.Count() == 0
                        };
                    }
                }
            }

            if (loginParameters.IsMobileLogin && !data.HasError)
            {
                data.CompanyLogins = data.CompanyLogins.Where(c => c.InternetAccess == true).ToList();

                List<int> tenants = (from a in data.CompanyLogins select a.Tenant).ToList();

                List<TenantPM> mobileTenants = tenantQueryService.GetMulti(t => tenants.Contains(t.Id));

                List<CompanyLogin> mobileActivatedTenants = new List<CompanyLogin>();
                foreach (CompanyLogin login in data.CompanyLogins)
                {
                    TenantPM? tenant = mobileTenants.FirstOrDefault(t => t.Id == login.Tenant);
                    if (tenant != null && tenant.IsMobileActivated)
                    {
                        List<string> LogoInfo = GetTenantLogoUri(login.Tenant, loginParameters.MobileVersion);

                        if (LogoInfo != null && LogoInfo.Count > 0) login.URL = LogoInfo[0];
                        if (LogoInfo != null && LogoInfo.Count > 1) login.Extension = LogoInfo[1];

                        mobileActivatedTenants.Add(login);
                    }
                }

                data.CompanyLogins = mobileActivatedTenants;
                data.InActive = mobileActivatedTenants.Count() == 0;
                data.Unlicensed = mobileActivatedTenants.Count() == 0;
                data.HasError = mobileActivatedTenants.Count() == 0;
                data.ContactsCount = mobileActivatedTenants.Count();
                data.InvalidMobileAccessPermission = mobileActivatedTenants.Count() == 0;
            }

            int executionTime = (int)((DateTime.Now.Ticks - DateBeforePostUserValidation.Ticks) / TimeSpan.TicksPerMillisecond);
            AddServerTimeToHeaderRespose(executionTime);

            #region PasswordExpirationDate

            if (data != null && !data.HasError && loginParameters.ClientType == "Web")
            {
                if (contactPassword == null)
                {
                    contactPassword = contactPasswordQueryService.GetMulti(c => c.Email.ToLower() == email).FirstOrDefault();
                }

                if (contactPassword?.PasswordExpirationDate != null)
                {
                    DateTime nowDate = DateTime.Now;
                    DateTime expirationDate = (DateTime)contactPassword.PasswordExpirationDate;

                    if (nowDate.Date > expirationDate.Date)
                    {
                        data.HasError = true;
                        data.MustChangePassword = true;
                    }
                    else
                    {
                        var days = (expirationDate - nowDate).TotalDays;
                        if (days.ToString().Contains("."))
                        {
                            days = Int32.Parse(days.ToString().Split('.')[0]) + 1;
                        }
                        if (days <= 10)
                        {
                            data.HasError = true;
                            data.PasswordExpirationDateMessage = $"Your password will expire in {days} days. Do you want to change it now?";
                        }
                    }
                }
            }
            #endregion

            if (data.HasError)
            {
                if (data.InValidMailOrPassword || data.IsLocked || data.IpRestricted)
                {
                    AddFailedLoginLog(data);

                    if (!data.IpRestricted && loginParameters.ClientType == "Web")
                    {
                        if (contactPassword == null)
                        {
                            contactPassword = contactPasswordQueryService.GetMulti(c => c.Email.ToLower() == email).FirstOrDefault();
                        }

                        if (contactPassword != null && contactPassword.NumberOfRetries++ >= 5)
                        {
                            CaptchaHelper captchaHelper = new CaptchaHelper();
                            captchaHelper.AddCaptchaKey(loginParameters.Email, data, "Login");

                            UpdateContactPassword(contactPassword, new ContactPasswordUpdateService(tenant), captchaKey: data.CaptchaKey);
                        }
                    }
                }
            }
            return data;
        }

        public UserData LoginUser(LoginParameters parameters, int tenant, bool? isFromCTool = false)
        {
            bool FromCTool = isFromCTool ?? false;

            DateTime DateBeforePostLoginData = DateTime.Now;
            if (!string.IsNullOrEmpty(parameters.Email)) parameters.Email = parameters.Email.ToLower();

            string email = parameters.Email;
            string password = parameters.Password;
            bool isUser = parameters.IsUser;
            string cardId = parameters.CardId;

            string userData = string.Empty;
            bool customerCare;
            string computerId = Guid.NewGuid().ToString("N");
            ContactPasswordPM? contactPassword = null;

            var passResult = ResolvePassword(password);
            bool OneTimePassword = passResult?.IsOneTimePassword ?? false;

            UserData? user = CheckCaptchaState(parameters, true);

            if (!user.InValidCaptcha)
            {
                user = null;
                customerCare = false;
                bool distributor = false;
                UserPM amitalUser = null;
                // when accessing from authenticateUser, the context may be regarding another tenant, so need to create a new context
                amitalCloudContext = AmitalCloudContext.GetContext(tenant);
                UserQueryService userQueryService = new UserQueryService(amitalCloudContext);
                GlobalContactPM? contact = globalContactQueryService.GetMulti(d => d.GlobalTenantId == 0 && d.Email.ToLower() == parameters.Email.ToLower() && d.InActive == false).FirstOrDefault();
                if (contact != null)
                {
                    amitalUser = userQueryService.GetSingle(contact.Id, false, false);
                    if (amitalUser?.Tenant == 0)
                    {
                        distributor = amitalUser.IsDistributor;
                        customerCare = !amitalUser.IsDistributor;
                    }
                }
                else
                {
                    contact = globalContactQueryService.GetMulti(c => c.Email == email && c.InActive == false && c.GlobalTenantId == tenant).FirstOrDefault();
                }
                if (contact != null)
                {
                    amitalUser = userQueryService.GetSingle(contact.Id, false, false);

                    user = ValidateUser(email, password, tenant, computerId, out userData, isUser, cardId, parameters.ByToken, parameters.IsMobileLogin ? "Mobile" : "PC", parameters.ClientType, FromCTool, parameters.IsCargoTracking, OneTimePassword);
                    if (user != null)
                    {
                        if (user.IsUser == false)
                        {
                            HttpContextHelper.SetCookie(user.UserName, user.UserId.ToString(), user.CurrentTenant.ToString(), user.IsAuthenticated, computerId);
                        }
                    }
                    else
                    {
                        contactPassword = null;
                        user = CheckUserState(email, password, ref contactPassword, parameters.ByToken, parameters.ClientType);
                    }
                }
                else
                {
                    user = new UserData() { HasError = true, InValidMailOrPassword = true };
                }

                TenantManagementPM tenantManagement = tenantManagementQueryService.GetSingle(user.CurrentTenant, false, true);
                if (tenantManagement != null && tenantManagement.EnableBranding) user.IsBrandingEnabled = tenantManagement.HideSharedlogistics;

                user.Technology = "AG";

                #region KeepUserLoggedIn
                TenantLoginPolicyQueryService securityPolicyQueryService = new TenantLoginPolicyQueryService(amitalCloudContext);
                TenantLoginPolicyPM securityPolicy = securityPolicyQueryService.GetMulti(d => d.Tenant == tenant).FirstOrDefault();

                if (securityPolicy != null)
                {
                    user.KeepUserLoggedIn = securityPolicy.KeepUserLoggedIn;

                    if (parameters.ClientType == "Web" && !securityPolicy.KeepUserLoggedIn)
                    {
                        user.SessionTimeout = securityPolicy.SessionTimeout;
                        SetSessionPolicy(user);
                    }
                }
                else if (parameters.ClientType == "Web") SetSessionPolicy(user);
                #endregion

                if (!user.HasError && (parameters.IsMobileLogin || parameters.GetToken))
                {
                    bool IsTwoFactorAuthenticationRequired = !parameters.IsAngularLogin && !parameters.IsMobileLogin && parameters.IsUser && !customerCare && !FromCTool && CheckLoginSecurityPolicy(tenant, user, amitalUser, securityPolicy);

                    if (!IsTwoFactorAuthenticationRequired)
                    {
                        if (!parameters.ByToken)
                        {
                            string hashedPassword = "";
                            ContactPassword contactPasswordPOCO = AuthenticationUtil.VerifyContactPassword(parameters.Email, parameters.Password);
                            if (contactPasswordPOCO != null)
                            {
                                contactPassword = new ContactPasswordPM(contactPasswordPOCO);
                                hashedPassword = contactPassword.Password;
                            }

                            if (passResult != null)
                            {
                                hashedPassword = passResult.Password;
                            }

                            string token = AuthenticationUtil.GenerateToken();

                            AuthenticationTokenUpdateService authenticationUpdateService = new AuthenticationTokenUpdateService(tenant);
                            AuthenticationToken authentication = new AuthenticationToken() { CreateDate = DateTime.Now, Email = email, Password = hashedPassword, Token = token, Tenant = user.CurrentTenant, ClientType = parameters.IsMobileLogin ? "Mobile" : parameters.ClientType };
                            if (!user.KeepUserLoggedIn && authentication.ClientType == "Web" && user.WebTokenLifeTimeInMinutes != 0) authentication.ExpirationDate = DateTime.Now.AddMinutes(user.WebTokenLifeTimeInMinutes);
                            AddAuthenticationToken(authentication, authenticationUpdateService);
                            user.Token = token;

                            #region Document Token
                            AuthenticationToken authenticationDocument = new AuthenticationToken() { CreateDate = DateTime.Now, ExpirationDate = DateTime.Now.AddMinutes(15), Email = email, Password = hashedPassword, Token = AuthenticationUtil.GenerateToken(), Tenant = user.CurrentTenant, ClientType = "DocumentDownload" };
                            AddAuthenticationToken(authenticationDocument, authenticationUpdateService);
                            user.DocumentDownloadToken = authenticationDocument.Token;

                            if (parameters.GetInvalidDocumentToken)
                            {
                                AuthenticationToken invalidDocumentToken = new AuthenticationToken() { CreateDate = DateTime.Now, ExpirationDate = DateTime.Now.AddMinutes(-5), Email = email, Password = hashedPassword, Token = AuthenticationUtil.GenerateToken(), Tenant = user.CurrentTenant, ClientType = "DocumentDownload" };
                                AddAuthenticationToken(invalidDocumentToken, authenticationUpdateService);
                                user.InvalidDocumentToken = invalidDocumentToken.Token;
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        user.IsTwoFactorAuthenticationRequired = true;
                    }
                }

                if (amitalUser != null)
                {
                    user.UserId = amitalUser.Id;
                    user.Tenant = amitalUser.Tenant;
                }

                user.HtmlVersion = GetHtmlVersion();
                user.IsAdmin = IsUserAdmin(email, tenant) || customerCare;
            }

            int executionTime = (int)((DateTime.Now.Ticks - DateBeforePostLoginData.Ticks) / TimeSpan.TicksPerMillisecond);
            AddServerTimeToHeaderRespose(executionTime);

            AuthenticationMixPanelService.CreateLoginEventForMixPanel(parameters, tenant);
            return user;
        }

        private void AddAuthenticationToken(AuthenticationToken authentication, AuthenticationTokenUpdateService authenticationUpdateService)
        {
            AuthenticationTokenPM authenticationPM = new AuthenticationTokenPM(authentication);
            authenticationPM.ChangeSetOp = ChangeSetOperation.Insert;
            authenticationUpdateService.Update(authenticationPM, true);

            string cacheKey = $"Token_({authentication.Token})";
            _memoryCache.Set(cacheKey, authentication, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)  // Set absolute expiration time
            });
        }

        private void MapHasLogboxAccessPrivateLabelTenants(List<CompanyLogin> loginsList, List<string> logboxAccessiblePrivateLabelTenantsIds)
        {
            foreach (CompanyLogin companyLogin in loginsList)
            {
                if (companyLogin.PrivateLabelId != null && logboxAccessiblePrivateLabelTenantsIds.Contains(companyLogin.PrivateLabelId))
                {
                    companyLogin.HasLogboxAccess = true;
                }
            }
        }

        private UserData? ValidateUser(string name, string password, int tenant, string computerId, out string? userData, bool isUser, string cardId, bool byToken, string via, string clientType, bool isFromCTool, bool IsFromCargoTracking, bool OneTimePassword)
        {
            ContactPasswordPM? contactPassword = null;
            UserData? user = null;
            userData = null;
            bool customerCare = false;
            bool distributor = false;
            UserPM? amitalUser = null;
            GlobalContactPM? member = null;

            name = name.ToLower();
            if (name.Contains("system@"))
            {
                return user;
            }

            string hashedPassword = byToken ? password : PasswordGenerator.GetHashedPassword(name, password);
            bool isHashPassword = byToken;

            if (!isHashPassword)
            {
                var passResult = ResolvePassword(password);
                if (passResult != null)
                {
                    hashedPassword = passResult.Password;
                    isHashPassword = passResult.isHashPassword;
                }
            }

            GlobalContactPM? contact = globalContactQueryService.GetMulti(d => d.GlobalTenantId == 0 && d.InActive == false && d.Email == name).FirstOrDefault();

            if (contact != null)
            {
                UserQueryService userQueryService = new UserQueryService(amitalCloudContext);
                amitalUser = userQueryService.GetSingle(contact.Id, false, false);

                if (amitalUser?.Tenant == 0)
                {
                    distributor = amitalUser.IsDistributor;
                    customerCare = !amitalUser.IsDistributor;
                }
            }
            else
            {
                List<GlobalContactPM> globalContacts = globalContactQueryService.GetMulti(m => m.Email == name && m.InActive == false && m.GlobalTenantId == tenant);
                if (globalContacts.Count > 1)
                {
                    contact = globalContactQueryService.GetMulti(m => m.Email == name && m.InActive == false && m.GlobalTenantId == tenant && m.IsUser == isUser).FirstOrDefault();
                }
                else
                {
                    contact = globalContacts.FirstOrDefault();
                }
            }
            if (contact != null)
            {
                if (!customerCare || IscustomerCareIpAuthenticated())
                {
                    string pass = isHashPassword ? hashedPassword : password;
                    ContactPassword contactPasswordPOCO = AuthenticationUtil.VerifyContactPassword(name, pass, isHashPassword);

                    if (contactPasswordPOCO != null)
                    {
                        contactPassword = new ContactPasswordPM(contactPasswordPOCO);
                        CheckLockedUser(contactPassword, clientType);

                        if ((!contactPassword.IsLocked || clientType == "Web") && (!contactPassword.MustChangePassword || OneTimePassword))
                        {
                            member = globalContactQueryService.GetMulti(m => m.Email == name && m.GlobalTenantId == tenant && m.InActive == false).FirstOrDefault();
                            if (member == null)
                            {
                                member = globalContactQueryService.GetMulti(m => m.Email == name && m.GlobalTenantId == 0).FirstOrDefault();
                            }
                            if (member == null)
                            {
                                List<GlobalContactPM> globalcontacts = globalContactQueryService.GetMulti(m => m.Email == name && m.InActive == false && (m.IsUser == true || m.InternetAccess == true) && m.GlobalTenantId == tenant);
                                if (globalcontacts.Count > 1)
                                {
                                    if (isUser)
                                    {
                                        member = globalcontacts.Where(m => m.Email == name && m.InActive == false && m.IsUser == true && m.GlobalTenantId == tenant).FirstOrDefault();
                                    }
                                    else
                                    {
                                        member = globalcontacts.Where(m => m.Email == name && m.InActive == false && m.InternetAccess == true && m.GlobalTenantId == tenant).FirstOrDefault();
                                    }
                                }
                                else
                                {
                                    member = globalcontacts.FirstOrDefault();
                                }
                            }

                            ContactPasswordUpdateService contactPasswordUpdateService = new ContactPasswordUpdateService(tenant);

                            if (member != null)
                            {
                                if (contactPassword.NumberOfRetries > 0)
                                {
                                    UpdateContactPassword(contactPassword, contactPasswordUpdateService, numberOfRetries: 0);
                                }

                                RoleQuery roleQuery = new RoleQuery(tenant);
                                List<RolePM> allRoles = roleQuery.GetRolesForContact(contact.Id, contact.GlobalTenantId).ToList();
                                List<RolePM> allCustomRoles = allRoles.Where(d => d.IsCustomRole == true).ToList();

                                List<string> allRolesIds = allRoles.Select(s => s.Id).ToList();

                                foreach (RolePM item in allCustomRoles)
                                {
                                    if (allRolesIds.Contains(item.ParentRoleId))
                                    {
                                        allRolesIds.Remove(item.ParentRoleId);
                                    }
                                }

                                ContactInfo myContactInfo = new ContactInfo()
                                {
                                    Tenant = tenant,
                                    ContactEmail = contact.Email,
                                    RolesIds = allRolesIds,
                                };

                                CacheManager.CacheWrapper.Insert(contact.Email + tenant, myContactInfo, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                            }
                            else
                            {
                                contactPassword.NumberOfRetries++;
                                UpdateContactPassword(contactPassword, contactPasswordUpdateService, contactPassword.NumberOfRetries);
                            }
                        }
                    }
                }

                IAmitalCloudContext myAmitalCloudContext = AmitalCloudContext.GetContext(contact.GlobalTenantId);

                if (member != null)
                {
                    if (!string.IsNullOrEmpty(cardId))
                    {
                        CardContactQueryService cardContactQueryService = new CardContactQueryService(myAmitalCloudContext);
                        CardContactPM cardContact = cardContactQueryService.GetMulti(d => d.ContactId == member.Id && d.CardId == cardId).FirstOrDefault();
                        if (cardContact == null)
                        {
                            GlobalContactPM member2 = globalContactQueryService.GetMulti(m => m.Email == name && m.InActive == false && (m.IsUser == true || m.InternetAccess == true) && m.GlobalTenantId == tenant).FirstOrDefault();
                            if (member2 != null)
                            {
                                member = member2;
                            }
                        }
                    }

                    user = new UserData()
                    {
                        UserName = name,
                        Id = member.Id,
                        Name = name,
                        CurrentTenant = tenant,
                        IsUser = isUser,
                    };
                }

                if (user != null)
                {
                    CardPM? card = null;
                    string? userAgent = !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"]) ? (_httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString().Length <= 500 ? _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() : _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString().Substring(0, 500)) : null;

                    if (!isFromCTool)
                    {
                        if (isUser)
                        {
                            UserLoginLogPM userLog = new UserLoginLogPM()
                            {
                                Id = IdCounter.GetNumber("UserLoginLog", tenant).ToString(),
                                Tenant = tenant,
                                Browser = getBrowserType(),
                                IP = AuthenticationUtil.GetIP4Address(),
                                UserId = user.Id,
                                GMTDateTime = DateTime.Now,
                                LocalDateTime = TenantServerConfigration.GetCurrentDateTime(tenant),
                                UserAgent = userAgent,
                                ComputerId = computerId,
                                ChangeSetOp = ChangeSetOperation.Insert,
                            };
                            string? currentIP = _httpContextAccessor.HttpContext?.Request.Headers["X-Real-IP"];
                            if (!string.IsNullOrEmpty(currentIP))
                            {
                                userLog.Browser = userLog.Browser.ToUpper();
                            }
                            using (TransactionScope scope = TransactionFactory.GetNewTransaction(TimeSpan.FromMinutes(10)))
                            {
                                UserLastLoginQueryService userLastLoginQueryService = new UserLastLoginQueryService(myAmitalCloudContext);
                                UserLastLoginPM lastLogin = userLastLoginQueryService.GetSingle(user.Id, false, false);
                                if (lastLogin == null)
                                {
                                    lastLogin = new UserLastLoginPM()
                                    {
                                        Id = user.Id,
                                        Tenant = tenant,
                                        ComputerId = computerId,
                                        WorkEnvironment = AmitalCloudSettingConfigration.GetWorkEnvironment(),
                                        IP = AuthenticationUtil.GetIP4Address(),
                                        ChangeSetOp = ChangeSetOperation.Insert
                                    };
                                    UserLastLoginUpdateService userLastLoginUpdateService = new UserLastLoginUpdateService(myAmitalCloudContext);
                                    userLastLoginUpdateService.Update(lastLogin, true);
                                }

                                user.LastLoginDateTime = lastLogin.LoginDateTime;
                                lastLogin.IP = AuthenticationUtil.GetIP4Address();
                                lastLogin.LoginDateTime = TenantServerConfigration.GetCurrentDateTime(tenant);
                                lastLogin.Tenant = tenant;

                                UserLoginLogUpdateService userLoginLogUpdateService = new UserLoginLogUpdateService(myAmitalCloudContext);
                                userLoginLogUpdateService.Update(userLog, true);
                                scope.Complete();
                            }
                        }
                        else
                        {
                            CardContactQueryService cardContactQueryService = new CardContactQueryService(myAmitalCloudContext);
                            CardQueryService cardQueryService = new CardQueryService(myAmitalCloudContext);
                            CardContactPM cardContact = cardContactQueryService.GetMulti(d => d.ContactId == member.Id && d.CardId == cardId).FirstOrDefault();
                            if (cardContact != null)
                            {
                                card = cardQueryService.GetSingle(cardId, false, false);
                                card.SharedLogisticsInvitationStatusCode = !IsFromCargoTracking ? "3" : card.SharedLogisticsInvitationStatusCode;
                                card.CargoTrackingInvitationStatusCode = IsFromCargoTracking ? "3" : card.CargoTrackingInvitationStatusCode;
                                card.LastLoginDate = TenantServerConfigration.GetCurrentDateTime(tenant);
                                cardContact.LastLoginDate = TenantServerConfigration.GetCurrentDateTime(tenant);
                                user.CardId = card.Id;
                                user.CardType = card.PartnerTypeId;

                                if (via == "Mobile")
                                {
                                    card.IsActiveForMobile = true;
                                }
                            }

                            if (card != null)
                            {
                                string activity = card.PartnerTypeId == "CS" ? "Customer Access" : "Agent Access";
                                CreateSharedLogisticsContactLastLogin(via, user, card);
                            }

                            ContactLoginLogPM contactLog = new ContactLoginLogPM()
                            {
                                Id = IdCounter.GetNumber("ContactLoginLog", tenant).ToString(),
                                Tenant = tenant,
                                Browser = getBrowserType(),
                                IP = AuthenticationUtil.GetIP4Address(),
                                ContactId = user.Id,
                                GMTDateTime = DateTime.Now,
                                LocalDateTime = TenantServerConfigration.GetCurrentDateTime(tenant),
                                ContactAgent = userAgent,
                                ComputerId = computerId,
                                Via = via,
                                ChangeSetOp = ChangeSetOperation.Insert,
                            };

                            ContactLastLoginQueryService contactLastLoginQueryService = new ContactLastLoginQueryService(myAmitalCloudContext);
                            ContactLastLoginPM lastLogin = contactLastLoginQueryService.GetSingle(user.Id, false, false);
                            if (lastLogin == null)
                            {
                                lastLogin = new ContactLastLoginPM()
                                {
                                    Id = user.Id,
                                    Tenant = tenant,
                                    LoginDateTime = TenantServerConfigration.GetCurrentDateTime(tenant),
                                    ChangeSetOp = ChangeSetOperation.Insert
                                };
                                ContactLastLoginUpdateService contactLastLoginUpdateService = new ContactLastLoginUpdateService(myAmitalCloudContext);
                                contactLastLoginUpdateService.Update(lastLogin, true);
                            }

                            user.DigitalLastLoginDateTime = lastLogin.LoginDateTime;
                            lastLogin.ComputerId = computerId;
                            lastLogin.LoginDateTime = TenantServerConfigration.GetCurrentDateTime(tenant);

                            ContactLoginLogUpdateService contactLoginLogUpdateService = new ContactLoginLogUpdateService(myAmitalCloudContext);
                            contactLoginLogUpdateService.Update(contactLog, true);
                        }
                    }

                    if (amitalUser != null)
                    {
                        distributor = amitalUser.IsDistributor;
                        customerCare = !amitalUser.IsDistributor;
                    }

                    string SimplogGuid = Guid.NewGuid().ToString("N");
                    userData = $"{member.Email}:{member.Id}" + (card != null ? $":{card.Id}:{card.PartnerTypeId}" : "") + $":{SimplogGuid}";
                }
            }

            if (contactPassword != null)
            {
                user.NumberOfRetries = contactPassword.NumberOfRetries;
            }
            return user;
        }

        private void CreateSharedLogisticsContactLastLogin(string via, UserData user, CardPM card)
        {
            IAmitalCloudContext cardContext = AmitalCloudContext.GetContext(card.Tenant);
            string loggedVia = string.IsNullOrEmpty(via) ? "PC" : via;
            SharedLogisticsContactLastLoginQueryService sharedLogisticsContactLastLoginQueryService = new SharedLogisticsContactLastLoginQueryService(cardContext);
            SharedLogisticsContactLastLoginPM sharedContactLastLogin = sharedLogisticsContactLastLoginQueryService.GetMulti(a => a.ContactId == user.Id && a.CardId == card.Id && a.PartnerTypeId == card.PartnerTypeId && a.Via == loggedVia).FirstOrDefault();
            if (sharedContactLastLogin == null)
            {
                sharedContactLastLogin = new SharedLogisticsContactLastLoginPM()
                {
                    ContactId = user.Id,
                    CardId = card.Id,
                    PartnerTypeId = card.PartnerTypeId,
                    Via = via,
                    Tenant = card.Tenant,
                    LoginDateTime = TenantServerConfigration.GetCurrentDateTime(card.Tenant),
                    ChangeSetOp = ChangeSetOperation.Insert
                };
            }
            else
            {
                sharedContactLastLogin.LoginDateTime = TenantServerConfigration.GetCurrentDateTime(card.Tenant);
                sharedContactLastLogin.ChangeSetOp = ChangeSetOperation.Update;
            }

            SharedLogisticsContactLastLoginUpdateService sharedLogisticsContactLastLoginUpdateService = new SharedLogisticsContactLastLoginUpdateService(cardContext);
            sharedLogisticsContactLastLoginUpdateService.Update(sharedContactLastLogin, true);
        }

        private void AddFailedLoginLog(UserData data)
        {
            FailedLoginLogUpdateService failedLoginLogUpdateService = new FailedLoginLogUpdateService(data.Tenant);
            FailedLoginLogPM failedLoginLog = new FailedLoginLogPM()
            {
                Id = IdCounter.GetNumber("FailedLoginLog", data.Tenant).ToString(),
                Browser = getBrowserType(),
                IP = AuthenticationUtil.GetIP4Address(),
                GMTDateTime = DateTime.Now,
                UserAgent = !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Request.Headers["User-Agent"]) ? (_httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString().Length <= 500 ? _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() : _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString().Substring(0, 500)) : null,
                Email = data.UserName,
                ChangeSetOp = ChangeSetOperation.Insert
            };

            if (data.Param1) failedLoginLog.Reason = "Wrong Email address";
            else if (data.InValidMailOrPassword) failedLoginLog.Reason = "Wrong Password";
            else if (data.IpRestricted) failedLoginLog.Reason = "Unauthorized IP address";
            else if (data.InValidCaptcha) failedLoginLog.Reason = "Valid Captcha";
            else if (data.IsLocked) failedLoginLog.Reason = "Locked User";

            data.Param1 = false;
            string? currentIP = _httpContextAccessor.HttpContext?.Request.Headers["X-Real-IP"];
            if (!string.IsNullOrEmpty(currentIP)) failedLoginLog.Browser = failedLoginLog.Browser.ToUpper();

            failedLoginLogUpdateService.Update(failedLoginLog, true);
        }

        private bool CheckLoginSecurityPolicy(int tenant, UserData user, UserPM amitalUser, TenantLoginPolicyPM securityPolicy)
        {
            string ipAddress = AuthenticationUtil.GetIP4Address();
            string? deviceDescription = !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Request.Headers["User-Agent"]) ? (_httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString().Length <= 500 ? _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() : _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString().Substring(0, 500)) : "";
            string? TwoFactorkey = _httpContextAccessor.HttpContext?.Request.Headers["TwoFactorkey"];

            bool IsTwoFactorAuthenticationRequired = false;
            if (securityPolicy != null && securityPolicy.LoginPolicyCode != "NOREST")
            {
                if (securityPolicy.LoginPolicyCode == "TFAUTH")
                {
                    if (!user.IsUser ||
                        (securityPolicy.IsEnabledForSpecificUsers && !amitalUser.IsTwoFactorAuthenticationEnabled)
                        || (securityPolicy.ExcludeInternalIPs && !string.IsNullOrEmpty(securityPolicy.TwoFactorInternalIPs) && securityPolicy.TwoFactorInternalIPs.Contains(ipAddress)))
                    {
                        return false;
                    }

                    ContactQueryService contactQueryService = new ContactQueryService(amitalCloudContext);
                    ContactPM loggedContact = contactQueryService.GetSingle(amitalUser.Id, false, false);

                    TwoFactorAuthenticationDevicePM? device = null;
                    if (!string.IsNullOrEmpty(TwoFactorkey))
                    {
                        TwoFactorAuthenticationDeviceQueryService twoFactorAuthenticationDeviceQueryService = new TwoFactorAuthenticationDeviceQueryService(amitalCloudContext);
                        List<TwoFactorAuthenticationDevicePM> devices = twoFactorAuthenticationDeviceQueryService.GetMulti(record => record.InActive == false && record.UserId == loggedContact.Id && record.Tenant == tenant);

                        device = devices.FirstOrDefault(d => TwoFactorkey.Contains(d.TwoFactorkey));
                    }

                    TwoFactorAuthenticationDeviceUpdateService twoFactorAuthenticationDeviceUpdateService = new TwoFactorAuthenticationDeviceUpdateService(tenant);

                    if (device == null || (device != null && device.InActive))
                    {
                        Random generator = new Random();
                        string authCode = generator.Next(0, 1000000).ToString("D6");
                        string key = StringHelper.GetRandomString(40);
                        device = new TwoFactorAuthenticationDevicePM()
                        {
                            Id = IdCounter.GetNumber("TwoFactorAuthenticationDevice", tenant),
                            TwoFactorkey = key,
                            Tenant = tenant,
                            CreateDate = TenantServerConfigration.GetCurrentDateTime(tenant),
                            UpdateDate = TenantServerConfigration.GetCurrentDateTime(tenant),
                            UserId = amitalUser.Id,
                            CodeExpirationDate = TenantServerConfigration.GetCurrentDateTime(tenant).AddMinutes(10),
                            AuthenticationCode = authCode,
                            DeviceDescription = deviceDescription,
                            ChangeSetOp = ChangeSetOperation.Insert
                        };

                        IsTwoFactorAuthenticationRequired = true;
                        user.IsTwoFactorAuthenticationRequired = true;
                        user.CodeExpirationDate = device.CodeExpirationDate;
                        user.TwoFactorkey = device.TwoFactorkey;
                        user.UserMobileNumber = GetContactMaskedMobileNumber(loggedContact);

                        twoFactorAuthenticationDeviceUpdateService.Update(device, true);

                        AddVerificationCodeSMSLog(loggedContact, device);
                    }
                    else
                    {
                        if (device.IsVerified)
                        {
                            device.DeviceDescription = deviceDescription;
                            device.LastLoginIP = ipAddress;
                            device.LastLoginDate = TenantServerConfigration.GetCurrentDateTime(tenant);
                            device.ChangeSetOp = ChangeSetOperation.Update;
                            twoFactorAuthenticationDeviceUpdateService.Update(device, true);
                        }
                        else
                        {
                            if (device.CodeExpirationDate < TenantServerConfigration.GetCurrentDateTime(tenant))
                            {
                                Random generator = new Random();
                                string authCode = generator.Next(0, 1000000).ToString("D6");
                                device.AuthenticationCode = authCode;
                                device.CodeExpirationDate = TenantServerConfigration.GetCurrentDateTime(tenant).AddMinutes(10);
                                device.UpdateDate = TenantServerConfigration.GetCurrentDateTime(tenant);
                                device.ChangeSetOp = ChangeSetOperation.Update;
                                twoFactorAuthenticationDeviceUpdateService.Update(device, true);

                                AddVerificationCodeSMSLog(loggedContact, device);
                            }

                            IsTwoFactorAuthenticationRequired = true;
                            user.UserMobileNumber = GetContactMaskedMobileNumber(loggedContact);
                            user.IsTwoFactorAuthenticationRequired = true;
                            user.CodeExpirationDate = TenantServerConfigration.GetCurrentDateTime(tenant).AddMinutes(10);
                            user.TwoFactorkey = device.TwoFactorkey;
                        }
                    }
                }
                else if (securityPolicy.LoginPolicyCode == "COMPIP")
                {
                    string[] authenticatedIPs = (securityPolicy.AllowedIPs ?? string.Empty).Split(',');
                    if (!authenticatedIPs.Contains(ipAddress) && !(Environment.GetEnvironmentVariable("ASPNETCORE_IIS_HTTPAUTH") != null && ipAddress == "::1"))
                    {
                        user.IpRestricted = true;
                        user.HasError = true;
                    }
                }
            }
            return IsTwoFactorAuthenticationRequired;
        }

        private string GetContactMaskedMobileNumber(ContactPM loggedContact)
        {
            if (!string.IsNullOrEmpty(loggedContact.Mobile))
            {
                var firstDigits = loggedContact.Mobile.Substring(0, 4);
                var lastDigits = loggedContact.Mobile.Substring(loggedContact.Mobile.Length - 2, 2);
                var requiredMask = new String('*', loggedContact.Mobile.Length - firstDigits.Length - lastDigits.Length);
                var maskedString = string.Concat(firstDigits, requiredMask, lastDigits);

                return maskedString;
            }
            else
            {
                return string.Empty;
            }
        }

        private UserData CheckCaptchaState(LoginParameters loginParameters, bool withoutCheckUsed = false)
        {
            CaptchaHelper captchaHelper = new CaptchaHelper();
            UserData data = new UserData();
            if (!loginParameters.IsMobileLogin && loginParameters.ClientType == "Web")
            {
                bool isCheckCaptchaCode = !string.IsNullOrEmpty(loginParameters.CaptchaCode) && !string.IsNullOrEmpty(loginParameters.CaptchaKey);
                ContactPasswordPM contactPassword = contactPasswordQueryService.GetMulti(c => c.Email.ToLower() == loginParameters.Email).FirstOrDefault();

                if (!isCheckCaptchaCode && contactPassword != null && contactPassword.NumberOfRetries++ >= 5)
                {
                    DateTime dateNowBefor5Minutes = DateTime.Now.AddMinutes(-5);
                    if (contactPassword.LockDateTime > dateNowBefor5Minutes) isCheckCaptchaCode = true;
                    if (!isCheckCaptchaCode)
                    {
                        int countCaptchaKey = new CaptchaKeyQueryService(0).GetMulti(a => a.Email == loginParameters.Email && a.Activity == "Login" && a.CreateDate >= dateNowBefor5Minutes).Count();
                        if (countCaptchaKey >= 5) isCheckCaptchaCode = true;
                    }
                }

                string userCaptchaKey = contactPassword?.CaptchaKey;

                if (isCheckCaptchaCode && !captchaHelper.CheckCaptchaCodeValidated(loginParameters.CaptchaCode, loginParameters.CaptchaKey, userCaptchaKey, withoutCheckUsed))
                {
                    captchaHelper.AddCaptchaKey(loginParameters.Email, data, "Login");
                    if (contactPassword != null)
                    {
                        UpdateContactPassword(contactPassword, new ContactPasswordUpdateService(tenant), captchaKey: data.CaptchaKey);
                    }
                }
            }
            return data;
        }

        private List<string> GetLogboxAccessiblePrivateLabelTenantsIds(string url)
        {
            List<string> logboxAccessiblePrivateLabelTenantsIds = new List<string>();
            if (url.Contains("system.logbox.co.il") || url.Contains("pre.logbox.co.il") || url.Contains("localhost"))
            {
                TenantManagmentPrivateLabelsQueryService tenantManagmentPrivateLabelsQueryService = new TenantManagmentPrivateLabelsQueryService(globalContext);
                logboxAccessiblePrivateLabelTenantsIds = tenantManagmentPrivateLabelsQueryService.GetMulti(a => a.InActive == false && a.HasLogboxAccess, a => a.Id);
            }
            return logboxAccessiblePrivateLabelTenantsIds;
        }

        private bool IscustomerCareIpAuthenticated()
        {
            string[] authenticatedIPs = (AmitalCloudSettings.CustomerCareIP ?? string.Empty).Split(',');

            if (authenticatedIPs.Contains("*") || (Environment.GetEnvironmentVariable("ASPNETCORE_IIS_HTTPAUTH") != null && (_httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() == "::1" || _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() == "127.0.0.1")))
            {
                return true;
            }
            else
            {
                string? currentIP = _httpContextAccessor.HttpContext?.Request.Headers["X-Real-IP"];
                if (string.IsNullOrEmpty(currentIP))
                {
                    currentIP = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                }
                return authenticatedIPs.Contains(currentIP);
            }
        }

        private UserData CheckUserState(string email, string password, ref ContactPasswordPM contactPassword, bool byToken, string clientType)
        {
            if (!string.IsNullOrEmpty(email)) email = email.ToLower();
            UserData userData = new UserData()
            {
                UserName = email,
            };
            string hashedPassword = byToken ? password : PasswordGenerator.GetHashedPassword(email, password);
            bool IsOneTimePassword = false;
            bool isHashPassword = byToken;

            if (!isHashPassword)
            {
                var passResult = ResolvePassword(password);
                if (passResult != null)
                {
                    hashedPassword = passResult.Password;
                    IsOneTimePassword = passResult.IsOneTimePassword;
                    isHashPassword = passResult.isHashPassword;
                }
            }

            string pass = isHashPassword ? hashedPassword : password;
            ContactPassword contactPasswordPOCO = AuthenticationUtil.VerifyContactPassword(email, pass, isHashPassword);

            if (contactPasswordPOCO != null)
            {
                contactPassword = new ContactPasswordPM(contactPasswordPOCO);
                CheckLockedUser(contactPassword, clientType);

                GlobalContactPM contact = globalContactQueryService.GetMulti(d => d.GlobalTenantId == 0 && d.InActive == false && d.Email.ToLower() == email).FirstOrDefault();
                bool customerCare = false;
                bool distributor = false;
                UserPM amitalUser = null;
                if (contact != null)
                {
                    UserQueryService userQueryService = new UserQueryService(amitalCloudContext);
                    amitalUser = userQueryService.GetSingle(contact.Id, false, false);
                    if (amitalUser?.Tenant == 0)
                    {
                        distributor = amitalUser.IsDistributor;
                        customerCare = !amitalUser.IsDistributor;
                    }
                    userData.Technology = amitalUser.Technology;
                    userData.IsUser = contact.IsUser;
                }

                userData.IsLocked = contactPassword.IsLocked;
                userData.MustChangePassword = !IsOneTimePassword && contactPassword.MustChangePassword;

                if (customerCare)
                {
                    userData.IpRestricted = !IscustomerCareIpAuthenticated();
                }
            }
            else
            {
                contactPassword = contactPasswordQueryService.GetMulti(d => d.Email.ToLower() == email).FirstOrDefault();
                if (contactPassword != null)
                {
                    contactPassword.NumberOfRetries++;
                    UpdateContactPassword(contactPassword, new ContactPasswordUpdateService(tenant), contactPassword.NumberOfRetries);
                }
                else userData.Param1 = true;

                userData.InValidMailOrPassword = !byToken;
            }

            userData.HasError = (userData.InValidMailOrPassword || userData.IpRestricted || (userData.IsLocked && clientType != "Web") || userData.MustChangePassword);

            if (contactPassword != null)
            {
                userData.NumberOfRetries = contactPassword.NumberOfRetries;
            }

            return userData;
        }

        private void CheckLockedUser(ContactPasswordPM contact, string clientType)
        {
            bool? IsLocked = null;
            int? NumberOfRetries = null;

            if (contact.IsLocked)
            {
                if (contact.LockDateTime != null)
                {
                    TimeSpan timeElapsed = (DateTime.Now - contact.LockDateTime.Value);
                    if (timeElapsed.TotalMinutes > 30 || clientType == "Web")
                    {
                        IsLocked = false;
                        NumberOfRetries = 0;
                    }
                }
                else
                {
                    IsLocked = false;
                }
            }
            else if (clientType == "Web")
            {
                IsLocked = false;
                NumberOfRetries = 0;
                contact.CaptchaKey = null;
            }

            if (IsLocked != null || NumberOfRetries != null)
            {
                UpdateContactPassword(contact, new ContactPasswordUpdateService(tenant), NumberOfRetries, IsLocked);
            }
        }

        private string? AddVerificationCodeSMSLog(ContactPM loggedContact, TwoFactorAuthenticationDevicePM device)
        {
            if (!string.IsNullOrEmpty(loggedContact.Mobile) && loggedContact.Mobile.Length > 7)
            {
                int tenant = device.Tenant;
                ObjectTableQueryService objectTableQueryService = new ObjectTableQueryService(amitalCloudContext);
                ObjectTablePM objectTable = objectTableQueryService.GetMultiFromCache(nameof(TwoFactorAuthenticationDevice) + 0, d => d.Name == nameof(TwoFactorAuthenticationDevice) && d.Tenant == 0).FirstOrDefault();

                string myObjectTableId = objectTable?.Id;
                string environment = AmitalCloudSettingConfigration.IsLogBoxEnvironment() ? "Logbox" : AmitalCloudSettings.WorkEnvironment == "cloud" ? "Cloud" : "Amital";
                string body = $"Please use the code {device.AuthenticationCode} to verify your {environment} Account";
                byte[] bytearray = Encoding.ASCII.GetBytes(body);

                DocumentPM document = new DocumentPM()
                {
                    CreateDate = DateTime.Now,
                    Extension = "txt",
                    FileSize = bytearray.Length,
                    Tenant = Convert.ToInt32(tenant),
                    Id = IdCounter.GetNumber("Document", tenant),
                    HasFile = true,
                    Folder = "MobileSMS",
                    ChangeSetOp = ChangeSetOperation.Insert,
                };
                DocumentUpdateService documentUpdateService = new DocumentUpdateService(tenant);
                documentUpdateService.Update(document, true);

                string subject = "Two-Factor Authentication Verification SMS";
                CommunicationLogPM commLog = new CommunicationLogPM()
                {
                    Id = IdCounter.GetNumber("CommunicationLog", tenant),
                    LastStatusDate = TenantServerConfigration.GetCurrentDateTime(tenant),
                    LastStatusDateUTC = DateTime.UtcNow,
                    To = loggedContact.Mobile,
                    InOut = "O",
                    From = "Two-Factor Authentication",
                    Subject = "Two-Factor Authentication Verification SMS",
                    Tenant = tenant,
                    CommunicationLogTypeCode = "SMS",
                    CreateDate = TenantServerConfigration.GetCurrentDateTime(tenant),
                    CommunicationStatusTypeCode = "W",
                    CreatedByUserId = loggedContact.Id,
                    DocumentId = document.Id,
                    SearchFields = $"{loggedContact.Mobile},SMS,O,{subject}",
                    CreateDateUTC = DateTime.UtcNow,
                    EntityId = device.TwoFactorkey,
                    ObjectTableId = myObjectTableId,
                    IsSecured = true,
                    ChangeSetOp = ChangeSetOperation.Insert
                };
                CommunicationLogUpdateService communicationLogUpdateService = new CommunicationLogUpdateService(tenant);
                communicationLogUpdateService.Update(commLog, true);

                BlobFileInfo fileInfo = new BlobFileInfo()
                {
                    FileName = document.Id,
                    FolderName = document.Folder,
                    Extension = document.Extension,
                    Tenant = tenant,
                    FileSize = bytearray.Length,
                };
                //todo
                //IBlobService storageservice = ContainerAccessor.Container.Resolve(typeof(IBlobService), "StorageService", new ParameterOverride("", 1)) as IBlobService;
                // storageservice.Write(bytearray, fileInfo);

                DbQueueService queueservice = new DbQueueService();
                queueservice.InitializeQueue("MobileSMS", 0);
                Dictionary<string, string> param = new Dictionary<string, string>() { { "LogId", commLog.Id }, { "Tenant", tenant.ToString() } };
                queueservice.Send(param, tenant);

                return GetContactMaskedMobileNumber(loggedContact);
            }

            return null;
        }

        private List<string>? GetTenantLogoUri(int companyId, int mobileVersion)
        {
            try
            {
                List<string> ImageInfo = new List<string>();

                BlobFileInfo fileInfo = new BlobFileInfo()
                {
                    FileName = $"verysmalllogo{companyId}",
                    FolderName = "logos",
                    Extension = "png",
                    Tenant = companyId,
                };

                //todo
                //IBlobService storageservice = ContainerAccessor.Container.Resolve(typeof(IBlobService), "StorageService", new ParameterOverride("", 1)) as IBlobService;
                IBlobService storageservice = null;
                byte[] datainByte = storageservice.Read(fileInfo);

                if (datainByte == null)
                {
                    fileInfo.Extension = "jpg";
                    datainByte = storageservice.Read(fileInfo);
                }

                if (datainByte == null)
                {
                    fileInfo.FileName = $"smalllogo{companyId}";
                    datainByte = storageservice.Read(fileInfo);
                }

                if (datainByte != null)
                {
                    if (mobileVersion > 1)
                    {
                        ImageInfo.Add(GetUrlImage(datainByte, fileInfo.Extension));
                        ImageInfo.Add(fileInfo.Extension);
                    }
                    else
                    {
                        ImageInfo.Add(GetUrlImage(datainByte, "jpg"));
                        ImageInfo.Add("jpg");
                    }
                    return ImageInfo;
                }
                else return null;
            }
            catch (Exception e)
            {
                ExceptionHandler.HandleException(e, DateTime.Now, companyId, "", "", "Uploader : DownloadFile Method", null);
                return null;
            }
        }

        private static string GetUrlImage(byte[] datainByte, string extension) => $"data:image/{extension};base64,{Convert.ToBase64String(datainByte, 0, datainByte.Length)}";

        private string GetHtmlVersion()
        {
            SettingQueryService settingQueryService = new SettingQueryService(globalContext);
            Setting mySettings = settingQueryService.GetFirst();
            return mySettings?.HtmlVersion ?? "";
        }

        private void AddServerTimeToHeaderRespose(int executionTime)
        {
            if (!string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Response.Headers["ServerTime"]))
            {
                _httpContextAccessor.HttpContext.Response.Headers["ServerTime"] = executionTime.ToString();
            }
            else
            {
                _httpContextAccessor.HttpContext?.Response?.Headers.Add("ServerTime", executionTime.ToString());
            }
        }

        private void SetSessionPolicy(UserData data)
        {
            SessionPolicyQueryService sessionPolicyQueryService = new SessionPolicyQueryService(globalContext);
            SessionPolicy sessionPolicy = sessionPolicyQueryService.GetFirst();
            if (sessionPolicy != null)
            {
                data.WebTokenLifeTimeInMinutes = sessionPolicy.WebTokenLifeTimeInMinutes;
                data.WebTokenExpirationWarningInMinutes = sessionPolicy.WebTokenExpirationWarningInMinutes;
            }
        }

        private PasswordParameter ResolvePassword(string password)
        {
            PasswordParameter? result = null;

            if (!string.IsNullOrEmpty(password) && (password.Contains("@OneTimePassword") || password.Contains(@"HashPassword")))
            {
                var passwordarray = password.Contains("@OneTimePassword") ? password.Split(new string[] { "@OneTimePassword" }, StringSplitOptions.None) : password.Split(new string[] { "@HashPassword" }, StringSplitOptions.None);
                if (passwordarray.Length > 0)
                {
                    result = new PasswordParameter();
                    result.Password = passwordarray[0];
                    if (password.Contains("@OneTimePassword")) result.IsOneTimePassword = true;
                    result.isHashPassword = true;
                }
            }
            return result;
        }

        private CompanyLogin CreateCompanyLogin(string email, string companyName, bool isUser, int tenantId, string cardId, string cardType, string contactId, bool licensedUser, bool internetAccess, string privateLabelId, string customerName = null)
            => new CompanyLogin
            {
                Email = email,
                CompanyName = companyName,
                IsUser = isUser,
                Tenant = tenantId,
                CardId = cardId,
                CardType = cardType,
                ContactId = contactId,
                LicensedUser = licensedUser,
                InternetAccess = internetAccess,
                PrivateLabelId = privateLabelId,
                CustomerName = customerName
            };

        private void UpdateContactPassword(ContactPasswordPM contactPassword, ContactPasswordUpdateService contactPasswordUpdateService, int? numberOfRetries = null, bool? isLocked = null, string? captchaKey = null)
        {
            if (numberOfRetries != null)
            {
                contactPassword.NumberOfRetries = (int)numberOfRetries;

                if (contactPassword.NumberOfRetries >= 5)
                {
                    contactPassword.IsLocked = true;
                    contactPassword.LockDateTime = DateTime.Now;
                }
            }
            if (isLocked == false || numberOfRetries == 0)
            {
                contactPassword.LockDateTime = null;
            }
            if (captchaKey != null)
            {
                contactPassword.CaptchaKey = captchaKey;
            }

            contactPassword.ChangeSetOp = ChangeSetOperation.Update;
            contactPasswordUpdateService.Update(contactPassword, true);
        }

        private bool IsUserAdmin(string email, int tenant)
        {
            UserQueryService userQueryService = new UserQueryService(amitalCloudContext);
            List<UserPM> entities = userQueryService.GetMulti(record => record.Contact.Email == email && (record.Tenant == tenant || record.Tenant == 0));
            UserPM loggedUser = entities.Where(a => a.Tenant == tenant).FirstOrDefault() ?? entities.Where(a => a.Tenant == 0).FirstOrDefault();
            return loggedUser?.UserRoles != null && loggedUser.UserRoles.Contains("Administrator");
        }

        private string getBrowserType()
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            var parser = Parser.GetDefault();
            ClientInfo clientInfo = parser.Parse(userAgent);

            string browser = clientInfo.UA.Family;
            string versionMajor = clientInfo.UA.Major;

            string browserType = $"{browser}{versionMajor}";
            return browserType;
        }

        public static bool GetIsBlockingFromDB(HttpContext httpContext)
        {
            bool hasBlockingDBRecords = new GlobalDBQueryService(0)
                .GetMulti(_ => true) 
                .Any();

            string? clientIp = GetClientIpAddress(httpContext);
            string[]? trustedIps = AmitalCloudSettings.CustomerCareIP?.Split(',');

            bool isTrustedIp = trustedIps?.Contains(clientIp) == true;

            return !isTrustedIp && hasBlockingDBRecords;
        }

      
        public static string? GetClientIpAddress(HttpContext httpContext)
        {
            var ip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            return !string.IsNullOrEmpty(ip)
                ? ip
                : httpContext.Connection.RemoteIpAddress?.ToString();
        }

    }
}
