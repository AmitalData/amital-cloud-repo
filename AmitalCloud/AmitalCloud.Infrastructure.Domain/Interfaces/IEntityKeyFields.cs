using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IEntityKeyFields<TEntityPM, T> where TEntityPM : class
    {
        string GetEntityPMName();
        T GetFullKey();
        void Initialize(IEnumerable<KeyValuePair<string, string>> paramList);
        Expression<Func<TEntityPM, bool>> Predicate { get; }
    }
}