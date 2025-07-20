using AmitalCloud.Infrastructure.Application.BaseClasses;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityKeys;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using Azure.Storage.Blobs.Models;
using System.Linq.Expressions;
using POCO = AmitalCloud.Infrastructure.Model.EntityClasses;


namespace AmitalCloud.Infrastructure.Application.EntityQueryServices
{
    public partial class TenantManagementQueryService : BaseEntityQueryService<POCO.TenantManagement, TenantManagementKeys<int>, TenantManagementPM, TenantManagementList, int>
	{
		private readonly IBaseQueryService<TenantManagementPM, TenantManagement, int> _tenantManagementQueryService;
		private readonly IBaseQueryService<UserPM,User,string> _userQueryService;

		public TenantManagementQueryService(IBaseQueryService<TenantManagementPM, TenantManagement, int> tenantManagementQueryService, IBaseQueryService<UserPM, User, string> userQueryService)
        {

			_tenantManagementQueryService = tenantManagementQueryService;
			_userQueryService = userQueryService;

		}
		public   TenantStatusPM GetTenantStatusPM(int tenant, string userId)
        {
            TenantManagementPM tenantPM = _tenantManagementQueryService.GetSingle(tenant, true, true);
            var pm = new TenantStatusPM();

            if (tenantPM.PaymentFailure)
                HandleBlocking(pm, tenantPM.SuspendDate, BlockingType.Suspend,
                               v => pm.SuspendDaysLeft = v);

            if (tenantPM.IsTrial)
                HandleBlocking(pm, tenantPM.TrialEndDate, BlockingType.Company,
                               v => pm.TrialDaysLeft = v);

            else if (!tenantPM.IsRecurring)
                HandleBlocking(pm, tenantPM.PaidUntilDate, BlockingType.Company,
                               v => pm.PaidDaysLeft = v);

            EnrichWithUser(pm, userId, tenant);


            return pm;
        }

        public List<TenantManagement> GetTenantsBySupportStatus(bool isDistributor, UserPM? user, int tenant)
        {
            var repository = new Repository<TenantManagement>(tenant);

            Expression<Func<TenantManagement, bool>> predicate = isDistributor && user != null
                ? (t =>
                    t.DistributorCode == user.DistributorCode &&
                    t.Id != 0 &&
                    t.IsDistributorSupportEnabled &&
                    t.GlobalTenant.IsActive)
                : t =>
                    (t.IsSystemSupportEnabled || t.Id == 0) &&
                    t.GlobalTenant.IsActive;

            List<TenantManagement> tenants = repository.GetMulti<TenantManagement>(predicate, "GlobalTenant");
            return tenants;
        }
        private void EnrichWithUser(TenantStatusPM pm, string userId, int tenant)
        {
            UserPM user = _userQueryService.GetSingle(userId, true, true);

            if (user?.ExpirationDate == null) return;

           pm.ExpirationDate = user.ExpirationDate ;

            HandleBlocking(pm, user.ExpirationDate, BlockingType.User,
                           daysLeftSetter: v => pm.ExpirationDaysLeft = v);
        }
        private static void HandleBlocking(
        TenantStatusPM pm,
        DateTime? date,
        BlockingType blockType,
        Action<int> daysLeftSetter)
        {
            if (date == null) return;

            var daysLeft = (date.Value.Date - DateTime.Now.Date).Days;

            if (daysLeft < 0)
            {
                pm.DoBlocking = true;
                pm.BlockType = blockType;
            }
            else
            {
                daysLeftSetter(daysLeft);
            }
        }


    }
}
