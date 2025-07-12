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
        public ODataTestController(IServiceProvider provider)
            : base(provider, "Feature", "Feature") { }
    }
}
