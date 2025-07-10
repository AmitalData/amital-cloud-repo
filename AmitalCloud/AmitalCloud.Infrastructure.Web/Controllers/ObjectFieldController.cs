using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ObjectFieldController : ControllerBase
    {
        private readonly IBaseEntityQueryService<ObjectFieldModificationPM, ObjectFieldModification> _objectFieldModificationQueryService;
		public ObjectFieldController(IBaseEntityQueryService<ObjectFieldModificationPM, ObjectFieldModification> objectFieldModificationQueryService)
		{
		   _objectFieldModificationQueryService = objectFieldModificationQueryService;
		}
	
        [HttpGet("GetObjectFieldModificationForLoggedTenant")]
        public IActionResult GetObjectFieldModificationForLoggedTenant()
        { 
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
			    List<ObjectFieldModificationPM> result = _objectFieldModificationQueryService.GetMulti(a=> a.Tenant == tenant);
				
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}