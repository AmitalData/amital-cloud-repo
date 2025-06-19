using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObjectTableLastUpdateController : ControllerBase
    {
        [HttpGet("GetLastUpdatedTables")]
        public IActionResult GetLastUpdatedTables(DateTime sinceDate, string clientEmail)
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                List<ObjectTableLastUpdatePM> list = new ObjectTableLastUpdateQueryService(tenant).GetMulti(a => a.LastUpdateDate > sinceDate && (a.Tenant == tenant || a.Tenant == 0) && a.ObjectTable.CacheOnClient && !a.ObjectTable.IsClosed, a => new ObjectTableLastUpdatePM(a)
                {
                    ObjectTableName = a.ObjectTable.Name,
                }, "ObjectTable").OrderByDescending(d => d.LastUpdateDate).ToList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}