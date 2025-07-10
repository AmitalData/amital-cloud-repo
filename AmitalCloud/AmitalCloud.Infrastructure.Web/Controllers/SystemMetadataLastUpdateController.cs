using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using Microsoft.AspNetCore.Mvc;
using AmitalCloud.Infrastructure.Domain.Interfaces;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SystemMetadataLastUpdateController : ControllerBase
    {
        private readonly ISystemMetadataLastUpdateQuery _systemMetadataLastUpdateQuery;
		public SystemMetadataLastUpdateController(ISystemMetadataLastUpdateQuery systemMetadataLastUpdateQuery)
        {
            _systemMetadataLastUpdateQuery = systemMetadataLastUpdateQuery;

		}
        [HttpGet("GetSystemMetadataLastUpdates")]
        public IActionResult GetSystemMetadataLastUpdates()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var metadatalastUpdates = _systemMetadataLastUpdateQuery.GetSystemMetadataLastUpdatesCacheHandle();
                return Ok(metadatalastUpdates);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}