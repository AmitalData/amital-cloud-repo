using AmitalCloud.Infrastructure.Data.Azure;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using Azure.Messaging.ServiceBus;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class CacheMessageSender
    {

        public static void SendMessageToTopic(string Key)
        {
            if (AmitalCloudSettings.IsCostomsDeploy || AmitalCloudSettings.DeploymentStage == "amitalstorage") return;

            string connectionString = StorageAcountDetails.GetSettingByName(AmitalCloudSettings.DeploymentStage);
            string topicName = StorageAcountDetails.DataCacheTopicName;

            var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topicName);

            var message = new ServiceBusMessage
            {
                Subject = "InvalidateCache",
                ApplicationProperties = { ["Key"] = Key }
                // TimeToLive = TimeSpan.FromMinutes(5) // optional
            };

            sender.SendMessageAsync(message).GetAwaiter().GetResult();
        }
    }
}
