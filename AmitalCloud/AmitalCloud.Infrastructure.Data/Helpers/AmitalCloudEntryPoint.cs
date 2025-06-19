using AmitalCloud.Infrastructure.Domain.DataContracts;
using System;
using System.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class AmitalCloudEntryPoint : RoleEntryPoint
    {
        public static bool UsingAzure = false; // when true, Use Azure methods
        public static bool IsComplieEnabled = false;
        public static bool CheckConnectionStrategy = false;
        public static DateTime CheckConnectionStartDate;
        //public static bool IsLogEnabled = true;
        // public static bool InAzureStorage = false; // when true, Azure storage will be used
        // public static bool IsStaging = false; // Use online staging for testing.
        //public static string DeploymentStage = "Dev";//Dev//Test1//Simplog//Test1//amital
        //public static bool UsingAzure = true; // when true, Use Azure methods
        //*****Test Branch******
        //int tt = 1;
        //**********************
        public override void Run()
        {
            base.Run();
        }
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;
            return base.OnStart();
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Return the name of queue depend on enviroment(Dev,Prodcution or Test)
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static string GetQueueByEnviroment(string queueName)
        {
            var savequeueName = queueName;
            if (AmitalCloudSettings.DeploymentStage == "Dev")
            {
                queueName = Environment.MachineName + "_" + queueName;
            }
            else if (AmitalCloudSettings.DeploymentStage == "customs")
            {
                queueName = "customs" + "_" + queueName;
            }
            else if (AmitalCloudSettings.DeploymentStage == "Simplog" || AmitalCloudSettings.DeploymentStage == "amitalstorage")
            {
                queueName = "Production" + "_" + queueName;
            }
            else
            {
                queueName = "Test" + "_" + queueName;
            }
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                var uri = new Uri(AmitalCloudSettings.AmitalURL);
                var branch = uri.LocalPath.Trim(@"\"[0]).Trim(@"/"[0]);
                queueName = AmitalCloudSettings.StorageAccountName + "_Customs" +
                    branch + "_" + savequeueName;
            }

            return queueName;

        }
    }
}
