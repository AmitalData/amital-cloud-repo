using Microsoft.AspNetCore.OData.Query;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IEntityListODataQueryService<TEntity, TEntityList>
        where TEntity : class
        where TEntityList : class
    {
        List<TEntityList> GetListOData(ODataQueryOptions<TEntityList> queryOptions);
        List<TEntity> GetOData(ODataQueryOptions<TEntity> queryOptions);
    }
}
