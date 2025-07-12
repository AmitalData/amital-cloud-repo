using AmitalCloud.Infrastructure.Data.Context;
using AutoMapper;

public  class BaseEntityListQueryProvider<TEntity, TEntityList>
    where TEntity : class
    where TEntityList : class
{
    protected readonly AmitalCloudContext _context;
    protected readonly IMapper _mapper;

    protected BaseEntityListQueryProvider(AmitalCloudContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public virtual IQueryable<TEntityList> BuildQuery()
    {
        return _mapper.ProjectTo<TEntityList>(_context.Set<TEntity>());
    }
}
