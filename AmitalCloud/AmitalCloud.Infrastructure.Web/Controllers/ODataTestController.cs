using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AmitalCloud.Infrastructure.Web.BaseClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System;
using System.Collections.Generic;

namespace AmitalCloud.Shipment.Web.Controllers.Generated.ListControllers
{
    public partial class ODataTestController : BaseListODataController<
        IEntityListODataQueryService<Feature, FeatureList>,
        Feature,
        FeatureList>
    {
 
        public ODataTestController(IServiceProvider provider)
            : base(provider, "Feature", "Feature") {
         }
         

        [EnableQuery , HttpGet("Test")]
        public IActionResult Test([FromServices] IUnitOfWork unitOfWork ,  ODataQueryOptions<Feature> queryOptions)
        {
            var result = GetService().GetOData(queryOptions);
            return Ok(result);
        }


    }
}
