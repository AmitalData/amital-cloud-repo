using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenticationController(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public string? GetSettingsLoginCode(int myDummyInteger, string myDummyString)
        {
            var logoCode = new SettingQueryService(0).GetMulti(a => true, a => a.LogoCode).FirstOrDefault();
            return logoCode;
        }

        [HttpPost("PostUserValidation")]
        [SwaggerOperation(
        Summary = "Validate user credentials",
        Description = "Validates user login credentials without initiating a full login session."
        )]
        public IActionResult PostUserValidation([FromServices] IUnitOfWork unitOfWork, LoginParameters loginParameters)
        {
            try
            {
                UserData data = new Authentication(unitOfWork, loginParameters.Tenant, _httpContextAccessor, _memoryCache).AuthenticateUser( loginParameters);
                return Ok(data);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, DateTime.Now, loginParameters.Tenant, loginParameters.Email, "", $"AuthenticationController : PostUserValidation", null);

                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }

        [HttpPost]
        [SwaggerOperation(
        Summary = "User login",
        Description = "Authenticates a user by email and password and returns their user data."
        )]
        public IActionResult PostLoginData([FromServices] IUnitOfWork unitOfWork, LoginParameters parameters, int tenant, bool? isFromCTool = false)
        {
            try
            {
                UserData user = new Authentication(unitOfWork, tenant, _httpContextAccessor, _memoryCache).LoginUser(parameters, tenant, isFromCTool);
                return Ok(user);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, DateTime.Now, tenant, parameters.Email, "", $"AuthenticationController : PostLoginData", null);
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }


    }
}