using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class UserLastLoginQuery
    {
        private readonly Repository<UserLastLogin> repository;

        public UserLastLoginQuery(int tenant) : this(AmitalCloudContext.GetContext(tenant)) { }
        public UserLastLoginQuery(IAmitalCloudContext context) : this(new Repository<UserLastLogin>(context)) { }
        public UserLastLoginQuery(Repository<UserLastLogin> repository)
        {
            this.repository = repository;
        }

        public UserLastLogin UpdateUserLastLogins(UserLastLogin entity, string workEnvironment)
        {
            using (TransactionScope scope = TransactionFactory.GetTransaction())
            {
                bool isDSVMobileCall = false;
                using (TransactionScope globalScope = TransactionFactory.GetNewTransaction())
                {
                    string Url = HttpContextHelper.HttpContext?.Request?.Headers["Referer"].ToString();
                    string privateLabelId = new Repository<GlobalTenant>(GlobalContext.GetContext()).GetSingle(a => a.Id == entity.Tenant).PrivateLabelId;
                    // todo
                    // isDSVMobileCall = !string.IsNullOrEmpty(privateLabelId) && HttpContextHelper.Request.Browser.IsMobileDevice && Url.Contains("Menu=DAPP");
                    isDSVMobileCall = !string.IsNullOrEmpty(privateLabelId) && Url.Contains("Menu=DAPP");
                }
                if (!isDSVMobileCall)
                {
                    UserLastLogin entityPoco = repository.GetSingle(record => record.Id == entity.Id && record.Tenant == entity.Tenant);
                    entityPoco.ComputerId = entity.ComputerId;
                    entityPoco.WorkEnvironment = workEnvironment;
                    repository.Update(entityPoco);
                    repository.SubmitChanges();
                }
                scope.Complete();
                return entity;
            }
        }
    }
}