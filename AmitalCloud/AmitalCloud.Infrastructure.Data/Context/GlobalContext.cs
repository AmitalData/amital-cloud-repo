using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Enums;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AmitalCloud.Infrastructure.Data.BaseClasses;
using AmitalCloud.Infrastructure.Data.Helpers;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using AmitalCloud.Infrastructure.Data.DBHelpers;
using Microsoft.Extensions.Logging;


namespace AmitalCloud.Infrastructure.Data.Context
{
    public class GlobalContext : DbContextBase, IGlobalContext
    {
        private GlobalContext(DbContextOptions options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
            this.ChangeTracker.AutoDetectChangesEnabled = false;
        }
        public void SetAsModified(object entity)
        {
            this.Entry(entity).State = EntityState.Modified;
        }
        public static IGlobalContext OverrideIGlobalContextFake { get; set; }

        public static IGlobalContext GetContext(int? ConnectionLifetime = null, bool? suppressPool = null)
        {
            if (OverrideIGlobalContextFake != null)
            {
                return OverrideIGlobalContextFake;
            }

            string dbConnectionInfo = ConfigurationHelper.GetConnectionString(DbContextBaseUtil.GlobalConnectionString);
            dbConnectionInfo = DbContextBaseUtil.GetConnectionStringWithAmitalNetRole(dbConnectionInfo);

            DbContextOptionsBuilder<GlobalContext> optionsBuilder = new DbContextOptionsBuilder<GlobalContext>();
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                DbConnection connection = DatabaseInitializer.GetConnection(dbConnectionInfo, ConnectionLifetime, suppressPool);
                optionsBuilder.UseOracle(connection);
            }
            else
            {
                dbConnectionInfo = DatabaseInitializer.GetConnectionString(dbConnectionInfo, ConnectionLifetime, suppressPool);
                optionsBuilder.UseSqlServer(dbConnectionInfo);
            }

            if (DbContextBaseUtil.ToLog.GetValueOrDefault())
            {
                optionsBuilder
                    .EnableSensitiveDataLogging()
                    .LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Debug);
            }

