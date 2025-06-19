using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityResourceController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEntityResourceByTableName(string objectTableName)
        {
            int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
            ObjectTable objectTable = new ObjectTableRepository(tenant).GetObjectTableByName(objectTableName, tenant, false);
            return Ok(objectTable?.EntityResource);
        }
    }
}
