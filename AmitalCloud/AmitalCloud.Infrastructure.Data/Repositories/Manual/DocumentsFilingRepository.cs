using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;

using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class DocumentsFilingRepository : Repository<DocumentsFiling>, IRepository<DocumentsFiling>
    {
        IAmitalCloudContext amitalCloudContext;
        public DocumentsFilingRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }

        public DocumentsFilingRepository(IAmitalCloudContext context) : base(context)
        {
            amitalCloudContext = context;
        }
        public DocumentsFilingRepository(IUnitOfWork uow) : base(uow)
        {
        }
        public IQueryable<DocumentsFiling> GetDocumentsFilings(int tenant)
        {
            return (from record in context.DocumentsFilings.Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                    where record.Tenant == tenant
                    select record);
        }

        public IQueryable<DocumentsFiling> GetAllDocumentsFilings()
        {
            return (from record in context.DocumentsFilings
                    select record);
        }
        public DocumentsFiling GetSingleDocumentsFilingDocument(string id, int tenant)
        {
            var q = (from a in context.DocumentsFilings.Include("Document").Include("DocumentType")
                     where a.Id == id && a.Tenant == tenant
                     select a);
            return q.FirstOrDefault();
        }

        public DocumentsFiling GetSingleDocumentsFiling(string id, int tenant)
        {
            DocumentsFiling d = (from a in context.DocumentsFilings.Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                                 where a.Id == id && a.Tenant == tenant
                                 select a).FirstOrDefault();
            return d;
        }

        public DocumentsFiling GetSingleWithIncludeDocumentType(string id, int tenant)
        {
            DocumentsFiling d = (from a in context.DocumentsFilings.Include("DocumentType")
                                 where a.Id == id && a.Tenant == tenant
                                 select a).FirstOrDefault();
            return d;
        }

        public List<DocumentsFiling> GetDocumentsFilingsByEntityId1(string entityId, int tenant)
        {
            List<DocumentsFiling> externalDocuments = (from a in context.DocumentsFilings.Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                                                       where (a.EntityId == entityId || a.ChildEntityId == entityId) && a.Tenant == tenant
                                                       select a).ToList();
            return externalDocuments;
        }
        public List<DocumentsFiling> GetDocumentsFilingsByEntityId_noInclude(string entityId, int tenant)
        {
            List<DocumentsFiling> externalDocuments = (from a in context.DocumentsFilings
                                                           //.Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                                                       where (a.EntityId == entityId || a.ChildEntityId == entityId) && a.Tenant == tenant
                                                       select a).ToList();
            return externalDocuments;
        }
        public List<DocumentsFiling> GetRequestedDocumentsFilingPMsByEntityId(string entityId, int tenant)
        {
            List<DocumentsFiling> externalDocuments = (from a in context.DocumentsFilings.Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                                                       where (a.EntityId == entityId) && a.Tenant == tenant && (a.IsDigitalSignRequired == true || a.IsRequested == true)
                                                       select a).ToList();
            return externalDocuments;
        }

        public List<DocumentsFiling> GetDocumentsFilingsForEntityTableId(string entityId, string objecttableId, int tenant)
        {
            List<DocumentsFiling> externalDocuments = (from a in context.DocumentsFilings.Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                                                       where (a.EntityId == entityId || a.ChildEntityId == entityId) && (a.ObjectTableId == objecttableId || a.ChildObjectTableId == objecttableId) && a.Tenant == tenant
                                                       select a).ToList();
            return externalDocuments;
        }

        public DocumentsFiling GetSingleDocumentsFilingByCode(string code, int tenant)
        {
            var externalDocument = (from a in context.DocumentsFilings.Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                                    where a.Code == code && a.Tenant == tenant
                                    select a).FirstOrDefault();
            return externalDocument;
        }
        public DocumentsFiling GetSingleDocumentsFilingByDocumentId(string DocumentId, int tenant, bool fromCache = true)
        {

            string name = "SingleDocumentsFilingByDocumentId" + DocumentId + "," + tenant;

            DocumentsFiling externalDocument = null;
            externalDocument = CacheManager.GetOrInsertNewObject<DocumentsFiling>(name, () =>
            {
                //(context as IObjectContextAdapter).ObjectContext.ContextOptions.UseCSharpNullComparisonBehavior = false; //Pasted from <http://stackoverflow.com/questions/682429/how-can-i-query-for-null-values-in-entity-framework?lq=1> 

                var poco = (from a in context.DocumentsFilings
                                ///.Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                            where a.DocumentId == DocumentId && a.Tenant == tenant
                            select a).FirstOrDefault();
                return poco;

            }, fromCache, true);


            return externalDocument;

        }
        public string GetSingleDocumentsFilingIdByDocumentId(string DocumentId, int tenant)
        {


            var DocumentsFilingId = (from a in context.DocumentsFilings
                                         ///.Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                                     where a.DocumentId == DocumentId && a.Tenant == tenant
                                     select a.Id).FirstOrDefault();
            return DocumentsFilingId;

        }

        public DocumentsFiling GetSingleDocumentFilingByDocumentId(string DocumentId, int tenant)
        {

            var poco = (from a in context.DocumentsFilings
                                .Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                        where a.DocumentId == DocumentId && a.Tenant == tenant
                        select a).FirstOrDefault();
            return poco;
        }
        public bool CheckIfDocumentTypeHasDocumentFilling(string documentTypeId, string objectTableId, string entityId, int tenant)
        {

            return (
                GetByDocType(documentTypeId, objectTableId, entityId, tenant)
                    ).Any();

        }

        private IQueryable<DocumentsFiling> GetByDocType(string documentTypeId, string objectTableId, string entityId, int tenant)
        {

            var q = (from a in context.DocumentsFilings
                     where a.DocumentTypeId == documentTypeId && a.Tenant == tenant && a.ObjectTableId == objectTableId && a.EntityId == entityId
                     select a);
            var isItFaster = false;
            if (isItFaster)
            {
                if (!String.IsNullOrWhiteSpace(documentTypeId) && !String.IsNullOrWhiteSpace(objectTableId) && !String.IsNullOrWhiteSpace(entityId))
                {

                    q = q.Where(r => !(r.ObjectTableId == null || r.ObjectTableId.Trim() == String.Empty));
                    q = q.Where(r => !(r.EntityId == null || r.EntityId.Trim() == String.Empty));

                }
            }
            return q;
        }
        public List<DocumentsFilingDTO> GetByEntity(string objectTableId, string entityId, int tenant)
        {
            var q = (from a in context.DocumentsFilings
                     where
                     //a.DocumentTypeId == documentTypeId && 
                     a.Tenant == tenant && a.ObjectTableId == objectTableId && a.EntityId == entityId
                     select new DocumentsFilingDTO()
                     {
                         Id = a.Id,
                         DocumentTypeId = a.DocumentTypeId,
                         DocumentId = a.DocumentId
                     });
            var allForEntity = q.ToList();
            return allForEntity;
        }
        public IQueryable<DocumentsFiling> GetByEntityAndChiled(string objectTableId, string entityId, string objectTableIdChiled, string entityIdChiled, int tenant, string documentTypeId)
        {
            var q = (from a in context.DocumentsFilings
                     where
                     a.Tenant == tenant && a.ObjectTableId == objectTableId && a.EntityId == entityId && a.ChildObjectTableId == objectTableIdChiled && a.ChildEntityId == entityIdChiled && a.DocumentTypeId == documentTypeId
                     select a);


            return q;
        }

        public string GetDocumentIdByDocumentType(string documentTypeId, string objectTableId, string entityId, int tenant, out string DocumentsFilingId)
        {
            DocumentsFilingId = null;
            bool ihabIsSureItsBetter = true;
            if (ihabIsSureItsBetter)
            {
                var q = (from a in context.DocumentsFilings
                         where
                         //a.DocumentTypeId == documentTypeId && 
                         a.Tenant == tenant && a.ObjectTableId == objectTableId && a.EntityId == entityId
                         select new
                         {
                             a.Id,
                             a.DocumentTypeId,
                             a.DocumentId
                         });
                var allForEntity = q.ToList();
                var rec4DocumentTypeId = allForEntity.FirstOrDefault(a => a.DocumentTypeId == documentTypeId);
                if (rec4DocumentTypeId == null) return null;
                DocumentsFilingId = rec4DocumentTypeId.Id;
                return rec4DocumentTypeId.DocumentId;
            }


            string resDocumentId = GetByDocType(documentTypeId, objectTableId, entityId, tenant).Select(r => r.DocumentId).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(resDocumentId))
            {
                return null;
            }
            return resDocumentId;
            var poco = GetByDocType(documentTypeId, objectTableId, entityId, tenant).FirstOrDefault();

            if (poco == null) return null;
            return poco.DocumentId;
        }

        public List<DocumentsFiling> GetDocumentsFilingsByIds(string ids, int tenant)
        {
            string[] docFilingIds = ids.Split(',');

            List<DocumentsFiling> d = (from a in context.DocumentsFilings.Include("DocumentType").Include("ObjectTable")
                                       where docFilingIds.Contains(a.Id) && a.Tenant == tenant
                                       select a).ToList();
            return d;
        }

        new public void Insert(DocumentsFiling entity)
        {

            if (string.IsNullOrEmpty(entity.ForwarderDocumentId))
            {
                entity.ComputedForwarderDocumentId = entity.Id;
            }
            else
            {
                entity.ComputedForwarderDocumentId = entity.ForwarderDocumentId;
            }
            //entity.ComputedCustomerDocumentId = string.IsNullOrEmpty(entity.CustomerDocumentId) ? entity.Id : entity.CustomerDocumentId;

            base.Insert(entity);
        }

        public IAmitalCloudContext context
        {

            get { return amitalCloudContext; }
        }

        public string GetEntityDocumentsSearchFields(string EntityId, string ObjectTableId, int tenant)
        {
            if (!string.IsNullOrEmpty(EntityId))
            {

                var entity = (from a in context.DocumentsFilings
                              where a.EntityId == EntityId && a.Tenant == tenant && a.ObjectTableId == ObjectTableId
                              select a.SearchFields).ToArray();
                var SField = String.Join(",", entity);
                return SField;
            }
            return null;
        }

        public IQueryable<DocumentsFiling> GetDocumentsFilingByEntityId(string EntityId, string ObjectTableId, int tenant)
        {
            var documentsFilings = (from a in context.DocumentsFilings
                                    where a.EntityId == EntityId && a.Tenant == tenant && a.ObjectTableId == ObjectTableId
                                    select a);

            return documentsFilings;
        }




        public string GetDocumentIdById(string documentsFilingId, int tenant)
        {
            return (from a in context.DocumentsFilings
                    where a.Id == documentsFilingId && a.Tenant == tenant
                    select a.DocumentId).FirstOrDefault();

        }




        public DocumentsFiling GetSingleDocumentsFiling(string id)
        {
            DocumentsFiling d = (from a in context.DocumentsFilings.Include("CreatedByUser.Contact").Include("Document").Include("Owner.Contact").Include("ObjectTable").Include("DocumentType")
                                 where a.Id == id
                                 select a).FirstOrDefault();
            return d;
        }

        public DocumentsFiling GetSingleDocumentFilingBySecurityId(string id, int tenant)
        {
            DocumentsFiling d = (from a in context.DocumentsFilings
                                                  .Include("CreatedByUser.Contact")
                                                  .Include("Document")
                                                  .Include("Owner.Contact")
                                                  .Include("ObjectTable")
                                                  .Include("DocumentType")
                                 where a.SecurityId == id && a.Tenant == tenant
                                 select a)
                                 .FirstOrDefault();
            return d;
        }

        public DocumentsFiling GetSingleDocumentFilingBySecurityId(string id)
        {
            DocumentsFiling d = (from a in context.DocumentsFilings
                                 where a.SecurityId == id
                                 select a).FirstOrDefault();
            return d;
        }


        public string GetDocumentIdByChild(string documentTypeId, string ChildEntityReference, int tenant)
        {
            DocumentsFiling d = (from a in context.DocumentsFilings
                                 where a.DocumentTypeId == documentTypeId && a.Tenant == tenant && a.ChildEntityReference == ChildEntityReference
                                 select a).FirstOrDefault();
            if (d == null) return null;
            return d.DocumentId;
        }

        //public IQueryable<DocumentsFilingsView> GetDocumentsFilingsViews(int tenant)
        //{
        //    IDocumentsFilingsViewContext viewContext = DocumentsFilingsViewContext.GetContext(tenant);
        //    return (from record in viewContext.DocumentsFilingsViews
        //            where record.Tenant == tenant
        //            select record);
        //}


        public List<DocumentsFiling> GetDocumentsFilingsBySecurityIdsAndObjectTable(List<string> securityIds, string objecttableId, int tenant)
        {
            List<DocumentsFiling> externalDocuments = (from a in context.DocumentsFilings
                                                       where (securityIds.Contains(a.SecurityId) && (a.ObjectTableId == objecttableId || a.ChildObjectTableId == objecttableId) && a.Tenant == tenant)
                                                       select a).ToList();
            return externalDocuments;
        }
        public double? GetDocumentFileSizeByDocumentFilingId(string DocumentsFilingId, int tenant)
        {

            return (from a in context.Documents
                    join d in context.DocumentsFilings
                    on a.Id equals d.DocumentId
                    where d.Id == DocumentsFilingId && a.Tenant == tenant
                    select a.FileSize).FirstOrDefault();
        }



    }
    public class DocumentsFilingDTO
    {
        public string Id { get; internal set; }
        public string DocumentTypeId { get; internal set; }
        public string DocumentId { get; internal set; }
    }



}
