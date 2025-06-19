using AmitalCloud.Infrastructure.Domain.DataContracts;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    internal class IEntityListQueryService
    {
    }
    public interface IEntityListQueryService<TEntityList> where TEntityList : class, new()
    {
        List<TEntityList> GetList(int tenant);
        List<TEntityList> GetList(QueryOperations queryOperations, int tenant);
        List<TEntityList> GetList(QueryOperations queryOperations, int tenant, TreeFilterQueryArgs treeFilterQueryArgs);
        int GetListCount(QueryOperations queryOperations);
        int GetListCount(QueryOperations queryOperations, int tenant, TreeFilterQueryArgs treeFilterQueryArgs);
        int GetListCount(QueryOperations queryOperations, TreeFilterQueryArgs treeFilterQueryArgs);
        TEntityList GetSingle(IEnumerable<KeyValuePair<string, string>> paramList);
    }

}
