using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserLastLoginsController : ControllerBase
    {
        [HttpGet("GetUserLastLogin")]
        public IActionResult GetUserLastLogin(string userId)
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var user = new UserLastLoginQueryService(tenant).GetMulti(a => a.Tenant == tenant && a.Id == userId).FirstOrDefault();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }

        [HttpPut]
        public IActionResult Put(UserLastLogin entity)
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant(entity.Tenant);
                entity = new UserLastLoginQuery(tenant).UpdateUserLastLogins(entity, AmitalCloudSettingConfigration.GetWorkEnvironment());
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}