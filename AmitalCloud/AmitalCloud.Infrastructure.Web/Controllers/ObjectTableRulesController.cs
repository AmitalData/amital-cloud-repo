using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObjectTableRulesController : ControllerBase
    {
        [HttpGet("GetObjectTableRulePMsByTenant")]
        public IActionResult GetObjectTableRulePMsByTenant()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var result = new ObjectTableRuleQuery(tenant).GetObjectTableRulePMsByTenant();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}