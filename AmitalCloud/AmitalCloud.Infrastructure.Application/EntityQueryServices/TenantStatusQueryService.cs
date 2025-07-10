using AmitalCloud.Infrastructure.Application.BaseClasses;
using AmitalCloud.Infrastructure.Domain.EntityKeys;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using Azure.Storage.Blobs.Models;
using POCO = AmitalCloud.Infrastructure.Model.EntityClasses;


namespace AmitalCloud.Infrastructure.Application.EntityQueryServices
{
    public  class TenantStatusQueryService : ITenantStatusQueryService
	{
		private readonly IBaseQueryService<TenantManagementPM, TenantManagement, int> _tenantManagementQueryService;
		private readonly IBaseQueryService<UserPM,User,string> _userQueryService;

		public TenantStatusQueryService(IBaseQueryService<TenantManagementPM, TenantManagement, int> tenantManagementQueryService, IBaseQueryService<UserPM, User, string> userQueryService)
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
