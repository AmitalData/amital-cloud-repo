using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GlobalController : ControllerBase
    {

        [HttpGet("GetTenantManagementStatus")]
        [SwaggerOperation(
        Summary = "Get tenant status",
        Description = "Returns information about the current tenant’s status for a given user, including blocking due to payment, trial, or user expiration."
        )]
        public IActionResult GetTenantManagementStatus(string loggeduserid)
        {

            try
            {
                int tenantId = AmitalCloudSecurityUtility.AuthenticateTenant();
                var tenantStatus = new TenantManagementQueryService(tenantId).GetTenantStatusPM(tenantId, loggeduserid);

                return Ok(tenantStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }




    }
}