using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace AmitalCloud.Infrastructure.Data.BaseClasses
{

    public abstract class BaseSpecification<T, Tkey> : ISpecification<T, Tkey>
    {
        public BaseSpecification(Expression<Func<T, bool>> criteria, Expression<Func<T, Tkey>> orderBy, OrderByDirection direction)
        {
            Criteria = criteria;
            OrderBy = orderBy;
            OrderByDirection = direction;
        }
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, Tkey>> OrderBy { get; }
        public OrderByDirection OrderByDirection { get; }
        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        // string-based includes allow for including children of children, e.g. Basket.Items.Product
        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
        public int Take { get; }
        public int Skip { get; }
    }
}
