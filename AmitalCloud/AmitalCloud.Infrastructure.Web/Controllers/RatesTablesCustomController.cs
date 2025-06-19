using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatesTablesCustomController : ControllerBase
    {
        [HttpGet("GetLastUpdateByCurrencyCode")]
        public IActionResult GetLastUpdateByCurrencyCode(string foreignCurrency, string tenantCurrencyId)
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var ratesTable = new RatesTableQuery(tenant).GetLastUpdateByCurrencyCode(foreignCurrency, tenantCurrencyId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}