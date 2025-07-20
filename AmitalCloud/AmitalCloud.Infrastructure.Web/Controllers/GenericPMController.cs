using System.Collections.Generic;
using AmitalCloud.Infrastructure.Web.BaseClasses;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using AmitalCloud.Infrastructure.Application.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace AmitalCloud.Shipment.Web.Controllers.Generated.ListControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class GenericPMController : ControllerBase
    {
        private readonly IGenericEntityQueryServiceFactory _queryServiceFactory;
        private readonly IWebHostEnvironment _env;

        public GenericPMController(
            IGenericEntityQueryServiceFactory queryServiceFactory,
            IWebHostEnvironment env)
        {
            _queryServiceFactory = queryServiceFactory;
            _env = env;
        }

        [HttpGet("getsingle/{entityName}")]
        public IActionResult GetSingle(string entityName, [FromQuery] Dictionary<string, string> keyParams)
        {
            if (keyParams == null || keyParams.Count == 0)
                return BadRequest("Missing key parameters");

            int tenant = _env.IsDevelopment() ? 106 : AmitalCloudSecurityUtility.AuthenticateTenant();

            try
            {
                var service = _queryServiceFactory.Create(entityName, tenant);
                var result = service.GetSingle(keyParams);


 
                return result == null ? NotFound() : Ok(result);
            }
            catch (Exception ex)
            {
                 return StatusCode(500, $"Error resolving entity '{entityName}': {ex.Message}");
            }
        }
    }
}
