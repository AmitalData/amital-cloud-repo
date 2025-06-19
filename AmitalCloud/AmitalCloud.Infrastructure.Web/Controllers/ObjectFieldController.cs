using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObjectFieldController : ControllerBase
    {
        [HttpGet("GetObjectFieldModificationForLoggedTenant")]
        public IActionResult GetObjectFieldModificationForLoggedTenant()
        { 
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant(); ;
                List<ObjectFieldModificationPM> result = new ObjectFieldModificationQueryService(tenant).GetMulti(a=> a.Tenant == tenant);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}