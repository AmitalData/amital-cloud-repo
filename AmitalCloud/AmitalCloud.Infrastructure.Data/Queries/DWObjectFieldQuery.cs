using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using System.Collections.Concurrent;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;
using AutoMapper.QueryableExtensions;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class DWObjectFieldQuery
    {
        readonly int tenant;
        readonly IAmitalCloudContext context;
        readonly Repository<DWObjectField> repository;
        private const string DIM_CustomPickLists = "DIM_CustomPickLists";
        private const string Fact = "Fact";
        private IMapper mapper;

        public DWObjectFieldQuery(int tenant)
        {
            this.tenant = tenant;
            context = AmitalCloudContext.GetContext(tenant);
            repository = new Repository<DWObjectField>(context);
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new DWObjectFieldDataMapping()));
            mapper = config.CreateMapper();
        }

        public List<DWObjectFieldPM> GetDWObjectFieldWithChildrenFieldsPMsByDWObjectTabelAndTenant(string dwotCode)
        {
            var TempList = new DWObjectFieldAdditionalFactService(new DWObjectFieldAdditionalFactArgs() { FactTableCode = dwotCode, Tenant = tenant }).DWObjectFieldPMs;
            var FinalList = TempList.Where(a => a.DimensionTableCode == null).ToList();
            var Parents = TempList.Where(a => a.DimensionTableCode != null).ToList();
            List<string> dimensionTable = Parents.GroupBy(d => d.DimensionTableCode).Select(d => d.First().DimensionTableCode).ToList();
            dimensionTable.Add(DIM_CustomPickLists);
            IEnumerable<IGrouping<string, DWObjectField>> DWObjectFieldPMDimensionGroups = GetDWObjectFieldPMDimensionListsGroups(dimensionTable);

            foreach (var parent in Parents)
            {
                var tempInnerList = new List<DWObjectFieldPM>();
                var dWObjectFieldPMDimensionGroupPoco = DWObjectFieldPMDimensionGroups.Where(d => d.Key == parent.DimensionTableCode).FirstOrDefault();
                if (dWObjectFieldPMDimensionGroupPoco != null)
                {
                    var dWObjectFieldPMDimensionGroup = mapper.Map<List<DWObjectFieldPM>>(dWObjectFieldPMDimensionGroupPoco);

                    string parentfieldName = parent.Name;
                    foreach (DWObjectFieldPM item in dWObjectFieldPMDimensionGroup.ToList().Where(d => (string.IsNullOrEmpty(d.RecordType) || (!string.IsNullOrEmpty(d.RecordType) && d.RecordType.Split(',').Contains(parentfieldName)))))
                    {
                        DWObjectFieldPM dWObjectFieldPM = GetNewInstanceFromDWObjectFieldPM(parent, item, dwotCode);
                        tempInnerList.Add(dWObjectFieldPM);
                    }

                    FinalList = FinalList.Concat(tempInnerList).OrderBy(a => a.DisplayName).ToList();
                }
            }


            return SetDWFullNameTextCode(FinalList);
        }

        public IQueryable<DWObjectFieldPM> GetDWObjectFieldByDWObjectTableCode(string dwotCode)
        {
            var TempList = repository.GetQueryable().Where(a => a.Tenant == tenant && a.DWObjectTableCode == dwotCode && a.CannotFilter == false).ProjectTo<DWObjectFieldPM>(mapper.ConfigurationProvider);
            return TempList;
        }

        private static DWObjectFieldPM GetNewInstanceFromDWObjectFieldPM(DWObjectFieldPM parent, DWObjectFieldPM item, string dwotCode)
        {
            return new DWObjectFieldPM()
            {
                Id = item.Id,
                Tenant = item.Tenant,
                Name = item.Name,
                Code = item.Code,
                DimensionTableCode = parent.DimensionTableCode,
                DataTypeCode = item.DataTypeCode,
                DWObjectTableCode = item.DWObjectTableCode,
                IsRequiered = item.IsRequiered,
                MaxLength = item.MaxLength,
                MinLength = item.MinLength,
                IsPrimaryKey = item.IsPrimaryKey,
                IsMeasurement = item.IsMeasurement,
                AggregationTypeCode = item.AggregationTypeCode,
                DisplayInQueryBuilder = item.DisplayInQueryBuilder,
                LOVAdditionalColumns = item.LOVAdditionalColumns,
                HideTree = item.HideTree,
                CannotFilter = item.CannotFilter,
                HelpText = item.HelpText,
                IsCustom = item.IsCustom,
                DimensionTableDisplayName = parent.Name,
                OriginalObjectFieldCode = item.OriginalObjectFieldCode,
                PartnerOriginalObjectFieldCode = parent.OriginalObjectFieldCode,
                DisplayName = item.Name,
                ViewFieldDisplayName = item.ViewFieldDisplayName,
                DontDisplayInView = item.DontDisplayInView,
                DimensionDataViewName = item.DimensionDataViewName,
                IsMultipleSelection = item.IsMultipleSelection,
                UseUnitSelection = item.UseUnitSelection,
                RecordType = item.RecordType,
                FactTableCode = dwotCode,
            };
        }

        private IEnumerable<IGrouping<string, DWObjectField>> GetDWObjectFieldPMDimensionListsGroups(List<string> dimensionTableLists)
        {
            IEnumerable<IGrouping<string, DWObjectField>> list = repository.GetMulti(a => a.Tenant == tenant && dimensionTableLists.Contains(a.DWObjectTableCode) && a.DisplayInQueryBuilder == true).ToList().GroupBy(d => d.DWObjectTableCode);
            return list;
        }

        public List<DWObjectFieldPM> GetDWObjectFieldPMsByDWObjectTabelAndTenantGroupedByCategory(string dwotCode, string recordType)
        {
            Repository<DWObjectFieldCategories> DWObjectFieldCategoriesRepo = new Repository<DWObjectFieldCategories>(context);
            Repository<DWCategories> DWCategoriesRepo = new Repository<DWCategories>(context);

            List<DWObjectFieldPM> results = (from aa in DWObjectFieldCategoriesRepo.GetQueryable()
                                             join a in repository.GetQueryable() on aa.DWObjectFieldCode equals a.Code
                                             join b in DWCategoriesRepo.GetQueryable() on aa.DWCategoryCode equals b.Code
                                             where a.Tenant == tenant && a.DWObjectTableCode == dwotCode && aa.DWObjectTableCode == dwotCode && (string.IsNullOrEmpty(a.RecordType) || (!string.IsNullOrEmpty(a.RecordType) && a.RecordType.IndexOf(recordType) > -1))
                                             select new DWObjectFieldPM()
                                             {
                                                 Id = a.Id,
                                                 Tenant = a.Tenant,
                                                 Name = a.Name,
                                                 Code = a.Code,
                                                 DimensionTableCode = a.DimensionTableCode,
                                                 DataTypeCode = a.DataTypeCode,
                                                 DWObjectTableCode = a.DWObjectTableCode,
                                                 IsRequiered = a.IsRequired,
                                                 MaxLength = a.MaxLength,
                                                 MinLength = a.MinLength,
                                                 IsPrimaryKey = a.IsPrimaryKey,
                                                 IsMeasurement = a.IsMeasurement,
                                                 AggregationTypeCode = a.AggregationTypeCode,
                                                 DisplayInQueryBuilder = a.DisplayInQueryBuilder,
                                                 LOVAdditionalColumns = a.LOVAdditionalColumns,
                                                 Category = aa.DWCategories.Name,
                                                 CategoryIndex = b.Index,
                                                 HideTree = a.HideTree,
                                                 CannotFilter = a.CannotFilter,
                                                 HelpText = a.HelpText,
                                                 IsCustom = a.IsCustom,
                                                 OriginalObjectFieldCode = a.OriginalObjectFieldCode,
                                                 ViewFieldDisplayName = a.ViewFieldDisplayName,
                                                 DontDisplayInView = a.DontDisplayInView,
                                                 DimensionDataViewName = a.DimensionDataViewName,
                                                 IsMultipleSelection = a.IsMultipleSelection,
                                                 UseUnitSelection = a.UseUnitSelection,
                                                 RecordType = a.RecordType,
                                             }).ToList();

            results = SetDWFullNameTextCode(results);

            return results;
        }

        private List<DWObjectFieldPM> SetDWFullNameTextCode(List<DWObjectFieldPM> dWObjectFieldPMs)
        {
            List<DWObjectFieldPM> results = dWObjectFieldPMs;

            Repository<ObjectField> objectFieldRepo = new Repository<ObjectField>(context);
            var objectFields = objectFieldRepo.GetMulti(a => (a.Tenant == tenant || a.Tenant == 0) && a.CopyToDW);

            if (objectFields.Count > 0)
            {
                foreach (DWObjectFieldPM dWObjectFieldPM in results.Where(d => !string.IsNullOrEmpty(d.OriginalObjectFieldCode) || !string.IsNullOrEmpty(d.PartnerOriginalObjectFieldCode)))
                {
                    var objectField = objectFields.Where(d => d.FieldCode == dWObjectFieldPM.OriginalObjectFieldCode).FirstOrDefault();
                    if (objectField != null) dWObjectFieldPM.FullNameTextCodeCode = objectField.FullNameTextCodeCode;

                    var partnerObjectField = objectFields.Where(d => d.FieldCode == dWObjectFieldPM.PartnerOriginalObjectFieldCode).FirstOrDefault();
                    if (partnerObjectField != null)
                    {
                        dWObjectFieldPM.PartnerFullNameTextCodeCode = partnerObjectField.FullNameTextCodeCode;
                    }

                }
            }
            return results;
        }
    }
}
