using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Web.Middlewares;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Application.Helpers;
using System.Reflection;
using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AmitalCloud.Infrastructure.Web.BaseClasses;
using Microsoft.EntityFrameworkCore;
using AmitalCloud.Infrastructure.Data.Queries;

namespace AmitalCloud.Infrastructure.Web.Helpers
{
    public class WebApp
    {
        public static void Start(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("Properties\\launchSettings.json", optional: true, reloadOnChange: true);

            // add request services
            builder.Services.AddControllers(options =>
            {
                // catch exceptions and return http code according to it
                options.Filters.Add<AuthenticationExceptionFilter>();

            }).AddJsonOptions(options =>
            {
                // preserve the original casing of JSON properties
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                options.EnableAnnotations();  

            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            // add Cache & HttpContext services
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ICacheWrapper, CacheWrapper>();
            builder.Services.AddHttpContextAccessor();

            // prepare Configuration for services
            builder.Services.AddScoped<GlobalDbHelper>();
            builder.Services.AddScoped<GlobalDBRepository>();
            ConfigurationHelper.Initialize(builder.Configuration);
            builder.Services.AddScoped<ILoggedContactUtil, AmitalCloud.Infrastructure.Data.Security.LoggedContactUtil>();
            builder.Services.AddScoped<ITreeFilterQueryService, TreeFilterQuery.TreeFilterQueryService>();
            builder.Services.AddScoped<LoggedContactResolver>();

			builder.Services.AddScoped<ITenantProvider, TenantProvider>();
			builder.Services.AddDbContext<AmitalCloudContext>(options =>
	        options.UseSqlServer(builder.Configuration.GetConnectionString("Mainstr")));
			builder.Services.AddScoped<IAmitalCloudContext, AmitalCloudContext>();


			builder.Services.AddDbContext<GlobalContext>(options =>
		    options.UseSqlServer(builder.Configuration.GetConnectionString("Globalstr1")));
			builder.Services.AddScoped<IGlobalContext, GlobalContext>();

			builder.Services.AddScoped<ITenantStatusQueryService, TenantStatusQueryService>();
			builder.Services.AddScoped<IAuthentication, Authentication>();

			builder.Services.AddScoped<IBaseQueryService<UserPM, User,string>, UserQueryService>();
			builder.Services.AddScoped<IBaseQueryService<TenantManagementPM, TenantManagement, int>,TenantManagementQueryService>();
			builder.Services.AddScoped<IBaseEntityQueryService<ObjectFieldModificationPM, ObjectFieldModification>, ObjectFieldModificationQueryService>();
			builder.Services.AddScoped<IBaseEntityQueryService<ObjectTableLastUpdatePM, ObjectTableLastUpdate>, ObjectTableLastUpdateQueryService>();
			builder.Services.AddScoped<ISystemMetadataLastUpdateQuery, SystemMetadataLastUpdateQuery>();

			var app = builder.Build();

            InitializeApp(app, builder.Configuration);

            // map the default route
            app.MapGet("/", () => MapGetContent(builder.Configuration));

            app.Run();
        }



		private static void InitializeApp(WebApplication app, IConfiguration configuration)
        {
            app.UseRouting();
            app.MapControllers();
            app.UseMiddleware<AuthenticationTokenMiddleware>();
            app.UseMiddleware<HttpContextHelperMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
                app.UseCors("AllowAll");
            }

            CacheManager.CacheWrapper = app.Services.GetRequiredService<ICacheWrapper>();

            if (string.IsNullOrEmpty(AmitalCloudSettings.DeploymentStage))
            {
                string? dbms = configuration["DBMS"];
                if (string.IsNullOrEmpty(dbms))
                {
                    throw new Exception("DBMS not found in appsettings.json.");
                }

                AmitalCloudSettings.DatabaseManagementSystem = dbms;
                AmitalCloudSettings.DebugKey = configuration["DebugKey"];
                FillAppSettings();
                InitInjectionUtil();
            }
        }

        public static void FillAppSettings()
        {
            AmitalCloud.Infrastructure.Domain.EntityPMs.SettingPM setting = new AmitalCloud.Infrastructure.Application.EntityQueryServices.SettingQueryService(0)
                .GetSingle(AmitalCloud.Infrastructure.Data.Queries.SettingQuery.GetDefaultSettingId(), false, true);

            AmitalCloudSettings.Id = setting.Id;
            AmitalCloudSettings.ChampEnv = setting.ChampEnv;
            AmitalCloudSettings.ChampURL = setting.ChampURL;
            AmitalCloudSettings.ChampTestAPIURL = setting.ChampTestAPIURL;
            AmitalCloudSettings.ChampTestAPIPassword = setting.ChampTestAPIPassword;
            AmitalCloudSettings.ChampProdAPIURL = setting.ChampProdAPIURL;
            AmitalCloudSettings.ChampProdAPIPassword = setting.ChampProdAPIPassword;
            AmitalCloudSettings.CustomerCareIP = setting.CustomerCareIP;
            AmitalCloudSettings.DeploymentStage = setting.DeploymentStage;
            AmitalCloudSettings.IsLogEnabled = setting.IsLogEnabled;
            AmitalCloudSettings.AmitalURL = setting.LogitudeURL;
            AmitalCloudSettings.TotangoServiceId = setting.TotangoServiceId;
            AmitalCloudSettings.UsingAzure = setting.UsingAzure;
            AmitalCloudSettings.StorageAccountKey = setting.StorageAccountKey;
            AmitalCloudSettings.StorageAccountName = setting.StorageAccountName;
            AmitalCloudSettings.StorageType = setting.StorageType;
            AmitalCloudSettings.AmitalCRMTenantNumber = setting.LogitudeCRMTenantNumber;
            AmitalCloudSettings.AutoSignupEmail = setting.AutoSignupEmail;
            AmitalCloudSettings.AutoSignupPassword = setting.AutoSignupPassword;
            AmitalCloudSettings.ForceHttps = setting.ForceHttps;
            AmitalCloudSettings.CheckConnectionURL = setting.CheckConnectionURL;
            AmitalCloudSettings.AndroidSharedAppMinimumVersion = setting.AndroidSharedAppMinimumVersion;
            AmitalCloudSettings.IOSSharedAppMinimumVersion = setting.IOSSharedAppMinimumVersion;
            AmitalCloudSettings.WorkEnvironment = setting.WorkEnvironment;
            AmitalCloudSettings.LogoCode = setting.LogoCode;
            AmitalCloudSettings.EnableHybridQueue = setting.EnableHybridQueue;
            AmitalCloudSettings.EmailAlertSignature = setting.EmailAlertSignature;
            AmitalCloudSettings.IOSAppLink = setting.IOSAppLink;
            AmitalCloudSettings.AndroidAppLink = setting.AndroidAppLink;
            AmitalCloudSettings.AndroidPodAppMinimumVersion = setting.AndroidPodAppMinimumVersion;
            AmitalCloudSettings.IOSPodAppMinimumVersion = setting.IOSPodAppMinimumVersion;
            AmitalCloudSettings.MinimumOutlookVersion = setting.MinimumOutlookVersion;
            AmitalCloudSettings.ABMProductId = setting.ABMProductId;
            AmitalCloudSettings.AzureFolderName = setting.AzureFolderName;
            AmitalCloudSettings.SignAppVersion = setting.SignAppVersion;
            AmitalCloudSettings.ReportsRunUsingWR = setting.ReportsRunUsingWR;
            AmitalCloudSettings.SMSServiceUserId = setting.SMSServiceUserId;
            AmitalCloudSettings.SMSServiceAuthToken = setting.SMSServiceAuthToken;
            AmitalCloudSettings.SMSServicePhoneNumber = setting.SMSServicePhoneNumber;
            AmitalCloudSettings.GLSHKEnv = setting.GLSHKEnv;
            AmitalCloudSettings.GLSHKURL = setting.GLSHKURL;
            AmitalCloudSettings.NotificationHubName = setting.NotificationHubName;
            AmitalCloudSettings.NotificationHubConnectionString = setting.NotificationHubConnectionString;
            AmitalCloudSettings.DomainName = setting.DomainName;
            AmitalCloudSettings.ProductName = setting.ProductName;
            AmitalCloudSettings.QueueServiceMode = setting.QueueServiceMode;
            AmitalCloudSettings.StorageServiceMode = setting.StorageServiceMode;
            AmitalCloudSettings.DropboxAppKey = setting.DropboxAppKey;
            AmitalCloudSettings.DropboxAppSecret = setting.DropboxAppSecret;
            AmitalCloudSettings.OceanInsightsToken = setting.OceanInsightsToken;
            AmitalCloudSettings.CPUIntensiveWebServicesURL = setting.CPUIntensiveWebServicesURL;
            AmitalCloudSettings.AmitalCloudEnvironmentURL = setting.AmitalCloudEnvironmentURL;
            AmitalCloudSettings.AmitalCloudAmitalTenantPrimaryKey = setting.AmitalCloudLogitudeTenantPrimaryKey;
            AmitalCloudSettings.OITenantNumber = setting.OITenantNumber;
            AmitalCloudSettings.AzurePrincipalSecretKey = setting.AzurePrincipalSecretKey;
            AmitalCloudSettings.DNSZone = setting.DNSZone;
            AmitalCloudSettings.DNSIPAddress = setting.DNSIPAddress;
            AmitalCloudSettings.WorkflowStorageAccountName = setting.WorkflowStorageAccountName;
            AmitalCloudSettings.WorkflowStorageAccountKey = setting.WorkflowStorageAccountKey;
            AmitalCloudSettings.System2RedirectFraction = setting.System2RedirectFraction;
            AmitalCloudSettings.WindWardSettings = setting.WindWardSettings;
            AmitalCloudSettings.AmitalIISURL = setting.LogitudeIISURL;
            AmitalCloudSettings.TempStorageConnection = setting.TempStorageConnection;
        }

        public static void InitInjectionUtil()
        {
            Func<IAmitalRestrictOwnerService>? createAmitalRestrictOwnerModelService = null;

            Func<int> getTenantFromToken = () =>
            {
                string? token = HttpContextHelper.Request?.Headers["Token"];
                AmitalCloud.Infrastructure.Model.EntityClasses.AuthenticationToken authToken = AuthenticationTokenRepository.GetSingleTokenFromCache(token);
                return authToken?.Tenant ?? 0;
            };

            InjectionUtil.Init(createAmitalRestrictOwnerModelService, getTenantFromToken, AmitalCloudSecurityUtility.CheckContactFeature,
                () => (new ByteCompressorUtil()) as IByteCompressorUtil,
                () => (new TreeFilterQuery.TreeFilterQueryService()) as ITreeFilterQueryService);
        }

        private static string MapGetContent(IConfiguration configuration)
        {
            string content = "Ready!";
            try
            {
                string applicationUrl = configuration.GetValue<string>("iisSettings:iisExpress:applicationUrl");
                content = $"{content}\n\n\nTo visit Swagger:\n{applicationUrl}/swagger\n{applicationUrl}/swagger/v1/swagger.json";
            }
            catch { }
            return content;
        }
    }
}
