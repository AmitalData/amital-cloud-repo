using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class UserLastSettingsQuery
    {
        IRepository<UserLastSettings> repository;
        IAmitalCloudContext context;


        public UserLastSettingsQuery(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public UserLastSettingsQuery(IAmitalCloudContext context) : this(new Repository<UserLastSettings>(context))
        {
            this.context = context;
        }
        public UserLastSettingsQuery(IRepository<UserLastSettings> repository)
        {
            this.repository = repository;
        }
        public UserLastSettingsPM GetSinglePM(string id, int tenant)
        {
            UserLastSettingsPM entity = (from a in context.UserLastSettings
                                         where a.Tenant == tenant
                                         && a.Id == id
                                         select new UserLastSettingsPM()
                                         {
                                             UserId = a.UserId,
                                             Tenant = a.Tenant,
                                             Id = a.Id,
                                             ControlNameSpace = a.ControlNameSpace,
                                             FilterName = a.FilterName,
                                             FilterValue = a.FilterValue
                                         }).FirstOrDefault();
            return entity;
        }
        public UserLastSettingsPM GetSinglePM(string userId, int tenant, string FilterName)
        {
            UserLastSettingsPM entity = (from a in context.UserLastSettings
                                         where a.Tenant == tenant
                                         && a.UserId == userId && a.FilterName == FilterName
                                         select new UserLastSettingsPM()
                                         {
                                             UserId = a.UserId,
                                             Tenant = a.Tenant,
                                             Id = a.Id,
                                             ControlNameSpace = a.ControlNameSpace,
                                             FilterName = a.FilterName,
                                             FilterValue = a.FilterValue
                                         }).FirstOrDefault();
            return entity;
        }
        public List<UserLastSettingsPM> GetAllUserSettingsPM(string userId, int tenant, string regionnamespace)
        {
            List<UserLastSettingsPM> entity = (from a in context.UserLastSettings
                                               where a.Tenant == tenant
                                               && a.UserId == userId && a.ControlNameSpace == regionnamespace
                                               select new UserLastSettingsPM()
                                               {
                                                   UserId = a.UserId,
                                                   Tenant = a.Tenant,
                                                   Id = a.Id,
                                                   ControlNameSpace = a.ControlNameSpace,
                                                   FilterName = a.FilterName,
                                                   FilterValue = a.FilterValue
                                               }).ToList();
            return entity;
        }
        public IQueryable<UserLastSettingsPM> GetUserLastSettingsPMsByTenant(int tenant)
        {
            IQueryable<UserLastSettingsPM> UserLastSettingsPMs = from a in context.UserLastSettings
                                                                 where a.Tenant == tenant
                                                                 select new UserLastSettingsPM()
                                                                 {
                                                                     UserId = a.UserId,
                                                                     Tenant = a.Tenant,
                                                                     Id = a.Id,
                                                                     ControlNameSpace = a.ControlNameSpace,
                                                                     FilterName = a.FilterName,
                                                                     FilterValue = a.FilterValue
                                                                 };
            return UserLastSettingsPMs;
        }
        public IQueryable<UserLastSettingsPM> GetUserLastSettingsPMsByUserIds(List<string> userIds)
        {
            IQueryable<UserLastSettingsPM> UserLastSettingsPMs = from a in context.UserLastSettings
                                                                 where userIds.Contains(a.Id)
                                                                 select new UserLastSettingsPM()
                                                                 {
                                                                     UserId = a.UserId,
                                                                     Tenant = a.Tenant,
                                                                     Id = a.Id,
                                                                     ControlNameSpace = a.ControlNameSpace,
                                                                     FilterName = a.FilterName,
                                                                     FilterValue = a.FilterValue
                                                                 };
            return UserLastSettingsPMs;
        }
    }
}