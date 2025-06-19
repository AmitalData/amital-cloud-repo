using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserExtendedController : ControllerBase
    {
        [HttpGet("GetCheckUserReleaseNotesToolTip")]
        public IActionResult GetCheckUserReleaseNotesToolTip(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var usersReleaseNotesDisplayPM = new UsersReleaseNotesDisplayQueryService(tenant)
                    .GetMulti(a => a.UserId == userId && a.Tenant == tenant)
                    .FirstOrDefault();

                return Ok(usersReleaseNotesDisplayPM == null);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}
