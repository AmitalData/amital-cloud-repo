using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using System;
using System.Linq;
using System.Web;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class UserLoginLogQuery
    {
        UserLoginLogRepository repository;


        public UserLoginLogQuery(int tenant) : this(new UserLoginLogRepository(tenant))
        {
        }
        public UserLoginLogQuery(UserLoginLogRepository repository)
        {
            this.repository = repository;
        }
        public UserLoginLogPM GetSinglePM(string id, int tenant)
        {
            string entityName = "UserLoginLogPM" + id + tenant;
            UserLoginLogPM entity;

            if (HttpContextHelper.HttpContext != null)
            {
                if (CacheManager.CacheWrapper.Get(entityName) == null)
                {
                    entity = entity = (from a in repository.context.UserLoginLogs
                                       where a.Tenant == tenant
                                       && a.Id == id
                                       select new UserLoginLogPM()
                                       {
                                           Id = a.Id,
                                           UserId = a.UserId,
                                           Browser = a.Browser,
                                           IP = a.IP,
                                           Tenant = a.Tenant,
                                           GMTDateTime = a.GMTDateTime,
                                           LocalDateTime = a.GMTDateTime,
                                           UserAgent = a.UserAgent,
                                           ComputerId = a.ComputerId,
                                       }).FirstOrDefault();

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
                    entity = (UserLoginLogPM)CacheManager.CacheWrapper.Get(entityName);
                }

            }
            else
            {
                entity = (from a in repository.context.UserLoginLogs
                          where a.Tenant == tenant
                          && a.Id == id
                          select new UserLoginLogPM()
                          {
                              Id = a.Id,
                              UserId = a.UserId,
                              Browser = a.Browser,
                              IP = a.IP,
                              Tenant = a.Tenant,
                              GMTDateTime = a.GMTDateTime,
                              LocalDateTime = a.GMTDateTime,
                              UserAgent = a.UserAgent,
                              ComputerId = a.ComputerId,
                          }).FirstOrDefault();
            }

            return entity;
        }

        public IQueryable<UserLoginLogPM> GetUserLoginLogPMsByTenant(int tenant)
        {
            IQueryable<UserLoginLogPM> userLoginLogs = from a in repository.context.UserLoginLogs
                                                       where a.Tenant == tenant
                                                       select new UserLoginLogPM()
                                                       {
                                                           Id = a.Id,
                                                           UserId = a.UserId,
                                                           Browser = a.Browser,
                                                           IP = a.IP,
                                                           Tenant = a.Tenant,
                                                           GMTDateTime = a.GMTDateTime,
                                                           LocalDateTime = a.GMTDateTime,
                                                           UserAgent = a.UserAgent,
                                                           ComputerId = a.ComputerId,
                                                       };
            return userLoginLogs;
        }



        public IQueryable<UserLoginLogList> GetUserLoginLogListsByTenant(string userId, int tenant)
        {
            IQueryable<UserLoginLogList> userLoginLogs = from a in repository.context.UserLoginLogs
                                                         where a.Tenant == tenant && a.UserId == userId
                                                         select new UserLoginLogList()
                                                         {
                                                             Id = a.Id,

                                                             Browser = a.Browser,
                                                             IP = a.IP,
                                                             GMTDateTime = a.GMTDateTime,
                                                             LocalDateTime = a.LocalDateTime,
                                                             //IPSiteUri = "http://www.infosniper.net/index.php?ip_address=" + a.IP,

                                                         };
            return userLoginLogs;
        }

    }



}