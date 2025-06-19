using Azure;
using Azure.Data.Tables;

namespace AmitalCloud.Infrastructure.Data.Azure
{
    public class CommunicationLogsAzureContext
    {
        public string TableName = "CommunicationLogsAzure";
        private TableEntity communicationLogsAzures;
        public TableClient ErrorLogtable;

        public CommunicationLogsAzureContext()
        {
            ErrorLogtable = StorageAcountDetails.TableClient.GetTableClient(TableName);
            ErrorLogtable.CreateIfNotExists();
        }

        public TableEntity CommunicationLogsAzureEntity
        {
            get { return communicationLogsAzures; }
        }

        public TableEntity CommunicationLogsAzures
        {
            get { return communicationLogsAzures; }
            set { communicationLogsAzures = value; }
        }

        public void Add(TableEntity entity)
        {
            ErrorLogtable.AddEntity(entity);
        }

        public void Update(TableEntity entity)
        {
            ErrorLogtable.UpdateEntity(entity, ETag.All, TableUpdateMode.Merge);
        }
    }
}