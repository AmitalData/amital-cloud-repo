using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GlobalController : ControllerBase
    {
		private readonly ITenantStatusQueryService _tenantStatusQueryService;


		public GlobalController(ITenantStatusQueryService tenantStatusQueryService)
        {
			_tenantStatusQueryService = tenantStatusQueryService;
		
		}

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
				var tenantStatus = _tenantStatusQueryService.GetTenantStatusPM(tenantId, loggeduserid);

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