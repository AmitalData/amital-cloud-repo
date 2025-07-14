using AmitalCloud.Infrastructure.Domain.Interfaces;
using Microsoft.AspNetCore.OData.Query;

public class BaseEntityListODataQueryService<TEntity, TEntityList>
    : IEntityListODataQueryService<TEntity, TEntityList>
    where TEntity : class
    where TEntityList : class
{
    private readonly IEntityListQueryProvider<TEntity, TEntityList> _provider;

    public BaseEntityListODataQueryService(IEntityListQueryProvider<TEntity, TEntityList> provider)
    {
        _provider = provider;
    }

    public List<TEntityList> GetListOData(ODataQueryOptions<TEntityList> queryOptions)
    {
        var query = _provider.BuildQuery();

        var settings = new ODataQuerySettings
        {
            HandleNullPropagation = HandleNullPropagationOption.True
        };

        query = (IQueryable<TEntityList>)queryOptions.ApplyTo(query, settings);

        return query.ToList();
    }
}
