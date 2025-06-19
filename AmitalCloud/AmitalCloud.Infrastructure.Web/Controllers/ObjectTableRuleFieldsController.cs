using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObjectTableRuleFieldsController : ControllerBase
    {
        [HttpGet("GetObjectTableRuleFieldPMsByTenant")]
        public IActionResult GetObjectTableRuleFieldPMsByTenant()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var queryService = new ObjectTableRuleFieldQueryService(tenant);
                var result = queryService.GetMulti(a => a.Tenant == tenant || a.Tenant == 0, a => new ObjectTableRuleFieldPM(a)
                {
                    ObjectFieldName = a.ObjectField.FieldName,
                    ObjectTableRuleCode = a.ObjectTableRule.RuleCode,
                    ObjectTableRuleTypeCode = a.ObjectTableRule.RuleTypeCode,
                }, "ObjectField,ObjectTableRule");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}