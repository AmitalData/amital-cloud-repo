 using AmitalCloud.Infrastructure.Domain.EntityPMs;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class JSGlobalSettings
    {
        public JSGlobalSettings(SettingPM mySetting) 
        {
            
                Id = mySetting.Id;
                AmitalURL = mySetting.LogitudeURL;
                LogoCode = mySetting.LogoCode;
                WorkEnvironment = mySetting.WorkEnvironment;
                SameUserLoginEnabled = mySetting.SameUserLoginEnabled;
                LayoutDirection = mySetting.LayoutDirection;
                ReportsRunUsingWR = mySetting.ReportsRunUsingWR;
                DocumentFilingEmailDomain = mySetting.DocumentFilingEmailDomain;
                DeploymentStage = mySetting.DeploymentStage;
                ReleaseNotesURL = mySetting.ReleaseNotesURL;
                AmitalDemoTenants = mySetting.LogitudeDemoTenants;
                TMPersonalAccessExpirationDate = mySetting.TMPersonalAccessExpirationDate;
                ReleaseDateString = mySetting.ReleaseDateString;
                DNSZone = mySetting.DNSZone;
                ProductInfo = AmitalCloudSettings.ProductInfo;
                ProductMessage = AmitalCloudSettings.ProductMessage;

        }

        [Key]
        public string Id { get; set; }
        public string AmitalURL { get; set; }
        public string LogoCode { get; set; }
        public string WorkEnvironment { get; set; } 
        public bool SameUserLoginEnabled { get; set; }
        public string LayoutDirection { get; set; }
        public string ProductInfo { get; set; }
        public string ProductMessage { get; set; }
        public bool ReportsRunUsingWR { get; set; }
        public string DocumentFilingEmailDomain { get; set; }
        public string DeploymentStage { get; set; }
        public string ReleaseNotesURL { get; set; }
        public string AmitalDemoTenants { get; set; }
        public DateTime? TMPersonalAccessExpirationDate { get; set; }
        public string ReleaseDateString { get; set; }
        public string DNSZone { get; set; }
    }

}
