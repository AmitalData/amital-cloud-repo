using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogitudeApplicationController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCheckIsupgradingSystem()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                bool isBlocking = GetIsBlockingFromDB(tenant);
                return Ok(isBlocking);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }

        private bool GetIsBlockingFromDB(int tenant)
        {
            bool isBlocking = new GlobalDBQueryService(tenant).GetFirst().IsBlocking;

            string? currentIP = HttpContext.Request.Headers["X-Real-IP"];
            if (string.IsNullOrEmpty(currentIP))
            {
                currentIP = HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            string[] authenticatedIPs = AmitalCloudSettings.CustomerCareIP.Split(',');

            if (!authenticatedIPs.Contains(currentIP))
            {
                isBlocking = false;
            }
            return isBlocking;
        }
    }
}