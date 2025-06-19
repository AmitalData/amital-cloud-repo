using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Reflection;
using AmitalCloud.Infrastructure.Model.BaseClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AmitalCloud.Infrastructure.Application.BaseClasses
{
    public interface IBaseService<TEntity, TEntityList, TEntityPM, TEntityKeys, TKeyType>
        where TEntity : BaseEntity
        where TEntityList : class, new()
        where TEntityPM : IEntityPM, new()
        where TEntityKeys : IEntityKeyFields<TEntity, TKeyType>, new()
    {
        List<TEntityList> GetList(int tenant);
        List<TEntityList> GetList(QueryOperations queryOperations, int tenant);
        List<TEntityList> GetList(QueryOperations queryOperations, int tenant, TreeFilterQueryArgs treeFilterQueryArgs);
        int GetListCount(QueryOperations queryOperations);
        int GetListCount(QueryOperations queryOperations, int tenant, TreeFilterQueryArgs treeFilterQueryArgs);
        int GetListCount(QueryOperations queryOperations, TreeFilterQueryArgs treeFilterQueryArgs);
        List<TEntityPM> GetMultiByParent<TEntityParentKeys>(TEntityParentKeys entityParentKeys, bool getFromCache, bool getComposition = true);
        TEntityList GetSingle(IEnumerable<KeyValuePair<string, string>> paramList);
        TEntityPM GetSingle(IEnumerable<KeyValuePair<string, string>> paramList, bool getComposition, bool getFromCache);
        TEntityPM GetSingle(TEntityKeys entityKeys, bool getComposition, bool getFromCache);
    }
    public abstract class BaseService<TEntity, TEntityList, TEntityPM, TEntityKeys, TKeyType> : IBaseService<TEntity, TEntityList, TEntityPM, TEntityKeys, TKeyType>
            where TEntityList : class, new()
            where TEntityPM : class, IEntityPM, new()
                where TEntity : BaseEntity
                where TEntityKeys : IEntityKeyFields<TEntity, TKeyType>, new()
    {
        protected IRepository<TEntity> repository;
        protected BaseService(IContext context) : this(new Repository<TEntity>(context)) { }
        public BaseService(IRepository<TEntity> repository)
        {
            this.repository = repository;
            this.InitializeSettings();
        }


        public List<TEntityList> GetList(int tenant) => GetList(new QueryOperations() { QueryFilterItems = new List<QueryFilterItem>(), PageIndex = 0, GetAll = true }, tenant);
        public List<TEntityList> GetList(QueryOperations queryOperations, int tenant) => GetList(queryOperations, tenant, new TreeFilterQueryArgs());
        public List<TEntityList> GetList(QueryOperations queryOperations, int tenant, TreeFilterQueryArgs treeFilterQueryArgs)
        {
            GenericSort sortClass;
            int skippedPorts;
            IQueryable<TEntityList> query = GetQuery(queryOperations, treeFilterQueryArgs, out sortClass, out skippedPorts);
            if (!string.IsNullOrEmpty(queryOperations.SortByColumnName) && !string.IsNullOrEmpty(queryOperations.SortDirection))
            {
                query = SortQuery(queryOperations, sortClass, query, tenant);
            }
            if (!queryOperations.GetAll)
            {
                query = query.Skip(skippedPorts).Take(queryOperations.PageSize);
            }
            return query.ToList();
        }

        private IQueryable<TEntityList> SortQuery(QueryOperations queryOperations, GenericSort sortClass, IQueryable<TEntityList> query, int tenant)
        {
            PropertyInfo propInfo = typeof(TEntityList).GetProperty(queryOperations.SortByColumnName);
            List<ObjectField> ObjectFields = ObjectFieldRepository.GetObjectFieldsByObjectTableName(nameof(AccountingInformationIdentifier), tenant).ToList();

            ObjectField? objectField = ObjectFields.Where(a => a.FieldName == queryOperations.SortByColumnName).FirstOrDefault();

            if (objectField != null)
            {
                if (objectField.IsCustom)
                {
                    query = sortClass.GetSorterQuery<TEntityList, string>(queryOperations, query);
                }
                else if (_sortStrategies.TryGetValue(objectField.DataTypeCode.ToLower(), out var strategy))
                {
                    query = strategy.ApplySort<TEntityList>(sortClass, queryOperations, query);
                }
            }
            return query;
        }

        #region Sort Strategic
        public interface ISortStrategy
        {
            IQueryable<TEntity> ApplySort<TEntity>(GenericSort sortClass, QueryOperations operations, IQueryable<TEntity> query);
        }
        private readonly Dictionary<string, ISortStrategy> _sortStrategies = new()
        {
            { "text", new StringSortStrategy() },
            { "ntext", new StringSortStrategy() },
            { "double", new DoubleSortStrategy() },
            { "sigdouble", new DoubleSortStrategy() },
            { "date", new DateTimeSortStrategy() },
            { "datetime", new DateTimeSortStrategy() },

            { "unsinteger", new IntSortStrategy() },
            { "integer", new IntSortStrategy() },
            { "boolean", new BoolSortStrategy() },
            { "unsdecimal", new DecimalSortStrategy() },
            { "decimal", new DecimalSortStrategy() },
        };
        public class StringSortStrategy : ISortStrategy
        {
            public IQueryable<TEntity> ApplySort<TEntity>(GenericSort sortClass, QueryOperations operations, IQueryable<TEntity> query)
            {
                return sortClass.GetSorterQuery<TEntity, string>(operations, query);
            }
        }
        public class DoubleSortStrategy : ISortStrategy
        {
            public IQueryable<TEntity> ApplySort<TEntity>(GenericSort sortClass, QueryOperations operations, IQueryable<TEntity> query)
            {
                return sortClass.GetSorterQuery<TEntity, double>(operations, query);
            }
        }
        public class DateTimeSortStrategy : ISortStrategy
        {
            public IQueryable<TEntity> ApplySort<TEntity>(GenericSort sortClass, QueryOperations operations, IQueryable<TEntity> query)
            {
                return sortClass.GetSorterQuery<TEntity, DateTime>(operations, query);
            }
        }
        public class IntSortStrategy : ISortStrategy
        {
            public IQueryable<TEntity> ApplySort<TEntity>(GenericSort sortClass, QueryOperations operations, IQueryable<TEntity> query)
            {
                return sortClass.GetSorterQuery<TEntity, int>(operations, query);
            }
        }
        public class BoolSortStrategy : ISortStrategy
        {
            public IQueryable<TEntity> ApplySort<TEntity>(GenericSort sortClass, QueryOperations operations, IQueryable<TEntity> query)
            {
                return sortClass.GetSorterQuery<TEntity, bool>(operations, query);
            }
        }
        public class DecimalSortStrategy : ISortStrategy
        {
            public IQueryable<TEntity> ApplySort<TEntity>(GenericSort sortClass, QueryOperations operations, IQueryable<TEntity> query)
            {
                return sortClass.GetSorterQuery<TEntity, decimal>(operations, query);
            }
        }
        #endregion

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
            return InjectionUtil.Instance.ApplyTreeFilter<TEntityList>(query, treeFilterQueryArgs);
        }
        public int GetListCount(QueryOperations queryOperations) => GetListCount(queryOperations, 0, new TreeFilterQueryArgs());
        public int GetListCount(QueryOperations queryOperations, TreeFilterQueryArgs treeFilterQueryArgs) => GetListCount(queryOperations, 0, treeFilterQueryArgs);
        public int GetListCount(QueryOperations queryOperations, int tenant, TreeFilterQueryArgs treeFilterQueryArgs)
        {
            GenericSort sortClass;
            int skippedPorts;
            var query = GetQuery(queryOperations, treeFilterQueryArgs, out sortClass, out skippedPorts);

            return query.Count();
        }
        public TEntityList? GetSingle(IEnumerable<KeyValuePair<string, string>> paramList)
        {
            var key = new TEntityKeys();
            key.Initialize(paramList);
            return GetIqueryableList(Query().Where(key.Predicate)).FirstOrDefault();
        }
        protected IQueryable<TEntity> ApplyBusinessUnitFilters(QueryOperations queryOperations, IQueryable<TEntity> iQueryable) => iQueryable;
        protected IQueryable<TEntity> ApplyCustomFilters(QueryOperations queryOperations, IQueryable<TEntity> iQueryable) => iQueryable;
        private IQueryable<TEntity> Query() => (from a in contextEntity select a);
        protected abstract DbSet<TEntity> contextEntity { get; }
        protected virtual IQueryable<TEntityList> GetIqueryableList(IQueryable<TEntity> iQueryable)
        {
            return (from a in iQueryable let list = GetNewList(a) select list);
        }
        private TEntityList GetNewList(TEntity entity)
        {
            return (TEntityList)typeof(TEntityList).GetConstructor(new Type[] { typeof(TEntity) }).Invoke(entity, null);
        }

        private TEntityPM GetNewPM(TEntity entity)
        {
            return (TEntityPM)typeof(TEntityPM).GetConstructor(new Type[] { typeof(TEntity) }).Invoke(entity, null);
        }

        //*****************
        public TEntityPM? GetSingle(TEntityKeys entityKeys, bool getComposition, bool getFromCache)
        {
            TEntityPM entityPM = default;
            if (getFromCache && (CacheManager.CacheWrapper != null))
            {
                string cacheKey = $"TEntityPMGetSingle_({entityKeys.GetEntityPMName()}_{entityKeys.GetFullKey()}_{getComposition})";
                var cacheObj = CacheManager.CacheWrapper.Get(cacheKey);

                entityPM = CacheManager.GetOrInsertNewObject<TEntityPM>(cacheKey, () => GetEntityPM(repository.GetSingle(entityKeys), getComposition));
            }
            else
            {
                entityPM = GetEntityPM(repository.GetSingle(entityKeys), getComposition);
            }
            return entityPM;
        }
        public TEntityPM GetSingle(IEnumerable<KeyValuePair<string, string>> paramList, bool getComposition, bool getFromCache)
        {
            var keys = new TEntityKeys();
            keys.Initialize(paramList);
            return GetSingle(keys, getComposition, getFromCache);
        }
        public List<TEntityPM> GetMultiByParent<TEntityParentKeys>(TEntityParentKeys entityParentKeys, bool getFromCache, bool getComposition = true)
            => repository.GetMultiByParent<TEntityParentKeys>(entityParentKeys)
                .Select(a => GetPMWithComposition(getComposition, a)).ToList()
                ;

        private TEntityPM GetEntityPM(TEntity entityPOCO, bool getComposition = false) => entityPOCO != null ? GetPMWithComposition(getComposition, entityPOCO) : default(TEntityPM);
        protected virtual void GetComposition(IEntityKeyFields<TEntity, TKeyType> entityKeys, TEntityPM entityPM) { }
        protected abstract IEntityKeyFields<TEntity, TKeyType> GetKeys(TEntity entityPOCO);
        protected virtual void InitializeSettings() { }
        private TEntityPM GetPMWithComposition(bool getComposition, TEntity entityPOCO)
        {
            TEntityPM entityPM = GetNewPM(entityPOCO);
            if (getComposition)
            {
                GetComposition(GetKeys(entityPOCO), entityPM);
            }
            return entityPM;
        }

        public bool DontAddTransaction { get; set; }

        protected virtual void Validate(TEntityPM entityPM)
        {
        }

        public virtual void InitializeUpdateService()
        {

        }

        public virtual void InitializeEntityPM(TEntityPM entityPM)
        {

        }
    }
}

