using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.BaseClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq.Expressions;

namespace AmitalCloud.Infrastructure.Application.BaseClasses
{
    public class BaseEntityQueryService<TEntityPOCO, TEntityKeys, TEntityPM, TEntityList, TkeyType> : IBaseEntityQueryService<TEntityPM, TEntityPOCO> where TEntityPOCO : BaseEntity, new()
    where TEntityPM : IEntityPM, new()
    where TEntityKeys : IEntityKeyFields<TEntityPOCO, TkeyType>, new()
    where TEntityList : class, new()
    {
        protected int Tenant;
        protected TEntityPOCO EntityPOCO { get; set; }
        protected IMapping<TEntityPM, TEntityPOCO, TEntityList> mapping;
        protected TEntityPM EntityPM;
        protected IRepository<TEntityPOCO> Repository;
        protected IContext MainContext;
        protected TEntityKeys EntityKeys;
        protected IMapper mapper;

        public BaseEntityQueryService(IRepository<TEntityPOCO> repository, IMapping<TEntityPM, TEntityPOCO, TEntityList> mapping)
        {
            this.Repository = repository;
            this.mapping = mapping;
            this.InitializeSettings();

            var config = new MapperConfiguration(cfg => cfg.AddProfile((Profile)mapping), new NullLoggerFactory());
            mapper = config.CreateMapper();
        }
        public TEntityPM GetSingle(TEntityKeys entityKeys, bool getComposition, bool getFromCache)
        {
            if (getFromCache && (CacheManager.CacheWrapper != null))
            {
                string cacheKey = $"TEntityPMGetSingle_({entityKeys.GetEntityPMName()}_{entityKeys.GetFullKey()}_{getComposition})";
                var cacheObj = CacheManager.CacheWrapper.Get(cacheKey, Tenant);
                if (cacheObj != null)
                {
                    if (IsNullCache(cacheObj))
                    {
                        EntityPM = default(TEntityPM);
                    }
                    else
                    {
                        EntityPM = (TEntityPM)cacheObj;
                    }
                }
                else
                {
                    EntityPOCO = Repository.GetSingle(entityKeys);
                    if (EntityPOCO != null)
                    {
                        EntityPM = GetEntityPM(EntityPOCO, getComposition, entityKeys);
                    }
                    else
                    {
                        EntityPM = default(TEntityPM);
                    }
                    if (EntityPM != null)
                    {
                        CacheManager.CacheWrapper.Insert(cacheKey, EntityPM);
                    }
                    else
                    {
                        CacheManager.CacheWrapper.Insert(cacheKey, new NullCache());
                    }
                }
            }
            else
            {
                EntityPOCO = Repository.GetSingle(entityKeys);
                EntityPM = EntityPOCO != null ? GetEntityPM(EntityPOCO, getComposition, entityKeys) : default;                
            }
            return EntityPM;
        }
        public TEntityPM GetSingle(IEnumerable<KeyValuePair<string, string>> paramList, bool getComposition, bool getFromCache)
        {
            var keys = new TEntityKeys();
            keys.Initialize(paramList);
            return GetSingle(keys, getComposition, getFromCache);
        }
        public TEntityPM GetEntityPM(TEntityPOCO entityPOCO, bool getComposition = false, IEntityKeyFields<TEntityPOCO, TkeyType> entityKeys = null)
        {
            var entityPM = new TEntityPM();
            if (entityPOCO == null)
            {
                return default(TEntityPM);
            }

            entityPM = mapper.Map<TEntityPM>(entityPOCO);

            if (getComposition && entityKeys != null)
            {
                GetComposition(entityKeys, entityPM);
            }
            return entityPM;
        }
        public virtual void GetComposition(IEntityKeyFields<TEntityPOCO, TkeyType> entityKeys, TEntityPM entityPM)
        {
        }
        protected virtual  IEntityKeyFields<TEntityPOCO, TkeyType> GetKeys(TEntityPOCO entityPOCO) { return null; }
        public virtual void InitializeSettings()
        {

        }
        public List<TEntityPM> GetMultiByParent<TEntityParentKeys>(TEntityParentKeys entityParentKeys, bool getFromCache, bool getComposition = true)
        {
            List<TEntityPOCO> entityPOCOs = Repository.GetMultiByParent<TEntityParentKeys>(entityParentKeys);
            List<TEntityPM> entityPMs = new List<TEntityPM>();
            foreach (TEntityPOCO entityPOCO in entityPOCOs)
            {
                TEntityPM entityPM = new TEntityPM();
                if (entityPOCO != null)
                {
                    var entityKeys = GetKeys(entityPOCO);
                    if (entityKeys != null && getComposition)
                    {
                        GetComposition(entityKeys, entityPM);
                    }
                }

                entityPM = mapper.Map<TEntityPM>(entityPOCO);
                entityPMs.Add(entityPM);
            }
            return entityPMs;
        }
        public List<TEntityPM> GetMultiFromCache(string cacheKey, Expression<Func<TEntityPOCO, bool>> predicate, string include = null)
        {
            List<TEntityPM> entityPMs;
            var formattedCacheKey = $"PMCache:{typeof(TEntityPOCO).Name}:{cacheKey}:{include}";

            var cacheObj = CacheManager.CacheWrapper.Get(formattedCacheKey);
            if (cacheObj != null)
            {
                entityPMs = (List<TEntityPM>)cacheObj;
            }
            else
            {
                if (!string.IsNullOrEmpty(include))
                {
                    entityPMs = Repository.GetQueryable().Where(predicate)
                        .ProjectTo<TEntityPM>(mapper.ConfigurationProvider)
                        .ToList();
                }
                else
                {
                    entityPMs = Repository.GetMulti<TEntityPM>(predicate, mapper);
                }

                if (entityPMs != null)
                {
                    CacheManager.CacheWrapper.Insert(formattedCacheKey, entityPMs);
                }
                else
                {
                    CacheManager.CacheWrapper.Insert(formattedCacheKey, new NullCache());
                }
            }
            return entityPMs;
        }
        public TEntityPOCO GetFirst() => Repository.GetFirst();
        public List<TEntityPM> GetMulti(Expression<Func<TEntityPOCO, bool>> predicate)
        {
            return Repository.GetMulti<TEntityPM>(predicate, mapper);
        }
        public List<TEntityPM> GetMulti(Expression<Func<TEntityPOCO, bool>> predicate, Expression<Func<TEntityPOCO, TEntityPM>> select)
        => Repository.GetMulti(predicate,select);
        public List<TEntityPM> GetMulti(Expression<Func<TEntityPOCO, bool>> predicate, Expression<Func<TEntityPOCO, TEntityPM>> select, string include)
        => Repository.GetMulti(predicate, select,include);

        public List<TResult> GetMulti<TResult>(Expression<Func<TEntityPOCO, bool>> predicate, Expression<Func<TEntityPOCO, TResult>> select)
            => Repository.GetMulti(predicate, select);
        public List<TResult> GetMulti<TResult>(Expression<Func<TEntityPOCO, bool>> predicate, Expression<Func<TEntityPOCO, TResult>> select, string include)
        => Repository.GetMulti(predicate, select, include);
        private bool IsNullCache(object obj) => obj is NullCache;
    }
}
