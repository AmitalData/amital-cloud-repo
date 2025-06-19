using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipsController : ControllerBase
    {
        [HttpGet("GetTipsPMs")]
        public IActionResult GetTipsPMs()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var tips = new TipQuery(tenant).GetTips();
                return Ok(tips);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}