using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System;
using AmitalCloud.Infrastructure.Domain.Interfaces;

namespace AmitalCloud.Infrastructure.Web.BaseClasses
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseListODataController<TService, TEntity, TEntityList> : ControllerBase
        where TService : class, IEntityListODataQueryService<TEntity, TEntityList>
        where TEntity : class
        where TEntityList : class, new()
    {
        private readonly IServiceProvider _provider;

        protected string ObjectTableName;
        protected string QuerySection;

        protected BaseListODataController(IServiceProvider provider, string objectTableName, string querySection)
        {
            _provider = provider;
            ObjectTableName = objectTableName;
            QuerySection = querySection;
        }

        protected TService GetService() => _provider.GetRequiredService<TService>();

        [HttpGet("GetByOData")]
        [EnableQuery]
        public IActionResult GetByOData(ODataQueryOptions<TEntityList> queryOptions)
        {
            var result = GetService().GetListOData(queryOptions);
            return Ok(result);
        }
    }
}
