using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Web.Middlewares;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Application.Helpers;
using System.Reflection;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Microsoft.AspNetCore.OData;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Model.EntityClasses;



namespace AmitalCloud.Infrastructure.Web.Helpers
{
    public class WebApp
    {
        public static void Start(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("Properties\\launchSettings.json", optional: true, reloadOnChange: true);

            // Serilog
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });

            //  Application Insights
            builder.Services.AddApplicationInsightsTelemetry();
            builder.Services.AddSingleton<ITelemetryInitializer, HttpDependenciesParsingTelemetryInitializer>();

            // add request services
            builder.Services.AddControllers(options =>
            {
                // catch exceptions and return http code according to it
                options.Filters.Add<AuthenticationExceptionFilter>();

            }).AddJsonOptions(options =>
            {
                // preserve the original casing of JSON properties
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            }).AddOData(opt =>
            opt.Filter().OrderBy().Count().Expand().Select().SetMaxTop(1000));

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
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<Feature, FeatureList>();
            });

            builder.Services.AddScoped<AmitalCloudContext>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

                var tenantId = int.TryParse(httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-ID"], out var tid) ? tid : 0;

                return (AmitalCloudContext)AmitalCloudContext.GetContext(tenantId);
            });



            var assemblies = AppDomain.CurrentDomain
              .GetAssemblies()
              .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.FullName))
              .ToList();

            var queryProviderInterfaceType = typeof(IEntityListQueryProvider<,>);

            var queryProviderImplementations = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == queryProviderInterfaceType)
                    .Select(i => new { Interface = i, Type = t }));

            foreach (var impl in queryProviderImplementations)
            {
                Console.WriteLine($"[DI] Registering QueryProvider: {impl.Interface} -> {impl.Type}");
                builder.Services.AddScoped(impl.Interface, impl.Type);
            }

            var odataServiceInterfaceType = typeof(IEntityListODataQueryService<,>);
            var odataServiceImplementationType = typeof(BaseEntityListODataQueryService<,>);

            var registeredOdataServices = queryProviderImplementations
                .Select(x =>
                {
                    var entityType = x.Interface.GenericTypeArguments[0];
                    var entityListType = x.Interface.GenericTypeArguments[1];

                    var serviceInterface = odataServiceInterfaceType.MakeGenericType(entityType, entityListType);
                    var serviceImplementation = odataServiceImplementationType.MakeGenericType(entityType, entityListType);

                    return new { serviceInterface, serviceImplementation };
                });

            foreach (var impl in registeredOdataServices)
            {
                Console.WriteLine($"[DI] Registering ODataService: {impl.serviceInterface} -> {impl.serviceImplementation}");
                builder.Services.AddScoped(impl.serviceInterface, impl.serviceImplementation);
            }
            builder.Services.AddTransient<IGenericEntityQueryServiceFactory, GenericEntityQueryServiceFactory>();



            //  builder.Services.AddAllQueryServices(typeof(TenantManagementQueryService).Assembly);


            var app = builder.Build();

            InitializeApp(app, builder.Configuration);

            // map the default route
            app.MapGet("/", () => MapGetContent(builder.Configuration));

            try
            {
                Log.Information("Starting up...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }



        private static void InitializeApp(WebApplication app, IConfiguration configuration)
        {
            app.UseRouting();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseMiddleware<AuthenticationTokenMiddleware>();
            app.UseMiddleware<HttpContextHelperMiddleware>();
            app.MapControllers();

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
            var settingKeys = new Dictionary<string, string>
         {                 
             { "Id", Data.Queries.SettingQuery.GetDefaultSettingId() }
          };


         //var setting = new GenericEntityQueryService()
         //       .GetSingle("Setting", settingKeys , 0) as SettingPM;
 
            //AmitalCloudSettings.Id = setting.Id;
            //AmitalCloudSettings.ChampEnv = setting.ChampEnv;
            //AmitalCloudSettings.ChampURL = setting.ChampURL;
            //AmitalCloudSettings.ChampTestAPIURL = setting.ChampTestAPIURL;
            //AmitalCloudSettings.ChampTestAPIPassword = setting.ChampTestAPIPassword;
            //AmitalCloudSettings.ChampProdAPIURL = setting.ChampProdAPIURL;
            //AmitalCloudSettings.ChampProdAPIPassword = setting.ChampProdAPIPassword;
            //AmitalCloudSettings.CustomerCareIP = setting.CustomerCareIP;
            //AmitalCloudSettings.DeploymentStage = setting.DeploymentStage;
            //AmitalCloudSettings.IsLogEnabled = setting.IsLogEnabled;
            //AmitalCloudSettings.AmitalURL = setting.LogitudeURL;
            //AmitalCloudSettings.TotangoServiceId = setting.TotangoServiceId;
            //AmitalCloudSettings.UsingAzure = setting.UsingAzure;
            //AmitalCloudSettings.StorageAccountKey = setting.StorageAccountKey;
            //AmitalCloudSettings.StorageAccountName = setting.StorageAccountName;
            //AmitalCloudSettings.StorageType = setting.StorageType;
            //AmitalCloudSettings.AmitalCRMTenantNumber = setting.LogitudeCRMTenantNumber;
            //AmitalCloudSettings.AutoSignupEmail = setting.AutoSignupEmail;
            //AmitalCloudSettings.AutoSignupPassword = setting.AutoSignupPassword;
            //AmitalCloudSettings.ForceHttps = setting.ForceHttps;
            //AmitalCloudSettings.CheckConnectionURL = setting.CheckConnectionURL;
            //AmitalCloudSettings.AndroidSharedAppMinimumVersion = setting.AndroidSharedAppMinimumVersion;
            //AmitalCloudSettings.IOSSharedAppMinimumVersion = setting.IOSSharedAppMinimumVersion;
            //AmitalCloudSettings.WorkEnvironment = setting.WorkEnvironment;
            //AmitalCloudSettings.LogoCode = setting.LogoCode;
            //AmitalCloudSettings.EnableHybridQueue = setting.EnableHybridQueue;
            //AmitalCloudSettings.EmailAlertSignature = setting.EmailAlertSignature;
            //AmitalCloudSettings.IOSAppLink = setting.IOSAppLink;
            //AmitalCloudSettings.AndroidAppLink = setting.AndroidAppLink;
            //AmitalCloudSettings.AndroidPodAppMinimumVersion = setting.AndroidPodAppMinimumVersion;
            //AmitalCloudSettings.IOSPodAppMinimumVersion = setting.IOSPodAppMinimumVersion;
            //AmitalCloudSettings.MinimumOutlookVersion = setting.MinimumOutlookVersion;
            //AmitalCloudSettings.ABMProductId = setting.ABMProductId;
            //AmitalCloudSettings.AzureFolderName = setting.AzureFolderName;
            //AmitalCloudSettings.SignAppVersion = setting.SignAppVersion;
            //AmitalCloudSettings.ReportsRunUsingWR = setting.ReportsRunUsingWR;
            //AmitalCloudSettings.SMSServiceUserId = setting.SMSServiceUserId;
            //AmitalCloudSettings.SMSServiceAuthToken = setting.SMSServiceAuthToken;
            //AmitalCloudSettings.SMSServicePhoneNumber = setting.SMSServicePhoneNumber;
            //AmitalCloudSettings.GLSHKEnv = setting.GLSHKEnv;
            //AmitalCloudSettings.GLSHKURL = setting.GLSHKURL;
            //AmitalCloudSettings.NotificationHubName = setting.NotificationHubName;
            //AmitalCloudSettings.NotificationHubConnectionString = setting.NotificationHubConnectionString;
            //AmitalCloudSettings.DomainName = setting.DomainName;
            //AmitalCloudSettings.ProductName = setting.ProductName;
            //AmitalCloudSettings.QueueServiceMode = setting.QueueServiceMode;
            //AmitalCloudSettings.StorageServiceMode = setting.StorageServiceMode;
            //AmitalCloudSettings.DropboxAppKey = setting.DropboxAppKey;
            //AmitalCloudSettings.DropboxAppSecret = setting.DropboxAppSecret;
            //AmitalCloudSettings.OceanInsightsToken = setting.OceanInsightsToken;
            //AmitalCloudSettings.CPUIntensiveWebServicesURL = setting.CPUIntensiveWebServicesURL;
            //AmitalCloudSettings.AmitalCloudEnvironmentURL = setting.AmitalCloudEnvironmentURL;
            //AmitalCloudSettings.AmitalCloudAmitalTenantPrimaryKey = setting.AmitalCloudLogitudeTenantPrimaryKey;
            //AmitalCloudSettings.OITenantNumber = setting.OITenantNumber;
            //AmitalCloudSettings.AzurePrincipalSecretKey = setting.AzurePrincipalSecretKey;
            //AmitalCloudSettings.DNSZone = setting.DNSZone;
            //AmitalCloudSettings.DNSIPAddress = setting.DNSIPAddress;
            //AmitalCloudSettings.WorkflowStorageAccountName = setting.WorkflowStorageAccountName;
            //AmitalCloudSettings.WorkflowStorageAccountKey = setting.WorkflowStorageAccountKey;
            //AmitalCloudSettings.System2RedirectFraction = setting.System2RedirectFraction;
            //AmitalCloudSettings.WindWardSettings = setting.WindWardSettings;
            //AmitalCloudSettings.AmitalIISURL = setting.LogitudeIISURL;
            //AmitalCloudSettings.TempStorageConnection = setting.TempStorageConnection;
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
