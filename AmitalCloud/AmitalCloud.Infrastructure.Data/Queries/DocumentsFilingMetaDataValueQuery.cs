using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;
using Microsoft.Extensions.Logging.Abstractions;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class DocumentsFilingMetaDataValueQuery
    {
        DocumentsFilingMetaDataValueRepository repository;


        public DocumentsFilingMetaDataValueQuery(IAmitalCloudContext context)
        {
            repository = new DocumentsFilingMetaDataValueRepository(context);
        }
        public DocumentsFilingMetaDataValueQuery(int tenant)
        {
            repository = new DocumentsFilingMetaDataValueRepository(tenant);
        }

        public void Create(DocumentsFilingPM parentPM, DocumentsFilingMetaDataValuePM itemPM)
        {
            itemPM.Id = IdCounter.GetNumber("DocumentsFilingMetaDataValue", parentPM.Tenant).ToString();
            itemPM.DocumentsFilingId = parentPM.Id;
            itemPM.Tenant = parentPM.Tenant;

            DocumentsFilingMetaDataValue itemPoco = new DocumentsFilingMetaDataValue()
            {
                Id = itemPM.Id,
            };

          //  DocumentsFilingMetaDataValueMapping.MapEntity(itemPM, itemPoco, true);
            repository.Insert(itemPoco);
        }



        //public static void UpSert_Del(DocumentsFilingPM documentsFilingPM, string MetaDataTypeCode, string MetaDataTypeValue)
        //{
        //    string documentsMetaDataTypeId = null;
        //    var context = AmitalCloudContext.GetContext(documentsFilingPM.Tenant);
        //    DocumentsMetaDataTypeRepository TypesRepo = new DocumentsMetaDataTypeRepository(context);
        //    var pocoMDType = TypesRepo.GetSingleDocumentsMetaDataTypeByCode(MetaDataTypeCode, documentsFilingPM.Tenant);
        //    if (pocoMDType == null)
        //    {
        //        LogMessagingUtil.Instance.AppendLine("GetDocumentsMetaDataTypeVERId is null !!!!! UpSertVERValue - failed ");
        //        return;
        //    }
        //    documentsMetaDataTypeId = pocoMDType.Id;

        //    var repository = new DocumentsFilingMetaDataValueRepository(context);
        //    var documentsFilingMetaDataValueQuery = new DocumentsFilingMetaDataValueQuery(context);
        //    var mydocumentsFilingMetaDataVERValue = documentsFilingMetaDataValueQuery
        //        .GetDocumentsFilingMetaDataValuePMsByDocumentIdTenant(documentsFilingPM.Id, documentsFilingPM.Tenant)
        //        //.FirstOrDefault(r => r.DocumentsMetaDataTypeId == documentsMetaDataTypeId);
        //        .Where(r => r.DocumentsMetaDataTypeId == documentsMetaDataTypeId)
        //        .ToList();

        //    var config = new MapperConfiguration(cfg => cfg.AddProfile(new DocumentsFilingMetaDataValueDataMapping()), new NullLoggerFactory());
        //    var mapper = config.CreateMapper();
        //    var mydocumentsFilingMetaDataVERValueList = mapper.Map<List<DocumentsFilingMetaDataValuePM>>(mydocumentsFilingMetaDataVERValue);

        //    DocumentsFilingMetaDataValuePM mydocumentsFilingMetaDataVERValuePM = null;
        //    if (mydocumentsFilingMetaDataVERValueList.Count() > 1)
        //    {
        //        var listId = mydocumentsFilingMetaDataVERValueList.Select(r => r.Id);
        //        var pocosDelete = repository.GetAll(documentsFilingPM.Tenant).Where(r => listId.Contains(r.Id)).ToList();
        //        pocosDelete.ForEach(itemPoco => { repository.Delete(itemPoco); });

        //        LogMessagingUtil.Instance.AppendLine("UpSert_Del Del there is more then 1 (hd#353339)");

        //        documentsFilingPM.DocumentsFilingMetaDataValues.Clear();
        //    }
        //    else if (mydocumentsFilingMetaDataVERValueList.Count() == 1)
        //    {
        //        mydocumentsFilingMetaDataVERValuePM = mydocumentsFilingMetaDataVERValueList.FirstOrDefault();
        //    }


        //    if (mydocumentsFilingMetaDataVERValuePM != null)
        //    {
        //        if (mydocumentsFilingMetaDataVERValuePM.MetaDataValue == MetaDataTypeValue)
        //        {
        //            LogMessagingUtil.Instance.AppendLine("mydocumentsFilingMetaDataVERValue.MetaDataValue == DeclarationNumber ");
        //            return;
        //        }
        //        var itemPoco = new DocumentsFilingMetaDataValue();

        //        mydocumentsFilingMetaDataVERValuePM.MetaDataValue = MetaDataTypeValue;
        //     //   DocumentsFilingMetaDataValueMapping.MapEntity(mydocumentsFilingMetaDataVERValuePM, itemPoco, true /*false - if false do not map keys !!*/ );
        //        //var repository = new DocumentsFilingMetaDataValueRepository(context);
        //        repository.Update(itemPoco);
        //        var pmInMem = documentsFilingPM.DocumentsFilingMetaDataValues.FirstOrDefault(r => r.DocumentsMetaDataTypeId == documentsMetaDataTypeId);
        //        if (pmInMem != null)
        //        {
        //            pmInMem.MetaDataValue = MetaDataTypeValue;
        //        }
        //        else
        //        {
        //            LogMessagingUtil.Instance.AppendLine("documentsFilingPM.DocumentsFilingMetaDataValues.Add(mydocumentsFilingMetaDataVERValuePM);");//nir ask to delete 

        //        }

        //    }
        //    else
        //    {
        //        var temp = new DocumentsFilingMetaDataValuePM()
        //        {
        //            DocumentsMetaDataTypeId = documentsMetaDataTypeId,
        //            MetaDataValue = MetaDataTypeValue,
        //            //DocumentsMetaDataTypeCode = MetaDataTypeCode,

        //        };
        //        documentsFilingMetaDataValueQuery.Create(documentsFilingPM, temp);
        //        documentsFilingPM.DocumentsFilingMetaDataValues.Add(temp);


        //    }
        //    repository.SubmitChanges();
        //}

        //public static void UpSert_OLD_MOVE2DELSERT(DocumentsFilingPM documentsFilingPM, string MetaDataTypeCode, string MetaDataTypeValue)
        //{
        //    string documentsMetaDataTypeId = null;
        //    var context = AmitalCloudContext.GetContext(documentsFilingPM.Tenant);
        //    DocumentsMetaDataTypeRepository TypesRepo = new DocumentsMetaDataTypeRepository(context);
        //    var pocoMDType = TypesRepo.GetSingleDocumentsMetaDataTypeByCode(MetaDataTypeCode, documentsFilingPM.Tenant);
        //    if (pocoMDType == null)
        //    {
        //        LogMessagingUtil.Instance.AppendLine("GetDocumentsMetaDataTypeVERId is null !!!!! UpSertVERValue - failed ");
        //        return;
        //    }
        //    documentsMetaDataTypeId = pocoMDType.Id;


        //    var documentsFilingMetaDataValueQuery = new DocumentsFilingMetaDataValueQuery(context);
        //    var mydocumentsFilingMetaDataVERValue = documentsFilingMetaDataValueQuery
        //        .GetDocumentsFilingMetaDataValuePMsByDocumentIdTenant(documentsFilingPM.Id, documentsFilingPM.Tenant)
        //        .FirstOrDefault(r => r.DocumentsMetaDataTypeId == documentsMetaDataTypeId);

        //    var config = new MapperConfiguration(cfg => cfg.AddProfile(new DocumentsFilingMetaDataValueDataMapping()), new NullLoggerFactory());
        //    var mapper = config.CreateMapper();
        //    var mydocumentsFilingMetaDataVERValuePM = mapper.Map<DocumentsFilingMetaDataValuePM>(mydocumentsFilingMetaDataVERValue);

        //    if (mydocumentsFilingMetaDataVERValuePM != null)
        //    {
        //        if (mydocumentsFilingMetaDataVERValuePM.MetaDataValue == MetaDataTypeValue)
        //        {
        //            LogMessagingUtil.Instance.AppendLine("mydocumentsFilingMetaDataVERValue.MetaDataValue == DeclarationNumber ");
        //            return;
        //        }
        //        var itemPoco = new DocumentsFilingMetaDataValue();

        //        mydocumentsFilingMetaDataVERValuePM.MetaDataValue = MetaDataTypeValue;
        //      //  DocumentsFilingMetaDataValueMapping.MapEntity(mydocumentsFilingMetaDataVERValuePM, itemPoco, true /*false - if false do not map keys !!*/ );
        //        var repository = new DocumentsFilingMetaDataValueRepository(context);
        //        repository.Update(itemPoco);
        //        var pmInMem = documentsFilingPM.DocumentsFilingMetaDataValues.FirstOrDefault(r => r.DocumentsMetaDataTypeId == documentsMetaDataTypeId);
        //        if (pmInMem != null)
        //        {
        //            pmInMem.MetaDataValue = MetaDataTypeValue;
        //        }
        //        else
        //        {
        //            documentsFilingPM.DocumentsFilingMetaDataValues.Add(mydocumentsFilingMetaDataVERValuePM);
        //        }
        //        TypesRepo.SubmitChanges();
        //    }
        //    else
        //    {
        //        var temp = new DocumentsFilingMetaDataValuePM()
        //        {
        //            DocumentsMetaDataTypeId = documentsMetaDataTypeId,
        //            MetaDataValue = MetaDataTypeValue,
        //            //DocumentsMetaDataTypeCode = MetaDataTypeCode,

        //        };
        //        documentsFilingMetaDataValueQuery.Create(documentsFilingPM, temp);
        //        documentsFilingPM.DocumentsFilingMetaDataValues.Add(temp);

        //        documentsFilingMetaDataValueQuery.repository.SubmitChanges();
        //    }

        //}


        public DocumentsFilingMetaDataValueQuery(DocumentsFilingMetaDataValueRepository documentsFilingMetaDataValueRepository)
        {
            repository = documentsFilingMetaDataValueRepository;
        }

        public DocumentsFilingMetaDataValuePM GetSinglePM(string id, int tenant)
        {
            DocumentsFilingMetaDataValue poco = repository.GetSingleDocumentsFilingMetaDataValue(id, tenant);

            return new DocumentsFilingMetaDataValuePM()
            {

                Id = poco.Id,
                DocumentsFilingId = poco.DocumentsFilingId,
                DocumentsMetaDataTypeId = poco.DocumentsMetaDataTypeId,
                MetaDataValue = poco.MetaDataValue,
                Tenant = poco.Tenant,
            };
        }

        public IQueryable<DocumentsFilingMetaDataValue> GetDocumentsFilingMetaDataValuePMsByDocumentIdTenant(string documentsFilingId, int tenant)
        => from a in repository.context.DocumentsFilingMetaDataValues.Include("DocumentsMetaDataTypes")
           where a.Tenant == tenant && a.DocumentsFilingId == documentsFilingId
           select a;


        public IQueryable<DocumentsFilingMetaDataValuePM> GetDocumentsFilingMetaDataValuePMsByTenant1(int tenant)
        {
            IQueryable<DocumentsFilingMetaDataValuePM> documents = from a in repository.context.DocumentsFilingMetaDataValues
                                                                   where a.Tenant == tenant
                                                                   select new DocumentsFilingMetaDataValuePM()
                                                                   {
                                                                       Id = a.Id,
                                                                       DocumentsFilingId = a.DocumentsFilingId,
                                                                       DocumentsMetaDataTypeId = a.DocumentsMetaDataTypeId,
                                                                       MetaDataValue = a.MetaDataValue,
                                                                       Tenant = a.Tenant,
                                                                   };
            return documents;
        }

        public IQueryable<DocumentsFilingMetaDataValuePM> GetDocumentsFilingMetaDataValuePMsByTenantAndDocumentIds(int tenant, string[] documentIds)
        {
            IQueryable<DocumentsFilingMetaDataValuePM> documents = from a in repository.context.DocumentsFilingMetaDataValues
                                                                   where a.Tenant == tenant && documentIds.Contains(a.Id)
                                                                   select new DocumentsFilingMetaDataValuePM()
                                                                   {
                                                                       Id = a.Id,
                                                                       DocumentsFilingId = a.DocumentsFilingId,
                                                                       DocumentsMetaDataTypeId = a.DocumentsMetaDataTypeId,
                                                                       MetaDataValue = a.MetaDataValue,
                                                                       Tenant = a.Tenant,
                                                                   };
            return documents;
        }

        public List<string> GetDocumentsFilingMetaDataValuesPMsByTenantMetaDataValueDocumentsMetaDataTypeId(int tenant, string CARFI, string courierhawb, string INTGR_R, string integratorCode)
        {
            var documents = (from a in repository.context.DocumentsFilingMetaDataValues.Include("DocumentsMetaDataTypes")
                             where a.Tenant == tenant && ((a.DocumentsMetaDataType.Code == CARFI && a.MetaDataValue == courierhawb) || (a.DocumentsMetaDataType.Code == INTGR_R && a.MetaDataValue == integratorCode))
                             orderby a.DocumentsFilingId
                             select a).GroupBy(p => p.DocumentsFilingId).Where(x => x.Count() == 2).Select(y => y.Key).ToList();
            return documents;
        }

    }
}
