using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using Microsoft.AspNetCore.OData.Query;

public class BaseEntityListODataQueryService<TEntity, TEntityList>
    : IEntityListODataQueryService<TEntity, TEntityList>
    where TEntity : class
    where TEntityList : class
{
    private readonly IEntityListQueryProvider<TEntity, TEntityList> _provider;
    private readonly AmitalCloudContext _context;

    public BaseEntityListODataQueryService(IEntityListQueryProvider<TEntity, TEntityList> provider, AmitalCloudContext context)
    {
        _provider = provider;
        _context = context;
    }

    public List<TEntityList> GetListOData(ODataQueryOptions<TEntityList> queryOptions)
    {
        var query = _provider.BuildQuery(); //

        var settings = new ODataQuerySettings
        {
            HandleNullPropagation = HandleNullPropagationOption.True
        };

        query = (IQueryable<TEntityList>)queryOptions.ApplyTo(query, settings);

        return query.ToList();
    }

    public List<TEntity> GetOData(ODataQueryOptions<TEntity> queryOptions)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>(); //  _context.Set<Feature>;  //_provider.BuildQuery(); //

        var settings = new ODataQuerySettings
        {
            HandleNullPropagation = HandleNullPropagationOption.True
        };

        query = (IQueryable<TEntity>)queryOptions.ApplyTo(query, settings);

        return query.ToList();
    }

}
