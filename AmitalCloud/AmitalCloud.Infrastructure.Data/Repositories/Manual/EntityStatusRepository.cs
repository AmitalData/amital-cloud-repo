using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;


namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class EntityStatusRepository : Repository<EntityStatus>, IRepository<EntityStatus>
    {
        IAmitalCloudContext currentContext;

        public EntityStatusRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }
        public EntityStatusRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))

        {
            currentContext = AmitalCloudContext.GetContext(tenant);
        }
        public EntityStatusRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public EntityStatus GetSingleEntityStatus(string id, int tenant)
        {

            EntityStatus entity = (from a in context.EntityStatus.Include("ObjectTable").Include("EntityStatusType")
                                   where a.Tenant == tenant && a.Id == id
                                   select a).FirstOrDefault(); ;

            return entity;


        }





        public EntityStatus GetSingleEntityStatusByCode(string code, int tenant)
        {
            EntityStatus entity = (from a in context.EntityStatus
                                   where a.Tenant == tenant && a.Code == code
                                   select a).FirstOrDefault();

            return entity;
        }

        public EntityStatus GetSingleEntityStatusByCodeTableId(string code, string objectTableId, int tenant)
        {
            EntityStatus entity = (from a in context.EntityStatus
                                   where a.Tenant == tenant && a.Code == code && a.ObjectTableId == objectTableId
                                   select a).FirstOrDefault();

            return entity;
        }


        public static EntityStatus GetSingleEntityStatusByCode(string code, int tenant, bool getFromCache)
        {
            if (!string.IsNullOrEmpty(code))
            {
                string entityName = "EntityStatus" + code + tenant;
                EntityStatus entity = null;
                if (getFromCache)
                {

                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        var entitystatuses = (from a in context.EntityStatus
                                              where a.Tenant == tenant
                                              select a);
                        foreach (var s in entitystatuses)
                        {
                            string name = "EntityStatus" + s.Code + tenant;
                            if (CacheManager.CacheWrapper.Get(name) == null)
                            {
                                CacheManager.CacheWrapper.Insert(name, s, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                            }
                        }
                        entity = (EntityStatus)CacheManager.CacheWrapper.Get(entityName);

                        //EntityStatus status = (from a in context.EntityStatus
                        //                         where a.Tenant == tenant && a.Id == id
                        //                         select a).FirstOrDefault();

                        //Entity = status;
                        //if (HttpContext.Current.Cache.Get(EntityName) == null)
                        //{
                        //    HttpContext.Current.Cache.Insert(EntityName, Entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        //}
                    }
                    else
                    {
                        entity = (EntityStatus)CacheManager.CacheWrapper.Get(entityName);
                        // HttpContext.Current.Cache.Insert(EntityNameValue, Entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }


                }
                else
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    EntityStatus status = (from a in context.EntityStatus
                                           where a.Tenant == tenant && a.Code == code
                                           select a).FirstOrDefault();

                    entity = status;

                }
                return entity;
            }
            return null;

        }

        public static EntityStatus GetSingleEntityStatus(string id, int tenant, bool getFromCache)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string entityName = "EntityStatus" + id + tenant;
                EntityStatus entity = null;
                if (getFromCache)
                {

                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        var entitystatuses = (from a in context.EntityStatus
                                              where a.Tenant == tenant
                                              select a);
                        foreach (var s in entitystatuses)
                        {
                            string name = "EntityStatus" + s.Id + tenant;
                            if (CacheManager.CacheWrapper.Get(name) == null)
                            {
                                CacheManager.CacheWrapper.Insert(name, s, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                            }
                        }
                        entity = (EntityStatus)CacheManager.CacheWrapper.Get(entityName);

                        //EntityStatus status = (from a in context.EntityStatus
                        //                         where a.Tenant == tenant && a.Id == id
                        //                         select a).FirstOrDefault();

                        //Entity = status;
                        //if (HttpContext.Current.Cache.Get(EntityName) == null)
                        //{
                        //    HttpContext.Current.Cache.Insert(EntityName, Entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        //}
                    }
                    else
                    {
                        entity = (EntityStatus)CacheManager.CacheWrapper.Get(entityName);
                        // HttpContext.Current.Cache.Insert(EntityNameValue, Entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }


                }
                else
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    EntityStatus status = (from a in context.EntityStatus
                                           where a.Tenant == tenant && a.Id == id
                                           select a).FirstOrDefault();

                    entity = status;

                }
                return entity;
            }
            return null;

        }

        public EntityStatus GetSingleEntityStatusByName(string name, int tenant, string objectTableId)
        {

            return (from a in context.EntityStatus
                    where a.Tenant == tenant && a.Name == name && a.ObjectTableId == objectTableId
                    select a).FirstOrDefault();
        }




        public string GetEntityStatusArrivals(int tenant)
        {
            return (from a in context.EntityStatus
                    where a.Tenant == tenant && a.Code == "SARR"
                    select a.Id).FirstOrDefault();
        }



        public IQueryable<EntityStatus> GetEntityStatusByTenant(int tenant)
        {
            return (from a in context.EntityStatus
                    where a.Tenant == tenant
                    select a);
        }


        public IQueryable<EntityStatus> GetEntityStatus(int tenant)
        {
            return (from a in context.EntityStatus.Include("ObjectTable")
                    where a.Tenant == tenant && !a.InActive
                    select a);
        }




        public IQueryable<EntityStatus> GetEntityStatusByTenantAndObjectTableId(int tenant, string objectTableId)
        {
            return (from a in context.EntityStatus
                    where a.Tenant == tenant && a.ObjectTableId == objectTableId
                    select a);
        }

        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }
    }

}
