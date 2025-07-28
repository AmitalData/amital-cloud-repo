using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using System;
using System.Transactions;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class MultiProcessLockTableUtil
    {



        public IDisposable LockItAndGetReleaseToken(int tenant, string key2Upsert, string requestLog)
        {
            if (Transaction.Current == null)
            {
                throw new Exception("MultiProcessLockTableUtil:4 use must be under Transaction");
            }
            ProcessLockReleaseToken processLockToken = null;

            using (var uow = new UnitOfWork(tenant))
            {

                var repo = new GeneralLockRepository(uow);
                LogMessagingUtil.Instance.AppendLine("MultiProcessLockTableUtil: trylock<<<" + key2Upsert.ToString());
                var lockPoco = repo.GetSingleGeneralLockNOWAIT(key2Upsert, tenant);
                if (lockPoco == null)
                {
                    uow.CreateTransactionScope(TransactionScopeOption.RequiresNew);
                    repo.Insert(new GeneralLock()
                    {
                        Tenant = tenant,
                        GeneralKey = key2Upsert,
                        CreatedAt = TenantServerConfigration.GetCurrentDateTime(tenant)
                    });
                    LogMessagingUtil.Instance.AppendLine("MultiProcessLockTableUtil: LockItAndGetReleaseToken:ADD<<<" + key2Upsert.ToString());
                    uow.Save();
                    uow.Commit();
                    LogMessagingUtil.Instance.AppendLine("MultiProcessLockTableUtil: trylock<<<" + key2Upsert.ToString());
                    lockPoco = repo.GetSingleGeneralLockNOWAIT(key2Upsert, tenant);
                }

                if (lockPoco == null)
                {
                    throw new Exception("MultiProcessLockTableUtil:lockPoco ==null");
                }

            }



            var newLock = new ProccesLockData()
            {
                Tenant = tenant,
                MyKey = key2Upsert,
                MyLog = requestLog,
                InsertTime = DateTime.Now

            };
            processLockToken = new ProcessLockReleaseToken()
            {

                ProccesLockData = newLock
            };



            return processLockToken as IDisposable;
        }
        private void RealseKey(ProcessLockReleaseToken disposeProcessLockToken)
        {


            var repo = new GeneralLockRepository(disposeProcessLockToken.ProccesLockData.Tenant);

            //var repo = new GeneralLockRepository(_Tenant);
            repo.FastDelete(disposeProcessLockToken.ProccesLockData.MyKey, disposeProcessLockToken.ProccesLockData.Tenant);

            LogMessagingUtil.Instance.AppendLine("MultiProcessLockTableUtil:RealseKey:Removed>>>:" + disposeProcessLockToken.ToString());


        }

        public class ProcessLockReleaseToken : IDisposable
        {
            public ProccesLockData ProccesLockData { get; internal set; }

            public void Dispose()
            {
                var multiProcessLockTableUtil = new MultiProcessLockTableUtil();
                multiProcessLockTableUtil.RealseKey(this);
            }
        }
    }

}
