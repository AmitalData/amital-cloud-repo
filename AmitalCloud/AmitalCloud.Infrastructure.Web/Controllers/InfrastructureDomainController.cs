using AmitalCloud.Infrastructure.Application.EntityListQueryServices;
using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AmitalCloud.Infrastructure.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InfrastructureDomainController : ControllerBase
    {
        [HttpGet("GetAllowedFeaturesForLoggedUser")]
        public IActionResult GetAllowedFeaturesForLoggedUser()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();

                string loggedUserEmail = AmitalCloudSecurityUtility.GetAuthenticatedUser();

                ContactQuery contactQuery = new ContactQuery(tenant);
                string? loggedUserId = contactQuery.GetContactByEmailOnly(loggedUserEmail, tenant)?.Id;
                if (string.IsNullOrEmpty(loggedUserId))
                {
                    return BadRequest("Logged user not found");
                }

                FeatureQuery featureQuery = new FeatureQuery(tenant);
                LoggedUserFeatures loggedUserFeatures = featureQuery.GetAllowedFeaturesForLoggedUser(loggedUserId, tenant);
                List<FeaturePM> myResult1 = loggedUserFeatures.Features;
                List<FeaturePM> myResult = new List<FeaturePM>();
                List<string> toggleCodes = myResult1.Where(d => !string.IsNullOrEmpty(d.ToggleCode)).Select(s => s.ToggleCode).ToList();
                if (toggleCodes.Count == 0)
                {
                    myResult = myResult1;
                }
                else
                {
                    FeatureToggleRepository featureToggleRepository = new FeatureToggleRepository(0);
                    List<FeatureToggle> featureToggles = featureToggleRepository.GetAllByToggleCodeList(toggleCodes, 0).ToList();
                    foreach (FeaturePM item in myResult1)
                    {
                        if (string.IsNullOrEmpty(item.ToggleCode))
                        {
                            myResult.Add(item);
                        }
                        else
                        {
                            FeatureToggle featureToggle = featureToggles.Where(d => d.ToggleCode == item.ToggleCode && (d.TenantNumber == tenant || (tenant >= d.FromTenantNumber && tenant <= d.ToTenantNumber))).FirstOrDefault();
                            if (featureToggle != null)
                            {
                                myResult.Add(item);
                            }
                        }
                    }
                }
                return Ok(myResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }

        [HttpGet("GetFeatureToggles")]
        public IActionResult GetFeatureToggles()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                List<FeatureToggleList> myResult = new FeatureToggleListQueryService(tenant).GetList(tenant).Where(a => a.Inactive == false).ToList();
                return Ok(myResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }

        [HttpGet("GetLastFilters")]
        public IActionResult GetLastFilters()
        {
            try
            {
                int tenant = AmitalCloudSecurityUtility.AuthenticateTenant();
                string? token = HttpContext.Request.Headers["Token"];
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Token is required");
                }
                string loggedUserEmail = AuthenticationTokenRepository.GetSingleTokenFromCache(token).Email;
                //ICRMContext crmContext = CRMContext.GetContext(tenant);
                //CRMFilterSettingListQueryService listService = new CRMFilterSettingListQueryService(crmContext);
                //List<CRMFilterSettingList> myResult = listService.GetList(tenant);
                return Ok(new List<string>());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AmitalCloudApiExceptionBuilder.BuildException(ex));
            }
        }
    }
}