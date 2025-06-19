using AmitalCloud.Infrastructure.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface ISpecification<T, Tkey>
    {
        Expression<Func<T, bool>> Criteria { get; }
        Expression<Func<T, Tkey>> OrderBy { get; }
        OrderByDirection OrderByDirection { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        int Take { get; }
        int Skip { get; }
    }
}
