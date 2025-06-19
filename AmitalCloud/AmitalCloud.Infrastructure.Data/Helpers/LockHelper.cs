using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class LockHelper
    {
        public void LockOrCrashOnCommitDueUnique(string GeneralKey, int tenant)
        {
            var repo = new GeneralLockRepository(tenant);
            LogMessagingUtil.Instance.AppendLine(" ** try lock, LockOrCrashOnCommitDueUnique, key: " + GeneralKey);
            using (var scope = TransactionFactory.GetTransaction())
            {
                repo.Insert(new GeneralLock()
                {
                    Tenant = tenant,
                    GeneralKey = GeneralKey,
                    CreatedAt = TenantServerConfigration.GetCurrentDateTime(tenant)
                });
                repo.SubmitChanges();
                scope.Complete();
            }
        }
        public void FreeLock(string GeneralKey, int tenant)
        {
            var repo = new GeneralLockRepository(tenant);
            LogMessagingUtil.Instance.AppendLine(" ** Rlease lock, FreeLock, key: " + GeneralKey);

            using (var scope = TransactionFactory.GetTransaction())
            {
                repo.FastDelete(GeneralKey, tenant);
                repo.SubmitChanges();
                scope.Complete();
            }
        }
        public void FreeLockIfCreated15MinOld(string cRSKey, int tenant)
        {
            var repo = new GeneralLockRepository(tenant);
            LogMessagingUtil.Instance.AppendLine(" ** Rlease old lock, FreeLockIfCreated15MinOld, key: " + cRSKey);
            using (var scope = TransactionFactory.GetTransaction())
            {
                repo.FastDeleteIfCreated15MinOld(cRSKey, tenant);
                repo.SubmitChanges();
                scope.Complete();
            }
        }
    }
}
