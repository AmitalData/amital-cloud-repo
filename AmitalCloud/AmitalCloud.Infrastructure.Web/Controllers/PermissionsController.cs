using AmitalCloud.Infrastructure.Application.EntityListQueryServices;
using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Web.DataContracts;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PermissionsController(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        [SwaggerOperation(
        Summary = "Get allowed features for logged-in user",
        Description = "Fetches the list of features available for the currently authenticated user based on their roles, packages, and feature toggles."
        )]

        [HttpGet("GetAllowedFeaturesForLoggedUser")]
        public IActionResult GetAllowedFeaturesForLoggedUser()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                string loggedUserEmail = AmitalCloudSecurityUtility.GetAuthenticatedUser();
                FeatureQueryService featureQuery = new FeatureQueryService(tenant);
                var features = featureQuery.GetAllowedFeaturesForLoggedUser(loggedUserEmail, tenant);
                return Ok(features);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }

        }


    }
}