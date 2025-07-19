using System.Collections.Generic;
using AmitalCloud.Infrastructure.Web.BaseClasses;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using AmitalCloud.Infrastructure.Application.Helpers;

namespace AmitalCloud.Shipment.Web.Controllers.Generated.ListControllers
{
    public partial class GenericPMController : ControllerBase
    {
        private readonly IGenericEntityQueryService _queryService;
        private readonly IWebHostEnvironment _env;

        public GenericPMController(IServiceProvider provider, IWebHostEnvironment env)
           {
            _queryService = provider.GetService(typeof(IGenericEntityQueryService)) as IGenericEntityQueryService
        ?? throw new InvalidOperationException("GenericEntityQueryService not registered");
            _env = env;
        }


        [HttpGet("getsingle/{entityName}")]
        public IActionResult GetSingle(string entityName, [FromQuery] Dictionary<string, string> keyParams)
        {
            if (keyParams.Count == 0)
                return BadRequest("Missing key parameters");

            int tenant = _env.IsDevelopment() ? 106 : AmitalCloudSecurityUtility.AuthenticateTenant();

            var result = _queryService.GetSingle(entityName, keyParams, tenant);

            return result == null ? NotFound() : Ok(result);
        }

    }
}
