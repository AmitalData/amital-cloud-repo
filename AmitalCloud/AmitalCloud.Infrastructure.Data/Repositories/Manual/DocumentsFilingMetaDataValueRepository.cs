using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class DocumentsFilingMetaDataValueRepository : Repository<DocumentsFilingMetaDataValue>, IRepository<DocumentsFilingMetaDataValue>
    {
        IAmitalCloudContext currentContext;



        public DocumentsFilingMetaDataValueRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }

        public DocumentsFilingMetaDataValueRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }
        public DocumentsFilingMetaDataValueRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<DocumentsFilingMetaDataValue> GetDocumentsFilingMetaDataValues(int tenant)
        {
            return (from record in context.DocumentsFilingMetaDataValues
                    where record.Tenant == tenant
                    select record);
        }

        public DocumentsFilingMetaDataValue GetSingleDocumentsFilingMetaDataValue(string id, int tenant)
        {
            DocumentsFilingMetaDataValue d = (from a in context.DocumentsFilingMetaDataValues
                                              where a.Id == id && a.Tenant == tenant
                                              select a).FirstOrDefault();
            return d;
        }



        public IAmitalCloudContext context
        {

            get { return currentContext; }
        }


        public IQueryable<DocumentsFilingMetaDataValue> GetDocumentsFilingMetaDataValuesByTenantDocFilingId(int tenant, string DocumentFilingId)
        {
            return (from record in context.DocumentsFilingMetaDataValues
                    where record.Tenant == tenant && record.DocumentsFilingId == DocumentFilingId
                    select record);
        }
    }
}
