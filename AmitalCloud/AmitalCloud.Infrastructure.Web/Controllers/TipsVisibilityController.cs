using System.Net;
using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipsVisibilityController : ControllerBase
    {
        [HttpGet("GetTipsVisibilities")]
        public IActionResult GetTipsVisibilities(string userId)
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var tipsVisibilityLists = new TipsVisibilityQuery(tenant).GetTipsVisibilities(tenant, userId);
                return Ok(tipsVisibilityLists);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}