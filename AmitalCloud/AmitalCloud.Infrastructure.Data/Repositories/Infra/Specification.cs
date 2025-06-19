using AmitalCloud.Infrastructure.Data.BaseClasses;
using AmitalCloud.Infrastructure.Domain.Enums;
using System;
using System.Linq.Expressions;
namespace AmitalCloud.Infrastructure.Data.Repositories.Infra
{
    public class Specification<T, TKeyType> : BaseSpecification<T, TKeyType>
    {
        public Specification(Expression<Func<T, bool>> criteria) : base(criteria)
        {
        }
        public Specification(Expression<Func<T, bool>> criteria, Expression<Func<T, TKeyType>> orderBy, OrderByDirection direction = OrderByDirection.Ascending) : base(criteria, orderBy, direction)
        {
        }
        public void Include(Expression<Func<T, object>> includeExpression) => AddInclude(includeExpression);
        public void Include(string includeExpression) => AddInclude(includeExpression);

    }
}
