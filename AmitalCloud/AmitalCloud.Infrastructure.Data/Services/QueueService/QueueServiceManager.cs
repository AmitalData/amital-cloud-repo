using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public static class QueueServiceManager
    {
        public static IQueueService GetQueueService(string queuename, int tenant = 0)
        {
            //todo
            // IQueueService queueservice = ContainerAccessor.Container.Resolve(typeof(IQueueService), "QueueService", new ParameterOverride("", 1)) as IQueueService;
            //queueservice.InitializeQueue(queuename, tenant);
            IQueueService queueservice = null;
            return queueservice;
        }

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
                    //customsDeploymentStage.ToString() + 
                    branch + "_" + savequeueName;
            }


            return queueName;

        }
    }
}
