using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

using AmitalCloud.Infrastructure.Application.Helpers;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AmitalCloudController : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(
            Summary = "Check if system is blocking",
            Description = "Checks if the system is currently in a blocked state (e.g., upgrading or maintenance) or IP is blocked."
        )]
        public IActionResult GetCheckIsupgradingSystem()
        {
            try
            {
                return Ok(Authentication.GetIsBlockingFromDB(HttpContext));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }



    }
}