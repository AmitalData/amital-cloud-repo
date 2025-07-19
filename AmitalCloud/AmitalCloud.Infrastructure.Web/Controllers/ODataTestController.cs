using System.Collections.Generic;
using AmitalCloud.Infrastructure.Web.BaseClasses;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AmitalCloud.Shipment.Web.Controllers.Generated.ListControllers
{
    public partial class ODataTestController : BaseListODataController<
        IEntityListODataQueryService<Feature, FeatureList>,
        Feature,
        FeatureList>
    {
        private readonly IGenericEntityQueryService _queryService;

        public ODataTestController(IServiceProvider provider)
            : base(provider, "Feature", "Feature") {
            _queryService = provider.GetService(typeof(IGenericEntityQueryService)) as IGenericEntityQueryService
        ?? throw new InvalidOperationException("GenericEntityQueryService not registered");
        }


        [HttpGet("getsingle/{entityName}")]
        public IActionResult GetSingle(string entityName)
        {
            var keyParams = Request.Query.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToString()
            );

            if (keyParams.Count == 0)
                return BadRequest("Missing key parameters");

            var result = _queryService.GetSingle(entityName, keyParams, tenant: 1);

            return result == null ? NotFound() : Ok(result);
        }
    }
}
