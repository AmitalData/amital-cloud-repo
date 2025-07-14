public interface IEntityListQueryProvider<TEntity,TEntityList>
{
    IQueryable<TEntityList> BuildQuery();
}