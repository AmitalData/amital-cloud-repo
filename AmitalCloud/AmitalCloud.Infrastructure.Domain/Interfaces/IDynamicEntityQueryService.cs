using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IDynamicEntityQueryService
    {
        object? GetSingle(Dictionary<string, string> keyParams, bool getComposition = true, bool getFromCache = false);
        object? GetFirst();
        List<object> GetMulti(object predicate);
        List<object> GetMultiByParent(object parentKeys, bool getFromCache = false, bool getComposition = true);
        List<object> GetMultiFromCache(string cacheKey, object predicate, string? include = null);
        List<TResult> GetMultiSelect<TResult>(object predicate, object selector);
        List<TResult> GetMultiSelect<TResult>(object predicate, object selector, string include);
    }
}
