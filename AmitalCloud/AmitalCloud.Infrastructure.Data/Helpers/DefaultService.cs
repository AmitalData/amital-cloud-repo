using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class DefaultService : BaseClasses.BaseInstance<DefaultService>
    {
        private int expirationTime = int.TryParse(Environment.GetEnvironmentVariable("CacheDefaultExpirationTime"), out var t) ? t : 30;
		private const string CachePrefix = "SettingsHelper_{0}";
        public List<DefaultAndConfiguration_Ext> Get(int tenant)
        {
            string cacheKey = GetCacheKey(tenant);
            return GetFromCache(cacheKey, () => FetchByTenant(tenant));
        }

        public List<DefaultAndConfiguration_Ext> Get(int tenant, string setKey)
        {
            string cacheKey = GetCacheKey(tenant, setKey);
            return GetFromCache(cacheKey, () => Fetch(tenant, setKey));
        }

        public DefaultAndConfiguration_Ext Get(int tenant, string setKey, string additionalKey)
        {
            string cacheKey = GetCacheKey(tenant, setKey, additionalKey);
            return GetFromCache(cacheKey, () => Fetch(tenant, setKey, additionalKey));
        }

        private T GetFromCache<T>(string cacheKey, Func<T> func) where T : class
        {
            if (CacheManager.CacheWrapper == null)
            {
                // todo
                //CacheManager.CacheWrapper = new CacheWrapper(HttpRuntime.Cache);
            }
            return CacheManager.GetOrInsertNewObject(
                cacheKey,
                func,
                fromCache: true,
                donotCacheNull: false,
                supressForceInsert: true,
                absoluteExpiration: expirationTime
            );
        }

        private DefaultAndConfiguration_Ext Fetch(int tenant, string setKey, string additionalKey)
        {
            List<DefaultAndConfiguration_Ext> defaultList = Get(tenant, setKey);
            DefaultAndConfiguration_Ext res = defaultList.FirstOrDefault(d => d.AdditionalKey == additionalKey);

            if (res != null)
                return res;

            foreach (var item in additionalKey.Split('.'))
            {
                if (additionalKey.Length == item.Length)
                    break;

                additionalKey = additionalKey.Substring(0, additionalKey.Length - item.Length - 1);

                res = defaultList.FirstOrDefault(d => d.AdditionalKey == additionalKey);

                if (res != null)
                    return res;
            }

            return null;
        }

        private List<DefaultAndConfiguration_Ext> Fetch(int tenant, string setKey) =>
            Get(tenant).Where(s => s.SetKey == setKey).ToList();

		private List<DefaultAndConfiguration_Ext> FetchByTenant(int tenant)
        {
            Repository<DefaultAndConfiguration> repository = new Repository<DefaultAndConfiguration>(AmitalCloudContext.GetContext(tenant));
			List<DefaultAndConfiguration_Ext> result = (tenant == 0 ? repository.GetQueryable() : repository.GetMulti(a => a.Tenant == tenant || (a.Tenant == 0 && a.AllowInheritance)).AsQueryable()).Select(a => new DefaultAndConfiguration_Ext(a)).ToList();
            return result;
        }

        private string GetCacheKey(int tenant, string setKey = null, string additionalKey = null)
        {
            string key = string.Format(CachePrefix, tenant);

            if (!string.IsNullOrEmpty(setKey))
                key += ";" + setKey;

            if (!string.IsNullOrEmpty(additionalKey))
                key += ";" + additionalKey;

            return key;
        }

        public void ClearCache()
        {
            var allCacheItems = CacheManager.GetAllCacheKeys();
            string cachePrefix = string.Format(CachePrefix, string.Empty);

            foreach (var key in allCacheItems)
                if (key.StartsWith(cachePrefix))
                    CacheManager.ClearCacheItems(k => k == key);
        }

        public void ClearCache(int tenant, string setKey = null, string additionalKey = null)
        {
            string cacheKey = GetCacheKey(tenant);
            CacheManager.ClearCacheItems(key => key == cacheKey);

            if (!string.IsNullOrEmpty(setKey))
            {
                cacheKey = GetCacheKey(tenant, setKey);
                CacheManager.ClearCacheItems(key => key == cacheKey);
            }

            if (!string.IsNullOrEmpty(additionalKey))
            {
                cacheKey = GetCacheKey(tenant, setKey, additionalKey);
                CacheManager.ClearCacheItems(key => key == cacheKey);
            }
        }
    }
}
