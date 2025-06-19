using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Security;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace AmitalCloud.Infrastructure.Web.BaseClasses
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseListController<TService, TEntityList> : ControllerBase
        where TService : class
        where TEntityList : class, new()
    {
        private protected string ObjectTableName;
        private protected string QuerySection;
        private protected bool EnableSecurity;
        protected int Tenant { get => AmitalCloudSecurityUtility.AuthenticateTenant(mode: "READ", objectTableName: ObjectTableName); }
        #region Constructors
        protected BaseListController(string objectTableName, string querySection, bool enableSecurity = false)
        {
            ObjectTableName = objectTableName;
            QuerySection = querySection;
            EnableSecurity = enableSecurity;
        }
        protected BaseListController()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public Methods
        [HttpGet("GetSingle")]
        public IActionResult GetSingle()
        {
            string logKey = PerformanceLogger.LogCurrentTime();
            try
            {
                var queryParams = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
                return Ok(GetResult(queryParams));
            }
            catch (Exception ex)
            {
                if (ex is AutenticationException)
                    throw;
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
                // return Request.CreateResponse(HttpStatusCode.BadRequest, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
            finally
            {
                PerformanceLogger.AddServerExecutionTimeHeader(logKey);
            }
        }

        [HttpGet("GetByFilters")]
        public IActionResult GetByFilters([FromQuery] ApiQueryFilters filters)
        {
            string logKey = PerformanceLogger.LogCurrentTime();
            try
            {
                QueryOperations queryOperations = new QueryOperations()
                {
                    ObjectTableName = ObjectTableName,
                    PageIndex = filters.PageIndex,
                    PageSize = filters.PageSize,
                    QuerySection = QuerySection,
                    SortByColumnName = filters.SortBy,
                    SortDirection = filters.SortDirection,
                    GetAll = filters.GetAll,
                };
                List<ObjectField> answerStatusObjectFields = ObjectFieldRepository.GetObjectFieldsByObjectTableName(queryOperations.ObjectTableName, Tenant);
                List<PropertyInfo> filterProperties = filters.GetType().GetProperties().ToList();
                for (int i = 1; i <= 10; i++)
                {
                    object filterNameProp = filterProperties.FirstOrDefault(f => f.Name == ("Filter" + i + "Name"))
                        ?.GetValue(filters);
                    object filterValue1 = filterProperties.FirstOrDefault(f => f.Name == ("Filter" + i + "Value"))
                        ?.GetValue(filters);
                    object filterOperatorProp = filterProperties
                        .FirstOrDefault(f => f.Name == ("Filter" + i + "Operator"))
                        ?.GetValue(filters);
                    object filterValue2 = null;
                    if (filterNameProp != null)
                    {
                        string filterName = filterNameProp.ToString();
                        string filterOperator = filterOperatorProp != null ? filterOperatorProp.ToString() : "Equals";
                        //ToDo: Get object field by name and set the remained filter properties
                        ObjectField field = answerStatusObjectFields.FirstOrDefault(f => f.FieldName == filterName);
                        if (field != null)
                        {
                            string valuestring1 = filterValue1 != null ? filterValue1.ToString() : null;
                            object value1 =
                                FieldValueResolver.GetFieldDataValue(field, valuestring1);
                            string valuestring2 = filterValue2 != null ? filterValue2.ToString() : null;
                            object value2 =
                                FieldValueResolver.GetFieldDataValue(field, valuestring2);
                            queryOperations.SetFilter(filterName, value1, field.IsCustomFilter, filterOperator, value2,
                                field.DisplayInList);
                        }
                        else
                            queryOperations.SetFilter(filterName, filterValue1, false, filterOperator, filterValue2,
                                true);
                    }
                }
                if (!string.IsNullOrEmpty(filters.AdditionalFilters))
                {
                    var filtersList = JsonSerializer.Deserialize<List<QueryFilterItem>>(filters.AdditionalFilters);
                    foreach (QueryFilterItem filter in filtersList)
                    {
                        ObjectField field =
                            answerStatusObjectFields.FirstOrDefault(f => f.FieldName == filter.FieldName);
                        if (field != null)
                        {
                            string valuestring1 = filter.FieldValue != null ? filter.FieldValue.ToString() : null;
                            object value1 =
                                FieldValueResolver.GetFieldDataValue(field, valuestring1);
                            string valuestring2 = filter.FieldValue2 != null ? filter.FieldValue2.ToString() : null;
                            object value2 =
                                FieldValueResolver.GetFieldDataValue(field, valuestring2);
                            queryOperations.SetFilter(filter.FieldName, value1, field.IsCustomFilter, filter.Operator,
                                value2, field.DisplayInList);
                        }
                        else
                        {
                            queryOperations.SetFilter(filter.FieldName, filter.FieldValue, filter.IsCustom,
                                filter.Operator, filter.FieldValue2, filter.DisplayInList);
                        }
                    }
                }
                TreeFilterQueryArgs treeFilterQueryArgs = new TreeFilterQueryArgs()
                {
                    AdditionalTreeFilter = filters.TreeFilters,
                    ObjectTableName = ObjectTableName,
                    ParentEntityId = filters.ParentEntityId,
                    ParentObjectTableName = filters.ParentObjectTableName,
                    Tenant = Tenant,
                    ParentEntity = filters.ParentEntity
                };


                ServiceResponse response = new ServiceResponse();
                int count = 0;
                response.Result = GetResult(queryOperations, treeFilterQueryArgs, filters.GetCount, ref count);
                if (filters.GetCount)
                {
                    response.Count = count;
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is AutenticationException)
                    throw;
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
            finally
            {
                PerformanceLogger.AddServerExecutionTimeHeader(logKey);
            }
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            string logKey = PerformanceLogger.LogCurrentTime();
            try
            {
                return Ok(GetResult());
            }
            catch (Exception ex)
            {
                if (ex is AutenticationException)
                    throw;
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
            finally
            {
                PerformanceLogger.AddServerExecutionTimeHeader(logKey);

            }
        }
        #endregion

        private object GetResult(IEnumerable<KeyValuePair<string, string>> paramList)
        {
            TEntityList result = GetService().GetSingle(paramList);
            if (hasCustomFields)
            {
                CustomFieldResolver customFieldResolver = new CustomFieldResolver(Tenant);
                customFieldResolver.SetCustomFieldsValues(ObjectTableName, Tenant, new List<TEntityList> { result }.Cast<object>().ToList());
            }
            return result;
        }
        private object GetResult()
        {
            List<TEntityList> result = GetService().GetList(Tenant);
            if (hasCustomFields)
            {
                CustomFieldResolver customFieldResolver = new CustomFieldResolver(Tenant);
                customFieldResolver.SetCustomFieldsValues(ObjectTableName, Tenant, result.Cast<object>().ToList());
            }
            return result;
        }
        //private object GetResult(IEnumerable<KeyValuePair<string, string>> paramList) => Request.CreateResponse(HttpStatusCode.OK, GetService().GetSingle(paramList));
        //private object GetResult() => Request.CreateResponse(HttpStatusCode.OK, GetService().GetList(Tenant));
        private object GetResult(QueryOperations queryOperations, TreeFilterQueryArgs treeFilterQueryArgs, bool getCount, ref int count)
        {
            IEntityListQueryService<TEntityList> queryService = GetService();
            if (getCount)
            {
                count = queryService.GetListCount(queryOperations, treeFilterQueryArgs);
            }
            return queryService.GetList(queryOperations, Tenant, treeFilterQueryArgs);
        }
        protected bool hasCustomFields = false;
        private IEntityListQueryService<TEntityList> GetService()
        {
            return (IEntityListQueryService<TEntityList>)typeof(TService).GetConstructor(new Type[] { typeof(int) }).Invoke(null, new object[] { Tenant });
        }
    }
}