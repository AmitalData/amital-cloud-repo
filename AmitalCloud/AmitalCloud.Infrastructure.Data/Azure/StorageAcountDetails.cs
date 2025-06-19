using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Data.Helpers;
using Azure.Data.Tables;
using Azure.Storage;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Azure.Messaging.ServiceBus.Administration;
using Azure.Messaging.ServiceBus;

namespace AmitalCloud.Infrastructure.Data.Azure
{
    public class StorageAcountDetails
    {
        public static void Initialize()
        {
            var accountName = AmitalCloudSettings.StorageAccountName;
            var accountKey = AmitalCloudSettings.StorageAccountKey;

            var blobQueueCredential = new StorageSharedKeyCredential(accountName, accountKey);
            var tableCredential = new TableSharedKeyCredential(accountName, accountKey);

            string blobUri, queueUri, tableUri;

            if (AmitalCloudSettings.StorageType.Equals("azureemulator", StringComparison.OrdinalIgnoreCase))
            {
                blobUri = $"http://127.0.0.1:10000/{accountName}";
                queueUri = $"http://127.0.0.1:10001/{accountName}";
                tableUri = $"http://127.0.0.1:10002/{accountName}";
            }
            else
            {
                var protocol = (AmitalCloudSettings.IsCostomsDeploy || accountName.Equals("amitalexporttest", StringComparison.OrdinalIgnoreCase)) ? "https" : "http";

                blobUri = $"{protocol}://{accountName}.blob.core.windows.net";
                queueUri = $"{protocol}://{accountName}.queue.core.windows.net";
                tableUri = $"{protocol}://{accountName}.table.core.windows.net";
            }

            BlobClient = new BlobServiceClient(new Uri(blobUri), blobQueueCredential);
            QueueClient = new QueueServiceClient(new Uri(queueUri), blobQueueCredential);
            TableClient = new TableServiceClient(new Uri(tableUri), tableCredential);
        }

        public static BlobServiceClient BlobClient { get; private set; }
        public static QueueServiceClient QueueClient { get; private set; }
        public static TableServiceClient TableClient { get; private set; }

        private static ServiceBusAdministrationClient _nameSpaceManager;
        public static ServiceBusAdministrationClient NameSpaceManager
        {
            get
            {
                if (_nameSpaceManager == null)
                {
                    var connectionString = GetSettingByName(AmitalCloudSettings.DeploymentStage);
                    _nameSpaceManager = new ServiceBusAdministrationClient(connectionString);
                }

                return _nameSpaceManager;
            }
        }

        private static string dataCacheTopicName;

        public static string DataCacheTopicName
        {
            get
            {
                if (AmitalCloudSettings.DeploymentStage == "Dev")
                {
                    dataCacheTopicName = Environment.MachineName;
                }
                else if (AmitalCloudSettings.DeploymentStage == "Simplog")
                {
                    dataCacheTopicName = "production";
                }
                else
                {
                    dataCacheTopicName = "test";
                }
                return dataCacheTopicName;
            }

        }

        private static string signalRHubTopicName;

        public static string SignalRHubTopicName
        {
            get
            {
                signalRHubTopicName = "SignalRHub";
                return signalRHubTopicName;
            }

        }

        private static ServiceBusClient _busClient;

        public static ServiceBusSender CreateServiceBusQueueClient(string queueName)
        {
            if (_busClient == null)
            {
                var connectionString = GetSettingByName(AmitalCloudSettings.DeploymentStage);
                _busClient = new ServiceBusClient(connectionString);
            }

            return _busClient.CreateSender(queueName);
        }

        public static ServiceBusReceiver CreateServiceBusQueueClient(string queueName, ServiceBusReceiveMode receiveMode)
        {
            if (_busClient == null)
            {
                var connectionString = GetSettingByName(AmitalCloudSettings.DeploymentStage);
                _busClient = new ServiceBusClient(connectionString);
            }

            return _busClient.CreateReceiver(queueName, new ServiceBusReceiverOptions
            {
                ReceiveMode = receiveMode
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenant">Current Tenant that request the method</param>
        /// <param name="Type">Determine type of container : available values : Tenant</param>
        /// <returns></returns>
        public static BlobContainerClient GetCurrentContainer(int tenant)
        {
            if (BlobClient == null)
            {
                Initialize();
            }

            string containername = "tenant" + tenant.ToString();
            BlobContainerClient blobContainer = BlobClient.GetBlobContainerClient(containername);

            if (!AmitalCloudSettings.IsCostomsDeploy)
            {
                blobContainer.CreateIfNotExists();
            }

            return blobContainer;
        }

        public static BlobContainerClient GetCurrentContainer(string containername)
        {
            BlobContainerClient blobContainer = BlobClient.GetBlobContainerClient(containername);
            blobContainer.CreateIfNotExists();
            return blobContainer;
        }

        public static string GetBlobNameByLocation(string blobname, string location)
        {
            string blobName = "";

            switch (location)
            {
                case "quotetemplatesectionfiles":
                    {
                        blobName = "quotetemplatesectionfiles/" + blobname;
                        break;
                    }

                case "docsin":
                    {
                        blobName = "docsin/" + blobname;
                        break;
                    }

                case "docsout":
                    {
                        blobName = "docsout/" + blobname;
                        break;
                    }

                case "logos":
                    {
                        blobName = "logos/" + blobname;
                        break;
                    }

                case "dlls":
                    {
                        blobName = "dlls/" + blobname;
                        break;
                    }

                case "tenantbackup":
                    {
                        blobName = "tenantbackup/" + blobname;
                        break;
                    }

                case "images":
                    {
                        blobName = "images/" + blobname;
                        break;
                    }

                case "champ":
                    {
                        blobName = "champ/" + blobname;
                        break;
                    }

                case "reports":
                    {
                        blobName = "reports/" + blobname;
                        break;
                    }

                case "charts":
                    {
                        blobName = "charts/" + blobname;
                        break;
                    }

                case "how-to":
                    {
                        blobName = blobname;
                        break;
                    }

                case "tariff":
                    {
                        blobName = "tariff/" + blobname;
                        //blobName = blobname;
                        break;
                    }
                case "multiprint":
                    {
                        blobName = "multiprint/" + blobname;
                        //blobName = blobname;
                        break;
                    }
                case "termsOfUse":
                    {
                        blobName = "termsOfUse/" + blobname;
                        break;
                    }
                case "others":
                default:
                    blobName = "others/" + blobname;
                    break;

            }

            return blobName;

        }

        public static string GetSettingByName(string enviroment)
        {
            return DefaultService.Instance.Get(0, "StorageAcount", "StorageAcountDetails." + enviroment)?.Value1;
        }

    }
}