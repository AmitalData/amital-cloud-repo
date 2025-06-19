using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Transactions;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class CacheWrapper : ICacheWrapper
    {
        IMemoryCache cache;
        public CacheWrapper(IMemoryCache cache)
		{
            this.cache = cache;
            List<GlobalTenant> globalTenants = new Repository<GlobalTenant>(GlobalContext.GetContext()).GetAll(0);
            cache.Set(GetCacheKey<GlobalTenant>(null), globalTenants);
        }

        public object Get(string key, int tenant = -1)
        {
            string cacheKey = GetCacheKey(key, tenant);
            if (CacheLogger.IsCacheLoggerEnabled)
                CacheLogger.LogKey(cacheKey);
            return cache.Get(cacheKey);
        }

        public System.Collections.IDictionaryEnumerator GetEnumerator()
        {
            return ((IDictionary)cache).GetEnumerator();
        }

        public void Insert(string key, object value, int tenant = -1)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30)
            };
            cache.Set(GetCacheKey(key, tenant), value, options);
        }

        public void Insert(string key, object value, string dependencies, int tenant = -1)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30)
            };
            cache.Set(GetCacheKey(key, tenant), value, options);
        }

        public void Insert(string key, object value, string dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, int tenant = -1)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };
            if (slidingExpiration > TimeSpan.Zero)
            {
                options.SlidingExpiration = slidingExpiration;
            }
            cache.Set(GetCacheKey(key, tenant), value, options);
        }

        public void Insert(string key, object value, string dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, Action<string> onUpdateCallback, int tenant = -1)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                if (reason == EvictionReason.Expired || reason == EvictionReason.TokenExpired)
                {
                    onUpdateCallback?.Invoke(key.ToString());
                }
            });
            cache.Set(GetCacheKey(key, tenant), value, options);
        }

        public void Insert(string key, object value, string dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, Action<object, EvictionReason> onRemoveCallback, int tenant = -1)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration,
                Priority = priority
            };
            if (slidingExpiration > TimeSpan.Zero)
            {
                options.SlidingExpiration = slidingExpiration;
            }

            if (onRemoveCallback != null)
            {
                options.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    onRemoveCallback(value, reason);
                });
            }

            cache.Set(GetCacheKey(key, tenant), value, options);
        }
        public void Insert<T>(int tenant, List<T> value)
        {
            Insert<T>(tenant, value, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
        }
        public void Insert<T>(int tenant, List<T> value, string dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, Action<object, EvictionReason> onRemoveCallback)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30)
            };
            if (onRemoveCallback != null)
            {
                options.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    onRemoveCallback(value, reason);
                });
            }
            cache.Set(GetCacheKey<T>(tenant), value, options);
        }
        public void Insert<T>(int tenant, List<T> value, string dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, Action<string> onUpdateCallback)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration,
            };
            if (slidingExpiration > TimeSpan.Zero)
            {
                options.SlidingExpiration = slidingExpiration;
            }

            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                if (reason == EvictionReason.Expired || reason == EvictionReason.TokenExpired)
                {
                    onUpdateCallback?.Invoke(key.ToString());
                }
            });

            cache.Set(GetCacheKey<T>(tenant), value, options);
        }
        public void Insert<T>(int tenant, List<T> value, string dependencies)
        {
            Insert<T>(tenant, value, dependencies, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
        }
        public void Insert<T>(int tenant, List<T> value, string dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            Insert<T>(tenant, value, dependencies, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, null);
        }
        public List<T> Get<T>(int tenant)
        {
            return (List<T>)cache.Get(GetCacheKey<T>(tenant));
        }
        public object Invalidate<T>(int tenant)
        {
            return Invalidate(GetCacheKey<T>(tenant), tenant);
        }

        public object Invalidate(string key, int tenant = -1)
        {
            try
            {
                using (TransactionScope scope = TransactionFactory.GetNewSerializableTransaction())//TransactionFactory.GetNewTransaction())
                {
                    Remove(key, tenant);
                    CacheMessageSender.SendMessageToTopic(GetCacheKey(key, tenant));
                    scope.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public object Remove(string key, int tenant = -1)
        {
            var fullKey = GetCacheKey(key, tenant);
            cache.TryGetValue(fullKey, out var value);
            cache.Remove(fullKey);
            return value;
        }

        private string GetCacheKey<T>(int? tenant) => $"Table_({typeof(T).Name}_{tenant})";
        private string GetCacheKey(string key, int tenant) => $"DB_({GetDB(tenant)})_OriginalKey_({key})";
        private string GetDB(int tenant)
        {
            if (tenant == -1)
            {
                if (HttpContextHelper.HttpContext?.Items?.ContainsKey("Tenant") == true)
                {
                    tenant = Convert.ToInt32(HttpContextHelper.HttpContext.Items["Tenant"]);
                }
                else if (HttpContextHelper.HttpContext?.Items?.ContainsKey("authToken") == true)
                {
                    tenant = (HttpContextHelper.HttpContext.Items["authToken"] as AuthenticationToken).Tenant;
                }
                else
                {
                    string? token = HttpContextHelper.HttpContext?.Request?.Headers["Token"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        string cacheKey = $"Token_({token})";
                        var authenticationToken = (AuthenticationToken)cache.Get(cacheKey);
                        if (authenticationToken?.Tenant != null)
                        {
                            tenant = authenticationToken.Tenant;
                        }
                    }
                    if (tenant == -1)
                    {
                        tenant = 0;
                    }
                }
            }
            List<GlobalTenant> globalTenants = (List<GlobalTenant>)cache.Get(GetCacheKey<GlobalTenant>(null));
            return globalTenants.Where(a => a.Id == tenant).FirstOrDefault().GlobalDBId;
        }
    }
}
