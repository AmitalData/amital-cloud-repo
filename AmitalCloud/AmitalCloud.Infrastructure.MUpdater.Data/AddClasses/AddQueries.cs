using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.MUpdater.Data.AddClasses
{
    public class AddQueries
    {
        public static Query AddQuery(QueryPM queryDetails, Repository<Query> queryRepository, Dictionary<string, Query> tenantQueries, int contextTenant = 0)
        {
            if (tenantQueries.Keys.Contains(queryDetails.Code + queryDetails.ObjectTableId))
            {
                Query query = tenantQueries[queryDetails.Code + queryDetails.ObjectTableId];
                query.DisplayCount = queryDetails.DisplayCount;
                query.IndexOrder = queryDetails.IndexOrder;
                query.IsAddNewEntityEnabled = queryDetails.IsAddNewEntityEnabled;
                query.OriginalQueryId = queryDetails.OriginalQueryId;
                query.OriginalQueryCode = queryDetails.OriginalQueryCode;
                query.QueryGroupCode = queryDetails.QueryGroupCode;
                query.QuerySection = queryDetails.QuerySection;
                query.SystemLevel = queryDetails.SystemLevel;
                query.Tenant = queryDetails.Tenant;
                query.TenantLevel = queryDetails.TenantLevel;
                query.UserId = queryDetails.UserId;
                query.DefaultSortDirection = queryDetails.DefaultSortDirection;
                query.DefaultSortColumn = queryDetails.DefaultSortColumn;
                query.NameTextCodeId = queryDetails.NameTextCodeId;
                query.NameTextCodeCode = queryDetails.NameTextCodeCode;
                query.SpotlightDataTemplate = queryDetails.SpotlightDataTemplate;
                query.Customer = queryDetails.Customer;
                query.Agent = queryDetails.Agent;
                query.Internal = queryDetails.Internal;
                query.EditWizardName = queryDetails.EditWizardName;
                query.EditWizardComponentPath = queryDetails.EditWizardComponentPath;
                query.FeatureId = queryDetails.FeatureId;
                query.Perspective = queryDetails.Perspective;
                query.IsHiddenFromView = queryDetails.IsHiddenFromView;
                query.IsNewFromTenantZeroOnly = queryDetails.IsNewFromTenantZeroOnly;
                query.UniqueCode = queryDetails.Tenant!=0? queryDetails.ObjectTableName+"."+ queryDetails.UserId+ "." + queryDetails.Code: queryDetails.ObjectTableName +"." + queryDetails.Code;
                query.FeatureUniqeCode = queryDetails.FeatureUniqeCode;
                queryRepository.Update(query);
                return query;
            }
            else
            {
                Query newQuery = new Query()
                {
                    UserId = queryDetails.UserId,
                    TenantLevel = queryDetails.TenantLevel,
                    Tenant = queryDetails.Tenant,
                    SystemLevel = queryDetails.SystemLevel,
                    QuerySection = queryDetails.QuerySection,
                    QueryGroupCode = queryDetails.QueryGroupCode,
                    OriginalQueryId = queryDetails.OriginalQueryId,
                    OriginalQueryCode = queryDetails.OriginalQueryCode,
                    ObjectTableId = queryDetails.ObjectTableId,
                    IsAddNewEntityEnabled = queryDetails.IsAddNewEntityEnabled,
                    IndexOrder = queryDetails.IndexOrder,
                    DisplayCount = queryDetails.DisplayCount,
                    Code = queryDetails.Code,
                    DefaultSortDirection = queryDetails.DefaultSortDirection,
                    DefaultSortColumn = queryDetails.DefaultSortColumn,
                    Id = IdCounter.GetNumber("Query", contextTenant).ToString(),
                    NameTextCodeId = queryDetails.NameTextCodeId,
                    NameTextCodeCode = queryDetails.NameTextCodeCode,
                    SpotlightDataTemplate = queryDetails.SpotlightDataTemplate,
                    Agent = queryDetails.Agent,
                    Customer = queryDetails.Customer,
                    Internal = queryDetails.Internal,
                    FeatureId = queryDetails.FeatureId,
                    EditWizardName = queryDetails.EditWizardName,
                    EditWizardComponentPath = queryDetails.EditWizardComponentPath,
                    Perspective = queryDetails.Perspective,
                    IsHiddenFromView = queryDetails.IsHiddenFromView,
                    IsNewFromTenantZeroOnly = queryDetails.IsNewFromTenantZeroOnly,
                    UniqueCode = queryDetails.Tenant != 0 ? queryDetails.ObjectTableName + "." + queryDetails.UserId + "." + queryDetails.Code : queryDetails.ObjectTableName + "." + queryDetails.Code,


                FeatureUniqeCode = queryDetails.FeatureUniqeCode,

                }; 
                queryRepository.Insert(newQuery);
                return newQuery;
            }
        }

        public static Query AddQuery(QueryPM queryDetails, List<Query> addedQueries, int contextTenant = 0)
        {

            Query newQuery = new Query()
            {
                UserId = queryDetails.UserId,
                TenantLevel = queryDetails.TenantLevel,
                Tenant = queryDetails.Tenant,
                SystemLevel = queryDetails.SystemLevel,
                QuerySection = queryDetails.QuerySection,
                QueryGroupCode = queryDetails.QueryGroupCode,
                OriginalQueryId = queryDetails.OriginalQueryId,
                OriginalQueryCode = queryDetails.OriginalQueryCode,
                ObjectTableId = queryDetails.ObjectTableId,
                IsAddNewEntityEnabled = queryDetails.IsAddNewEntityEnabled,
                IndexOrder = queryDetails.IndexOrder,
                DisplayCount = queryDetails.DisplayCount,
                Code = queryDetails.Code,
                DefaultSortDirection = queryDetails.DefaultSortDirection,
                DefaultSortColumn = queryDetails.DefaultSortColumn,
                Id = IdCounter.GetIdWithIdsRange("Query",100, contextTenant).ToString(),
                NameTextCodeId = queryDetails.NameTextCodeId,
                NameTextCodeCode = queryDetails.NameTextCodeCode,
                SpotlightDataTemplate = queryDetails.SpotlightDataTemplate,
                Agent = queryDetails.Agent,
                Customer = queryDetails.Customer,
                Internal = queryDetails.Internal,
                FeatureId = queryDetails.FeatureId,
                EditWizardName = queryDetails.EditWizardName,
                EditWizardComponentPath = queryDetails.EditWizardComponentPath,
                Perspective = queryDetails.Perspective,
                IsHiddenFromView = queryDetails.IsHiddenFromView,
                IsNewFromTenantZeroOnly = queryDetails.IsNewFromTenantZeroOnly,
                UniqueCode = queryDetails.Tenant != 0 ? queryDetails.ObjectTableName + "." + queryDetails.UserId + "." + queryDetails.Code : queryDetails.ObjectTableName + "." + queryDetails.Code,


                FeatureUniqeCode = queryDetails.FeatureUniqeCode,

            };
            addedQueries.Add(newQuery);
            return newQuery;

        }

        public static QueryColumn AddQueryColumn(QueryColumnPM queryColumnDetails, Repository<QueryColumn> queryColumnRepository, Dictionary<string, QueryColumn> tenantQueryColumn, int contextTenant = 0)
        {
            if (tenantQueryColumn.Keys.Contains(queryColumnDetails.QueryCode + queryColumnDetails.ObjectFieldCode))
            {
                QueryColumn queryColumn = tenantQueryColumn[queryColumnDetails.QueryCode + queryColumnDetails.ObjectFieldCode];
                queryColumn.ColumnWidth = queryColumnDetails.ColumnWidth;
                queryColumn.IndexOrder = queryColumnDetails.IndexOrder;
                queryColumn.Tenant = queryColumnDetails.Tenant;
                queryColumnRepository.Update(queryColumn);
                return queryColumn;
            }

            else
            {
                QueryColumn newQureyColumn = new QueryColumn()
                {
                    Tenant = queryColumnDetails.Tenant,
                    ObjectFieldId = queryColumnDetails.ObjectFieldId,
                    IndexOrder = queryColumnDetails.IndexOrder,
                    ColumnWidth = queryColumnDetails.ColumnWidth,
                    Id = IdCounter.GetNumber("QueryColumn", contextTenant).ToString(),
                    QueryId = queryColumnDetails.QueryId,
                    QueryCode = queryColumnDetails.QueryCode,
                    ObjectFieldCode = queryColumnDetails.ObjectFieldCode,
                };
                queryColumnRepository.Insert(newQureyColumn);
                return newQureyColumn;
            }
        }

        public static QueryColumn AddQueryColumn(QueryColumnPM queryColumnDetails, List<QueryColumn> addedColumns,int contextTenant=0)
        {

            QueryColumn newQureyColumn = new QueryColumn()
            {
                Tenant = queryColumnDetails.Tenant,
                ObjectFieldId = queryColumnDetails.ObjectFieldId,
                IndexOrder = queryColumnDetails.IndexOrder,
                ColumnWidth = queryColumnDetails.ColumnWidth,
                Id = IdCounter.GetIdWithIdsRange("QueryColumn", 100, contextTenant).ToString(),
                QueryId = queryColumnDetails.QueryId,
                QueryCode = queryColumnDetails.QueryCode,
                ObjectFieldCode = queryColumnDetails.ObjectFieldCode,
            };
            addedColumns.Add(newQureyColumn);
            return newQureyColumn;

        }



        public static AdvancedQueryFilter AddAdvancedQueryFilter(AdvancedQueryFilterPM advancedQueryFilterDetails, Repository<AdvancedQueryFilter> advancedQueryFilterRepository, Dictionary<string, AdvancedQueryFilter> tenantAdvancedQueryFilter, int contextTenant = 0)
        {
            if (tenantAdvancedQueryFilter.Keys.Contains(advancedQueryFilterDetails.QueryCode + advancedQueryFilterDetails.ObjectFieldCode))
            {
                AdvancedQueryFilter advancedQueryFilter = tenantAdvancedQueryFilter[advancedQueryFilterDetails.QueryCode + advancedQueryFilterDetails.ObjectFieldCode];

                advancedQueryFilter.IndexOrder = advancedQueryFilterDetails.IndexOrder;
                advancedQueryFilter.IsPredefined = advancedQueryFilterDetails.IsPredefined;
                advancedQueryFilter.Operator = advancedQueryFilterDetails.Operator;
                advancedQueryFilter.PredefinedValue = advancedQueryFilterDetails.PredefinedValue;
                advancedQueryFilter.PredefinedValue2 = advancedQueryFilterDetails.PredefinedValue2;
                advancedQueryFilter.CustomPredefined = advancedQueryFilterDetails.CustomPredefined;
                advancedQueryFilter.Tenant = advancedQueryFilterDetails.Tenant;
                advancedQueryFilter.ObjectFieldCode = advancedQueryFilterDetails.ObjectFieldCode;
                advancedQueryFilterRepository.Update(advancedQueryFilter);
                return advancedQueryFilter;
            }
            else
            {
                AdvancedQueryFilter newAdvancedQueryFilter = new AdvancedQueryFilter()
                {
                    Tenant = advancedQueryFilterDetails.Tenant,
                    ObjectFieldId = advancedQueryFilterDetails.ObjectFieldId,
                    IndexOrder = advancedQueryFilterDetails.IndexOrder,
                    IsPredefined = advancedQueryFilterDetails.IsPredefined,
                    Operator = advancedQueryFilterDetails.Operator,
                    PredefinedValue = advancedQueryFilterDetails.PredefinedValue,
                    PredefinedValue2 = advancedQueryFilterDetails.PredefinedValue2,
                    CustomPredefined = advancedQueryFilterDetails.CustomPredefined,
                    Id = IdCounter.GetNumber("AdvancedQueryFilter", contextTenant).ToString(),
                    QueryId = advancedQueryFilterDetails.QueryId,
                    QueryCode = advancedQueryFilterDetails.QueryCode,
                    ObjectFieldCode = advancedQueryFilterDetails.ObjectFieldCode,
                };
                advancedQueryFilterRepository.Insert(newAdvancedQueryFilter);
                return newAdvancedQueryFilter;
            }
        }
        public static AdvancedQueryFilter AddAdvancedQueryFilter(AdvancedQueryFilterPM advancedQueryFilterDetails, List<AdvancedQueryFilter> addedFilters, int contextTenant = 0)
        {

            AdvancedQueryFilter newAdvancedQueryFilter = new AdvancedQueryFilter()
            {
                Tenant = advancedQueryFilterDetails.Tenant,
                ObjectFieldId = advancedQueryFilterDetails.ObjectFieldId,
                IndexOrder = advancedQueryFilterDetails.IndexOrder,
                IsPredefined = advancedQueryFilterDetails.IsPredefined,
                Operator = advancedQueryFilterDetails.Operator,
                PredefinedValue = advancedQueryFilterDetails.PredefinedValue,
                PredefinedValue2 = advancedQueryFilterDetails.PredefinedValue2,
                CustomPredefined = advancedQueryFilterDetails.CustomPredefined,
                Id = IdCounter.GetIdWithIdsRange("AdvancedQueryFilter",50, contextTenant).ToString(),
                QueryId = advancedQueryFilterDetails.QueryId,
                QueryCode = advancedQueryFilterDetails.QueryCode,
                ObjectFieldCode = advancedQueryFilterDetails.ObjectFieldCode,
            };
            addedFilters.Add(newAdvancedQueryFilter);
            return newAdvancedQueryFilter;

        }
    }
}