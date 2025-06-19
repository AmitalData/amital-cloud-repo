//using AmitalCloud.Infrastructure.Data.Helpers;
using System;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{

    public class AmitalCloudSettings
    {
        public static string Id { get; set; }
        public static string AmitalURL { get; set; }
        public static string ChampURL { get; set; }
        public static string ChampTestAPIURL { get; set; }
        public static string ChampTestAPIPassword { get; set; }
        public static string ChampProdAPIURL { get; set; }
        public static string ChampProdAPIPassword { get; set; }
        public static string DeploymentStage { get; set; }
        public static string ChampEnv { get; set; }
        public static string CustomerCareIP { get; set; }
        public static string TotangoServiceId { get; set; }
        public static bool UsingAzure { get; set; }
        public static bool IsLogEnabled { get; set; }
        public static string StorageAccountName { get; set; }
        public static string StorageAccountKey { get; set; }
        public static string StorageType { get; set; }
        public static int AmitalCRMTenantNumber { get; set; }
        public static string AutoSignupEmail { get; set; }
        public static string AutoSignupPassword { get; set; }
        public static bool ForceHttps { get; set; }
        public static string CheckConnectionURL { get; set; }
        public static string WorkEnvironment { get; set; }
        public static string LogoCode { get; set; }
        public static bool EnableHybridQueue { get; set; }
        public static bool IsOracleMode { get; set; }
        public static string GLSHKURL { get; set; }
        public static string GLSHKEnv { get; set; }
        public static double IOSSharedAppMinimumVersion { get; set; }
        public static double AndroidSharedAppMinimumVersion { get; set; }
        public static string DomainName { get; set; }
        public static string ProductName { get; set; }
        public static string EmailAlertSignature { get; set; }
        public static string ABMProductId { get; set; }

        public static string QBOClientID { get; set; }
        public static string QBOClientSecret { get; set; }
        public static int QBOOAuthDefault { get; set; }

        public static string AndroidAppLink { get; set; }
        public static string IOSAppLink { get; set; }

        public static double AndroidPodAppMinimumVersion { get; set; }
        public static double IOSPodAppMinimumVersion { get; set; }

        public static string MinimumOutlookVersion { get; set; }
        public static string SignAppVersion { get; set; }
        public static bool ReportsRunUsingWR { get; set; }
        public static string QueueServiceMode { get; set; }
        public static string StorageServiceMode { get; set; }
        public static string? DebugKey { get; set; } //Add new Virtual Dir with JustAppSetting.config with key (iis + AmitalCustomsWindowsService)
        public static string DropboxAppKey { get; set; }
        public static string DropboxAppSecret { get; set; }
        public static string OceanInsightsToken { get; set; }
        public static int EmailSendingQuota { get; set; }
        public static string CPUIntensiveWebServicesURL { get; set; }

        public static string AmitalCloudEnvironmentURL { get; set; }
        public static string AmitalCloudAmitalTenantPrimaryKey { get; set; }
        public static int OITenantNumber { get; set; }
        public static bool IsCostomsDeploy
        {
            get
            {
                return (WorkEnvironment ?? "").Equals("customs", StringComparison.InvariantCultureIgnoreCase);
            }
        }
        public static string AzurePrincipalSecretKey { get; set; }
        public static string DNSIPAddress { get; set; }
        public static string DNSZone { get; set; }

        public static Func<int, string> GetUserNameInject { get; set; }
        // this project no need but in FilingManager is must 
        public static Func<int, string> GetUnfDBConnectionInfoFromTenantInject { get; set; }// this project no need but in FilingManager is must 
        public static Func<int, AmitalCloudCustomsSettingsM> GetAmitalCustomsSettingsMInject { get; set; }
        public static Action<Exception, string, string> HandleDbExceptionInject { get; set; }

        public static Action<bool, bool> HandleBuildObjectTablesZipFilesData_Inject { get; set; }

        static string _DatabaseManagementSystem;

        public static string DatabaseManagementSystem
        {
            get { return AmitalCloudSettings._DatabaseManagementSystem; }
            set
            {
                //if (!string.IsNullOrWhiteSpace(AmitalCloudSettings._DatabaseManagementSystem) && AmitalCloudSettings._DatabaseManagementSystem != value)
                //{
                //    //if (Debugger.IsAttached) Debugger.Break();
                //    AmitalCloudDebuggerUtil.Break(AmitalDebuggerLevel.Critical);

                //}
                AmitalCloudSettings._DatabaseManagementSystem = value;
            }
        }


        public static string NotificationHubName { get; set; }
        public static string NotificationHubConnectionString { get; set; }

        public static string AzureFolderName { get; set; }


        public static Action<string, bool, string, DateTime> HandleLogMe { get; set; }
        public static string ProductInfo { get; set; }

        public static string ProductMessage { get; set; }



        public static string SMSServiceUserId { get; set; }
        public static string SMSServiceAuthToken { get; set; }
        public static string SMSServicePhoneNumber { get; set; }

        public static string WorkerRoleName { get; set; }
        public static bool RunWorkerRoleAutomaticBreakPoint { get; set; }
        public static int System2RedirectFraction { get; set; }

        public static string WorkflowStorageAccountName { get; set; }
        public static string WorkflowStorageAccountKey { get; set; }
        public static string WindWardSettings { get; set; }
        public static string AmitalIISURL { get; set; }
        public static string TempStorageConnection { get; set; }
    }

    public class AmitalCloudCustomsSettingsM
    {
        public string UnfConnectionString { get; set; }

        public string OnPremiseFillingService { get; set; }
        public bool IsConnectedToUniFreight { get; set; }
        public string Id { get; set; }

    }

    public class AmitalCloudAppSettings
    {
        public static DateTime StartDateTime { get; set; }
        public static bool IsRecycled { get; set; }
        public static DateTime EndDateTime { get; set; }
        public static bool WarmingIsFinished { get; set; }


    }


}
