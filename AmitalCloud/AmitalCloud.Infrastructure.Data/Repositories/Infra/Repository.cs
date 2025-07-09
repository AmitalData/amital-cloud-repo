using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model;
using AmitalCloud.Infrastructure.Model.Enums;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AmitalCloud.Infrastructure.Data.DBHelpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>, IAsyncRepository<TEntity>, IDisposable
        where TEntity : class
    {
        private readonly IContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
        private string _errorMessage = string.Empty;
        private bool _isDisposed;
        IUnitOfWork? _unitOfWork;
        protected DbSet<TEntity> DbSet => _dbSet;
        internal Repository(int tenant)
        {
            _dbContext = GetContext(tenant);
            _isDisposed = false;
            _dbSet = (_dbContext).Set<TEntity>();
        }
        internal Repository(IUnitOfWork unitOfWork) : this(unitOfWork.Context)
        {
            _unitOfWork = unitOfWork;
        }
        internal Repository(IContext dbContext)
        {
            Type type = typeof(TEntity);
            var attribute = (DataBaseAttribute)Attribute.GetCustomAttribute(type, typeof(DataBaseAttribute));
            if (attribute.Name != dbContext.AmitalCloudDBSchema)
            {
                throw new Exception($"Entity {type.Name} does not match the Context schema {dbContext.AmitalCloudDBSchema}");
            }

            //todo: validate if entity match dbcontext schema
            _isDisposed = false;
            _dbContext = dbContext;
            _dbSet = (dbContext).Set<TEntity>();
        }
        #region ASync Methods
        public async Task<TEntity> GetSingleAsync<TKeyType>(IEntityKeyFields<TEntity, TKeyType> entityKeys) => await _dbSet.Where(entityKeys.Predicate).FirstOrDefaultAsync();
        public async Task<List<TEntity>> GetAllAsync(int tenant) => await GetQuery(tenant).ToListAsync();
        public async Task<List<TEntity>> GetMultiAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();
        public async Task<List<TEntity>> GetAllAsync(int tenant, bool fromCache = true)
        {
            if (fromCache)
            {
                Task<List<TEntity>> task = new Task<List<TEntity>>(() => GetFromCache(tenant));
                task.Start();
                return await task;
            }
            else
            {
                return await GetAllAsync(tenant);
            }
        }
        public async Task<List<TEntity>> GetAllAsync<TKey>(int tenant, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending) => await ApplyOrderedBy<TKey>(orderBy, orderByDirection, GetQuery(tenant)).ToListAsync();
        public async Task<List<TEntity>> GetMultiAsync<TKeyType>(IEntityKeyFields<TEntity, TKeyType> entityKeys) => await GetMultiAsync(entityKeys.Predicate);
        public async Task<List<TEntity>> GetMultiAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, int skip, int take) => await _dbSet.Where(predicate).OrderBy(orderBy).Skip(skip).Take(take).ToListAsync();
        public async Task<IEnumerable<TEntity>> GetMultiAsync<TKey>(ISpecification<TEntity, TKey> spec) => await GetQuery(spec).ToListAsync();
        public async Task<List<TResult>> GetMultiAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select) => await _dbSet.Where(predicate).Select(select).ToListAsync();
        public async Task<List<TEntity>> GetMultiAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select) => await _dbSet.Where(predicate).Select(select).ToListAsync();
        public async Task SubmitChangesAsync()
        {
            if (_unitOfWork == null)
            {
                await SaveAsync();
            }
            else
            {
                throw new Exception("Use UOW.Save()");
            }

        }
        public async Task<List<TEntity>> GetMultiAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select, Expression<Func<TEntity, TKey>> orderBy, int skip, int take) => await _dbSet.Where(predicate).Select(select).OrderBy(orderBy).Skip(skip).Take(take).ToListAsync();
        public async Task<List<TResult>> GetMultiAsync<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, Expression<Func<TResult, TKey>> orderBy, int skip, int take) => await _dbSet.Where(predicate).Select(select).OrderBy(orderBy).Skip(skip).Take(take).ToListAsync();
        public async Task<List<TEntity>> GetMultiAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select, string include, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending)
            => orderByDirection == OrderByDirection.Ascending ? await ApplyInclude(predicate, include).Select(select).OrderBy(orderBy).OrderBy(orderBy).ToListAsync()
            : await ApplyInclude(predicate, include).Select(select).OrderBy(orderBy).OrderByDescending(orderBy).ToListAsync();
        #endregion ASync Methods 

        #region Sync Methods 
        public void Insert(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("Empty Entity");
                }
                if (_dbContext == null || _isDisposed)
                {
                    throw new Exception("Disposed");
                }
                _dbSet.Add(entity);
            }
            catch (DbUpdateException dbEx)
            {
                HandleUnitOfWorkException(dbEx);
                throw new Exception(_errorMessage, dbEx);
            }

        }
        public void Delete(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("Entity");
                }
                if (_dbContext == null || _isDisposed)
                {
                    throw new Exception("Disposed");
                }
                _dbSet.Remove(entity);
            }
            catch (DbUpdateException dbEx)
            {
                HandleUnitOfWorkException(dbEx);
                throw new Exception(_errorMessage, dbEx);
            }
        }
        public void Update(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("Entity");
                }
                if (_dbContext == null || _isDisposed)
                {
                    throw new Exception("Disposed");
                }
                _dbSet.Attach(entity);
                _dbContext.SetAsModified(entity);
            }
            catch (DbUpdateException dbEx)
            {
                HandleUnitOfWorkException(dbEx);
                throw new Exception(_errorMessage, dbEx);
            }
        }
        public TEntity GetFirst() => GetAll(0, true).FirstOrDefault();
        public List<TEntity> GetAll(int tenant) => GetQuery(tenant).ToList();
        public IQueryable<TEntity> GetQueryable() => _dbSet; // todo: change it to GetQuery()

        protected IQueryable<TEntity> GetQuery(int tenant)
        {
            var property = typeof(TEntity).GetProperty("Tenant");
            if (property == null)
                return _dbSet.AsQueryable();

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var body = Expression.Equal(
                Expression.Property(parameter, property),
                Expression.Constant(tenant)
            );
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

            return _dbSet.Where(lambda);
        }
        public List<TEntity> GetAll(int tenant, bool fromCache = false)
        {
            if (fromCache)
            {
                return GetFromCache(tenant);
            }
            else
            {
                return GetAll(tenant);
            }
        }
        public List<TEntity> GetAll<TKey>(int tenant, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending) => ApplyOrderedBy<TKey>(orderBy, orderByDirection, GetQuery(tenant)).ToList();
        public List<TEntity> GetMulti<TKeyType>(IEntityKeyFields<TEntity, TKeyType> entityKeys) => GetMulti(entityKeys.Predicate);
        public List<TResult> GetMulti<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, string include) => ApplyInclude(predicate, include).Select(select).ToList();
        public List<TResult> GetMulti<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, params Expression<Func<TEntity, object>>[] includes) => ApplyInclude(predicate, includes).Select(select).ToList();

        public List<TResult> GetMulti<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select) => _dbSet.Where(predicate).Select(select).ToList();

        public List<TResult> GetMulti<TResult>(Expression<Func<TEntity, bool>> predicate, IMapper mapper)
        {
            var result = _dbSet.Where(predicate).ProjectTo<TResult>(mapper.ConfigurationProvider).ToList();
            return result;
        }

        public List<TResult> GetMulti<TResult>(Expression<Func<TEntity, bool>> predicate, string include) => ApplyInclude(predicate, include).ToList().AsEnumerable().Select(a => NewObject<TResult>(a)).ToList();

        public List<TResult> GetMultiFromCache<TResult>(string cacheKey, Expression<Func<TEntity, bool>> predicate, string include = null, Expression<Func<TEntity, TResult>> select = null)
        {
            List<TResult> entityPMs;

            cacheKey = $"TResultGetMulti_({cacheKey};{include ?? string.Empty})";
            var cacheObj = Helpers.CacheManager.CacheWrapper.Get(cacheKey);
            if (cacheObj != null)
            {
                entityPMs = (List<TResult>)cacheObj;
            }
            else
            {
                if (!string.IsNullOrEmpty(include))
                {
                    if (select == null)
                    {
                        throw new Exception("you can not include tables without selecting columns");
                    }
                    entityPMs = this.GetMulti<TResult>(predicate, select, include);
                }
                else
                {
                    throw new NotImplementedException("not implemented without include");
                    // entityPMs = this.GetMulti<TResult>(predicate);
                }

                if (entityPMs != null)
                {
                    Helpers.CacheManager.CacheWrapper.Insert(cacheKey, entityPMs);
                }
                else
                {
                    Helpers.CacheManager.CacheWrapper.Insert(cacheKey, new Helpers.NullCache());
                }
            }
            return entityPMs;
        }
 
        public List<TEntity> GetMulti(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select) => _dbSet.Where(predicate).Select(select).ToList();
        public List<TEntity> GetMulti(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select, string include) => ApplyInclude(predicate, include).Select(select).ToList();
        public List<TEntity> GetMulti(Expression<Func<TEntity, bool>> predicate) => _dbSet.Where(predicate).ToList();


        public IEnumerable<TEntity> GetMulti<TKey>(ISpecification<TEntity, TKey> spec) => GetQuery(spec).AsEnumerable();

        public TEntity GetSingle<TKeyType>(IEntityKeyFields<TEntity, TKeyType> entityKeys) => GetMulti(entityKeys.Predicate).FirstOrDefault();

        public TResult GetSingle<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, string include = null)
        {
            IQueryable<TEntity> entity = include != null ? ApplyInclude(predicate, include) : _dbSet.Where(predicate);
            return entity.Select(select).FirstOrDefault();
        }
        public TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> entity = includes != null ? ApplyInclude(predicate, includes) : _dbSet.Where(predicate);
            return entity.FirstOrDefault();
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, string include = null)
        {
            IQueryable<TEntity> entity = include != null ? ApplyInclude(predicate, include) : _dbSet.Where(predicate);
            return entity.FirstOrDefault();
        }

        public List<TEntity> GetMulti<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, int skip, int take) => _dbSet.Where(predicate).OrderBy(orderBy).Skip(skip).Take(take).ToList();
        public List<TEntity> GetMulti<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select, Expression<Func<TEntity, TKey>> orderBy, int skip, int take)
            => _dbSet.Where(predicate).Select(select).OrderBy(orderBy).Skip(skip).Take(take).ToList();
        public List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, Expression<Func<TResult, TKey>> orderBy, int skip, int take)
            => _dbSet.Where(predicate).Select(select).OrderBy(orderBy).Skip(skip).Take(take).ToList();
        public List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, int skip, int take)
            => _dbSet.Where(predicate).OrderBy(orderBy).Skip(skip).Take(take).ToList().AsEnumerable().Select(a => NewObject<TResult>(a)).ToList();
        public List<TEntity> GetMulti<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending)
            => (orderByDirection == OrderByDirection.Ascending) ?
            _dbSet.Where(predicate).OrderBy(orderBy).ToList() : _dbSet.Where(predicate).OrderByDescending(orderBy).ToList();
        public List<TEntity> GetMulti<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending)
            => (orderByDirection == OrderByDirection.Ascending) ?
            _dbSet.Where(predicate).Select(select).OrderBy(orderBy).ToList() : _dbSet.Where(predicate).Select(select).OrderByDescending(orderBy).ToList();
        public List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, string include, Expression<Func<TResult, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending)
                    => (orderByDirection == OrderByDirection.Ascending) ?
            ApplyInclude(predicate, include).Select(select).OrderBy(orderBy).ToList() : ApplyInclude(predicate, include).Select(select).OrderByDescending(orderBy).ToList();
        public List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, string include, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending)
                    => (orderByDirection == OrderByDirection.Ascending) ?
            ApplyInclude(predicate, include).OrderBy(orderBy).ToList().AsEnumerable().Select(a => NewObject<TResult>(a)).ToList() : ApplyInclude(predicate, include).OrderByDescending(orderBy).ToList().AsEnumerable().Select(a => NewObject<TResult>(a)).ToList();
        public List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, Expression<Func<TResult, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending)
            => (orderByDirection == OrderByDirection.Ascending) ?
            _dbSet.Where(predicate).Select(select).OrderBy(orderBy).ToList() : _dbSet.Where(predicate).Select(select).OrderByDescending(orderBy).ToList();
        public List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending)
            => (orderByDirection == OrderByDirection.Ascending) ?
            _dbSet.Where(predicate).OrderBy(orderBy).ToList().AsEnumerable()
            .Select(a => NewObject<TResult>(a)).ToList() : _dbSet.Where(predicate).OrderByDescending(orderBy).ToList().AsEnumerable().Select(a => NewObject<TResult>(a)).ToList();



        public List<object> GetMulti<TInner, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TInner, bool>> innerpredicate,
           Expression<Func<TEntity, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector
            ) where TInner : class
        {
            var res = _dbSet.Where(predicate)
                .Join((_dbContext).Set<TInner>().Where(innerpredicate), outerKeySelector, innerKeySelector, (a, b) => new { a, b })
                .ToList<object>();
            return res;
        }


        public List<TEntity> GetMultiByParent<TEntityParentKeys>(TEntityParentKeys entityKeys)
        {
            throw new NotImplementedException();
        }

        protected List<T> GetListNOWAITWhere<T>(Expression<Func<T, bool>> filter) where T : class
        {
            throw new NotImplementedException();

            //using (var myIDbContextLogger = (dbContext as DbContextBase).CreateLogger())
            //{
            //    List<T> myOut = null;
            //    try
            //    {
            //        var adapter = (System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext;
            //        var objectContext = adapter.ObjectContext;
            //        myOut = GetListNOWAITWhere<T>(filter);
            //    }
            //    catch (Exception e)
            //    {
            //        e.ChangeExceptionMess(myIDbContextLogger.ToString());
            //        throw;
            //    }
            //    return myOut;
            //}
        }

        //protected List<T> GetListNOWAITWhere<T>( Expression<Func<T, bool>> filter) where T : class
        //{
        //    try
        //    {

        //        var newselectSql = "";
        //        //var query = db.Set<T>().Where(filter);
        //        var query = db.CreateObjectSet<T>().Where(filter) as ObjectQuery;

        //        string selectSql = query.ToTraceString();


        //        //newselectSql = "SELECT 1 MyCount  " + selectSql.Substring(indexOffROM) + " FOR UPDATE NOWAIT ";

        //        if (AmitalCLoudSettings.DatabaseManagementSystem == "oracle")
        //        {
        //            newselectSql = selectSql + " FOR UPDATE NOWAIT ";
        //            //newselectSql = selectSql + " FOR UPDATE WAIT 1 ";
        //        }
        //        else
        //        {
        //            var indexOfWhere = selectSql.LastIndexOf("WHERE ");
        //            var sqlServer = " WITH(NOWAIT) ";
        //            sqlServer = " WITH(UPDLOCK, NOWAIT) ";

        //            newselectSql = selectSql.Insert(indexOfWhere, sqlServer);
        //        }
        //        var parameters = query.Parameters.Select(p => GetDbParameter(p.Name, p.Value)).ToArray();

        //        //db.Database.ExecuteSqlCommand(deleteSql, parameters);
        //        return db.ExecuteStoreQuery<T>(newselectSql, parameters).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        e.ChangeExceptionMess(myIDbContextLogger.ToString());
        //        throw;
        //    }

        //}

        protected bool DeleteWhere(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = _dbSet.Where(predicate);
            foreach (var entity in entities)
            {
                Delete(entity);
            }
            return true;
        }

        //TODO change access modifier to protected after UOW is implemented
        public void SubmitChanges()
        {
            if (_unitOfWork == null)
            {
                Save();
            }
            else
            {
                throw new Exception("Use UOW.Save()");
            }
        }

        public void Dispose()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
            _isDisposed = true;
        }

        #endregion Sync Methods

        #region Private Methods 

        private void Save()
        {
            var method = _dbContext.GetType().GetMethods().FirstOrDefault(m => m.Name == "SaveChanges" && m.GetParameters().Length == 0);
            if (method != null)
            {
                method.Invoke(_dbContext, null);
            }
            else
            {
                throw new InvalidOperationException("No matching SaveChanges() method found.");
            }
        }
        private async Task SaveAsync()
        {
            await (Task)_dbContext.GetType().GetMethod("SaveChangesAsync").Invoke(_dbContext, null);
        }

        private void HandleUnitOfWorkException(DbUpdateException dbEx)
        {
            _errorMessage = GetValidationErrors();
        }
        private string GetValidationErrors()
        {
            var errorMessage = new StringBuilder();

            var context = _dbContext.GetActiveDbContext();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                var entity = entry.Entity;
                var validationContext = new ValidationContext(entity);
                var validationResults = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(
                    entity,
                    validationContext,
                    validationResults,
                    validateAllProperties: true
                );

                foreach (var validationResult in validationResults)
                {
                    errorMessage.AppendLine($"Error: {validationResult.ErrorMessage}");
                }
            }

            return errorMessage.ToString();
        }
        private IQueryable<TEntity> GetQuery<TKey>(ISpecification<TEntity, TKey> spec)
        {
            var query = spec.Includes.Aggregate(_dbSet.AsQueryable(),
                    (current, include) => current.Include(include));
            query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            if (spec.OrderBy != null)
            {
                query = ApplyOrderedBy<TKey>(spec.OrderBy, spec.OrderByDirection, query);
                if (spec.Skip > 0) query = query.Skip(spec.Skip);
                if (spec.Take > 0) query = query.Take(spec.Take);
            }
            return query.Where(spec.Criteria);
        }
        private IQueryable<TEntity> ApplyOrderedBy<TKey>(Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection, IQueryable<TEntity> query)
        {
            if (orderBy == null) return query;
            if (orderByDirection == OrderByDirection.Ascending)
            {
                query = query.OrderBy(orderBy);
            }
            else
            {
                query = query.OrderByDescending(orderBy);
            }
            return query;
        }
        private List<TEntity> GetFromCache(int tenant)
        {
            List<TEntity> entity = Helpers.CacheManager.CacheWrapper.Get<TEntity>(tenant);
            if (entity == null)
            {
                entity = GetAll(tenant);
                Helpers.CacheManager.CacheWrapper.Insert<TEntity>(tenant, entity);
            }
            return entity;
        }
        private TResult NewObject<TResult>(TEntity entity)
        {
            var type = typeof(TResult);
            return type.GetConstructor(new[] { typeof(TEntity) }) is { } ctor
                ? (TResult)ctor.Invoke(new object[] { entity })
                : type == typeof(TEntity)
                    ? (TResult)(object)entity
                    : throw new Exception($"Type {type.FullName} must have a constructor with {typeof(TEntity).FullName} or be assignable from it.");
        }
        private IQueryable<TEntity> ApplyInclude(Expression<Func<TEntity, bool>> predicate, string include)
        => include.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(_dbSet.Where(predicate), (current, next) => { return current.Include(next); });

        private IContext GetContext(int tenant)=> DbContextBaseUtil.GetContext<TEntity>(tenant);
        private IQueryable<TEntity> ApplyInclude(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        => includes.Aggregate(_dbSet.Where(predicate), (current, include) => current.Include(include));

     
        #endregion  Private Methods 
    }
}