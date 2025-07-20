using AmitalCloud.Infrastructure.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        void Insert(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
        TEntity GetFirst();
        List<TEntity> GetAll(int tenant);
        List<TEntity> GetAll(int tenant, bool fromCache = true);
        List<TEntity> GetAll<TKey>(int tenant, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending);

        List<TEntity> GetMulti<TKeyType>(IEntityKeyFields<TEntity, TKeyType> entityKeys);
        List<TEntity> GetMulti(Expression<Func<TEntity, bool>> predicate);
        List<TEntity> GetMulti(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select);
        List<TEntity> GetMulti(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select, string include);



        List<TEntity> GetMulti<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, int skip, int take);
        List<TEntity> GetMulti<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending);
        List<TEntity> GetMulti<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select, Expression<Func<TEntity, TKey>> orderBy, int skip, int take);
        List<TEntity> GetMulti<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending);


        List<TResult> GetMulti<TResult>(Expression<Func<TEntity, bool>> predicate);
        public List<TResult> GetMulti<TResult>(Expression<Func<TEntity, bool>> predicate, string include);
        List<TResult> GetMulti<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select);
        List<TResult> GetMulti<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, string include);
        List<TResult> GetMulti<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, params Expression<Func<TEntity, object>>[] includes);

        List<TResult> GetMultiFromCache<TResult>(string cacheKey, Expression<Func<TEntity, bool>> predicate, string include = null, Expression<Func<TEntity, TResult>> select = null);

        List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, int skip, int take);
        List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, string include, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending);

        List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, Expression<Func<TResult, TKey>> orderBy, int skip, int take);
        List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, Expression<Func<TResult, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending);
        List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, string include, Expression<Func<TResult, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending);

        List<TResult> GetMulti<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending);

        IEnumerable<TEntity> GetMulti<TKey>(ISpecification<TEntity, TKey> spec);
        TEntity GetSingle<TKeyType>(IEntityKeyFields<TEntity, TKeyType> entityKeys);
        TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, string include = null);
        TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        TResult GetSingle<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select = null, string include = null);
        List<TEntity> GetMultiByParent<TEntityParentKeys>(TEntityParentKeys entityKeys);
    }

    public interface IAsyncRepository<TEntity> where TEntity : class
    {
        #region Async Methods
        Task<List<TEntity>> GetAllAsync(int tenant);
        Task<List<TEntity>> GetMultiAsync(Expression<Func<TEntity, bool>> predicate);

        Task<List<TResult>> GetMultiAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select);
        Task<List<TEntity>> GetMultiAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select);
        Task<TEntity> GetSingleAsync<TKeyType>(IEntityKeyFields<TEntity, TKeyType> entityKeys);
        Task<List<TEntity>> GetAllAsync(int tenant, bool fromCache = true);
        Task<List<TEntity>> GetAllAsync<TKey>(int tenant, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending);
        Task<List<TEntity>> GetMultiAsync<TKeyType>(IEntityKeyFields<TEntity, TKeyType> entityKeys);
        Task<List<TEntity>> GetMultiAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, int skip, int take);
        Task<IEnumerable<TEntity>> GetMultiAsync<TKey>(ISpecification<TEntity, TKey> spec);
        Task<List<TEntity>> GetMultiAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select, Expression<Func<TEntity, TKey>> orderBy, int skip, int take);
        Task<List<TResult>> GetMultiAsync<TResult, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> select, Expression<Func<TResult, TKey>> orderBy, int skip, int take);

        Task<List<TEntity>> GetMultiAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> select, string include, Expression<Func<TEntity, TKey>> orderBy, OrderByDirection orderByDirection = OrderByDirection.Ascending);


        #endregion Async Methods
    }
}
