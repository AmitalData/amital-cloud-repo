using AmitalCloud.Infrastructure.Application.BaseClasses;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Data.Helpers;
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
    public abstract class BasePMControler<TService, TUpdateService, TEntityPM, TEntityPOCO> : ControllerBase
        where TService : class, IBaseEntityQueryService<TEntityPM, TEntityPOCO>
        where TUpdateService : class, IBaseEntityUpdateService<TEntityPM>
        where TEntityPM : class, IEntityPM, new()
        where TEntityPOCO : IEntity
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
        public IActionResult Post(IEntityPM entityPM)
        {
            if (ModelState.IsValid)
            {
                string logKey = PerformanceLogger.LogCurrentTime();
                try
                {
                    var tenant = AuthenticationToken("NEW", entityPM.Tenant);
                    using (TransactionScope scope = TransactionFactory.GetTransaction())
                    {
                        SaveEntity(entityPM, ChangeSetOperation.Insert);
                        if (SaveHistory) TableLastUpdateClass.UpdateTableHistory(entityPM.Tenant, ObjectTableName);
                        scope.Complete();
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

        [HttpPut]
        public IActionResult Put(IEntityPM entityPM)
        {
            if (ModelState.IsValid)
            {
                string logKey = PerformanceLogger.LogCurrentTime();
                try
                {
                    using (TransactionScope scope = TransactionFactory.GetTransaction())
                    {
                        var tenant = AuthenticationToken("UPDATE", entityPM.Tenant);
                        SaveEntity(entityPM, ChangeSetOperation.Update);
                        //TableLastUpdateClass.UpdateTableHistory(entityPM.Tenant, "AWBAdditionalHandlingInfo");

                        scope.Complete();
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

        // DELETE api/<controller>/5
        [HttpDelete]
        public void Delete(int id)
        {
        }

        private object GetResult(int tenant, IEnumerable<KeyValuePair<string, string>> paramList) => GetService(tenant).GetSingle(paramList, true, false);
        private void SaveEntity(IEntityPM entityPM, ChangeSetOperation changeSetOperation)
        {
            if (!HasTenant) return;
            IBaseEntityUpdateService<TEntityPM> service = GetUpdateService(entityPM.Tenant);
            if (changeSetOperation == ChangeSetOperation.Update) service.InitializeEntityPM((TEntityPM)entityPM);
            entityPM.ChangeSetOp = changeSetOperation;
            service.Update((TEntityPM)entityPM, true);
        }
        #endregion
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
        private IBaseEntityQueryService<TEntityPM, TEntityPOCO> GetService(int tenant)
        {
            return (IBaseEntityQueryService<TEntityPM, TEntityPOCO>)typeof(TService).GetConstructor(new Type[] { typeof(int) }).Invoke(null, new object[] { tenant });
        }
        private IBaseEntityUpdateService<TEntityPM> GetUpdateService(int tenant)
        {
            return (IBaseEntityUpdateService<TEntityPM>)typeof(TUpdateService).GetConstructor(new Type[] { typeof(int) }).Invoke(null, new object[] { tenant });
        }

    }
}
