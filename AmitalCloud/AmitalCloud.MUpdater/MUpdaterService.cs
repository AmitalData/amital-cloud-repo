using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using Microsoft.Data.SqlClient;
using AmitalCloud.Infrastructure.MUpdater.Data.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace AmitalCloud.MUpdater
{
    public class MUpdaterService
    {

        public Dictionary<int, string> _globalDBs = new Dictionary<int, string>();

        public void RunModulesUpdate(string moduleName, int? tenantId = null, string[] args = null)
        {
            try
            {
                InitializeSettings(args);
                Dictionary<int, string> processedDBConnections = new Dictionary<int, string>();
                bool multiDB = false;
                if (_globalDBs.Count > 1)
                    multiDB = true;


                foreach (var db in _globalDBs)
                {
                    int currentTenantId = db.Key;
                    string globalDbId = db.Value;

                    if (tenantId.HasValue && currentTenantId != tenantId.Value)
                    {
                        continue;
                    }

                    if (processedDBConnections.ContainsKey(currentTenantId))
                    {
                        continue;
                    }

                    var dbConnection = db.Value;

                    if (processedDBConnections.ContainsValue(dbConnection))
                    {
                        continue;
                    }

                    var moduleToIncule = GetIncludeModules(DatabaseInitializer.GetConnection(dbConnection, dbConnection).ConnectionString);
                   
                     if(moduleToIncule.Include && moduleToIncule.Modules.Any(m => m.Equals(moduleName, StringComparison.OrdinalIgnoreCase)))
                     {
                        Console.WriteLine($"Updating module '{moduleName}' for Tenant {currentTenantId}, DB Connection: {dbConnection}");
                        TenantsUpdateClass.UpdateDataForTenant(currentTenantId, moduleName.ToLower(), false, multiDB);
                     }
                     else
                     {
                        continue;
                     }

                    Console.WriteLine($"Building Object table zip files for DB Connection: {dbConnection} ...");
                    bool buildCustomsZipFiles = false;
                   
                    TenantsUpdateClass.BuildObjectTablesZipFilesData(buildCustomsZipFiles, currentTenantId);
                    Console.WriteLine($"Building zip files finished for DB Connection: {dbConnection} ...");

                    processedDBConnections.Add(currentTenantId, dbConnection);
                }

                Console.WriteLine("Updating all modules and building zip files finished successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine("Error: " + e.StackTrace);
                Console.WriteLine(e.InnerException?.Message);
                Console.WriteLine("Inner Exception Stack Trace:");
                Console.WriteLine(e.InnerException?.StackTrace);
                Environment.Exit(1);
            }
        }

        public void InitializeSettings(string[] args)
        {
            Console.WriteLine("Initializing Settings ...");
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json");
            ConfigurationHelper.Initialize(builder.Build());

            WebApp.FillAppSettings();
            WebApp.InitInjectionUtil();
            ContainerAccessor.InitContainer();
            Infrastructure.Domain.Interfaces.IRepository<GlobalDB> repository;
            Infrastructure.Model.Interfaces.IAmitalCloudContext context;
            repository = new Repository<GlobalDB>(0);
            var globalTenants = repository.GetMulti(x => x.IsActive && !string.IsNullOrEmpty(x.DBConnection)).ToList();
            foreach (var item in globalTenants)
            {
                _globalDBs.Add(Convert.ToInt32(item.Id), item.DBConnection);
            }
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            CacheManager.CacheWrapper = new CacheWrapper(memoryCache);
        }

        private IncludedModulesClass GetIncludeModules(string connectionString)
        {
            string queryString = "SELECT * FROM [dbo].[DBMigrationSettings]";
            IncludedModulesClass? includedModules = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var modeObj = reader["Mode"];
                                var modulesListObj = reader["ModulesList"];

                                if (modeObj != DBNull.Value && modulesListObj != DBNull.Value)
                                {
                                    includedModules = new IncludedModulesClass
                                    {
                                        Include = modeObj.ToString().ToLower() == "include",
                                        Modules = modulesListObj.ToString().ToLower().Split(',').ToList()
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"שגיאה בקריאת מסד הנתונים: {ex.Message}");
            }

            return includedModules;
        }
 
        public class IncludedModulesClass
        {
            public bool Include { get; set; }

            public List<string> Modules { get; set; }
        }
    }
}


