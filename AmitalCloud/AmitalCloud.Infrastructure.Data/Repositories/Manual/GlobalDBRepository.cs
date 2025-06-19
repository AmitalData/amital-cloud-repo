using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.Extensions.Configuration;
namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class GlobalDBRepository : Repository<GlobalDB>
    {
        private readonly IConfiguration _configuration;
        IGlobalContext globalContext;
        #region Constructors
        public GlobalDBRepository(IGlobalContext context, IConfiguration configuration) : base(context)
        {
            globalContext = context;
            _configuration = configuration;
        }
        public GlobalDBRepository(IConfiguration configuration) : this(GlobalContext.GetContext(), configuration)
        {
        }
        #endregion Constructors

        public GlobalDB GetGlobalDBById(string id) => GetSingleGlobalDB(id);
        public GlobalDB GetSingleGlobalDB(string id) => _configuration["ENVIROMENT"] == "azure app service" ? GetGlobalDbFromEnviroment() : GetAll(0, true).Where(a => a.Id == id).FirstOrDefault();
        public int GetDataBasesCount() => GetAll(0, true).Count();
        public List<GlobalDB> GetActiveDataBases() => GetAll(0, true).Where(a => a.IsActive == true).ToList();
        public IQueryable<GlobalDB> GetGlobalDBs() => context.GlobalDBs;
        public IGlobalContext context
        {
            get { return globalContext == null ? GlobalContext.GetContext() : globalContext; }
        }
        public GlobalDB GetGlobalDBByTenant(int tenant) => _configuration["ENVIROMENT"] == "azure app service" ? GetGlobalDbFromEnviroment() : GetByGlobalTenant(tenant);
        private static GlobalDB GetByGlobalTenant(int tenant)
        {
            IGlobalContext context = (IGlobalContext)CacheManager.CacheWrapper.Get("globalContext");
            if (context == null)
            {
                context = GlobalContext.GetContext();
                CacheManager.CacheWrapper.Insert("globalContext", context, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
            }
            GlobalTenant globaltenant = new Repository<GlobalTenant>(context).GetAll(0, true).Where(a => a.Id == tenant).FirstOrDefault();
            string usedTenant = globaltenant != null ? globaltenant.GlobalDBId : tenant.ToString();
            return new Repository<GlobalDB>(context).GetAll(0, true).Where(a => a.Id == usedTenant).FirstOrDefault();
        }
        private GlobalDB GetGlobalDbFromEnviroment()
        {

            return new GlobalDB()
            {
                Id = SettingUtil.GetCurrentTenant().ToString(),
                DBConnection = _configuration.GetConnectionString("SystemMainStr"),
                IsUpgrading = false,
                IsActive = true,
                SharedDWConnection = null,
                SecondaryAzureDBConnection = null,
                IsBlocking = false
            };
        }
    }
}