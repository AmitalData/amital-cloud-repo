using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TermsofUseController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCheckIfGoToTermUseComponent(string userId)
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("missing user id");
                }

                var result = new TermsofUseQuery(tenant).CheckIfGoToTermUseComponent(userId, AmitalCloudSecurityUtility.getLoggedDomain());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}