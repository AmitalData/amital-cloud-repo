using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;
using AutoMapper;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class DocumentTypeQuery
    {
        IRepository<DocumentType> repository;
        private bool isFullAccounting;
        private IMapper mapper;
        #region Constructor


        public DocumentTypeQuery(int tenant)
        {
            repository = new Repository<DocumentType>(tenant);
            isFullAccounting = IsFullAccountingActivated(tenant);
            CreateMapper();
        }

        public DocumentTypeQuery(IRepository<DocumentType> repository)
        {
            this.repository = repository;
            CreateMapper();
        }
        #endregion Constructor

        #region Privare Methods
        private IQueryable<DocumentTypePM> FilterDocumentTypePMByTransportModeIdAndShipmentLevelCode(string transportModeId, string shipmentLevelCode, IQueryable<DocumentTypePM> documentTypes)
        {
            #region transportModeId

            if (!string.IsNullOrEmpty(transportModeId))
            {
                switch (transportModeId)
                {
                    case "A":
                        {

                            documentTypes = documentTypes.Where(a => a.IsAir == true);
                            break;
                        }
                    case "O":
                        {
                            documentTypes = documentTypes.Where(a => a.IsOcean == true);
                            break;
                        }
                    case "I":
                        {
                            documentTypes = documentTypes.Where(a => a.IsInland == true);
                            break;
                        }
                }
            }


            #endregion transportModeId


            #region shipmentLevelCode
            if (!string.IsNullOrEmpty(shipmentLevelCode))
            {
                switch (shipmentLevelCode)
                {
                    case "C":
                        {
                            documentTypes = documentTypes.Where(a => a.IsMaster == true);
                            break;
                        }

                    case "M":
                        {
                            documentTypes = documentTypes.Where(a => a.IsMaster == true);
                            break;
                        }

                    case "D":
                        {
                            documentTypes = documentTypes.Where(a => a.IsDirect == true);
                            break;
                        }
                    case "H":
                        {
                            documentTypes = documentTypes.Where(a => a.IsHouse == true);
                            break;
                        }
                }

            }
            #endregion shipmentLevelCode
            return documentTypes;
        }
        private bool IsFullAccountingActivated(int tenant)
        {
            var tenantRepository = new Repository<Tenant>(tenant);
            Tenant tenantPOCO = tenantRepository.GetMulti(a => a.Id == tenant).FirstOrDefault();
            if (tenantPOCO == null) return false;
            bool isFullAccountingActivated = tenantPOCO.AccountingActivated;
            return isFullAccountingActivated;
        }
        private List<DocumentTypePM> GetPMList(Expression<Func<DocumentType, bool>> predicate) => repository.GetMulti(predicate, a => CreateDocumentTypePM(a), "ObjectTable,DocumentTypeCategory");
        private List<DocumentTypeCopyPM> MarkIsOriginalDocumentCopy(DocumentTypePM documentTypePM)
        {
            List<DocumentTypeCopyPM> documentTypeCopies = documentTypePM.DocumentTypeCopies;
            //for (int i = 0; i < documentTypeCopies.Count(); i++)
            //{
            //    if (documentTypeCopies[i].Code == documentTypePM.Code)
            //    {
            //        documentTypeCopies[i].IsOriginal = true;
            //        break;
            //    }
            //}

            return documentTypeCopies;
        }
        private DocumentTypeList CreateDocumentTypeList(DocumentType a)
        {
            return new DocumentTypeList(a)
            {
                ObjectTableName = a.ObjectTable != null ? a.ObjectTable.Name : "",
                DocumentTypeCategoryName = a.DocumentTypeCategory != null ? a.DocumentTypeCategory.Name : null,
            };
        }
        private List<DocumentTypeList> GetList(Expression<Func<DocumentType, bool>> predicate) => repository.GetMulti(predicate, a => CreateDocumentTypeList(a));
        private DocumentTypePM CreateDocumentTypePM(DocumentType a)
        {
            return mapper.Map<DocumentTypePM>(a);
        }
        private IQueryable<DocumentTypeList> FilterDocumentTypeListByTransportModeIdAndShipmentLevelCode(string transportModeId, string shipmentLevelCode, IQueryable<DocumentTypeList> documentTypes)
        {
            #region transportModeId

            if (!string.IsNullOrEmpty(transportModeId))
            {
                switch (transportModeId)
                {
                    case "A":
                        {

                            documentTypes = documentTypes.Where(a => a.IsAir == true);
                            break;
                        }
                    case "O":
                        {
                            documentTypes = documentTypes.Where(a => a.IsOcean == true);
                            break;
                        }
                    case "I":
                        {
                            documentTypes = documentTypes.Where(a => a.IsInland == true);
                            break;
                        }
                }
            }


            #endregion transportModeId


            #region shipmentLevelCode
            if (!string.IsNullOrEmpty(shipmentLevelCode))
            {
                switch (shipmentLevelCode)
                {
                    case "C":
                        {
                            documentTypes = documentTypes.Where(a => a.IsMaster == true);
                            break;
                        }

                    case "M":
                        {
                            documentTypes = documentTypes.Where(a => a.IsMaster == true);
                            break;
                        }

                    case "D":
                        {
                            documentTypes = documentTypes.Where(a => a.IsDirect == true);
                            break;
                        }
                    case "H":
                        {
                            documentTypes = documentTypes.Where(a => a.IsHouse == true);
                            break;
                        }
                }

            }
            #endregion shipmentLevelCode
            return documentTypes;
        }
        #endregion Privare Methods

        #region GetSingle DocumentTypePM
        public DocumentTypePM GetSinglePMWithOutInclude(string id, int tenant) => repository.GetMulti(a => a.Id == id && a.Tenant == tenant, a => CreateDocumentTypePM(a)).FirstOrDefault();
        public DocumentTypePM GetSinglePM(string id, int tenant)
        {
            DocumentTypePM d = GetPMList(a => a.Id == id && a.Tenant == tenant).FirstOrDefault();
            //DocumentTypeCustomFieldQuery documentTypeCustomFieldQuery = new DocumentTypeCustomFieldQuery(tenant);
            //DocumentTypeCopyQuery documentCopiesQuery = new DocumentTypeCopyQuery(tenant);
            //d.DocumentTypeCustomFields = documentTypeCustomFieldQuery.GetDocumentTypeCusotmFieldPMsByDocumentTypeId(d.Id, d.Tenant).ToList();
            //d.DocumentTypeCopies = documentCopiesQuery.GetDocumentTypeCopiesByDocumentType(d.Id, null, d.Tenant);
            return d;
        }
        public DocumentTypePM GetSinglePM(string id, string documentOutId, int tenant)
        {
            DocumentTypePM d = GetSinglePM(id, tenant);
            d.DocumentTypeCopies = MarkIsOriginalDocumentCopy(d);
            return d;
        }
        public DocumentTypePM GetSingelDocumentTypeById(string documentId, int tenant) => GetPMList(a => a.Id == documentId && a.Tenant == tenant).FirstOrDefault();
        public DocumentTypePM GetSinglePMByCode(string code, int tenant) => GetPMList(a => a.Code == code && a.Tenant == tenant).FirstOrDefault();
        public DocumentTypePM GetSinglePMByCodeAndTenant(string code, int tenant) => GetSinglePMByCode(code, tenant);
        public DocumentTypePM GetDigitalSinglePMByCodeAndTenant(string code, int tenant) => GetSinglePMByCode(code, tenant);
        internal DocumentTypePM GetDocumentType(string documentTypeId) => GetPMList(a => a.Id == documentTypeId).FirstOrDefault();

        #endregion GetSingle DocumentTypePM


        #region GetList<DocumentTypePM>
        public List<DocumentTypePM> GetDocumentTypePMsByTenant(int tenant)
        {
            List<DocumentTypePM> d = GetPMList(a => a.Tenant == tenant);
            //DocumentTypeCustomFieldQuery documentTypeCustomFieldQuery = new DocumentTypeCustomFieldQuery(tenant);
            //DocumentTypeCopyQuery documentCopiesQuery = new DocumentTypeCopyQuery(tenant);

            //foreach (DocumentTypePM doc in d)
            //{
            //    doc.DocumentTypeCustomFields = documentTypeCustomFieldQuery.GetDocumentTypeCusotmFieldPMsByDocumentTypeId(doc.Id, doc.Tenant).ToList();
            //    doc.DocumentTypeCopies = documentCopiesQuery.GetDocumentTypeCopiesByDocumentType(doc.Id, null, doc.Tenant);
            //}

            return d;
        }
        public List<DocumentTypePM> GetDocumentTypePMsByEnityIdAndTenant(string transportModeId, string shipmentLevelCode, string objecttableId, int tenant, string childrenObjectTableIds = null)
        {
            string[] childrenIds = !string.IsNullOrEmpty(childrenObjectTableIds) ? childrenObjectTableIds.Split(',') : new string[] { "" };
            var documentTypes =
                GetPMList(a => a.Tenant == tenant && (a.ObjectTableId == objecttableId || childrenIds.Contains(a.ObjectTableId)) && a.IsDocOut) as IQueryable<DocumentTypePM>;
            if (string.IsNullOrEmpty(childrenObjectTableIds))
            {
                documentTypes = documentTypes.Where(d => d.ObjectTableId == objecttableId);
            }
            documentTypes = FilterDocumentTypePMByTransportModeIdAndShipmentLevelCode(transportModeId, shipmentLevelCode, documentTypes);
            return documentTypes.ToList();
        }
        public List<DocumentTypePM> GetDocumentTypePMsByObjectTableAndTenant(string objectTableid, int tenant) => isFullAccounting
                ? GetPMList(a => !a.InActive & a.Tenant == tenant && a.ObjectTableId == objectTableid && (a.IsDocIn || a.IsDocOut))
                : GetPMList(a => a.Tenant == tenant && a.ObjectTableId == objectTableid && (a.IsDocIn || a.IsDocOut));

        //public List<DocumentTypePM> GetFollowUpDocumentTypeByEntityId(string entityId, string objectTableName, int tenant)
        //{
        //    FollowUpQuery followUpQuery = new FollowUpQuery();
        //    FollowUpRepository followUpRepository = new FollowUpRepository(tenant);
        //    List<FollowUp> followUpLists = followUpRepository.GetFollowUpsByEntityId(entityId, objectTableName, tenant);
        //    List<string> followUpDocumenttypeIds = new List<string>();
        //    foreach (FollowUp item in followUpLists)
        //    {
        //        if (!string.IsNullOrEmpty(item.DocumentTypeId)) followUpDocumenttypeIds.Add(item.DocumentTypeId);
        //    }
        //    return GetPMList(a => a.Tenant == tenant && followUpDocumenttypeIds.Contains(a.Id) && a.IsDocOut).ToList();
        //}
        public List<DocumentTypePM> GetTop5DocumentTypePMsByObjectTableId(string objectTableid, int tenant)
        {
            var documentTypes = repository.GetMulti(a => a.Tenant == tenant && a.ObjectTableId == objectTableid && a.InActive == false, a => a.OrderBy, 0, 4);
            var documentTypePMs = mapper.Map<List<DocumentTypePM>>(documentTypes);
            return documentTypePMs;
        }
        public List<DocumentTypePM> GetDocumentTypesPMByObjectTableIdForDocumentPremissions(string objectTableid, int tenant)
        {
            List<DocumentTypePM> documentTypes =
                GetPMList(a => a.Tenant == tenant && a.ObjectTableId == objectTableid && (a.IsDocIn || (a.TemplateFormatCode == "P" && a.IsDocOut)) && !a.InActive);
            //DocumentTypeCopyQuery documentCopiesQuery = new DocumentTypeCopyQuery(tenant);
            //foreach (DocumentTypePM doc in documentTypes)
            //{
            //    doc.DocumentTypeCopies = documentCopiesQuery.GetDocumentTypeCopiesByDocumentType(doc.Id, null, doc.Tenant);
            //}

            return documentTypes;
        }


        #endregion GetList<DocumentTypePM>



        #region GetSingle DocumentTypeList

        public DocumentTypeList GetDocumentTypeListById(string id, int tenant) =>
             repository.GetMulti(a => a.Id == id && a.Tenant == tenant, a => CreateDocumentTypeList(a)).FirstOrDefault();
        public DocumentTypeList GetSingleListByCodeAndTenant(string code, int tenant) => GetList(a => a.Code == code && a.Tenant == tenant).FirstOrDefault();

        #endregion GetSingle DocumentTypeList


        #region GetList<DocumentTypeList>
        public List<DocumentTypeList> GetDocumentTypeListsByEnityIdAndTenant(string transportModeId, string shipmentLevelCode, string objecttableId, int tenant)
        {
            IQueryable<DocumentTypeList> documentTypes = GetList(a => a.Tenant == tenant && a.ObjectTableId == objecttableId && a.IsDocOut && !a.InActive) as IQueryable<DocumentTypeList>;
            documentTypes = FilterDocumentTypeListByTransportModeIdAndShipmentLevelCode(transportModeId, shipmentLevelCode, documentTypes);
            return documentTypes.ToList();
        }
        public List<DocumentTypeList> GetDocumentTypeListsByObjectTableId(string objectTableid, int tenant) =>
            repository.GetMulti(a => a.Tenant == tenant && a.ObjectTableId == objectTableid && a.IsDocOut
            , a => CreateDocumentTypeList(a));
        public List<DocumentTypeList> GetDocumentTypeListsByObjectTableIdForAutomations(string objectTableid, int tenant) =>
            repository.GetMulti(a => a.Tenant == tenant && a.ObjectTableId == objectTableid && a.IsDocOut && a.TemplateFormatCode == "M"
            , a => CreateDocumentTypeList(a));
        public List<DocumentTypeList> GetDocumentTypeListsByObjectTableAndTenant(string objectTableid, int tenant)
        {
            return repository.GetMulti(a => a.Tenant == tenant && a.ObjectTableId == objectTableid && a.IsDocOut && a.TemplateFormatCode == "M"
            , a => CreateDocumentTypeList(a));
        }
        #endregion GetList<DocumentTypeList>


        public IQueryable<DocumentTypePM> GetDocumentTypesByTenantAndTransportMode(int tenant, string transport)
        {
            IQueryable<DocumentTypePM> d = null;
            switch (transport)
            {
                case "A":
                    {
                        d = GetPMList(a => a.Tenant == tenant && a.IsAir == true) as IQueryable<DocumentTypePM>;
                        break;
                    }
                case "O":
                    {
                        d = GetPMList(a => a.Tenant == tenant && a.IsOcean == true) as IQueryable<DocumentTypePM>;
                        break;
                    }
                case "I":
                    {
                        d = GetPMList(a => a.Tenant == tenant && a.IsInland == true) as IQueryable<DocumentTypePM>;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return d;
        }
        public string GetDocumentTypeListIdByCodeAndTenant(string Code, int tenant) => repository.GetMulti(a => a.Code == Code && a.Tenant == tenant, a => a.Id).FirstOrDefault();
        public IQueryable<DocumentTypeList> GetIQueryableEntityList(IQueryable<DocumentType> iQueryable)
        {
            return from documentType in iQueryable.Include("ObjectTable").Include("DocumentTypeCategory")
                   select CreateDocumentTypeList(documentType);
        }

        private void CreateMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new DocumentTypeDataMapping()));
            mapper = config.CreateMapper();
        }
    }

}
