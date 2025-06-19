using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using Microsoft.Extensions.Configuration;
using System.Configuration;
namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class GlobalDbHelper
    {
        private readonly GlobalDBRepository _globalDBRepository;
        private readonly IConfiguration _configuration;
        public GlobalDbHelper(IConfiguration configuration)
        {
            _globalDBRepository = new GlobalDBRepository(configuration);
        }
        public GlobalDB GetGlobalDB(int tenant)
        {
            return _globalDBRepository.GetGlobalDBByTenant(tenant);
        }
        private GlobalDB GetGlobalDbFromEnviroment()
        {
            return new GlobalDB()
            {
                Id = "0",
                DBConnection = _configuration.GetConnectionString("SystemMainStr"),
                IsUpgrading = false,
                IsActive = true,
                SharedDWConnection = null,
                SecondaryAzureDBConnection = null,
                IsBlocking = false
            };
        }
        private static GlobalDB GetGlobalDbWithoutProxy(GlobalDB currentDb)
        {
            GlobalDB currentDbCacheWithoutProxy = new GlobalDB()
            {
                DBConnection = currentDb.DBConnection,
                Id = currentDb.Id,
                IsActive = currentDb.IsActive,
                IsUpgrading = currentDb.IsUpgrading,
                SecondaryAzureDBConnection = currentDb.SecondaryAzureDBConnection
            };
            return currentDbCacheWithoutProxy;
        }
        //public static GlobalDB GetGlobalDBById(string id) => new GlobalDBRepository().GetGlobalDBById(id);
    }
}
