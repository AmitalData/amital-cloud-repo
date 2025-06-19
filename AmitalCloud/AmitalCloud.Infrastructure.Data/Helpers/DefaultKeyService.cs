using Microsoft.Extensions.Caching.Memory;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using System.Collections.Concurrent;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class DefaultKeyService
    {
        private static readonly IMemoryCache _cache;
        private static readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
        private static readonly ConcurrentBag<string> _cacheKeys = new();

        public static List<DefaultAndConfigurationKey> GetByTenant(int tenant)
        {
            if (_cache.TryGetValue(tenant.ToString(), out List<DefaultAndConfigurationKey> value))
            {
                return value;
            }
            else
            {
                List<DefaultAndConfigurationKey> settings = FetchSettingsByTenant(tenant);
                _cache.Set(tenant.ToString(), settings, DateTimeOffset.Now.Add(_cacheExpiration));
                _cacheKeys.Add(tenant.ToString());
                return settings;
            }
        }

        public static DefaultAndConfigurationKey GetByTenant(int tenant, string key)
        {
            var settings = GetByTenant(tenant);
			return settings.SingleOrDefault(s => string.Equals(s.SetKey, key, StringComparison.OrdinalIgnoreCase));
        }

        public static void ClearCacheByTenant(string tenant)
        {
            if (_cache.TryGetValue(tenant, out List<DefaultAndConfigurationKey> value))
            {
                _cache.Remove(tenant);
            }
        }

        public static void ClearAllCache()
        {
            foreach (var key in _cacheKeys)
            {
                _cache.Remove(key);
            }
            _cacheKeys.Clear();
        }

        private static List<DefaultAndConfigurationKey> FetchSettingsByTenant(int tenant)
        {
            try
            {
                Repository<DefaultAndConfigurationKey> repository = new Repository<DefaultAndConfigurationKey>(AmitalCloudContext.GetContext(tenant));
				List<DefaultAndConfigurationKey> result = repository.GetMulti(a => a.Tenant == tenant);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
