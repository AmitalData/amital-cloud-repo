using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class ObjectTableRepository : Repository<ObjectTable>, IObjectTableRepository
    {
        IAmitalCloudContext currentContext;
        public ObjectTableRepository(IUnitOfWork unitOfWork) : this((AmitalCloudContext)unitOfWork.Context)
        {
        }
        public ObjectTableRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }

        public ObjectTableRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public bool IsObjectTableMaster(string objectTableId)
        {
            bool isMaster = (from a in context.ObjectTables
                             where a.Id == objectTableId && a.Name == "Master"
                             select a).Any();
            return isMaster;
        }

        public bool IsObjectTableShipment(string objectTableId)
        {
            bool isMaster = (from a in context.ObjectTables
                             where a.Id == objectTableId && a.Name == "Shipment"
                             select a).Any();
            return isMaster;
        }

        public IQueryable<ObjectTable> GetObjects()
        {
            return context.ObjectTables;
        }

        public ObjectTable GetObjectTableByName(string name, int tenant, bool getFromCache)
        {
            ObjectTable entity = null;
            string entityName = "ObjectTable" + name + tenant;

            if (getFromCache && CacheManager.CacheWrapper.Get(entityName) != null)
            {
                entity = (ObjectTable)CacheManager.CacheWrapper.Get(entityName);
            }

            if (entity == null) 
            {
                entity = GetSingle(d => d.Name == name && (d.Tenant == tenant || d.Tenant == 0));
                if (entity != null)
                {
                    CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                }
            }
            return entity;
        }

        public ObjectTable GetSingleObjectTable(string id, int tenant, bool getFromCache)
        {
            string entityName = "ObjectTable" + id + tenant;
            ObjectTable entity;
            getFromCache = true;

            if (getFromCache)
            {

                if (CacheManager.CacheWrapper.Get(entityName) == null)
                {
                    entity = context.ObjectTables.Where(d => d.Id == id && (d.Tenant == tenant || d.Tenant == 0)).FirstOrDefault();
                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        if (entity != null)
                        {
                            CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                }
                else
                {
                    entity = (ObjectTable)CacheManager.CacheWrapper.Get(entityName);
                }


            }
            else
            {
                entity = context.ObjectTables.Include("FullNameTextCode").Where(d => d.Id == id && (d.Tenant == tenant || d.Tenant == 0)).FirstOrDefault();

            }
            return entity;
        }

        public IQueryable<ObjectTable> GetObjectsByTenant(int tenant)
        {
            IQueryable<ObjectTable> objectTables = from a in context.ObjectTables
                                                   where a.Tenant == tenant || a.Tenant == 0
                                                   select a;
            return objectTables;
        }

        public IQueryable<ObjectTable> GetObjectsByTenantOrTenantZero(int tenant)
        {
            IQueryable<ObjectTable> objectTables = from a in context.ObjectTables
                                                   where a.Tenant == tenant || a.Tenant == 0
                                                   select a;
            return objectTables;
        }

        public ObjectTable GetObjectTableById(string id, int tenant)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string entityName = "ObjectTable" + id + tenant;
                ObjectTable entity;

                if (CacheManager.CacheWrapper != null)
                {
                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        entity = context.ObjectTables.Where(d => d.Id == id && (d.Tenant == tenant || d.Tenant == 0)).FirstOrDefault();
                        if (CacheManager.CacheWrapper.Get(entityName) == null)
                        {
                            if (entity != null)
                            {
                                CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                            }
                        }
                    }
                    else
                    {
                        entity = (ObjectTable)CacheManager.CacheWrapper.Get(entityName);

                    }
                }
                else
                {
                    entity = context.ObjectTables.Where(d => d.Id == id && (d.Tenant == tenant || d.Tenant == 0)).FirstOrDefault();
                }

                return entity;
            }
            return null;
        }

        public ObjectTable GetFirstObjectTable(int tenant)
        {
            return (from a in context.ObjectTables
                    where a.Tenant == tenant || a.Tenant == 0
                    select a).FirstOrDefault();
        }


        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }
        public static string GetObjectTableByName(string objectTableName)
        {
            if (String.IsNullOrWhiteSpace(objectTableName)) return "";//not must 
            var objectTableRepository = new ObjectTableRepository(0); // ObjectTabelRepository tenant must be zero !!
            var objectTable = objectTableRepository.GetObjectTableByName(objectTableName,// "Customs.PhysicalCheck", 
                0, true);
            if (objectTable != null)
                return objectTable.Id;
            return null;
        }

        public static ObjectTable GetSingleObjectTableById(string id, int tenant)
        {
            ObjectTable table = null;
            if (!string.IsNullOrEmpty(id))
            {
                table = GetObjectTablesWithTenantZero(tenant).Where(t => t.Id == id).FirstOrDefault();
            }

            return table;
        }
        public static List<ObjectTable> GetObjectTablesWithTenantZero(int tenant)
        {
            string listName = "tenantzerotextobjecttables";
            string tenantListName = "tenantobjecttables" + tenant;
            List<ObjectTable> result = new List<ObjectTable>();
            List<ObjectTable> currentTenantTables = new List<ObjectTable>();
            List<ObjectTable> zeroTenantTables = new List<ObjectTable>();
            if (tenant != 0)
            {
                if (CacheManager.CacheWrapper.Get(tenantListName) == null)
                {
                    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        currentTenantTables = (from a in context.ObjectTables//.Include("HeaderScreen").Include("DescriptionTextCode").Include("NewButtonTextCode")
                                               where (a.Tenant == tenant && a.InActive == false)
                                               select a).ToList();
                        scope.Complete();
                    }
                    CacheManager.CacheWrapper.Insert(tenantListName, currentTenantTables, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                }
                else
                {
                    currentTenantTables = (List<ObjectTable>)CacheManager.CacheWrapper.Get(tenantListName);
                }
            }
            if (CacheManager.CacheWrapper.Get(listName) == null)
            {
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    zeroTenantTables = (from a in context.ObjectTables//.Include("HeaderScreen").Include("DescriptionTextCode").Include("NewButtonTextCode")
                                        where (a.Tenant == 0 && a.InActive == false)
                                        select a).ToList();
                    scope.Complete();
                }
                CacheManager.CacheWrapper.Insert(listName, zeroTenantTables, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
            }
            else
            {
                zeroTenantTables = (List<ObjectTable>)CacheManager.CacheWrapper.Get(listName);
            }
            zeroTenantTables = zeroTenantTables == null ? new List<ObjectTable>() : zeroTenantTables;
            currentTenantTables = currentTenantTables == null ? new List<ObjectTable>() : currentTenantTables;
            result = zeroTenantTables.Concat(currentTenantTables).ToList();
            return result;
        }
        public string GetObjectTableIdByName(string tablename)
        {
            return (from a in context.ObjectTables
                    where a.Name == tablename
                    select a.Id).FirstOrDefault();
        }
        public string GetObjectTableIdByName(string tablename, int tenant)
        {
            return (from a in context.ObjectTables
                    where a.Name == tablename && (a.Tenant == tenant || a.Tenant == 0)
                    select a.Id).FirstOrDefault();
        }
        public List<ObjectTable> GetAllCacheOnClient(int tenant)
        {
            IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
            var q = (from a in context.ObjectTables//.Include("HeaderScreen").Include("DescriptionTextCode").Include("NewButtonTextCode")
                     where (a.Tenant == tenant && a.InActive == false && a.CacheOnClient == true)
                     select a);
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                q = q
                    .Where(r => r.Name != null)
                    .Where(r => r.Name.StartsWith("Customs."));
            }
            var currentTenantTables = q.ToList();
            return currentTenantTables;
        }
        public static string GetNameById(string id, int tenant)
        {
            IAmitalCloudContext currentContext = AmitalCloudContext.GetContext(tenant);
            return (from a in currentContext.ObjectTables
                    where a.Id == id
                    select a.Name).FirstOrDefault();
        }
        public static bool IsApplyGenericCustomFields(string name, int tenant)
        {
            IAmitalCloudContext currentContext = AmitalCloudContext.GetContext(tenant);
            return (from a in currentContext.ObjectTables
                    where a.Name == name && !a.IsCustom
                    select a.ApplyGenericCustomFields).FirstOrDefault();
        }
    }
}
