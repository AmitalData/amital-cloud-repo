using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class UserData
    {
        public string UserName { get; set; } = string.Empty;
        [Key]
        public string Id { get; set; } = string.Empty;
        public string CardId { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public int CurrentTenant { get; set; }
        public bool IsUser { get; set; }
        public string Technology { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public bool IpRestricted { get; set; }
        public bool InValidMailOrPassword { get; set; }
        public bool HasError { get; set; }
        public bool MustChangePassword { get; set; }
        public List<CompanyLogin> CompanyLogins { get; set; } = new List<CompanyLogin>();
        public int ContactsCount { get; set; }
        public string HtmlVersion { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int Tenant { get; set; }
        public bool InvalidMobileAccessPermission { get; set; }
        public bool InActive { get; set; }
        public bool PrivateLablehasZeroTenant { get; set; }
        public bool IsBrandingEnabled { get; set; }
        public bool Unlicensed { get; set; }
        public bool IsTwoFactorAuthenticationRequired { get; set; }
        public DateTime? CodeExpirationDate { get; set; }
        public string UserMobileNumber { get; set; } = string.Empty;
        public string TwoFactorkey { get; set; } = string.Empty;
        public bool KeepUserLoggedIn { get; set; }
        public string PasswordExpirationDateMessage { get; set; } = string.Empty;
        public string DocumentDownloadToken { get; set; } = string.Empty;
        public int NumberOfRetries { get; set; }
        public decimal SessionTimeout { get; set; }
        public int WebTokenExpirationWarningInMinutes { get; set; }
        public int WebTokenLifeTimeInMinutes { get; set; }
        public bool InvalidEmailAddress { get; set; }
        public string CaptchaImage { get; set; } = string.Empty;
        public string CaptchaKey { get; set; } = string.Empty;
        public bool InValidCaptcha { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime? DigitalLastLoginDateTime { get; set; }
        public string InvalidDocumentToken { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? Name { get; set; }
        public string AzureAdToken { get; set; } = string.Empty;

    }


    public class CompanyLogin
    {
        public int Tenant { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public bool IsUser { get; set; }
        public string CardId { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactId { get; set; } = string.Empty;
        public bool LicensedUser { get; set; }
        public bool InternetAccess { get; set; }
        public string URL { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string PrivateLabelId { get; set; } = string.Empty;
        public bool HasLogboxAccess { get; set; }
    }


}