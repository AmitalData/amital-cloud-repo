using AmitalCloud.Infrastructure.Application.EntityListQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportGroupController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetReportGroupLists()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var list = new ReportGroupListQueryService(0).GetList(0);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}