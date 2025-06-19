using AmitalCloud.Infrastructure.Model.EntityClasses;
using Microsoft.EntityFrameworkCore;

namespace AmitalCloud.Infrastructure.Model.Interfaces
{
    public interface IGlobalContext : IContext
    {
        DbSet<GlobalContact> GlobalContacts { get; }
        DbSet<GlobalTenant> GlobalTenants { get; }
        DbSet<GlobalDB> GlobalDBs { get; }
        DbSet<ConvertProgramInfo> ConvertProgramInfoes { get; }
        DbSet<PerformanceLog> PerformanceLogs { get; }
        DbSet<LogitudeLead> LogitudeLeads { get; }
        DbSet<TenantManagement> TenantManagements { get; }
        DbSet<AnalyzeQueue> AnalyzeQueues { get; }
        DbSet<AnalyzeQueueStatus> AnalyzeQueueStatus { get; }
        DbSet<RecurringPeriod> RecurringPeriods { get; }
        DbSet<PaymentChannel> PaymentChannels { get; }
        DbSet<PaymentMethod> PaymentMethods { get; }
        DbSet<ContactPassword> ContactPasswords { get; }
        DbSet<PasswordResetRequest> PasswordResetRequests { get; }
        DbSet<Setting> Settings { get; }
        DbSet<PaymentCurrency> PaymentCurrencies { get; }
        DbSet<AutoSignupEmail> AutoSignupEmails { get; }
        DbSet<BluesnapContract> BluesnapContracts { get; }
        DbSet<BluesnapTransaction> BluesnapTransactions { get; }
        DbSet<AWBMessagesCCSType> AWBMessagesCCSTypes { get; }
        DbSet<ContactMobileDevice> ContactMobileDevices { get; }
        DbSet<MobileNotificationLog> MobileNotificationLogs { get; }
        DbSet<SystemMetadataLastUpdate> SystemMetadataLastUpdates { get; }
        DbSet<MonitorServiceLastUpdate> MonitorServiceLastUpdates { get; }
        DbSet<HelpResource> HelpResources { get; }
        DbSet<BatchServicesDefinition> BatchServicesDefinitions { get; }
        DbSet<ChangePasswordLog> ChangePasswordLogs { get; }
        DbSet<TenantType> TenantTypes { get; }
        DbSet<ApiCredintials> ApiCredintials { get; }
        DbSet<TenantManagementLicense> TenantManagementLicenses { get; }
        DbSet<TenantAddOn> TenantAddOns { get; }
        DbSet<OneTimePassword> OneTimePasswords { get; }
        DbSet<BatchServicesDefinitionMods> BatchServicesDefinitionMods { get; }
        DbSet<TenantManagmentPrivateLabels> TenantManagmentPrivateLabels { get; }
        DbSet<AgentSharedLogisticsKey> AgentSharedLogisticsKeys { get; }
        DbSet<SessionPolicy> SessionPolicies { get; }
        DbSet<CaptchaKey> CaptchaKeys { get; }
        DbSet<InvalidEmailResetPassword> InvalidEmailResetPasswords { get; }
        DbSet<WebhookKeys> WebhookKeys { get; }
        DbSet<BluesnapContractType> BluesnapContractTypes { get; }
        DbSet<AuthenticationToken> AuthenticationTokens { get;  }
        string GetCurrentConnection();
        void DetectChanges();
    }
}