            return new GlobalContext(optionsBuilder.Options);
        }

        //public static GlobalContext GetContextByDBId(string dbId)
        //{
        //    GlobalDB currentDb;
        //    currentDb = GlobalDbHelper.GetGlobalDBById(dbId);
        //    string dbConnectionInfo = "";
        //    if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
        //    {
        //        dbConnectionInfo = ConfigurationManager.ConnectionStrings["Oracle_Globalstr"].ConnectionString;
        //    }
        //    else
        //    {
        //        dbConnectionInfo = ConfigurationManager.ConnectionStrings["Globalstr"].ConnectionString;
        //    }
        //    if (dbConnectionInfo.Contains("Main"))
        //    { }

        //     dbConnectionInfo = DBHelpers.DbContextBaseUtil.GetConnectionStringWithAmitalNetRole(dbConnectionInfo);
        //    DbConnection connection = DatabaseInitializer.GetConnection(dbConnectionInfo);
        //    GlobalContext context = new GlobalContext(connection);

        //    return context;
        //}

        public override AmitalCloudDBSchema AmitalCloudDBSchema
        {
            get { return AmitalCloudDBSchema.AMITAL_GLOBAL; }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            //{
            //    var config = OracleEntityProviderConfig.Instance;
            //    config.Workarounds.DisableQuoting = true;
            //    //config.QueryOptions.CaseInsensitiveComparison = true;
            //    //config.QueryOptions.CaseInsensitiveLike = true;

            //    //modelBuilder.Conventions.Remove<ColumnTypeCasingConvention>();
            //}



            //Database.SetInitializer<GlobalContext>(null);
            //string databasename = DatabaseInitializer.GetDatabaseName();
            //Database.DefaultConnectionFactory.CreateConnection(databasename);
            //modelBuilder.Configurations.Add(new AnalyzeQueueMap());
            //modelBuilder.Configurations.Add(new AnalyzeQueueStatuMap());
            //modelBuilder.Configurations.Add(new ConvertProgramInfoMap());
            //modelBuilder.Configurations.Add(new GlobalContactMap());
            //modelBuilder.Configurations.Add(new GlobalDBMap());
            //modelBuilder.Configurations.Add(new GlobalTenantCounterMap());
            //modelBuilder.Configurations.Add(new GlobalTenantMap());
            //modelBuilder.Configurations.Add(new LogitudeLeadsMap());
            //modelBuilder.Configurations.Add(new PaymentChannelMap());
            //modelBuilder.Configurations.Add(new PaymentMethodMap());
            //modelBuilder.Configurations.Add(new PerformanceLogMap());
            //modelBuilder.Configurations.Add(new RecurringPeriodMap());
            //modelBuilder.Configurations.Add(new TenantManagementMap());
            //modelBuilder.Configurations.Add(new ContactPasswordMap());
            //modelBuilder.Configurations.Add(new PasswordResetRequestMap());
            //modelBuilder.Configurations.Add(new SettingMap());
            //modelBuilder.Configurations.Add(new PaymentCurrencyMap());
            //modelBuilder.Configurations.Add(new AutoSignupEmailMap());
            //modelBuilder.Configurations.Add(new BluesnapContractMap());
            //modelBuilder.Configurations.Add(new BluesnapTransactionMap());
            //modelBuilder.Configurations.Add(new BluesnapContractTypeMap());
            //modelBuilder.Configurations.Add(new AWBMessagesCCSTypeMap());
            //modelBuilder.Configurations.Add(new MobileNotificationLogMap());
            //modelBuilder.Configurations.Add(new ContactMobileDeviceMap());
            //modelBuilder.Configurations.Add(new SystemMetadataLastUpdateMap());
            //modelBuilder.Configurations.Add(new MonitorServiceLastUpdateMap());
            //modelBuilder.Configurations.Add(new HelpResourceMap());
            //modelBuilder.Configurations.Add(new ChangePasswordLogMap());
            //modelBuilder.Configurations.Add(new TenantTypeMap());
            //modelBuilder.Configurations.Add(new ApiCredintialsMap());
            //modelBuilder.Configurations.Add(new TenantManagementLicenseMap());
            //modelBuilder.Configurations.Add(new TenantAddOnMap());
            //modelBuilder.Configurations.Add(new BatchServicesDefinitionModsMap());
            //modelBuilder.Configurations.Add(new OneTimePasswordMap());
            //modelBuilder.Configurations.Add(new TenantManagmentPrivateLabelsMap());
            ////modelBuilder.Configurations.Add(new AgentSharedLogisticsKeyMap());
            //modelBuilder.Configurations.Add(new SessionPolicyMap());
            //modelBuilder.Configurations.Add(new CaptchaKeyMap());
            //modelBuilder.Configurations.Add(new InvalidEmailResetPasswordMap());
            //modelBuilder.Configurations.Add(new WebhookKeysMap());

            ////Was Missing
            //modelBuilder.Configurations.Add(new BatchServicesDefinitionMap());
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose();
            }

        }

        public override void Dispose()
        {
            //Database.Connection.Close();
            Dispose(true);
        }

        public void DetectChanges()
        {
            this.ChangeTracker.DetectChanges();
        }

        public override int SaveChanges()
        {
            DetectChanges();
            return base.SaveChanges();
        }

        public string GetCurrentConnection()
        {
            return this.Database.GetDbConnection().ConnectionString;
        }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<ContactMobileDevice> ContactMobileDevices { get; set; }
        public DbSet<MobileNotificationLog> MobileNotificationLogs { get; set; }
        public DbSet<GlobalContact> GlobalContacts { get; set; }
        public DbSet<GlobalTenant> GlobalTenants { get; set; }
        public DbSet<GlobalDB> GlobalDBs { get; set; }
        public DbSet<ConvertProgramInfo> ConvertProgramInfoes { get; set; }
        public DbSet<PerformanceLog> PerformanceLogs { get; set; }
        public DbSet<LogitudeLead> LogitudeLeads { get; set; }
        public DbSet<TenantManagement> TenantManagements { get; set; }
        public DbSet<AnalyzeQueue> AnalyzeQueues { get; set; }
        public DbSet<AnalyzeQueueStatus> AnalyzeQueueStatus { get; set; }
        public DbSet<RecurringPeriod> RecurringPeriods { get; set; }
        public DbSet<PaymentChannel> PaymentChannels { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<ContactPassword> ContactPasswords { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        public DbSet<PaymentCurrency> PaymentCurrencies { get; set; }
        public DbSet<BluesnapContract> BluesnapContracts { get; set; }
        public DbSet<BluesnapTransaction> BluesnapTransactions { get; set; }
        public DbSet<BluesnapContractType> BluesnapContractTypes { get; set; }
        public DbSet<AutoSignupEmail> AutoSignupEmails { get; set; }
        public DbSet<AWBMessagesCCSType> AWBMessagesCCSTypes { get; set; }
        public DbSet<SystemMetadataLastUpdate> SystemMetadataLastUpdates { get; set; }
        public DbSet<MonitorServiceLastUpdate> MonitorServiceLastUpdates { get; set; }
        public DbSet<HelpResource> HelpResources { get; set; }
        public DbSet<BatchServicesDefinition> BatchServicesDefinitions { get; set; }
        public DbSet<ChangePasswordLog> ChangePasswordLogs { get; set; }
        public DbSet<TenantType> TenantTypes { get; set; }
        public DbSet<ApiCredintials> ApiCredintials { get; set; }
        public DbSet<TenantManagementLicense> TenantManagementLicenses { get; set; }
        public DbSet<TenantAddOn> TenantAddOns { get; set; }
        public DbSet<BatchServicesDefinitionMods> BatchServicesDefinitionMods { get; set; }
        public DbSet<OneTimePassword> OneTimePasswords { get; set; }
        public DbSet<TenantManagmentPrivateLabels> TenantManagmentPrivateLabels { get; set; }

        public DbSet<AgentSharedLogisticsKey> AgentSharedLogisticsKeys { get; set; }
        public DbSet<SessionPolicy> SessionPolicies { get; set; }
        public DbSet<CaptchaKey> CaptchaKeys { get; set; }
        public DbSet<InvalidEmailResetPassword> InvalidEmailResetPasswords { get; set; }
        public DbSet<WebhookKeys> WebhookKeys { get; set; }
        public DbSet<AuthenticationToken> AuthenticationTokens { get; set; }
        public int Tenant => throw new System.NotImplementedException();

        public DbConnection GetConnection()
        {
            return this.Database.GetDbConnection();
        }

        public DbContext GetActiveDbContext()
        {
            return this;
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}