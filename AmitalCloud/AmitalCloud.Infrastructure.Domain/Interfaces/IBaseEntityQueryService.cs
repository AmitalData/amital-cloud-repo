using AmitalCloud.Infrastructure.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IBaseEntityQueryService<TEntityPM, TEntityPOCO> 
        where TEntityPM : IEntityPM, new()
        where TEntityPOCO : IEntity
    {
        TEntityPM GetSingle(IEnumerable<KeyValuePair<string, string>> paramList, bool getComposition, bool getFromCache);
        List<TEntityPM> GetMultiByParent<TEntityParentKeys>(TEntityParentKeys entityParentKeys, bool getFromCache, bool getComposition = true);
        List<TEntityPM> GetMulti(Expression<Func<TEntityPOCO, bool>> predicate);
        List<TEntityPM> GetMulti(Expression<Func<TEntityPOCO, bool>> predicate, Expression<Func<TEntityPOCO, TEntityPM>> select);
        List<TEntityPM> GetMulti(Expression<Func<TEntityPOCO, bool>> predicate, Expression<Func<TEntityPOCO, TEntityPM>> select, string include);
        List<TResult> GetMulti<TResult>(Expression<Func<TEntityPOCO, bool>> predicate, Expression<Func<TEntityPOCO, TResult>> select);
        List<TResult> GetMulti<TResult>(Expression<Func<TEntityPOCO, bool>> predicate, Expression<Func<TEntityPOCO, TResult>> select, string include);

    }
}
