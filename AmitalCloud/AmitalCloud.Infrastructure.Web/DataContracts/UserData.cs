using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Web.DataContracts
{
    public class UserData
    {
        public string UserName { get; set; }
        [Key]
        public string Id { get; set; }

        public string CardId { get; set; }
        public string CardType { get; set; }
        public int CurrentTenant { get; set; }
        //public string CompanyName { get; set; }
        public bool IsUser { get; set; }
        public string Technology { get; set; }
        public bool IsLocked { get; set; }
        public bool IpRestricted { get; set; }
        public bool InValidMailOrPassword { get; set; }
        public bool HasError { get; set; }
        public string ExceptionMessage { get; set; }
        public bool MustChangePassword { get; set; }

        public List<CompanyLogin> CompanyLogins { get; set; }
        public int ContactsCount { get; set; }
        public string HtmlVersion { get; set; }
        public string Token { get; set; }
        //#region IUser Members
        public string UserId { get; set; }
        public int Tenant { get; set; }
        public bool InvalidToken { get; set; }

        public bool InvalidMobileAccessPermission { get; set; }

        public CompanyLogin SelectedCompanyLogin { get; set; }

        public bool InActive { get; set; }

        public bool PrivateLablehasZeroTenant { get; set; }

        public bool IsBrandingEnabled { get; set; }

        public bool Unlicensed { get; set; }

        public string SilverlightEndDate { get; set; }

        public bool IsTwoFactorAuthenticationRequired { get; set; }
        public DateTime? CodeExpirationDate { get; set; }
        public string UserMobileNumber { get; set; }
        public string TwoFactorkey { get; set; }
        public bool KeepUserLoggedIn { get; set; }
        public string PasswordExpirationDateMessage { get; set; }
        public bool IsPasswordExpirationDate { get; set; }

        public string DocumentDownloadToken { get; set; }

        public int NumberOfRetries { get; set; }

        public decimal SessionTimeout { get; set; }
        public int WebTokenExpirationWarningInMinutes { get; set; }
        public int WebTokenLifeTimeInMinutes { get; set; }

        public bool Param1 { get; set; } // Email IsValid 


        public string CaptchaImage { get; set; }
        public string CaptchaKey { get; set; }
        public bool InValidCaptcha { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime? DigitalLastLoginDateTime { get; set; }
        public string InvalidDocumentToken { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? Name { get; set; }
    }


    public class CompanyLogin
    {
        public int Tenant { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public bool IsUser { get; set; }
        public string CardId { get; set; }
        public string CardType { get; set; }
        public string Email { get; set; }
        public string ContactId { get; set; }
        public bool LicensedUser { get; set; }
        public bool InternetAccess { get; set; }
        public string URL { get; set; }
        public string Extension { get; set; }
        public string Id { get; set; }
        public string PrivateLabelId { get; set; }
        public bool HasLogboxAccess { get; set; }
    }

    public class ApiCredential
    {
        public int Tenant { get; set; }
        public bool CustomerCare { get; set; }
        public bool InValidKey { get; set; }
        public bool IpRestricted { get; set; }
        public bool HasError { get; set; }
        public string Token { get; set; }
        public string DocumentDownloadToken { get; set; }
        public int? TokenExpirationTime { get; set; }
    }
}