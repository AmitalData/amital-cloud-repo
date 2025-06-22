using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SystemMetadataLastUpdateController : ControllerBase
    {
        [HttpGet("GetSystemMetadataLastUpdates")]
        public IActionResult GetSystemMetadataLastUpdates()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var metadatalastUpdates = new SystemMetadataLastUpdateQuery(tenant).GetSystemMetadataLastUpdatesCacheHandle();
                return Ok(metadatalastUpdates);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}