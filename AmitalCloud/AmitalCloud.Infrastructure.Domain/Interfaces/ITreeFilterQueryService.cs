using AmitalCloud.Infrastructure.Domain.DataContracts;
using System.Linq;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface ITreeFilterQueryService
    {
        IQueryable<T> Apply<T>(IQueryable<T> queryable, TreeFilterQueryArgs treeFilterQueryArgs);
    }

}
