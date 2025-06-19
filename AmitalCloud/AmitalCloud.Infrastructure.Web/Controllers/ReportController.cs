using AmitalCloud.Infrastructure.Application.EntityListQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetReportListsByGroupId(string groupId)
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                List<ReportList> reportLists = new ReportListQueryService(tenant).GetList(tenant).Where(d => d.ReportGroupId == groupId).OrderBy(d => d.Name).ToList();
                return Ok(reportLists);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}