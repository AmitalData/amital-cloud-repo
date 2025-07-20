
namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class LoginParameters
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsUser { get; set; }
        public string? CardId { get; set; }
        public string? CardType { get; set; }
        public bool ByToken { get; set; }
        public bool IsMobileLogin { get; set; }
        public bool GetToken { get; set; }
        public int MobileVersion { get; set; }
        public bool IsAngularLogin { get; set; }
        public bool InternalLoginValidationCall { get; set; }
        public string ClientType { get; set; } = string.Empty;
        public string? CaptchaKey { get; set; }
        public string? CaptchaCode { get; set; }
        public bool GetInvalidDocumentToken { get; set; }
        public bool IsCargoTracking { get; set; }
        public bool IsFromPLSignApp { get; set; }
        public int Tenant { get; set; }
        public bool IsCustomsBook { get; set; }
        public bool IsAzureAdLogin { get; set; }
        public string AzureAdToken { get; set; } = string.Empty;

    }
}
