using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityKeys;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Linq;
using System.Transactions;
using System.Web;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public class WorkerNameService
    {
        public static int GetWorkerWaitingStatusForSending(int tenant)
        {
            int waitingStatus = 0;
            string workerName = GetCurrentWorkerName();
            if (FeatureToggleHelper.HasFeatureToggle("NWR", tenant))
            {
                waitingStatus = -1030;
                if (!string.IsNullOrEmpty(workerName))
                {
                    if (workerName.ToLower() == "staging")
                    {
                        workerName = "stagingnew";
                    }
                    else if (workerName.ToLower() == "production")
                    {
                        workerName = "productionnew";
                    }
                }
            }
            if (!string.IsNullOrEmpty(workerName))
            {
                waitingStatus = GetWatingStatusByWorkerRoleName(tenant, workerName);
            }

            return waitingStatus;
        }

        public static int GetWorkerWaitingStatusForReceiving(int tenant)
        {
            string workerName = GetCurrentWorkerName();
            if (!string.IsNullOrEmpty(workerName))
            {
                return GetWatingStatusByWorkerRoleName(tenant, workerName);
            }
            else
            {
                throw new Exception("Worker role name is not set!");
            }

        }

        private static int GetWatingStatusByWorkerRoleName(int tenant, string workerName)
        {
            if (workerName.ToLower() == "development")
                return -10;

            if (workerName.ToLower() == "production")
                return 0;
            if (workerName.ToLower() == "staging")
                return -100;

            return GetWaitingSatusForWorkerName(tenant, workerName);
        }

        private static string GetCurrentWorkerName()
        {
            string workerName = null;
            if (HttpContextHelper.HttpContext != null)
            {
                if (HttpContextHelper.HttpContext?.Items?.ContainsKey("workerrolename") == true)
                {
                    workerName = HttpContextHelper.HttpContext.Items["workerrolename"].ToString();

                }
            }
            else
            {
                workerName = AmitalCloudSettings.WorkerRoleName;
            }

            return workerName;
        }

        private static int GetWaitingSatusForWorkerName(int tenant, string workerName)
        {
            string entityName = "WorkerRoleNamePM" + workerName;

            WorkerRoleName entity = GetWorkerRoleNameFromDB(workerName, tenant);
            if (entity == null)
            {
                entity = CreateNewWorkerRoleName(tenant, workerName, entityName);
            }

            return entity.WaitingStatus;
        }

        private static WorkerRoleName CreateNewWorkerRoleName(int tenant, string workerName, string entityName)
        {
            using (IUnitOfWork uow = new UnitOfWork<AmitalCloudContext>(tenant))
            {
                uow.CreateTransactionScope(TransactionScopeOption.RequiresNew);
                int newWatingStatus = -1000;
                IRepository<WorkerRoleName> workerRoleNameRepository = new Repository<WorkerRoleName>(uow);
                WorkerRoleName lastAdded = workerRoleNameRepository.GetAll(tenant).OrderByDescending(a => a.CreateDate).FirstOrDefault();
                if (lastAdded == null)
                    newWatingStatus = -1000;
                else
                    newWatingStatus = lastAdded.WaitingStatus - 1;
                WorkerRoleName entity = new WorkerRoleName()
                {
                    CreateDate = TenantServerConfigration.GetCurrentDateTime(tenant),
                    Name = workerName,
                    WaitingStatus = newWatingStatus,
                };
                workerRoleNameRepository.Insert(entity);
                if (HttpContextHelper.HttpContext != null)
                {
                    if (CacheManager.CacheWrapper.Get(entityName) == null)
                    {
                        CacheManager.CacheWrapper.Insert(entityName, entity, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }
                }
                uow.Save();
                uow.Commit();
                return entity;
            }
        }

        private static WorkerRoleName GetWorkerRoleNameFromDB(string workerName, int tenant)
        {
            string cacheKey = $"WorkerRoleNamePM_({workerName})";
            WorkerRoleName entity;
            if (HttpContextHelper.HttpContext != null)
            {
                entity = (WorkerRoleName)CacheManager.CacheWrapper.Get(cacheKey);
                if (entity == null)
                {
                    entity = GetEntity(workerName, tenant);
                    if (entity != null)
                    {
                        if (CacheManager.CacheWrapper.Get(cacheKey) == null)
                        {
                            CacheManager.CacheWrapper.Insert(cacheKey, entity, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                        }
                    }
                }
            }
            else
            {
                entity = GetEntity(workerName, tenant);
            }
            return entity;
        }

        private static WorkerRoleName GetEntity(string workerName, int tenant)
        {
            return new Repository<WorkerRoleName>(tenant).GetSingle(new WorkerRoleNameKeys<string>() { Name = workerName });
            //GetSingleWorkerRoleName(workerName);
        }
    }
}
