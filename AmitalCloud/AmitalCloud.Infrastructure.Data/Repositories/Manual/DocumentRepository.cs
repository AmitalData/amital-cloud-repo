using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class DocumentRepository : Repository<Document>, IRepository<Document>
    {
        IAmitalCloudContext currentContext;



        public DocumentRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }

        public DocumentRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public DocumentRepository(IUnitOfWork uow) : base(uow)
        {
        }

        public IQueryable<Document> GetDocuments(int tenant)
        {
            return (from record in context.Documents where record.Tenant == tenant select record);
        }

        public Document GetSingleDocument(int tenant, string id)
        {
            Document d = (from a in context.Documents
                          where a.Id == id && a.Tenant == tenant
                          select a).FirstOrDefault();
            return d;
        }

        public Document GetSingleDocument(string id)
        {
            Document d = (from a in context.Documents
                          where a.Id == id
                          select a).FirstOrDefault();
            return d;
        }

        public List<Document> GetDocumentsByDocumentIds(List<string> documentIds, int tenant)
        {
            List<Document> result = (from a in context.Documents
                                     where documentIds.Contains(a.Id) && a.Tenant == tenant
                                     select a).ToList();
            return result;
        }
        public List<Document> GetDocumentsByIds(List<string> documentIds)
        {
            List<Document> result = (from a in context.Documents
                                     where documentIds.Contains(a.Id)
                                     select a).ToList();
            return result;
        }
        public string GetDocumentIdByFileName(string fileName, string extension, int tenant)
        {
            char[] delimiterChars = { '-' };
            var query = (from a in context.Documents
                         where a.Tenant == tenant && a.Extension == extension && a.FileName.StartsWith(fileName)
                         orderby a.CreateDate descending
                         select a.Id);
            return query.FirstOrDefault();
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }

        public double? GetUsedSpaceForTenant(int tenant)
        {
            double? usedSpace = (from a in context.Documents
                                 where a.Tenant == tenant
                                 select a).Sum(d => d.FileSize);
            return usedSpace;
        }
        public string GetCalculatedFileNameById(string id, int tenant)
        {
            string calculatedFileName = (from a in context.Documents
                                         where a.Tenant == tenant && a.Id == id
                                         select a.CalculatedFileName).FirstOrDefault();
            return calculatedFileName;
        }



        public bool CheckIfDocumentsExistOnTenant(List<string> documentIds, int tenant)
        {
            bool isValid = (from a in context.Documents
                            where documentIds.Contains(a.Id) && a.Tenant != tenant
                            select a).Any();
            return !isValid;
        }

        public string GetFileNameByDocumentId(string id, int tenant)
        {
            string calculatedFileName = (from a in context.Documents
                                         where a.Tenant == tenant && a.Id == id
                                         select a.FileName).FirstOrDefault();
            return calculatedFileName;
        }

    }
}
