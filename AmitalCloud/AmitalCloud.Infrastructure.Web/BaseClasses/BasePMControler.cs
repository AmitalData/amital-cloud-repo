using AmitalCloud.Infrastructure.Application.BaseClasses;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace AmitalCloud.Infrastructure.Web.BaseClasses
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BasePMControler<TService, TUpdateService, TEntity, TEntityDTO> : ControllerBase
        where TService : class, IBaseEntityQueryService<TEntity, TEntityDTO>
        where TUpdateService : class, IBaseEntityUpdateService<TEntity>
        where TEntity : class, IEntityPM, new()
        where TEntityDTO : IEntity
    {
        private protected string ObjectTableName;
        private protected bool EnableSecurity;
        private protected bool HasTenant;
        private protected bool SaveHistory;
        #region Constructors
        protected BasePMControler(string objectTableName, bool enableSecurity = false, bool hasTenant = true, bool saveHistory = false)
        {
            ObjectTableName = objectTableName;
            EnableSecurity = enableSecurity;
            HasTenant = hasTenant;
            SaveHistory = saveHistory;

        }
        protected BasePMControler()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public Methods
        [HttpGet]
        public IActionResult GetSingle()
        {
            string logKey = PerformanceLogger.LogCurrentTime();
            try
            {
                var tenant = AuthenticationToken("READ");
                var queryParams = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
                return Ok(GetResult(tenant, queryParams));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
            finally
            {
                PerformanceLogger.AddServerExecutionTimeHeader(logKey);
            }

        }
        [HttpPost]
        public IActionResult Post([FromServices] IUnitOfWork unitOfWork, IEntityPM entityPM)=> Save(unitOfWork, entityPM, "NEW", ChangeSetOperation.Insert);
        [HttpPut]
        public IActionResult Put([FromServices] IUnitOfWork unitOfWork, IEntityPM entityPM)=> Save(unitOfWork, entityPM, "UPDATE", ChangeSetOperation.Update);
        // DELETE api/<controller>/5
        [HttpDelete]
        public void Delete([FromServices] IUnitOfWork unitOfWork, int id)
        {
        }
        #endregion

        #region Private Methods
        private object GetResult(int tenant, IEnumerable<KeyValuePair<string, string>> paramList) => GetService(tenant).GetSingle(paramList, true, false);
        private void SaveEntity(IUnitOfWork unitOfWork,IEntityPM entityPM, ChangeSetOperation changeSetOperation)
        {
            if (!HasTenant) return;
            IBaseEntityUpdateService<TEntity> service = GetUpdateService(unitOfWork);
            if (changeSetOperation == ChangeSetOperation.Update) service.InitializeEntityPM((TEntity)entityPM);
            entityPM.ChangeSetOp = changeSetOperation;
            service.Update((TEntity)entityPM, true);
        }
        private IActionResult Save(IUnitOfWork unitOfWork, IEntityPM entityPM, string authOption, ChangeSetOperation operation)
        {
            if (ModelState.IsValid)
            {
                string logKey = PerformanceLogger.LogCurrentTime();
                try
                {
                    using (var uow = unitOfWork)
                    {
                        var tenant = AuthenticationToken(authOption, entityPM.Tenant);
                        uow.CreateTransactionScope(TransactionScopeOption.Required);
                        SaveEntity(uow, entityPM, ChangeSetOperation.Update);
                        if (SaveHistory) TableLastUpdateClass.UpdateTableHistory(uow, entityPM.Tenant, ObjectTableName);
                        uow.Save();
                        uow.Commit();
                        return Ok(entityPM);
                    }
                }

                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
                }
                finally
                {
                    PerformanceLogger.AddServerExecutionTimeHeader(logKey);
                }
            }
            else
            {
                return BadRequest(AmitalCloudApiExceptionBuilder.BuildModelException(ModelState));
            }
        }
        private int AuthenticationToken(string mode, int tenant = 0)
        {
            int? entityTenant;
            if (EnableSecurity && HasTenant)
            {
                entityTenant = tenant;
            }
            else
            {
                entityTenant = null;
            }
            int authTokenTenant = AmitalCloudSecurityUtility.AuthenticateTenant(entityTenant, EnableSecurity ? mode : null, ObjectTableName);
            return authTokenTenant;
        }
        private IBaseEntityQueryService<TEntity, TEntityDTO> GetService(int tenant)
        {
            return (IBaseEntityQueryService<TEntity, TEntityDTO>)typeof(TService).GetConstructor(new Type[] { typeof(int) }).Invoke(null, new object[] { tenant });
        }
        //private IBaseEntityUpdateService<TEntity> GetUpdateService(int tenant)
        //{
        //    return (IBaseEntityUpdateService<TEntity>)typeof(TUpdateService).GetConstructor(new Type[] { typeof(int) }).Invoke(null, new object[] { tenant });
        //}
        private IBaseEntityUpdateService<TEntity> GetUpdateService(IUnitOfWork unitOfWork)
        {
            return typeof(TUpdateService).GetConstructor(new Type[] { typeof(IUnitOfWork) }).Invoke(null, new object[] { unitOfWork }) as IBaseEntityUpdateService<TEntity>;
        }
        #endregion Private Methods
    }
}
