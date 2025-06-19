using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.Interfaces;
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
        public ObjectTableQuery(int tenant) => repository = new ObjectTableRepository(tenant);
        public IQueryable<ObjectTablePM> GetObjectPMsByTenant(int tenant)
        {
            List<ObjectTablePM> currentObjectTables = new List<ObjectTablePM>();
            List<ObjectTablePM> zeroObjectTables = new List<ObjectTablePM>();

            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                currentObjectTables = repository.GetMulti(a => a.Tenant == tenant, a => new ObjectTablePM(a)
                {
                    FullNameTextCodeDefaultText = a.FullNameTextCode != null ? a.FullNameTextCode.DefaultText : a.Name,
                }, "FullNameTextCode"); ;
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
            List<ObjectTablePM> zeroObjectTables = repository.GetMulti(a => a.Tenant == 0, a => new ObjectTablePM(a)
            {
                FullNameTextCodeDefaultText = a.FullNameTextCode != null ? a.FullNameTextCode.DefaultText : a.Name,
            }, "FullNameTextCode").ToList();

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

            if (HttpContextHelper.HttpContext != null)
            {
                zeroTenantTables = (List<ObjectTablePM>)CacheManager.CacheWrapper.Get(listName);

                if (zeroTenantTables == null)
                {
                    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        zeroTenantTables = (from a in context.ObjectTables.Include("HeaderScreen").Include("DescriptionTextCode").Include("NewButtonTextCode").Include("FullNameTextCode")
                                            where (a.Tenant == 0 && a.InActive == false)
                                            select new ObjectTablePM(a)
                                            {
                                                FullNameTextCodeDefaultText = a.FullNameTextCode != null ? a.FullNameTextCode.DefaultText : a.Name,
                                            }).ToList();
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
                    zeroTenantTables = (from a in context.ObjectTables.Include("HeaderScreen").Include("DescriptionTextCode").Include("NewButtonTextCode").Include("FullNameTextCode")
                                        where (a.Tenant == 0 && a.InActive == false)
                                        select new ObjectTablePM(a )
                                        {
                                            FullNameTextCodeDefaultText = a.FullNameTextCode != null ? a.FullNameTextCode.DefaultText : a.Name,
                                        }).ToList();


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
                currentTenantTables = new ObjectTableRepository(tenant).GetMulti(a => a.Tenant == tenant && a.InActive == false,
                    a => new ObjectTablePM(a)
                    {
                        FullNameTextCodeDefaultText = a.FullNameTextCode != null ? a.FullNameTextCode.DefaultText : a.Name,
                    }, "FullNameTextCode").ToList();
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
