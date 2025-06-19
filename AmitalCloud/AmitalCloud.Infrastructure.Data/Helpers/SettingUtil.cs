using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using System;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AmitalCloud.Infrastructure.Domain.Helpers
{
    public class SettingUtil
    {
        public static int GetCurrentTenant()
        {
            var tenant = -1;
            if (HttpContextHelper.HttpContext?.Items?.ContainsKey("Tenant") == true)
            {
                tenant = Convert.ToInt32(HttpContextHelper.HttpContext.Items["Tenant"]);
            }
            else
            {
                throw new NotImplementedException("Tenant not found");
            }
            return tenant;
        }

        public class Emails
        {
            public const string FromNoReply = "no-reply@amital.co.il";
            public const string FromNoReplyLogbox = "no-reply@logbox.co.il";


            public const string CrmManagers = "crm-managers@amital.co.il";//tzuri@...adi@
            public const string AccountingManagers = "accounting-managers@amital.co.il";//eyal@...eitan
            public const string LogboxManagers = "logbox-managers@amital.co.il";//boazelkana@gmail.com;anatl@amital.co.il;sana@.....
            public const string CustomsManagers = "customs-managers@amital.co.il";//anatl@amital.co.il;sana@.....
            public const string AutoSignupGroup = "AutoSignup@amital.co.il";

            public const string DeploymentTeam = "Dev_DeploymentTeam@amital.co.il";
            public const string QATeam = "QA_Team@amital.co.il";//sraya@,lital@,
            public const string DevTeamManagers = "Dev_TeamLeaders@amital.co.il";//simon@,elisheva@,
        }
        public class DeploymentStage
        {
            public static readonly string[] Development = { "Dev", "Test2" };
            public static readonly string Simplog = "Simplog";
            public static readonly string Customs = "Customs";

            public static readonly string[] Cloud = { "Amitalstorage" };//, "Accounting", "Crm", "CargoTracking" };

            public static readonly string[] Logbox = { "logboxwe1" }; //logboxpre,test2

            public static readonly string AmitalOracle = "amitaloracletk1";


            public static readonly string[] LogboxAndAccountingProduction = Cloud.Concat(Logbox).ToArray();




            public static bool IsDBStage(params string[] deploymentstage)
            {

                if (string.IsNullOrEmpty(AmitalCloudSettings.DeploymentStage))
                    return false;
                if (deploymentstage == null || deploymentstage.Length == 0)
                    return false;

                if (deploymentstage.Any(x => x.ToLower() == AmitalCloudSettings.DeploymentStage.ToLower()))
                {
                    return true;
                }
                return false;
            }
        }
        //private const string _CustomsDeploymentStage = "CustomsDeploymentStage";
        //public enum CustomsDeploymentStage
        //{
        //    None,
        //    Test,
        //    ///PrePilot,//for diffrent SBQueue
        //    Pilot,
        //    Production
        //}

        //public static CustomsDeploymentStage GetCustomsDeploymentStage()
        //{
        //    if (String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings.Get(_CustomsDeploymentStage)))
        //    {
        //        return CustomsDeploymentStage.None;
        //    }

        //    var deploymentStage = ConfigurationManager.AppSettings.Get(_CustomsDeploymentStage).ToString();
        //    CustomsDeploymentStage myCustomsDeploymentStage = CustomsDeploymentStage.None;
        //    Enum.TryParse<CustomsDeploymentStage>(deploymentStage, out myCustomsDeploymentStage);

        //    return myCustomsDeploymentStage;
        //}
        //public static bool ForceDownloadXapFromIIS()
        //{
        //    return false; //by jalal : because it doesn't download xap files from storage online. Islam and MOhammad will check with Ihab.
        //    switch (GetCustomsDeploymentStage())
        //    {

        //        case CustomsDeploymentStage.Test:
        //        case CustomsDeploymentStage.Pilot:
        //        case CustomsDeploymentStage.Production:
        //            return true;
        //            break;
        //        case CustomsDeploymentStage.None:
        //        default:
        //            return true;
        //            break;
        //    }
        //}
    }

}
