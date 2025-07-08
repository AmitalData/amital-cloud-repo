using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class ObjectTableQuery
    {
        ObjectTableRepository repository;
        private IMapper mapper;
        public ObjectTableQuery(int tenant)
        {
            repository = new ObjectTableRepository(tenant);
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new ObjectTableDataMapping()));
            mapper = config.CreateMapper();
        }
        public IQueryable<ObjectTablePM> GetObjectPMsByTenant(int tenant)
        {
            List<ObjectTablePM> currentObjectTables = new List<ObjectTablePM>();
            List<ObjectTablePM> zeroObjectTables = new List<ObjectTablePM>();

            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                var currentObjectTablesPoco = repository.GetMulti(a => a.Tenant == tenant, a => a, "FullNameTextCode");
                currentObjectTables = mapper.Map<List<ObjectTablePM>>(currentObjectTablesPoco);
            }
            if (tenant != 0)
            {
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    zeroObjectTables = GetTenantZeroObjectTables();
                }
            }
            return currentObjectTables.Concat(zeroObjectTables).AsQueryable<ObjectTablePM>();
        }
        private List<ObjectTablePM> GetTenantZeroObjectTables()
        {
            string tenantZeroObjectTablesCacheKeyName = "tenantZeroObjectTables";
            if (HttpContextHelper.HttpContext != null && CacheManager.CacheWrapper.Get(tenantZeroObjectTablesCacheKeyName) != null)
            {
                return (List<ObjectTablePM>)CacheManager.CacheWrapper.Get(tenantZeroObjectTablesCacheKeyName);
            }
            var zeroObjectTablesPoco = repository.GetMulti(a => a.Tenant == 0, a => a, "FullNameTextCode").ToList();
            var zeroObjectTables = mapper.Map<List<ObjectTablePM>>(zeroObjectTablesPoco);

            CacheManager.CacheWrapper.Insert(tenantZeroObjectTablesCacheKeyName, zeroObjectTables, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
            return zeroObjectTables;
        }
        public static List<ObjectTablePM> GetObjectTablesWithTenantZero(int tenant)
        {
            string entityKeyString = $"GetObjectTablesWithTenantZero({tenant})";
            List<ObjectTablePM> myres = CacheManager.GetOrInsertNewObject<List<ObjectTablePM>>(entityKeyString, () =>
            {
                return GetObjectTablesWithTenantZeroBadCache(tenant);
            });
            return myres;
        }
        static List<ObjectTablePM> GetObjectTablesWithTenantZeroBadCache(int tenant)
        {
            string listName = "tenantzerotextobjecttablepms";
            string tenantListName = "tenantobjecttablepms" + tenant;
            List<ObjectTablePM> result = new List<ObjectTablePM>();
            List<ObjectTablePM> currentTenantTables = new List<ObjectTablePM>();
            List<ObjectTablePM> zeroTenantTables = new List<ObjectTablePM>();
            if (tenant != 0)
            {
                if (HttpContextHelper.HttpContext != null)
                {
                    currentTenantTables = (List<ObjectTablePM>)CacheManager.CacheWrapper.Get(tenantListName);
                    if (currentTenantTables == null)
                    {
                        currentTenantTables = GetCurrentTenantTables(tenant);
                        CacheManager.CacheWrapper.Insert(tenantListName, currentTenantTables, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }
                }
                else
                {
                    currentTenantTables = GetCurrentTenantTables(tenant);
                }
            }

            var config = new MapperConfiguration(cfg => cfg.AddProfile(new ObjectTableDataMapping()));
            var mapper = config.CreateMapper();

            if (HttpContextHelper.HttpContext != null)
            {
                zeroTenantTables = (List<ObjectTablePM>)CacheManager.CacheWrapper.Get(listName);

                if (zeroTenantTables == null)
                {
                    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        var zeroTenantTablesPoco = (from a in context.ObjectTables.Include("HeaderScreen").Include("DescriptionTextCode").Include("NewButtonTextCode").Include("FullNameTextCode")
                                            where (a.Tenant == 0 && a.InActive == false)
                                            select a).ToList();
                        zeroTenantTables = mapper.Map<List<ObjectTablePM>>(zeroTenantTablesPoco);

                        scope.Complete();
                    }
                    CacheManager.CacheWrapper.Insert(listName, zeroTenantTables, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                }

            }
            else
            {
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    var zeroTenantTablesPoco = (from a in context.ObjectTables.Include("HeaderScreen").Include("DescriptionTextCode").Include("NewButtonTextCode").Include("FullNameTextCode")
                                        where (a.Tenant == 0 && a.InActive == false)
                                        select a).ToList();
                    zeroTenantTables = mapper.Map<List<ObjectTablePM>>(zeroTenantTablesPoco);

                    scope.Complete();
                }

            }
            zeroTenantTables = zeroTenantTables == null ? new List<ObjectTablePM>() : zeroTenantTables;
            currentTenantTables = currentTenantTables == null ? new List<ObjectTablePM>() : currentTenantTables;

            result = zeroTenantTables.Concat(currentTenantTables).ToList();

            return result;
        }
        private static List<ObjectTablePM> GetCurrentTenantTables(int tenant)
        {
            List<ObjectTablePM> currentTenantTables;
            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                var currentTenantTablesPoco = new ObjectTableRepository(tenant).GetMulti(a => a.Tenant == tenant && a.InActive == false, a => a, "FullNameTextCode").ToList();

                var config = new MapperConfiguration(cfg => cfg.AddProfile(new ObjectTableDataMapping()));
                var mapper = config.CreateMapper();
                currentTenantTables = mapper.Map<List<ObjectTablePM>>(currentTenantTablesPoco);

                scope.Complete();
            }

            return currentTenantTables;
        }
        public static ObjectTablePM GetObjectTableByCode(string name, int tenant)
        {
            ObjectTablePM table = null;
            if (!string.IsNullOrEmpty(name))
            {
                table = GetObjectTablesWithTenantZero(tenant).Where(t => t.Name.ToLower() == name.ToLower()).FirstOrDefault();
            }

            return table;
        }
        public string GetObjectTableIdByName(string tableName)
        {
            return repository.GetObjectTableIdByName(tableName);
        }
    }
}
