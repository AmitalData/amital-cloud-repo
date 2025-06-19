using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class DocumentsMetaDataTypeRepository : Repository<DocumentsMetaDataType>, IRepository<DocumentsMetaDataType>
    {
        IAmitalCloudContext currentContext;



        public DocumentsMetaDataTypeRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }

        public DocumentsMetaDataTypeRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }

        public IQueryable<DocumentsMetaDataType> GetDocumentsMetaDataTypes(int tenant)
        {
            return (from record in context.DocumentsMetaDataTypes
                    where record.Tenant == tenant
                    select record);
        }

        public DocumentsMetaDataType GetSingleDocumentsMetaDataType(string id, int tenant)
        {
            DocumentsMetaDataType d = (from a in context.DocumentsMetaDataTypes
                                       where a.Id == id && a.Tenant == tenant
                                       select a).FirstOrDefault();
            return d;
        }

        public DocumentsMetaDataType GetSingleDocumentsMetaDataTypeByCode(string code, int tenant, bool fromCache = false)
        {

            if (fromCache)
            {

                string entityKeyString = $"GetSingleDocumentsMetaDataTypeByCode({code},{tenant})";
                DocumentsMetaDataType myres = CacheManager.GetOrInsertNewObject<DocumentsMetaDataType>(entityKeyString, () =>
                {


                    return (from a in context.DocumentsMetaDataTypes
                            where a.Code == code && a.Tenant == tenant
                            select a).FirstOrDefault();

                }
                );
                return myres;
            }

            DocumentsMetaDataType d = (from a in context.DocumentsMetaDataTypes
                                       where a.Code == code && a.Tenant == tenant
                                       select a).FirstOrDefault();
            return d;
        }

        public DocumentsMetaDataType GetSingleDocumentsMetaDataTypeByCustomsMetaDataCode(string code, int tenant)
        {

            string entityKeyString = $"GetSingleDocumentsMetaDataTypeByCustomsMetaDataCode({code},{tenant})";
            var res = CacheManager.GetOrInsertNewObject<DocumentsMetaDataType>(entityKeyString, () =>
            {
                DocumentsMetaDataType d = (from a in context.DocumentsMetaDataTypes
                                           where a.CustomsMetaDataCode == code && a.Tenant == tenant
                                           select a).FirstOrDefault();
                return d;
            });
            return res;

        }

        public void Add(DocumentsMetaDataType entity)
        {
            context.DocumentsMetaDataTypes.Add(entity);
        }


        public IAmitalCloudContext context
        {

            get { return currentContext; }
        }
    }

}
