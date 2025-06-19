using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace AmitalCloud.Infrastructure.Domain.BaseClasses
{
    public abstract class BaseEntityKeyFields<TEntityPM, T> : IEntityKeyFields<TEntityPM, T>
        where TEntityPM : class
    {
        public abstract Expression<Func<TEntityPM, bool>> Predicate { get; }
        protected BaseEntityKeyFields(IEnumerable<KeyValuePair<string, string>> paramList)
        {
            Initialize(paramList);
        }
        public BaseEntityKeyFields()
        {
        }
        public abstract T GetFullKey();
        public abstract string GetEntityPMName();
        public abstract void Initialize(IEnumerable<KeyValuePair<string, string>> paramList);
    }
}
