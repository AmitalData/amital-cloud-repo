using Microsoft.Extensions.Configuration;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public static class ConfigurationHelper
    {
        private static IConfiguration _configuration;
        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetValue(string key)
        {
            return _configuration[key];
        }

        public static string GetConnectionString(string key)
        {
            return _configuration.GetConnectionString(key);
        }
        public static IConfiguration Conf => _configuration;
    }
}
