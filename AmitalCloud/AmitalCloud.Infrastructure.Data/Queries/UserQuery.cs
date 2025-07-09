using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class UserQuery
    {
        IRepository<User> repository;
        IAmitalCloudContext context;
        #region Constructors

        public UserQuery(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public UserQuery(IAmitalCloudContext context) : this(new Repository<User>(context))
        {
            this.context = context;
        }

        public UserQuery(IRepository<User> repository)
        {
            this.repository = repository;
        }
        #endregion Constructors
        #region Private Methods
        private UserPM GetSinglePMFromCache(Expression<Func<UserPM, bool>> predicate, string entityName)
        {
            UserPM entity;
            if (HttpContextHelper.HttpContext != null)
            {
                if (CacheManager.CacheWrapper.Get(entityName) != null)
                {
                    entity = (UserPM)CacheManager.CacheWrapper.Get(entityName);
                }
                else
                {
                    entity = GetSinglePMFromDB(predicate);
                    if (CacheManager.CacheWrapper.Get(entityName) == null && entity != null)
                    {
                        CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }
                }
            }
            else
            {
                entity = GetSinglePMFromDB(predicate);
            }

            return entity;
        }
        private UserPM GetSinglePMFromDB(Expression<Func<UserPM, bool>> predicate)
        {
            var query = (from a in context.Users.Include("Branch").Include("Contact").Include("Department").Include("Freelancer").Include("ProductType")
                         select new UserPM()
                         {
                             BranchId = a.BranchId,
                             //BranchName = a.Branch != null ? a.Branch.EnglishName : "",
                             //DepartmentName = a.Department != null ? a.Department.EnglishName : "",
                             DepartmentId = a.DepartmentId,
                             Id = a.Id,
                             Notes = a.Notes,
                             Tenant = a.Tenant,
                             //Email = a.Contact.Email,
                             //EnglishName = a.Contact.EnglishName,
                             //Anniversary = (DateTime)a.Contact.Anniversary,
                             //Birthday = (DateTime)a.Contact.Birthday,
                             //BusinessPhone = a.Contact.BusinessPhone,
                             //FacebookId = a.Contact.FacebookId,
                             //Fax = a.Contact.Fax,
                             //InActive = a.Contact.InActive,
                             //LocalName = a.Contact.LocalName,
                             SearchFields = a.SearchFields,
                             //Mobile = a.Contact.Mobile,
                             //DontShowLocalLabels = a.Contact.DontShowLocalLabels,
                             //Position = a.Contact.Position,
                             //ComputedLocalName = string.IsNullOrEmpty(a.Contact.LocalName) ? a.Contact.EnglishName : a.Contact.LocalName,
                             IsBranchRestricted = a.IsBranchRestricted,
                             IsSalesman = a.IsSalesman,
                             IsFreelancer = a.IsFreelancer,
                             FreelancerId = a.FreelancerId,
                             //FreelancerName = a.Freelancer != null ? a.Freelancer.EnglishName : null,
                             BusinessUnitId = a.BusinessUnitId,
                             Code = a.Code,
                             CreateDate = a.CreateDate,
                             ExpirationDate = a.ExpirationDate,
                             LicencedUser = a.LicencedUser,
                             IsProductRestricted = a.IsProductRestricted,
                             ProductTypeCode = a.ProductTypeCode,
                             //ProductTypeName = a.ProductType != null ? a.ProductType.Name : null,
                             IsDistributor = a.IsDistributor,
                             DistributorCode = a.DistributorCode,
                             //IsCustomerCare = a.Tenant == 0 && !a.IsDistributor,
                             PersonalId = a.PersonalId,
                             IsShowContactDetailsInTheMobileApp = a.IsShowContactDetailsInTheMobileApp,
                             Technology = a.Technology,
                             SetAngularAsDefault = a.SetAngularAsDefault,
                             //DisplayGettingStarted = a.Contact.DisplayGettingStarted,
                             IsTwoFactorAuthenticationEnabled = a.IsTwoFactorAuthenticationEnabled,
                             DocumentFilingInbox = a.DocumentFilingInbox,
                             ShowLogBoxToolTip = a.ShowLogBoxToolTip,
                             ShowInboxToolTip = a.ShowInboxToolTip,
                             ShowLocalNameInLOV = a.ShowLocalNameInLOV,
                             UserRoles = a.UserRoles,
                             SecurityLevel = a.SecurityLevel,
                             AdditionalPackagesOnly = a.AdditionalPackagesOnly,
                             LayoutDirection = a.LayoutDirection,
                             SignatureImageId = a.SignatureImageId,
                         });
            query = query.Where(predicate);
            UserPM entity = query.FirstOrDefault();
            if (entity != null)
            {
                //entity.IsHRUser = this.CheckIfIsHRUser(entity);
                //entity.ExpirationDaysLeft = ComputeDaysLeft(entity.ExpirationDate);
                UserLastLoginRepository rep = new UserLastLoginRepository(context);
                UserLastLoginQuery userLastLoginQuery = new UserLastLoginQuery(rep);
                //entity.UserLastLogin = userLastLoginQuery.GetSinglePM(entity.Id, tenant);
                UserPermittedBranchRepository userPermRep = new UserPermittedBranchRepository(this.context);
                UserPermittedBranchQuery userPermittedBranchQuery = new UserPermittedBranchQuery(userPermRep);
                entity.UserPermittedBranches = userPermittedBranchQuery.GetContactFromUserPermittedBranchPMsByUserId(entity.Id, entity.Tenant).ToList();
                UserPermittedProductRepository perProductRep = new UserPermittedProductRepository(this.context);
                UserPermittedProductQuery perProductQuery = new UserPermittedProductQuery(perProductRep);
                entity.UserPermittedProducts = perProductQuery.GetContactFromUserPermittedProductPMsByUserId(entity.Id, entity.Tenant).ToList();
            }

            return entity;
        }
        private int ComputeDaysLeft(DateTime? date)
        {
            DateTime? startDate = DateTime.Now.Date;
            int days = 0;

            if (startDate != null && date != null)
            {
                TimeSpan? day = (date - startDate);

                if (day.Value.Days == 0)
                {
                    days = 0;
                }
                else if (day.Value.Days < 0)
                {
                    days = day.Value.Days;
                }
                else
                {
                    days = day.Value.Days;
                }
            }
            return days;
        }
        private string GetTechnologyOfUser(string userId)
        {
            string technology = "";
            User user = context.Users.Where(D => D.Id == userId).FirstOrDefault();

            if (user != null)
            {
                technology = user.Technology;
            }

            return technology;
        }
        #endregion Private Methods

        public bool CheckIfUserExistInTenant(string userId, int tenant)
        {
            bool isExist = context.Users.Where(d => d.Id == userId && (d.Tenant == tenant || d.Tenant == 0)).Any();
            return isExist;
        }
        public List<string> GetUserIdsByTenant(int tenant)
        {
            var usersIds = (from a in context.Users
                            where a.Tenant == tenant
                            select a.Id).ToList();
            return usersIds;
        }
        //public string GetSystemUserIdIfItIsCustomerCare(int tenant)
        //{
        //    if (HttpContext.Current != null && HttpContext.Current.User != null)
        //    {
        //        string email = HttpContext.Current.User.Identity.Name;
        //        User user = repository.GetSingleUserByEmail(email, 0, false);
        //        if (user != null)
        //        {
        //            User systemUser = repository.GetSingleUserByEmail("system@tenant" + tenant + ".com", tenant, true);
        //            if (systemUser != null)
        //            {
        //                return systemUser.Id;
        //            }

        //        }
        //    }
        //    return "";
        //}

        #region Get Single User
        public UserPM GetSinglePM(string id, int tenant) => GetSinglePMFromCache(a => (a.Tenant == tenant || a.Tenant == 0) && a.Id == id, "UserPM" + id + tenant);
        //------
        #endregion Get Single User

        #region Get Users IQueryable<UserPM>
        //public IQueryable<UserPM> GetUserPMsByTenant(int tenant)
        //{
        //    IQueryable<UserPM> users = from a in context.Users.Include("Contact").Include("Department").Include("Branch").Include("Freelancer").Include("ProductType")
        //                               where a.Tenant == tenant
        //                               select new UserPM()
        //                               {
        //                                   BranchId = a.BranchId,
        //                                   BranchName = a.Branch != null ? a.Branch.EnglishName : "",
        //                                   DepartmentName = a.Department != null ? a.Department.EnglishName : "",
        //                                   DepartmentId = a.DepartmentId,
        //                                   Id = a.Id,
        //                                   Notes = a.Notes,
        //                                   Tenant = a.Tenant,
        //                                   Email = a.Contact.Email,
        //                                   EnglishName = a.Contact.EnglishName,
        //                                   Anniversary = a.Contact.Anniversary,
        //                                   Birthday = a.Contact.Birthday,
        //                                   BusinessPhone = a.Contact.BusinessPhone,
        //                                   FacebookId = a.Contact.FacebookId,
        //                                   Fax = a.Contact.Fax,
        //                                   InActive = a.Contact.InActive,
        //                                   LocalName = a.Contact.LocalName,
        //                                   SearchFields = a.SearchFields,
        //                                   Mobile = a.Contact.Mobile,
        //                                   DontShowLocalLabels = a.Contact.DontShowLocalLabels,
        //                                   Position = a.Contact.Position,
        //                                   IsBranchRestricted = a.IsBranchRestricted,
        //                                   IsSalesman = a.IsSalesman,
        //                                   IsFreelancer = a.IsFreelancer,
        //                                   FreelancerId = a.FreelancerId,
        //                                   FreelancerName = a.Freelancer != null ? a.Freelancer.EnglishName : null,
        //                                   BusinessUnitId = a.BusinessUnitId,
        //                                   Code = a.Code,
        //                                   CreateDate = a.CreateDate,
        //                                   ExpirationDate = a.ExpirationDate,
        //                                   LicencedUser = a.LicencedUser,
        //                                   IsProductRestricted = a.IsProductRestricted,
        //                                   ProductTypeCode = a.ProductTypeCode,
        //                                   ProductTypeName = a.ProductType != null ? a.ProductType.Name : null,
        //                                   IsDistributor = a.IsDistributor,
        //                                   DistributorCode = a.DistributorCode,
        //                                   IsCustomerCare = a.Tenant == 0 && !a.IsDistributor,
        //                                   IsShowContactDetailsInTheMobileApp = a.IsShowContactDetailsInTheMobileApp,
        //                                   PersonalId = a.PersonalId,
        //                                   Technology = a.Technology,
        //                                   SetAngularAsDefault = a.SetAngularAsDefault,
        //                                   DisplayGettingStarted = a.Contact.DisplayGettingStarted,
        //                                   DontShowLocal = a.Contact.DontShowLocalLabels,
        //                                   IsTwoFactorAuthenticationEnabled = a.IsTwoFactorAuthenticationEnabled,
        //                                   DocumentFilingInbox = a.DocumentFilingInbox,
        //                                   ShowLogBoxToolTip = a.ShowLogBoxToolTip,
        //                                   ShowInboxToolTip = a.ShowInboxToolTip,
        //                                   ShowLocalNameInLOV = a.ShowLocalNameInLOV,
        //                                   UserRoles = a.UserRoles,
        //                                   SecurityLevel = a.SecurityLevel,
        //                                   AdditionalPackagesOnly = a.AdditionalPackagesOnly,
        //                                   LayoutDirection = a.LayoutDirection,
        //                                   SignatureImageId = a.SignatureImageId,
        //                               };
        //    return users;
        //}
        //public IQueryable<UserPM> GetUsersByEmailOrName(string email, string name, int tenant)
        //{
        //    string emailNew = "";
        //    emailNew = email;
        //    string nameNew = "";
        //    nameNew = name;
        //    var query = from a in context.Users.Include("Contact").Include("Department").Include("Branch").Include("Freelancer").Include("ProductType")
        //                where a.Tenant == tenant
        //                select new UserPM()
        //                {
        //                    BranchId = a.BranchId,
        //                    BranchName = a.Branch != null ? a.Branch.EnglishName : "",
        //                    DepartmentName = a.Department != null ? a.Department.EnglishName : "",
        //                    DepartmentId = a.DepartmentId,
        //                    Id = a.Id,
        //                    Notes = a.Notes,
        //                    Tenant = a.Tenant,
        //                    Email = a.Contact.Email,
        //                    EnglishName = a.Contact.EnglishName,
        //                    Anniversary = a.Contact.Anniversary,
        //                    Birthday = a.Contact.Birthday,
        //                    BusinessPhone = a.Contact.BusinessPhone,
        //                    FacebookId = a.Contact.FacebookId,
        //                    Fax = a.Contact.Fax,
        //                    InActive = a.Contact.InActive,
        //                    LocalName = a.Contact.LocalName,
        //                    SearchFields = a.SearchFields,
        //                    Mobile = a.Contact.Mobile,
        //                    DontShowLocalLabels = a.Contact.DontShowLocalLabels,
        //                    Position = a.Contact.Position,
        //                    IsBranchRestricted = a.IsBranchRestricted,
        //                    IsSalesman = a.IsSalesman,
        //                    IsFreelancer = a.IsFreelancer,
        //                    FreelancerId = a.FreelancerId,
        //                    FreelancerName = a.Freelancer != null ? a.Freelancer.EnglishName : null,
        //                    BusinessUnitId = a.BusinessUnitId,
        //                    Code = a.Code,
        //                    CreateDate = a.CreateDate,
        //                    ExpirationDate = a.ExpirationDate,
        //                    LicencedUser = a.LicencedUser,
        //                    IsProductRestricted = a.IsProductRestricted,
        //                    ProductTypeCode = a.ProductTypeCode,
        //                    ProductTypeName = a.ProductType != null ? a.ProductType.Name : null,
        //                    IsDistributor = a.IsDistributor,
        //                    DistributorCode = a.DistributorCode,
        //                    IsCustomerCare = a.Tenant == 0 && !a.IsDistributor,
        //                    IsShowContactDetailsInTheMobileApp = a.IsShowContactDetailsInTheMobileApp,
        //                    PersonalId = a.PersonalId,
        //                    Technology = a.Technology,
        //                    SetAngularAsDefault = a.SetAngularAsDefault,
        //                    DisplayGettingStarted = a.Contact.DisplayGettingStarted,
        //                    DontShowLocal = a.Contact.DontShowLocalLabels,
        //                    IsTwoFactorAuthenticationEnabled = a.IsTwoFactorAuthenticationEnabled,
        //                    DocumentFilingInbox = a.DocumentFilingInbox,
        //                    ShowLogBoxToolTip = a.ShowLogBoxToolTip,
        //                    ShowInboxToolTip = a.ShowInboxToolTip,
        //                    ShowLocalNameInLOV = a.ShowLocalNameInLOV,
        //                    UserRoles = a.UserRoles,
        //                    SecurityLevel = a.SecurityLevel,
        //                    AdditionalPackagesOnly = a.AdditionalPackagesOnly,
        //                    LayoutDirection = a.LayoutDirection,
        //                    SignatureImageId = a.SignatureImageId,
        //                };
        //    IQueryable<UserPM> query2 = null;
        //    if (!string.IsNullOrEmpty(email))
        //    {
        //        query2 = query.Where(d => d.Email.ToUpper().StartsWith(email.ToUpper()) && d.Tenant == tenant);
        //    }
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        if (query2 != null)
        //        {
        //            if (query2.Count() == 0)
        //            {
        //                query2 = query.Where(d => d.EnglishName.ToUpper().StartsWith(name.ToUpper()) && d.Tenant == tenant);
        //            }
        //        }
        //        else
        //        {
        //            query2 = query.Where(d => d.EnglishName.ToUpper().StartsWith(name.ToUpper()) && d.Tenant == tenant);
        //        }
        //    }
        //    if (query2 != null)
        //    {
        //        return query2;
        //    }
        //    else
        //        return query;
        //}

        #endregion Get Users IQueryable<UserPM>

        #region Get Users IQueryable<UserList>
        //public IQueryable<UserList> GetIQueryableEntityList(IQueryable<User> iQueryable)
        //{
        //    IQueryable<UserList> result = from user in iQueryable.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("Freelancer").Include("BusinessUnit").Include("ProductType")
        //                                  select new UserList()
        //                                  {
        //                                      Email = user.Contact.Email,
        //                                      EnglishName = user.Contact.EnglishName ?? "",
        //                                      Id = user.Id,
        //                                      InActive = user.Contact.InActive,
        //                                      Notes = user.Notes,
        //                                      Tenant = user.Tenant,
        //                                      BranchId = user.BranchId,
        //                                      DepartmentId = user.DepartmentId,
        //                                      BranchName = user.Branch != null ? user.Branch.EnglishName : "",
        //                                      DepartmentName = user.Department != null ? user.Department.EnglishName : "",
        //                                      LocalName = user.Contact.LocalName ?? "",
        //                                      SearchFields = user.Contact.SearchFields,
        //                                      IsBranchRestricted = user.IsBranchRestricted,
        //                                      IsSalesman = user.IsSalesman,
        //                                      IsFreelancer = user.IsFreelancer,
        //                                      FreelancerId = user.FreelancerId,
        //                                      FreelancerName = user.Freelancer != null ? user.Freelancer.EnglishName : null,
        //                                      BusinessUnitId = user.BusinessUnitId,
        //                                      BusinessUnitName = user.BusinessUnit == null ? "" : user.BusinessUnit.Name,
        //                                      CreateDate = user.CreateDate,
        //                                      ExpirationDate = user.ExpirationDate,
        //                                      LicencedUser = user.LicencedUser,
        //                                      LastLoginDate = user.UserLastLogin == null ? null : user.UserLastLogin.LoginDateTime,
        //                                      IsProductRestricted = user.IsProductRestricted,
        //                                      ProductTypeCode = user.ProductTypeCode,
        //                                      ProductTypeName = user.ProductType != null ? user.ProductType.Name : null,
        //                                      IsDistributor = user.IsDistributor,
        //                                      DistributorCode = user.DistributorCode,
        //                                      IsShowContactDetailsInTheMobileApp = user.IsShowContactDetailsInTheMobileApp,
        //                                      Technology = user.Technology,
        //                                      PersonalId = user.PersonalId,
        //                                      SetAngularAsDefault = user.SetAngularAsDefault,
        //                                      IsTwoFactorAuthenticationEnabled = user.IsTwoFactorAuthenticationEnabled,
        //                                      DocumentFilingInbox = user.DocumentFilingInbox,
        //                                      ShowLocalNameInLOV = user.ShowLocalNameInLOV,
        //                                      UserRoles = user.UserRoles,
        //                                      SecurityLevel = user.SecurityLevel,
        //                                      AdditionalPackagesOnly = user.AdditionalPackagesOnly,
        //                                      SignatureImageId = user.SignatureImageId,
        //                                      Mobile = user.Contact.Mobile ?? "",
        //                                  };
        //    return result;
        //}
        //public IQueryable<UserList> GetDemoTenantUserList(IQueryable<User> iQueryable, string loggedUserId, int tenant)
        //{
        //    List<UserList> result = new List<UserList>();
        //    int index = 1;
        //    foreach (User user in iQueryable.Include("UserLastLogin").Include("Contact").Include("Department").Include("Branch").Include("Freelancer").Include("BusinessUnit").Include("ProductType"))
        //    {
        //        string email = user.Contact.Email;
        //        string name = user.Contact.EnglishName;
        //        if (user.Id != loggedUserId)
        //        {
        //            if (!string.IsNullOrEmpty(email))
        //            {
        //                string[] emailParts = user.Contact.Email.Split('@');
        //                email = "user" + index + "@democompany.com ";
        //            }
        //            name = "User" + index;
        //        }
        //        UserList newItem = new UserList()
        //        {
        //            Email = email,
        //            EnglishName = name,
        //            Id = user.Id,
        //            InActive = user.Contact.InActive,
        //            Notes = user.Notes,
        //            Tenant = user.Tenant,
        //            BranchId = user.BranchId,
        //            DepartmentId = user.DepartmentId,
        //            BranchName = user.Branch != null ? user.Branch.EnglishName : "",
        //            DepartmentName = user.Department != null ? user.Department.EnglishName : "",
        //            LocalName = user.Contact.LocalName ?? "",
        //            SearchFields = user.Contact.SearchFields,
        //            IsBranchRestricted = user.IsBranchRestricted,
        //            IsSalesman = user.IsSalesman,
        //            IsFreelancer = user.IsFreelancer,
        //            FreelancerId = user.FreelancerId,
        //            FreelancerName = user.Freelancer != null ? user.Freelancer.EnglishName : null,
        //            BusinessUnitId = user.BusinessUnitId,
        //            BusinessUnitName = user.BusinessUnit == null ? "" : user.BusinessUnit.Name,
        //            CreateDate = user.CreateDate,
        //            ExpirationDate = user.ExpirationDate,
        //            LicencedUser = user.LicencedUser,
        //            LastLoginDate = user.UserLastLogin == null ? null : user.UserLastLogin.LoginDateTime,
        //            IsProductRestricted = user.IsProductRestricted,
        //            ProductTypeCode = user.ProductTypeCode,
        //            ProductTypeName = user.ProductType != null ? user.ProductType.Name : null,
        //            IsDistributor = user.IsDistributor,
        //            DistributorCode = user.DistributorCode,
        //            IsShowContactDetailsInTheMobileApp = user.IsShowContactDetailsInTheMobileApp,
        //            Technology = user.Technology,
        //            PersonalId = user.PersonalId,
        //            SetAngularAsDefault = user.SetAngularAsDefault,
        //            IsTwoFactorAuthenticationEnabled = user.IsTwoFactorAuthenticationEnabled,
        //            DocumentFilingInbox = user.DocumentFilingInbox,
        //            ShowLocalNameInLOV = user.ShowLocalNameInLOV,
        //            SignatureImageId = user.SignatureImageId,
        //        };
        //        result.Add(newItem);
        //        index++;
        //    }
        //    return result.AsQueryable();
        //}

        #endregion Get Users IQueryable<UserList>

        #region Get Users List<UserList>

        public List<UserList> GetUserListByUserIds(List<string> userIds, int tenant)
        {
            List<UserList> users = (from a in context.Users.Include("Contact")
                                    where userIds.Contains(a.Id) && a.Tenant == tenant
                                    select new UserList()
                                    {
                                        BranchId = a.BranchId,
                                        BranchName = a.Branch != null ? a.Branch.EnglishName : "",
                                        DepartmentName = a.Department != null ? a.Department.EnglishName : "",
                                        DepartmentId = a.DepartmentId,
                                        Id = a.Id,
                                        Notes = a.Notes,
                                        Tenant = a.Tenant,
                                        Email = a.Contact.Email,
                                        EnglishName = a.Contact.EnglishName,
                                        InActive = a.Contact.InActive,
                                        LocalName = a.Contact.LocalName,
                                        SearchFields = a.SearchFields,
                                        IsBranchRestricted = a.IsBranchRestricted,
                                        IsSalesman = a.IsSalesman,
                                        IsFreelancer = a.IsFreelancer,
                                        FreelancerId = a.FreelancerId,
                                        FreelancerName = a.Freelancer != null ? a.Freelancer.EnglishName : null,
                                        BusinessUnitId = a.BusinessUnitId,
                                        CreateDate = a.CreateDate,
                                        ExpirationDate = a.ExpirationDate,
                                        LicencedUser = a.LicencedUser,
                                        IsProductRestricted = a.IsProductRestricted,
                                        ProductTypeCode = a.ProductTypeCode,
                                        ProductTypeName = a.ProductType != null ? a.ProductType.Name : null,
                                        IsDistributor = a.IsDistributor,
                                        DistributorCode = a.DistributorCode,
                                        IsShowContactDetailsInTheMobileApp = a.IsShowContactDetailsInTheMobileApp,
                                        PersonalId = a.PersonalId,
                                        Technology = a.Technology,
                                        SetAngularAsDefault = a.SetAngularAsDefault,
                                        Mobile = a.Contact.Mobile,
                                        Fax = a.Contact.Fax,
                                        BusinessPhone = a.Contact.BusinessPhone,
                                        IsTwoFactorAuthenticationEnabled = a.IsTwoFactorAuthenticationEnabled,
                                        DocumentFilingInbox = a.DocumentFilingInbox,
                                        ShowLocalNameInLOV = a.ShowLocalNameInLOV,
                                        SignatureImageId = a.SignatureImageId,
                                    }).ToList();
            return users;
        }
        public List<UserList> GetUserListsByidsString(string ids, int tenant)
        {
            List<UserList> users = new List<UserList>();
            List<string> emailsList = ids.Split(';').Select(p => p.Trim()).ToList().Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (emailsList.Count() > 0)
            {
                users = (from a in context.Users.Include("Contact")
                         where emailsList.Contains(a.Contact.Id) && a.Tenant == tenant
                         select new UserList()
                         {
                             Id = a.Id,
                             Email = a.Contact.Email,
                             SearchFields = a.SearchFields,
                             EnglishName = a.Contact.EnglishName,
                             LocalName = a.Contact.LocalName,
                             DocumentFilingInbox = a.DocumentFilingInbox,
                         }).ToList();
                #region SetInActiveUsers
                ContactQuery contactQuery = new ContactQuery(tenant);
                List<string> contactId = users.Select(d => d.Id).ToList();
                List<ContactList> contactLists = contactQuery.GetContactListsByListIds(contactId, tenant).Where(d => d.InActive).ToList();
                foreach (ContactList contact in contactLists)
                {
                    var user = users.Where(d => d.Id == contact.Id).FirstOrDefault();
                    user.InActive = contact.InActive;
                }
                #endregion
            }
            return users;
        }
        public List<UserList> GetUserListsByEmailsString(string emails, int tenant)
        {
            List<UserList> users = new List<UserList>();
            List<string> emailsList = emails.Split(';').Select(p => p.Trim()).ToList().Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (emailsList.Count() > 0)
            {
                users = (from a in context.Users.Include("Contact")
                         where emailsList.Contains(a.Contact.Email) && a.Tenant == tenant
                         select new UserList()
                         {
                             Id = a.Id,
                             Email = a.Contact.Email,
                             SearchFields = a.SearchFields,
                             EnglishName = a.Contact.EnglishName,
                             LocalName = a.Contact.LocalName,
                             DocumentFilingInbox = a.DocumentFilingInbox,
                         }).ToList();
            }
            return users;
        }
        public List<UserList> GetUserListsByTenant(int tenant, bool includeInactiveUsers)
        {
            List<UserList> result = new List<UserList>();
            IQueryable<UserList> users = (from a in context.Users.Include("Contact")
                                          where a.Tenant == tenant
                                          select new UserList()
                                          {
                                              Id = a.Id,
                                              Email = a.Contact.Email,
                                              EnglishName = a.Contact.EnglishName,
                                              InActive = a.Contact.InActive,
                                          });
            if (!includeInactiveUsers)
            {
                users = users.Where(d => !d.InActive);
            }
            result = users.ToList();
            List<string> userIds = result.GroupBy(d => d.Id).Select(d => d.First().Id).ToList();
            UserLastLoginRepository rep = new UserLastLoginRepository(this.context);
            UserLastLoginQuery query = new UserLastLoginQuery(rep);
            return result;
        }
        #endregion Get Users List<UserList>

    }
}