using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DWObjectFieldsController : ControllerBase
    {
        [HttpGet("GetDWObjectFieldsWithChildren")]
        public IActionResult GetDWObjectFieldsWithChildren()
        {
            string logKey = PerformanceLogger.LogCurrentTime();
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var dWObjectFieldPM = new DWObjectFieldQuery(tenant).GetDWObjectFieldWithChildrenFieldsPMsByTenant();

                PerformanceLogger.AddServerExecutionTimeHeader(logKey);
                return Ok( dWObjectFieldPM);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}