using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.DBHelpers;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model;
using AmitalCloud.Infrastructure.Model.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Enums;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Application.BaseClasses
{

    public abstract class BaseEntityListQueryService<TEntityList, TEntity, TEntityKeys, TKeyType> : IEntityListQueryService<TEntityList> where TEntityList : class, new()
            where TEntity : BaseEntity
            where TEntityKeys : IEntityKeyFields<TEntity, TKeyType>, new()
    {
        private readonly IContext _context;
        private readonly DbSet<TEntity> _dbSet;
        protected BaseEntityListQueryService(IContext context)
        {
            _context = context;
            _dbSet = (context).Set<TEntity>();
        }
        protected BaseEntityListQueryService(int tenant)
        {
            _context = GetContext(tenant);
            _dbSet = (_context).Set<TEntity>();
        }
        public List<TEntityList> GetList(int tenant) => GetList(new QueryOperations() { QueryFilterItems = new List<QueryFilterItem>(), PageIndex = 0, GetAll = true }, tenant);
        public List<TEntityList> GetList(QueryOperations queryOperations, int tenant) => GetList(queryOperations, tenant, new TreeFilterQueryArgs());
        public List<TEntityList> GetList(QueryOperations queryOperations, int tenant, TreeFilterQueryArgs treeFilterQueryArgs)
        {
            GenericSort sortClass;
            int skipCount;
            IQueryable<TEntityList> query = GetQuery(queryOperations, treeFilterQueryArgs, out sortClass, out skipCount);
            if (!string.IsNullOrEmpty(queryOperations.SortByColumnName) && !string.IsNullOrEmpty(queryOperations.SortDirection))
            {
                PropertyInfo? propInfo = typeof(TEntityList).GetProperty(queryOperations.SortByColumnName);
                if (propInfo == null)
                    throw new InvalidOperationException($"Sort column '{queryOperations.SortByColumnName}' does not exist on type {typeof(TEntityList).Name}");

                List<ObjectField> objectFields = ObjectFieldRepository.GetObjectFieldsByObjectTableName(nameof(AccountingInformationIdentifier), tenant).ToList();
                ObjectField? objectField = (from a in objectFields
                                           where a.FieldName == queryOperations.SortByColumnName
                                           select a).FirstOrDefault();
                if (objectField != null)
                {
                    if (objectField.IsCustom)
                    {
                        query = sortClass.GetSorterQuery<TEntityList, string>(queryOperations, query);
                    }
                    else
                    {
                        switch (objectField.DataTypeCode.ToLower())
                        {
                            case "ntext":
                            case "text":
                                {
                                    query = sortClass.GetSorterQuery<TEntityList, string>(queryOperations, query);
                                    break;
                                }
                            case "sigdouble":
                            case "double":
                                {
                                    query = sortClass.GetSorterQuery<TEntityList, double>(queryOperations, query);
                                    break;
                                }
                            case "date":
                            case "datetime":
                                {
                                    query = sortClass.GetSorterQuery<TEntityList, DateTime>(queryOperations, query);
                                    break;
                                }
                            case "unsinteger":
                            case "integer":
                                {
                                    query = sortClass.GetSorterQuery<TEntityList, int>(queryOperations, query);
                                    break;
                                }
                            case "boolean":
                                {
                                    query = sortClass.GetSorterQuery<TEntityList, bool>(queryOperations, query);
                                    break;
                                }
                            case "unsdecimal":
                            case "decimal":
                                {
                                    query = sortClass.GetSorterQuery<TEntityList, decimal>(queryOperations, query);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }
            }
            if (!queryOperations.GetAll && queryOperations.PageSize > 0)
            {
                query = query.Skip(skipCount).Take(queryOperations.PageSize);
            }
            return query.ToList();
        }
        private IQueryable<TEntityList> GetQuery(QueryOperations queryOperations, TreeFilterQueryArgs treeFilterQueryArgs, out GenericSort sortClass, out int skippedPorts)
        {
            GenericFilter filter = new GenericFilter();
            sortClass = new GenericSort();
            IQueryable<TEntity> iQueryable = Query();
            iQueryable = ApplyBusinessUnitFilters(queryOperations, iQueryable);
            iQueryable = ApplyCustomFilters(queryOperations, iQueryable);
            QueryOperations nonListQueryOperation = new QueryOperations();
            nonListQueryOperation.QueryFilterItems = queryOperations.QueryFilterItems.Where(d => d.DisplayInList == false && !d.IsListFilter).ToList();
            QueryOperations listQueryOperation = new QueryOperations();
            listQueryOperation.QueryFilterItems = queryOperations.QueryFilterItems.Where(d => d.DisplayInList == true || d.IsListFilter).ToList();
            iQueryable = filter.GetFilteredQuery<TEntity>(nonListQueryOperation, iQueryable);
            skippedPorts = queryOperations.PageIndex;
            IQueryable<TEntityList> query = filter.GetFilteredQuery<TEntityList>(listQueryOperation, GetIqueryableList(iQueryable));
            treeFilterQueryArgs.Tenant = _context.Tenant;
            return InjectionUtil.Instance.ApplyTreeFilter<TEntityList>(query, treeFilterQueryArgs);
        }
        public int GetListCount(QueryOperations queryOperations) => GetListCount(queryOperations, 0, new TreeFilterQueryArgs());
        public int GetListCount(QueryOperations queryOperations, TreeFilterQueryArgs treeFilterQueryArgs) => GetListCount(queryOperations, 0, treeFilterQueryArgs);
        public int GetListCount(QueryOperations queryOperations, int tenant, TreeFilterQueryArgs treeFilterQueryArgs)
        {
            GenericSort sortClass;
            int skipCount;
            var query = GetQuery(queryOperations, treeFilterQueryArgs, out sortClass, out skipCount);
            return query.Count();
        }
        public TEntityList GetSingle(IEnumerable<KeyValuePair<string, string>> paramList)
        {
            var key = new TEntityKeys();
            key.Initialize(paramList);
            return GetIqueryableList(Query().Where(key.Predicate)).FirstOrDefault();
        }
        protected IQueryable<TEntity> ApplyBusinessUnitFilters(QueryOperations queryOperations, IQueryable<TEntity> iQueryable) => iQueryable;
        protected virtual IQueryable<TEntity> ApplyCustomFilters(QueryOperations queryOperations, IQueryable<TEntity> iQueryable) => iQueryable;
        private IQueryable<TEntity> Query() => (from a in _dbSet select a);

        protected virtual IQueryable<TEntityList> GetIqueryableList(IQueryable<TEntity> iQueryable)
        {
            return (from a in iQueryable let list = GetNewList(a) select list);
        }
        private TEntityList GetNewList(TEntity entity)
        {
            return _factory(entity);
        }

        private static readonly Func<TEntity, TEntityList> _factory = CreateFactory();
        private static Func<TEntity, TEntityList> CreateFactory()
        {
            var param = Expression.Parameter(typeof(TEntity), "entity");
            var ctor = typeof(TEntityList).GetConstructor(new[] { typeof(TEntity) });
            var newExpr = Expression.New(ctor, param);
            var lambda = Expression.Lambda<Func<TEntity, TEntityList>>(newExpr, param);
            return lambda.Compile();
        }
        private IContext GetContext(int tenant) => DbContextBaseUtil.GetContext<TEntity>(tenant);
    }
}