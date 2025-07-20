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
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using AmitalCloud.Infrastructure.Model.Interfaces;
using UAParser;
using Microsoft.Extensions.Caching.Memory;
using static AmitalCloud.Infrastructure.Domain.Helpers.SettingUtil;
using System.Diagnostics;

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
        readonly private CardQueryService cardQueryService;
        readonly private TenantQueryService tenantQueryService;
        readonly private UserQueryService userQueryService;
        readonly private GlobalTenantQueryService globalTenantQueryService;
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
            cardQueryService = new CardQueryService(amitalCloudContext);
            tenantQueryService = new TenantQueryService(amitalCloudContext);
            userQueryService = new UserQueryService(amitalCloudContext);
            globalTenantQueryService = new GlobalTenantQueryService(globalContext);
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        #region Public API
        public static (string? Email, string? Token, string? Error) ExtractAzureAdCredentials(HttpContext httpContext)
        {
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return (null, null, "Authorization header with Bearer token is required.");
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();
            string? email = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                return (null, null, "Email is required for validation.");
            }

            return (email, token, null);
        }
        public UserData AuthenticateUser(LoginParameters loginParameters)
        {
            var url = AmitalCloudSecurityUtility.getLoggedDomain();
            url = url.Split(':')[0];
            TenantManagmentPrivateLabelsPM? privatelabel = GetPrivateLabelIfNeeded(loginParameters, url);

            DateTime DateBeforePostUserValidation = DateTime.Now;
            string email = loginParameters.Email.Trim();
            string password = loginParameters.Password;
            UserData data = new UserData();
            ContactPasswordPM? contactPassword = null;
            
            data = CheckUserState(email.ToLower(), password, ref contactPassword, loginParameters.ByToken, loginParameters.ClientType, loginParameters.IsAzureAdLogin);

            if (!data.HasError)
            {
                data = GetUserTenantLogins(loginParameters, data, email, password, privatelabel, url);
            }
            data = FilterMobileLogins(data, loginParameters); 
            
            int executionTime = (int)((DateTime.Now.Ticks - DateBeforePostUserValidation.Ticks) / TimeSpan.TicksPerMillisecond);
            AddServerTimeToHeaderRespose(executionTime);

            #region PasswordExpirationDate
            if (!loginParameters.IsAzureAdLogin && !data.HasError && loginParameters.ClientType == "Web")
            {
                if (contactPassword == null)
                {
                    contactPassword = GetContactPasswordByEmail(email);
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
                if (data.InValidMailOrPassword || data.IsLocked || data.IpRestricted || data.InvalidEmailAddress)
                {
                    AddFailedLoginLog(data);                 
                }
            }
            return data;
        }
        public UserData LoginUser(LoginParameters parameters, int tenant, bool? isFromCTool = false)
        {
            bool fromCTool = isFromCTool.GetValueOrDefault();
            parameters.Email = parameters.Email.ToLower();

            return ProcessLoginFlow(parameters, tenant, fromCTool);
        }
        #endregion

        #region Authentication Flow
        private UserData ProcessLoginFlow(LoginParameters parameters, int tenant, bool fromCTool)
        {
            var stopwatch = Stopwatch.StartNew();

            if (!parameters.IsAzureAdLogin)
            {
                var user = CheckCaptchaState(parameters, true);
                if (user.InValidCaptcha)
                    return user;
            }

            var email = parameters.Email;
            var password = parameters.Password;

            var contact = GetContact(email, tenant);

            if (contact == null)
                return new UserData() { HasError = true, InValidMailOrPassword = true };

            var amitalUser = GetUser(contact.Id, tenant);

            var validatedUser = ValidateLogin(email, password, tenant, parameters, fromCTool);

            if (validatedUser == null)
                return new UserData() { HasError = true, InValidMailOrPassword = true };

            if (!validatedUser.IsUser)
                HttpContextHelper.SetCookie(validatedUser.UserName, validatedUser.UserId.ToString(), validatedUser.CurrentTenant.ToString(), validatedUser.IsAuthenticated, Guid.NewGuid().ToString("N"));

            SetupBranding(validatedUser);

            validatedUser.Technology = "AG";
            SetupUserSessionPolicy(validatedUser, parameters, tenant);

            var customerCare = amitalUser != null  && amitalUser.Tenant == 0 ? !amitalUser.IsDistributor : false;
            HandleAuthenticationTokens(parameters, tenant, validatedUser, amitalUser, fromCTool, customerCare);

            validatedUser.UserId = amitalUser?.Id ?? validatedUser.UserId;
            validatedUser.Tenant = amitalUser?.Tenant ?? validatedUser.Tenant;
            validatedUser.IsAdmin = IsUserAdmin(email, tenant) || customerCare;

            stopwatch.Stop();
            AddServerTimeToHeaderRespose((int)stopwatch.ElapsedMilliseconds);

            return validatedUser!;
        }
        private void AddFailedLoginLog(UserData data)
        {
            FailedLoginLogUpdateService failedLoginLogUpdateService = new FailedLoginLogUpdateService(data.Tenant);
            FailedLoginLogPM failedLoginLog = new FailedLoginLogPM()
            {
                Id = IdCounter.GetNumber("FailedLoginLog", data.Tenant).ToString(),
                Browser = GetBrowserType(),
                IP = AuthenticationUtil.GetIP4Address(),    
                GMTDateTime = DateTime.Now,
                UserAgent = !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Request.Headers["User-Agent"]) ? (_httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString().Length <= 500 ? _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() : _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString().Substring(0, 500)) : null,
                Email = data.UserName,
                ChangeSetOp = ChangeSetOperation.Insert
            };

            if (data.InvalidEmailAddress) failedLoginLog.Reason = "Wrong Email address";
            else if (data.InValidMailOrPassword) failedLoginLog.Reason = "Wrong Password";
            else if (data.IpRestricted) failedLoginLog.Reason = "Unauthorized IP address";
            else if (data.InValidCaptcha) failedLoginLog.Reason = "Valid Captcha";
            else if (data.IsLocked) failedLoginLog.Reason = "Locked User";

            data.InvalidEmailAddress = false;
            string? currentIP = _httpContextAccessor.HttpContext?.Request.Headers["X-Real-IP"];
            if (!string.IsNullOrEmpty(currentIP)) failedLoginLog.Browser = failedLoginLog.Browser.ToUpper();

            failedLoginLogUpdateService.Update(failedLoginLog, true);
        }
        private void AddServerTimeToHeaderRespose(int executionTime)
        {
            var headers = _httpContextAccessor.HttpContext?.Response?.Headers;
            if (headers == null) return;
            headers["ServerTime"] = executionTime.ToString();
        }
        private void SetupUserSessionPolicy(UserData user, LoginParameters parameters, int tenant)
        {
            TenantLoginPolicyQueryService securityPolicyQueryService = new TenantLoginPolicyQueryService(amitalCloudContext);
            TenantLoginPolicyPM? securityPolicy = securityPolicyQueryService.GetMulti(d => d.Tenant == tenant).FirstOrDefault();

            if (securityPolicy != null)
            {
                user.KeepUserLoggedIn = securityPolicy.KeepUserLoggedIn;

                if (parameters.ClientType == "Web" && !securityPolicy.KeepUserLoggedIn)
                {
                    user.SessionTimeout = securityPolicy.SessionTimeout;
                    SetSessionPolicy(user);
                }
            }
            else if (parameters.ClientType == "Web")
            {
                SetSessionPolicy(user);
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
        UserData HandleLoginResult(List<CompanyLogin> logins, LoginParameters loginParams, string email, string password, TenantManagmentPrivateLabelsPM? pl, List<CompanyLogin> unlicensed)
        {
            if (logins.Count == 1)
            {
                var access = logins.First();

                bool isValidPL = pl != null && access.PrivateLabelId == pl.Id;
                bool isInvalidAccess = (pl == null && !string.IsNullOrEmpty(access.PrivateLabelId) && !access.HasLogboxAccess && !loginParams.IsFromPLSignApp)
                                    || (pl != null && string.IsNullOrEmpty(access.PrivateLabelId));

                if (isValidPL || !isInvalidAccess)
                {
                    var userData = LoginUser(new LoginParameters
                    {
                        Email = email,
                        Password = password,
                        IsUser = access.IsUser,
                        CardId = access.CardId,
                        CardType = access.CardType,
                        ByToken = loginParams.ByToken,
                        IsMobileLogin = loginParams.IsMobileLogin,
                        GetToken = loginParams.GetToken,
                        IsAngularLogin = loginParams.IsAngularLogin,
                        InternalLoginValidationCall = true,
                        ClientType = loginParams.ClientType,
                        IsAzureAdLogin = loginParams.IsAzureAdLogin,
                        AzureAdToken = loginParams.AzureAdToken
                    }, access.Tenant);

                    userData.CompanyLogins = logins;
                    userData.ContactsCount = 1;
                    return userData;
                }

                return new UserData
                {
                    UserName = email,
                    CompanyLogins = new List<CompanyLogin>(),
                    HasError = true,
                    ContactsCount = 0,
                    InActive = true,
                    Unlicensed = unlicensed.Any(),
                    PrivateLablehasZeroTenant = true
                };
            }

            var filtered = loginParams.IsFromPLSignApp
                ? logins.Where(x => x.PrivateLabelId != null).ToList()
                : pl != null
                    ? logins.Where(x => x.PrivateLabelId == pl.Id).ToList()
                    : logins.Where(x => x.PrivateLabelId == null || x.HasLogboxAccess).ToList();

            if (filtered.Count == 1)
            {
                var access = filtered.First();
                var userData = LoginUser(new LoginParameters
                {
                    Email = email,
                    Password = password,
                    IsUser = access.IsUser,
                    CardId = access.CardId,
                    CardType = access.CardType,
                    ByToken = loginParams.ByToken,
                    IsMobileLogin = loginParams.IsMobileLogin,
                    GetToken = loginParams.GetToken,
                    IsAngularLogin = loginParams.IsAngularLogin,
                    InternalLoginValidationCall = true,
                    ClientType = loginParams.ClientType,
                    IsAzureAdLogin = loginParams.IsAzureAdLogin,
                    AzureAdToken = loginParams.AzureAdToken
                }, access.Tenant);

                userData.CompanyLogins = filtered;
                userData.ContactsCount = 1;
                return userData;
            }

            return new UserData
            {
                UserName = email,
                CompanyLogins = filtered,
                HasError = filtered.Count == 0,
                ContactsCount = filtered.Count,
                InActive = pl == null && filtered.Count == 0,
                Unlicensed = unlicensed.Any(),
                PrivateLablehasZeroTenant = pl != null && filtered.Count == 0
            };
        }
        private UserData FilterMobileLogins(UserData data, LoginParameters loginParams)
        {
            if (!loginParams.IsMobileLogin || data.HasError)
                return data;

            var validLogins = data.CompanyLogins
                .Where(c => c.InternetAccess)
                .ToList();

            if (!validLogins.Any())
            {
                data.HasError = data.InActive = data.Unlicensed = data.InvalidMobileAccessPermission = true;
                data.ContactsCount = 0;
                data.CompanyLogins = new List<CompanyLogin>();
                return data;
            }

            var tenantIds = validLogins.Select(c => c.Tenant).Distinct().ToList();
            var mobileTenants = tenantQueryService.GetMulti(t => tenantIds.Contains(t.Id))
                                .Where(t => t.IsMobileActivated)
                                .ToDictionary(t => t.Id);

            var mobileLogins = validLogins
                .Where(login => mobileTenants.ContainsKey(login.Tenant))
                .Select(login =>
                {
                    var tenantId = login.Tenant;
                    var logoInfo = GetTenantLogoUri(tenantId, loginParams.MobileVersion);

                    if (logoInfo?.Count > 0)
                        login.URL = logoInfo[0];
                    if (logoInfo?.Count > 1)
                        login.Extension = logoInfo[1];

                    return login;
                })
                .ToList();

            data.CompanyLogins = mobileLogins;
            var hasNoMobileLogins = !mobileLogins.Any();

            data.HasError = data.InActive = data.Unlicensed = data.InvalidMobileAccessPermission = hasNoMobileLogins;
            data.ContactsCount = mobileLogins.Count;

            return data;
        }
        private void SetupBranding(UserData validatedUser)
        {
            var tenantManagement = tenantManagementQueryService.GetSingle(validatedUser.CurrentTenant, false, true);
            if (tenantManagement != null && tenantManagement.EnableBranding)
            {
                validatedUser.IsBrandingEnabled = tenantManagement.HideSharedlogistics;
            }
        }

        #endregion

        #region User/Contact Management
        private string HandleUserOrContactLogin(UserData user, GlobalContactPM? member, string? cardId, string via, bool isUser, bool isFromCTool,
            bool IsFromCargoTracking, int tenant, string computerId, UserPM? amitalUser, bool distributor, bool customerCare, IAmitalCloudContext amitalCloudContext)
        {
            CardPM? card = null;
            string? userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            if (!string.IsNullOrEmpty(userAgent) && userAgent.Length > 500)
                userAgent = userAgent.Substring(0, 500);

            if (!isFromCTool)
            {
                if (isUser)
                {
                    HandleUserLogin(user, tenant, computerId, userAgent, amitalCloudContext);
                }
                else
                {
                    card = HandleContactLogin(user, member, cardId, via, IsFromCargoTracking, tenant, computerId, userAgent, amitalCloudContext);
                }
            }

            if (amitalUser != null)
            {
                distributor = amitalUser.IsDistributor;
                customerCare = !amitalUser.IsDistributor;
            }

            string simplogGuid = Guid.NewGuid().ToString("N");
            return $"{member?.Email}:{member?.Id}" + (card != null ? $":{card.Id}:{card.PartnerTypeId}" : "") + $":{simplogGuid}";
        }
        private void HandleUserLogin(UserData user, int tenant, string computerId, string? userAgent, IAmitalCloudContext context)
        {
            var userLog = new UserLoginLogPM
            {
                Id = IdCounter.GetNumber("UserLoginLog", tenant).ToString(),
                Tenant = tenant,
                Browser = GetBrowserType(),
                IP = AuthenticationUtil.GetIP4Address(),
                UserId = user.Id,
                GMTDateTime = DateTime.Now,
                LocalDateTime = TenantServerConfigration.GetCurrentDateTime(tenant),
                UserAgent = userAgent,
                ComputerId = computerId,
                ChangeSetOp = ChangeSetOperation.Insert,
            };

            if (!string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Request.Headers["X-Real-IP"]))
                userLog.Browser = userLog.Browser.ToUpper();

            using var scope = TransactionFactory.GetNewTransaction(TimeSpan.FromMinutes(10));

            var userLastLoginQuery = new UserLastLoginQueryService(context);
            var lastLogin = userLastLoginQuery.GetSingle(user.Id, false, false) ??
                            new UserLastLoginPM
                            {
                                Id = user.Id,
                                Tenant = tenant,
                                ComputerId = computerId,
                                WorkEnvironment = AmitalCloudSettingConfigration.GetWorkEnvironment(),
                                IP = AuthenticationUtil.GetIP4Address(),
                                ChangeSetOp = ChangeSetOperation.Insert
                            };

            user.LastLoginDateTime = lastLogin.LoginDateTime;
            lastLogin.IP = AuthenticationUtil.GetIP4Address();
            lastLogin.LoginDateTime = TenantServerConfigration.GetCurrentDateTime(tenant);
            lastLogin.Tenant = tenant;
            lastLogin.ChangeSetOp = lastLogin.ChangeSetOp != ChangeSetOperation.Insert ? ChangeSetOperation.Update : ChangeSetOperation.Insert;

            new UserLastLoginUpdateService(context).Update(lastLogin, true);
            new UserLoginLogUpdateService(context).Update(userLog, true);

            scope.Complete();
        }
        private CardPM? HandleContactLogin(UserData user, GlobalContactPM? member, string? cardId, string via, bool isFromCargoTracking, int tenant, string computerId, string? userAgent, IAmitalCloudContext context)
        {
            var now = TenantServerConfigration.GetCurrentDateTime(tenant);
            CardPM? card = null;

            if (member != null && !string.IsNullOrEmpty(cardId))
            {
                var cardContactService = new CardContactQueryService(context);
                var cardService = new CardQueryService(context);


                Expression<Func<CardContact, bool>> predicate = d =>
                    d.ContactId == member.Id &&
                    d.CardId == cardId;

                var cardContact = cardContactService.GetMulti<CardContactPM>(predicate, "Card").FirstOrDefault();
                if (cardContact?.Card == null)
                    return null;

                card = cardContact.Card;
                card.SharedLogisticsInvitationStatusCode = !isFromCargoTracking ? 3 : card.SharedLogisticsInvitationStatusCode;
                card.CargoTrackingInvitationStatusCode = isFromCargoTracking ? 3 : card.CargoTrackingInvitationStatusCode;
                card.LastLoginDate = now;
                cardContact!.LastLoginDate = now;

                user.CardId = card.Id;
                user.CardType = card.PartnerTypeId;

                if (via == "Mobile")
                    card.IsActiveForMobile = true;

                CreateSharedLogisticsContactLastLogin(via, user, card);
            }
            var contactLog = new ContactLoginLogPM
            {
                Id = IdCounter.GetNumber("ContactLoginLog", tenant).ToString(),
                Tenant = tenant,
                Browser = GetBrowserType(),
                IP = AuthenticationUtil.GetIP4Address(),
                ContactId = user.Id,
                GMTDateTime = DateTime.Now,
                LocalDateTime = now,
                ContactAgent = userAgent,
                ComputerId = computerId,
                Via = via,
                ChangeSetOp = ChangeSetOperation.Insert,
            };

            var contactLastLoginService = new ContactLastLoginQueryService(context);
            var lastLogin = contactLastLoginService.GetSingle(user.Id, false, false) ??
                            new ContactLastLoginPM
                            {
                                Id = user.Id,
                                Tenant = tenant,
                                LoginDateTime = now,
                                ChangeSetOp = ChangeSetOperation.Insert
                            };

            user.DigitalLastLoginDateTime = lastLogin.LoginDateTime;
            lastLogin.ComputerId = computerId;
            lastLogin.LoginDateTime = now;
            lastLogin.ChangeSetOp = lastLogin.ChangeSetOp != ChangeSetOperation.Insert ? ChangeSetOperation.Update : ChangeSetOperation.Insert;

            new ContactLastLoginUpdateService(context).Update(lastLogin, true);
            new ContactLoginLogUpdateService(context).Update(contactLog, true);

            return card;
        }
        private TenantManagmentPrivateLabelsPM? GetPrivateLabelIfNeeded(LoginParameters loginParameters, string url)
        {
            if (!loginParameters.IsFromPLSignApp && !url.Contains("system.logbox.co.il") && !url.Contains("cloud.amital.co.il"))
            {
                TenantManagmentPrivateLabelsQueryService tenantManagmentPrivateLabelsQueryService = new TenantManagmentPrivateLabelsQueryService(globalContext);
                return tenantManagmentPrivateLabelsQueryService.GetMulti(a => a.PrivateLabelUrl == url && a.InActive == false).FirstOrDefault();
            }
            return null;
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
        private void CreateSharedLogisticsContactLastLogin(string via, UserData user, CardPM card)
        {
            IAmitalCloudContext context = AmitalCloudContext.GetContext(card.Tenant);

            string loginVia = string.IsNullOrEmpty(via) ? "PC" : via;
            var now = TenantServerConfigration.GetCurrentDateTime(card.Tenant);
            var queryService = new SharedLogisticsContactLastLoginQueryService(context);
            var existingLogin = queryService.GetMulti(a =>
                a.ContactId == user.Id &&
                a.CardId == card.Id &&
                a.PartnerTypeId == card.PartnerTypeId &&
                a.Via == loginVia).FirstOrDefault();

            var login = existingLogin ?? new SharedLogisticsContactLastLoginPM
            {
                ContactId = user.Id,
                CardId = card.Id,
                PartnerTypeId = card.PartnerTypeId,
                Via = loginVia,
                Tenant = card.Tenant,
                ChangeSetOp = ChangeSetOperation.Insert
            };

            login.LoginDateTime = now;
            login.ChangeSetOp = existingLogin == null ? ChangeSetOperation.Insert : ChangeSetOperation.Update;

            var updateService = new SharedLogisticsContactLastLoginUpdateService(context);
            updateService.Update(login, true);
        }
        private CompanyLogin CreateCompanyLogin(string email, string companyName, bool isUser, int tenantId, string? cardId, string? cardType, string contactId, bool licensedUser, bool internetAccess, string privateLabelId, string? customerName = "")
    => new CompanyLogin
    {
        Email = email,
        CompanyName = companyName,
        IsUser = isUser,
        Tenant = tenantId,
        CardId = cardId ?? string.Empty,
        CardType = cardType ?? string.Empty,
        ContactId = contactId,
        LicensedUser = licensedUser,
        InternetAccess = internetAccess,
        PrivateLabelId = privateLabelId,
        CustomerName = customerName ?? string.Empty
    };
        public UserData GetUserTenantLogins(LoginParameters loginParams, UserData data, string email, string password, TenantManagmentPrivateLabelsPM? privatelabel, string url)
        {

            var logins = new List<CompanyLogin>();
            List<GlobalContactPM>? contacts = GetContacts(email, tenant);

            var zeroContact = contacts.FirstOrDefault(x => x.GlobalTenantId == 0);
            var user = zeroContact != null ? GetUser(zeroContact.Id, tenant) : null;

            bool isDistributor = user != null && user.IsDistributor && user.Tenant == 0;
            bool isCustomerCare = user != null && !isDistributor && user.Tenant == 0;

            if (isCustomerCare || isDistributor)
                logins.AddRange(GetSupportLogins(zeroContact!, user, isDistributor));

            var filteredContacts = contacts.Where(c => isCustomerCare || isDistributor ? c.InternetAccess : (c.IsUser || c.InternetAccess)).ToList();

            foreach (GlobalContactPM contact in filteredContacts)
            {
                var tenant = contact.GlobalTenant;
                bool shouldAddUser = contact.IsUser &&
                                     !isCustomerCare &&
                                     !isDistributor;
                if (shouldAddUser)
                {
                    bool licensed = IsLicensed(contact);
                    logins.Add(CreateCompanyLogin(
                        contact.Email,
                        $"{tenant.CompanyName} ({tenant.Id})",
                        true,
                        tenant.Id,
                        null,
                        null,
                        contact.Id,
                        true,
                        contact.InternetAccess,
                        tenant.PrivateLabelId
                    ));
                }

                if (contact.InternetAccess && HasAccess(tenant.Id, loginParams))
                {
                    logins.AddRange(GetCardLogins(contact, tenant));
                }
            }


            var unlicensed = logins.Where(x => x.IsUser && !x.LicensedUser).ToList();
            logins = logins.Where(x => x.LicensedUser || !x.IsUser).OrderBy(x => x.CompanyName).ToList();

            if (!loginParams.IsFromPLSignApp)
                MapHasLogboxAccessPrivateLabelTenants(logins, GetLogboxAccessiblePrivateLabelTenantsIds(url));

            return HandleLoginResult(logins, loginParams, email, password, privatelabel, unlicensed);
        }
        IEnumerable<CompanyLogin> GetSupportLogins(GlobalContactPM contact, UserPM? user, bool isDistributor)
        {
            List<TenantManagement> tenants = tenantManagementQueryService.GetTenantsBySupportStatus(isDistributor, user, tenant);

            return tenants.Select(t =>
                    CreateCompanyLogin(
                        contact.Email,
                        $"{t.GlobalTenant.CompanyName} ({t.Id})",
                        true,
                        t.Id,
                        null,
                        null,
                        contact.Id,
                        true,
                        false,
                        t.GlobalTenant.PrivateLabelId));
        }
        List<GlobalContactPM> GetContacts(string email, int tenant) =>
           globalContactQueryService.GetContactsByEmail(email, tenant);
        ContactPasswordPM? GetContactPasswordByEmail(string email) =>
            contactPasswordQueryService
                .GetMulti(d => d.Email.ToLower() == email.ToLower())
                .FirstOrDefault();
        IEnumerable<CompanyLogin> GetCardLogins(GlobalContactPM contact, GlobalTenantPM tenant)
        {
            var cardContactQueryService = new CardContactQueryService(amitalCloudContext);

            Expression<Func<CardContact, bool>> predicate = c =>
                c.ContactId == contact.Id &&
                c.InternetAccess &&
                 c.Tenant == contact.GlobalTenantId;

            var cardContacts = cardContactQueryService.GetMulti<CardContactPM>(predicate, "Card");

            return cardContacts?.Select(cardContact =>
            {
                var card = cardContact.Card;
                return CreateCompanyLogin(
                    contact.Email,
                    $"{tenant.CompanyName}-{card?.EnglishName} ({tenant.Id})",
                    false,
                    tenant.Id,
                    card?.Id,
                    card?.PartnerTypeId,
                    contact.Id,
                    false,
                    true,
                    tenant.PrivateLabelId,
                    card?.EnglishName
                );
            }) ?? new List<CompanyLogin>();
        }
        private GlobalContactPM? GetContact(string email, int tenant)
        {
            var contacts = GetContacts(email, tenant);
            return contacts.FirstOrDefault(c => c.GlobalTenantId == 0)
                ?? contacts.FirstOrDefault(c => c.GlobalTenantId == tenant);
        }
        private UserPM? GetUser(string contactId, int tenant)
        {
            var context = AmitalCloudContext.GetContext(tenant);
            var userQueryService = new UserQueryService(context);
            return userQueryService.GetMulti(x=> x.Id == contactId).FirstOrDefault();
        }
        private UserData? BuildUserData(GlobalContactPM member, string name, int tenant, bool isUser, string? cardId, IAmitalCloudContext context, out GlobalContactPM finalMember)
        {
            finalMember = member;

            if (!string.IsNullOrEmpty(cardId))
            {
                var cardContactQuery = new CardContactQueryService(context);
                var cardContact = cardContactQuery.GetMulti(d => d.ContactId == member.Id && d.CardId == cardId).FirstOrDefault();

                if (cardContact == null)
                {
                    var fallback = globalContactQueryService.GetMulti(m =>
                        m.Email == name &&
                        !m.InActive &&
                        (m.IsUser || m.InternetAccess) &&
                        m.GlobalTenantId == tenant).FirstOrDefault();

                    if (fallback != null)
                        finalMember = fallback;
                }
            }

            return new UserData
            {
                UserName = name,
                Id = finalMember.Id,
                Name = name,
                CurrentTenant = tenant,
                IsUser = isUser
            };
        }
        private (bool passedAuthentication, GlobalContactPM? member, ContactPasswordPM? contactPassword)
        TryAuthenticateContact(string name, string? password, bool byToken, bool OneTimePassword,
        bool IsAzureAdLogin, List<GlobalContactPM>? contacts, int tenant, bool isUser, string clientType)
        {
            ContactPasswordPM? contactPassword = null;
            GlobalContactPM? member = null;
            bool passedAuthentication = false;

            string hashedPassword = string.Empty;
            bool isHashPassword = false;

            if (!IsAzureAdLogin)
            {
                hashedPassword = byToken ? password! : PasswordGenerator.GetHashedPassword(name, password!);
                isHashPassword = byToken;

                if (!isHashPassword)
                {
                    var passResult = ResolvePassword(password!);
                    if (passResult != null)
                    {
                        hashedPassword = passResult.Password;
                        isHashPassword = passResult.isHashPassword;
                    }
                }
            }

            string? pass = !IsAzureAdLogin ? (isHashPassword ? hashedPassword : password) : null;
            ContactPassword? contactPasswordPOCO = AuthenticationUtil.VerifyContactPassword(name, pass, isHashPassword);

            if (contactPasswordPOCO != null)
            {
                contactPassword = new ContactPasswordPM(contactPasswordPOCO);
                CheckLockedUser(contactPassword, clientType);

                if ((!contactPassword.IsLocked || clientType == "Web") && (!contactPassword.MustChangePassword || OneTimePassword))
                {
                    passedAuthentication = true;

                    if (contacts != null)
                    {
                        var tenantContacts = contacts.Where(m => m.GlobalTenantId == tenant).ToList();

                        member = tenantContacts.Count switch
                        {
                            > 1 => isUser
                                ? tenantContacts.FirstOrDefault(m => m.IsUser)
                                : tenantContacts.FirstOrDefault(m => m.InternetAccess),
                            1 => tenantContacts.First(),
                            _ => contacts.FirstOrDefault(m => m.GlobalTenantId == 0)
                        };
                    }

                    var contactPasswordUpdateService = new ContactPasswordUpdateService(tenant);
                    if (contactPassword?.NumberOfRetries > 0)
                        UpdateContactPassword(contactPassword, contactPasswordUpdateService, numberOfRetries: 0);

                    if (member != null)
                    {
                        CacheUserRoles(member, tenant);
                    }
                    else if (contactPassword != null)
                    {
                        contactPassword.NumberOfRetries++;
                        UpdateContactPassword(contactPassword, contactPasswordUpdateService, contactPassword.NumberOfRetries);
                    }
                }
            }

            return (passedAuthentication, member, contactPassword);
        }

        private void CacheUserRoles(GlobalContactPM contact, int tenant)
        {
            var roleQuery = new RoleQuery(tenant);
            var allRoles = roleQuery.GetRolesForContact(contact.Id, contact.GlobalTenantId).ToList();

            var customRoles = allRoles.Where(r => r.IsCustomRole).ToList();
            var roleIds = allRoles.Select(r => r.Id).ToList();

            // הסרת ParentRoles במקרה של CustomRoles
            foreach (var customRole in customRoles)
            {
                if (roleIds.Contains(customRole.ParentRoleId))
                    roleIds.Remove(customRole.ParentRoleId);
            }

            var contactInfo = new ContactInfo
            {
                Tenant = tenant,
                ContactEmail = contact.Email,
                RolesIds = roleIds
            };

            CacheManager.CacheWrapper.Insert(
                contact.Email + tenant,
                contactInfo,
                null,
                DateTime.UtcNow.AddMinutes(30),
                TimeSpan.Zero
            );
        }
        #endregion

        #region Security & Two-Factor
        private bool IsCustomerCareIpAuthenticated()
        {
            var authenticatedIPs = (AmitalCloudSettings.CustomerCareIP ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var httpContext = _httpContextAccessor.HttpContext;
            var remoteIp = httpContext?.Connection.RemoteIpAddress?.ToString();

            bool isLocalhost = remoteIp == "::1" || remoteIp == "127.0.0.1";
            bool isHttpAuth = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_IIS_HTTPAUTH"));

            if (authenticatedIPs.Contains("*") || (isHttpAuth && isLocalhost))
                return true;

            string? currentIp = httpContext?.Request.Headers["X-Real-IP"].FirstOrDefault() ?? remoteIp;
            return authenticatedIPs.Contains(currentIp);
        }
        private bool IsUserAdmin(string email, int tenant)
        {
            UserQueryService userQueryService = new UserQueryService(amitalCloudContext);
            List<UserPM> entities = userQueryService.GetMulti(record => record.Contact.Email == email && (record.Tenant == tenant || record.Tenant == 0));
            UserPM? loggedUser = entities.Where(a => a.Tenant == tenant).FirstOrDefault() ?? entities.Where(a => a.Tenant == 0).FirstOrDefault();
            return loggedUser?.UserRoles != null && loggedUser.UserRoles.Contains("Administrator");
        }
        bool IsLicensed(GlobalContactPM contact)
        {
            var managePerUser = tenantManagementQueryService.GetSingle(contact.GlobalTenantId, false, true)?.ManageLicencesPerUser ?? false;
            return !managePerUser || (GetUser(contact.Id, contact.GlobalTenantId)?.LicencedUser ?? false);
        }
        bool HasAccess(int tenantId, LoginParameters loginParams)
        {
            var tenant = tenantQueryService.GetSingle(tenantId, false, true);
            return loginParams.IsMobileLogin
                ? tenant.IsMobileActivated
                : tenant.IsWebAccessActivated || tenant.IsCargoTrackWebAccessActivated || tenant.IsDigitalPortalAccessActivated;
        }
        private UserData? ValidateLogin(string email, string? password, int tenant, LoginParameters parameters, bool fromCTool)
        {
            var validatedUser = ValidateUser(email, password, tenant, Guid.NewGuid().ToString("N"), out _, parameters.IsUser, parameters.CardId,
                parameters.ByToken, parameters.IsMobileLogin ? "Mobile" : "PC", parameters.ClientType, fromCTool, parameters.IsCargoTracking,
                false, parameters.IsAzureAdLogin);

            if (validatedUser != null)
                return validatedUser;

            ContactPasswordPM? contactPassword = null;
            return CheckUserState(email, password, ref contactPassword, parameters.ByToken, parameters.ClientType, parameters.IsAzureAdLogin);
        }
        private UserData? ValidateUser(string name, string? password, int tenant, string computerId, out string? userData, bool isUser, string? cardId, bool byToken,
           string via, string clientType, bool isFromCTool, bool IsFromCargoTracking, bool OneTimePassword, bool IsAzureAdLogin)
        {
            var myAmitalCloudContext = AmitalCloudContext.GetContext(tenant);
            ContactPasswordPM? contactPassword = null;
            UserData? user = null;
            userData = null;
            bool customerCare = false;
            bool distributor = false;
            UserPM? amitalUser = null;
            GlobalContactPM? member = null;
            name = name.ToLower();

            if (name.Contains("system@"))
                return user;

            var contacts = GetContacts(name, tenant)?.Where(d => d.GlobalTenantId == 0 || d.GlobalTenantId == tenant)?.ToList();

            var contact = contacts?.FirstOrDefault(c => c.GlobalTenantId == 0) ??
                contacts?.Where(c => c.GlobalTenantId == tenant).OrderByDescending(c => c.IsUser).FirstOrDefault();
           
            if (contact == null) return user;

            if (contact.GlobalTenantId == 0)
            {
                amitalUser = GetUser(contact.Id, tenant);

                if (amitalUser?.Tenant == 0)
                {
                    distributor = amitalUser.IsDistributor;
                    customerCare = !amitalUser.IsDistributor;
                }
            }


            if (!customerCare || IsCustomerCareIpAuthenticated())
            {
                var authenticationResult = TryAuthenticateContact(
                    name, password, byToken, OneTimePassword, IsAzureAdLogin,
                    contacts, tenant, isUser, clientType);

                if (authenticationResult.passedAuthentication)
                {
                    contactPassword = authenticationResult.contactPassword;
                    member = authenticationResult.member;
                }
            }
            if (member != null)
                  user = BuildUserData(member, name, tenant, isUser, cardId, myAmitalCloudContext, out var finalMember);

            if (user != null)
                userData = HandleUserOrContactLogin(user, member, cardId, via, isUser, isFromCTool, IsFromCargoTracking, tenant, computerId, amitalUser, distributor, customerCare, myAmitalCloudContext);

            if (contactPassword != null && user != null)
                user.NumberOfRetries = contactPassword.NumberOfRetries;

            return user;
        }
        private UserData CheckUserState(string email, string? password, ref ContactPasswordPM? contactPassword, bool byToken, string clientType, bool isAzureAdLogin)
        {
            if (!string.IsNullOrEmpty(email))
                email = email.ToLower();

            UserData userData = new UserData()
            {
                UserName = email,
            };

            bool isOneTimePassword = false;
            bool isHashedPassword = byToken;
            string? hashedPassword = null;

            if (!isAzureAdLogin)
            {
                if (!isHashedPassword)
                {
                    var passResult = ResolvePassword(password!);
                    if (passResult != null)
                    {
                        hashedPassword = passResult.Password;
                        isOneTimePassword = passResult.IsOneTimePassword;
                        isHashedPassword = passResult.isHashPassword;
                    }
                }
                else
                {
                    hashedPassword = password;
                }
            }

            string? pass = isAzureAdLogin ? null : (isHashedPassword ? hashedPassword : password);
            ContactPassword? contactPasswordPOCO = AuthenticationUtil.VerifyContactPassword(email, pass, isHashedPassword);

            if (contactPasswordPOCO != null)
            {
                contactPassword = new ContactPasswordPM(contactPasswordPOCO);
                CheckLockedUser(contactPassword, clientType);

                var contact = GetContact(email!, tenant);
                var user = contact != null ? GetUser(contact.Id, tenant) : null;

                bool isCustomerCare = user?.Tenant == 0 && user?.IsDistributor == false;
                bool isDistributor = user?.Tenant == 0 && user?.IsDistributor == true;

                userData.Technology = user?.Technology ?? string.Empty;
                userData.IsUser = contact?.IsUser ?? false;
                userData.IsLocked = contactPassword.IsLocked;
                userData.MustChangePassword = !isOneTimePassword && contactPassword.MustChangePassword;

                if (isCustomerCare)
                    userData.IpRestricted = !IsCustomerCareIpAuthenticated();
            }
            else
            {
                contactPassword = GetContactPasswordByEmail(email);
                if (contactPassword != null)
                {
                    contactPassword.NumberOfRetries++;
                    UpdateContactPassword(contactPassword, new ContactPasswordUpdateService(tenant), contactPassword.NumberOfRetries);
                }
                else
                    userData.InvalidEmailAddress = true;

                userData.InValidMailOrPassword = !byToken;
            }

            userData.HasError = (userData.InValidMailOrPassword || userData.IpRestricted || (userData.IsLocked && clientType != "Web") || userData.MustChangePassword);

            userData.NumberOfRetries = contactPassword?.NumberOfRetries ?? 0;

            return userData;
        }
        private void CheckLockedUser(ContactPasswordPM contact, string clientType)
        {
            bool? IsLocked = null;
            int? NumberOfRetries = null;

            if (contact.IsLocked)
            {
                if (contact.LockDateTime.HasValue)
                {
                    var minutesLocked = (DateTime.Now - contact.LockDateTime.Value).TotalMinutes;
                    if (minutesLocked > 30 || clientType == "Web")
                    {
                        IsLocked = false;
                        NumberOfRetries = 0;
                    }
                }
                else
                    IsLocked = false;
            }
            else if (clientType == "Web")
            {
                IsLocked = false;
                NumberOfRetries = 0;
                contact.CaptchaKey = null;
            }

            if (IsLocked.HasValue || NumberOfRetries.HasValue)
                UpdateContactPassword(contact, new ContactPasswordUpdateService(tenant), NumberOfRetries, IsLocked);
        }
        private bool CheckLoginSecurityPolicy(int tenant, UserData user, UserPM? amitalUser, TenantLoginPolicyPM securityPolicy, bool isAzureAdLogin)
        {
            string ipAddress = AuthenticationUtil.GetIP4Address();
            string? deviceDescription = !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Request.Headers["User-Agent"]) ? (_httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString().Length <= 500
                ? _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() : _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString().Substring(0, 500)) : "";

            string? TwoFactorkey = _httpContextAccessor.HttpContext?.Request.Headers["TwoFactorkey"];

            bool IsTwoFactorAuthenticationRequired = false;
            if (securityPolicy != null && securityPolicy.LoginPolicyCode != "NOREST")
            {
                if (!isAzureAdLogin && securityPolicy.LoginPolicyCode == "TFAUTH")
                {
                    if (!user.IsUser ||
                        (securityPolicy.IsEnabledForSpecificUsers && amitalUser?.IsTwoFactorAuthenticationEnabled == false)
                        || (securityPolicy.ExcludeInternalIPs && !string.IsNullOrEmpty(securityPolicy.TwoFactorInternalIPs) && securityPolicy.TwoFactorInternalIPs.Contains(ipAddress)))
                    {
                        return false;
                    }

                    ContactQueryService contactQueryService = new ContactQueryService(amitalCloudContext);
                    ContactPM loggedContact = contactQueryService.GetSingle(amitalUser?.Id, false, false);

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
                            UserId = amitalUser?.Id,
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
                        if (device!.IsVerified)
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
        private string? AddVerificationCodeSMSLog(ContactPM loggedContact, TwoFactorAuthenticationDevicePM device)
        {
            if (!string.IsNullOrEmpty(loggedContact.Mobile) && loggedContact.Mobile.Length > 7)
            {
                int tenant = device.Tenant;
                ObjectTableQueryService objectTableQueryService = new ObjectTableQueryService(amitalCloudContext);
                var objectTable = objectTableQueryService.GetMultiFromCache(nameof(TwoFactorAuthenticationDevice) + 0, d => d.Name == nameof(TwoFactorAuthenticationDevice) && d.Tenant == 0).FirstOrDefault();

                string? myObjectTableId = objectTable?.Id;
                string environment = AmitalCloudSettings.WorkEnvironment == "cloud" ? "Cloud" : "Amital";
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
        private UserData CheckCaptchaState(LoginParameters loginParameters, bool withoutCheckUsed = false)
        {
            UserData data = new UserData();
            if (!loginParameters.IsMobileLogin && loginParameters.ClientType == "Web")
            {
                bool isCheckCaptchaCode = !string.IsNullOrEmpty(loginParameters.CaptchaCode) && !string.IsNullOrEmpty(loginParameters.CaptchaKey);
                ContactPasswordPM? contactPassword = GetContactPasswordByEmail(loginParameters.Email);

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


            }
            return data;
        }
        private bool CheckAndHandleTwoFactorAuthentication(int tenant, UserData user, UserPM? amitalUser, bool fromCTool, bool customerCare, LoginParameters parameters)
        {
            if (parameters.IsAngularLogin || parameters.IsMobileLogin || !parameters.IsUser || customerCare || fromCTool)
            {
                return false;
            }

            amitalCloudContext = AmitalCloudContext.GetContext(tenant);
            TenantLoginPolicyQueryService securityPolicyQueryService = new TenantLoginPolicyQueryService(amitalCloudContext);
            TenantLoginPolicyPM? securityPolicy = securityPolicyQueryService.GetMulti(d => d.Tenant == tenant).FirstOrDefault();

            if (securityPolicy == null || securityPolicy.LoginPolicyCode == "NOREST")
            {
                return false;
            }

            return CheckLoginSecurityPolicy(tenant, user, amitalUser, securityPolicy, parameters.IsAzureAdLogin);
        }
        private void UpdateContactPassword(ContactPasswordPM contactPassword, ContactPasswordUpdateService contactPasswordUpdateService, int? numberOfRetries = null, bool? isLocked = null, string? captchaKey = null)
        {
            if (numberOfRetries.HasValue)
            {
                contactPassword.NumberOfRetries = numberOfRetries.Value;

                if (contactPassword.NumberOfRetries >= 5)
                {
                    contactPassword.IsLocked = true;
                    contactPassword.LockDateTime = DateTime.Now;
                }
            }
            if (isLocked == false || numberOfRetries == 0)
            {
                contactPassword.LockDateTime = null;
                contactPassword.IsLocked = false;
                contactPassword.NumberOfRetries = 0;
            }

            if (!string.IsNullOrWhiteSpace(captchaKey))
                contactPassword.CaptchaKey = captchaKey;

            contactPassword.ChangeSetOp = ChangeSetOperation.Update;
            contactPasswordUpdateService.Update(contactPassword, true);
        }

        #endregion

        #region Token Management
        private void HandleAuthenticationTokens(LoginParameters parameters, int tenant, UserData validatedUser, UserPM? amitalUser, bool fromCTool, bool customerCare)
        {
            if (validatedUser.HasError)
                return;

            if (parameters.IsMobileLogin || parameters.GetToken)
            {
                bool twoFactorRequired = CheckAndHandleTwoFactorAuthentication(tenant, validatedUser, amitalUser, fromCTool, customerCare, parameters);

                if (!twoFactorRequired && !parameters.ByToken)
                {
                    GenerateTokensForUser(validatedUser, parameters, tenant);
                }
                else if (twoFactorRequired)
                {
                    validatedUser.IsTwoFactorAuthenticationRequired = true;
                }
            }
        }
        private void GenerateTokensForUser(UserData user, LoginParameters parameters, int tenant)
        {
            string hashedPassword = string.Empty;
            bool isAzureAdLogin = parameters.IsAzureAdLogin;
            if (!isAzureAdLogin)
            {
                ContactPassword? contactPasswordPOCO = AuthenticationUtil.VerifyContactPassword(user.UserName, parameters.Password);
                if (contactPasswordPOCO != null)
                {
                    ContactPasswordPM contactPasswordPM = new ContactPasswordPM(contactPasswordPOCO);
                    hashedPassword = contactPasswordPM.Password;
                }
                else
                {
                    var passResult = ResolvePassword(parameters.Password);
                    if (passResult != null)
                    {
                        hashedPassword = passResult.Password;
                    }
                }
            }
            string token = !isAzureAdLogin ? AuthenticationUtil.GenerateToken() : parameters.AzureAdToken;

            AuthenticationTokenUpdateService authenticationUpdateService = new AuthenticationTokenUpdateService(tenant);
            AuthenticationToken authentication = new AuthenticationToken()
            {
                CreateDate = DateTime.Now,
                Email = user.UserName,
                Password = hashedPassword,
                Token = token,
                Tenant = user.CurrentTenant,
                ClientType = parameters.IsMobileLogin ? "Mobile" : parameters.ClientType
            };

            if (!user.KeepUserLoggedIn && authentication.ClientType == "Web" && user.WebTokenLifeTimeInMinutes != 0)
                authentication.ExpirationDate = DateTime.Now.AddMinutes(user.WebTokenLifeTimeInMinutes);

            AddAuthenticationToken(authentication, authenticationUpdateService);
            user.Token = token;

            if (isAzureAdLogin)
                return;
            // יצירת טוקן להורדת מסמכים
            AuthenticationToken authenticationDocument = new AuthenticationToken()
            {
                CreateDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddMinutes(15),
                Email = user.UserName,
                Password = hashedPassword,
                Token = AuthenticationUtil.GenerateToken(),
                Tenant = user.CurrentTenant,
                ClientType = "DocumentDownload"
            };
            AddAuthenticationToken(authenticationDocument, authenticationUpdateService);
            user.DocumentDownloadToken = authenticationDocument.Token;

            if (parameters.GetInvalidDocumentToken)
            {
                AuthenticationToken invalidDocumentToken = new AuthenticationToken()
                {
                    CreateDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddMinutes(-5),
                    Email = user.UserName,
                    Password = hashedPassword,
                    Token = AuthenticationUtil.GenerateToken(),
                    Tenant = user.CurrentTenant,
                    ClientType = "DocumentDownload"
                };
                AddAuthenticationToken(invalidDocumentToken, authenticationUpdateService);
                user.InvalidDocumentToken = invalidDocumentToken.Token;
            }
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
        #endregion

        #region Utility
        private PasswordParameter? ResolvePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return null;

            const string OneTimeMarker = "@OneTimePassword";
            const string HashMarker = "@HashPassword";

            string? marker = password.Contains(OneTimeMarker) ? OneTimeMarker :
                             password.Contains(HashMarker) ? HashMarker : null;

            if (marker == null)
                return null;

            var split = password.Split(new[] { marker }, StringSplitOptions.None);
            if (split.Length == 0 || string.IsNullOrWhiteSpace(split[0]))
                return null;

            return new PasswordParameter
            {
                Password = split[0],
                IsOneTimePassword = marker == OneTimeMarker,
                isHashPassword = true
            };
        }
        private static string GetUrlImage(byte[] datainByte, string extension) => $"data:image/{extension};base64,{Convert.ToBase64String(datainByte, 0, datainByte.Length)}";
        private string GetBrowserType()
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            var parser = Parser.GetDefault();
            ClientInfo clientInfo = parser.Parse(userAgent);

            string browser = clientInfo.UA.Family;
            string versionMajor = clientInfo.UA.Major;

            string browserType = $"{browser}{versionMajor}";
            return browserType;
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
        public static string? GetClientIpAddress(HttpContext httpContext)
        {
            var ip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            return !string.IsNullOrEmpty(ip)
                ? ip
                : httpContext.Connection.RemoteIpAddress?.ToString();
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

        #endregion


    }
}
