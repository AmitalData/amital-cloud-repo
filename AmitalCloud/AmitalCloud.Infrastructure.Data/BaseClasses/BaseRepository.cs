using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.BaseClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AmitalCloud.Infrastructure.Data.BaseClasses
{
    public abstract class BaseRepository<TEntity, TKeyType, TContext> : Repository<TEntity>, IRepository<TEntity>
        where TEntity : BaseEntity
        where TContext : class, IContext
    {
        private TContext currentContext;
        protected TContext Context => currentContext;
        public BaseRepository(TContext context) : base(context) => currentContext = context;
        public IQueryable<TEntity> GetAll() => Query();
        protected IQueryable<TEntity> Query() => from a in contextEntity select a;
        #region Abstract Methods
        protected abstract DbSet<TEntity> contextEntity { get; }
        #endregion Abstract Methods
    }
}