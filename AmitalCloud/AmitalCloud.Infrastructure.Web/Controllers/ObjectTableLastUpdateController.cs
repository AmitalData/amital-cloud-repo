using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Web.Helpers;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ObjectTableLastUpdateController : ControllerBase
    {
        [HttpGet("GetLastUpdatedTables")]
        public IActionResult GetLastUpdatedTables(DateTime sinceDate, string clientEmail)
        {
            try
            { 
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                var poco = new ObjectTableLastUpdateQueryService(tenant).GetMulti(a => a.LastUpdateDate > sinceDate && (a.Tenant == tenant || a.Tenant == 0) && a.ObjectTable.CacheOnClient && !a.ObjectTable.IsClosed, a => a, "ObjectTable").OrderByDescending(d => d.LastUpdateDate).ToList();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile(new ObjectTableLastUpdateDataMapping()));
                var mapper = config.CreateMapper();
                List<ObjectTableLastUpdatePM> list = mapper.Map<List<ObjectTableLastUpdatePM>>(poco);

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}