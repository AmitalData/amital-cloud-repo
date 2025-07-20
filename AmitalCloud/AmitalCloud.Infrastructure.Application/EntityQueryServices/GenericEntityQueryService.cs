
using AmitalCloud.Infrastructure.Application.BaseClasses;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.BaseClasses;
using System.Linq.Expressions;

public class GenericEntityQueryService<TPOCO, TKeys, TPM, TList, TKeyType> :
    BaseEntityQueryService<TPOCO, TKeys, TPM, TList, TKeyType>,
    IGenericEntityQueryService<TPM>
    where TPOCO : BaseEntity, new()
    where TPM : IEntityPM, new()
    where TKeys : IEntityKeyFields<TPOCO, TKeyType>, new()
    where TList : class, new()
{
    public GenericEntityQueryService(IRepository<TPOCO> repo, IMapping<TPM, TPOCO, TList> mapping)
        : base(repo, mapping) { }

    public TPM GetSingle(Dictionary<string, string> keyParams, bool getComposition = true, bool getFromCache = false)
        => base.GetSingle(keyParams, getComposition, getFromCache);

    public object? GetFirst() => base.GetFirst();

    public List<TPM> GetMulti() => base.GetMulti(e => true);

    public List<TPM> GetMultiByParent<TParentKeys>(TParentKeys parentKeys, bool getFromCache = false, bool getComposition = true)
        => base.GetMultiByParent(parentKeys, getFromCache, getComposition);

    public List<TPM> GetMultiFromCache(string cacheKey, Expression<Func<object, bool>> predicate, string? include = null)
        => base.GetMultiFromCache(cacheKey, ConvertPredicate<TPOCO>(predicate), include);

    public List<TPM> GetMulti(Expression<Func<object, bool>> predicate)
        => base.GetMulti(ConvertPredicate<TPOCO>(predicate));

    public List<TPM> GetMulti(Expression<Func<object, bool>> predicate, Expression<Func<object, TPM>> selector)
        => base.GetMulti(ConvertPredicate<TPOCO>(predicate), ConvertSelector<TPOCO, TPM>(selector));

    public List<TPM> GetMulti(Expression<Func<object, bool>> predicate, Expression<Func<object, TPM>> selector, string include)
        => base.GetMulti(ConvertPredicate<TPOCO>(predicate), ConvertSelector<TPOCO, TPM>(selector), include);

    public List<TResult> GetMulti<TResult>(Expression<Func<object, bool>> predicate, Expression<Func<object, TResult>> selector)
        => base.GetMulti(ConvertPredicate<TPOCO>(predicate), ConvertSelector<TPOCO, TResult>(selector));

    public List<TResult> GetMulti<TResult>(Expression<Func<object, bool>> predicate, Expression<Func<object, TResult>> selector, string include)
        => base.GetMulti(ConvertPredicate<TPOCO>(predicate), ConvertSelector<TPOCO, TResult>(selector), include);

    private static Expression<Func<TTarget, bool>> ConvertPredicate<TTarget>(Expression<Func<object, bool>> expression)
    {
        var parameter = Expression.Parameter(typeof(TTarget), "x");
        var body = ReplacingVisitor.Replace(expression.Parameters[0], parameter, expression.Body);
        return Expression.Lambda<Func<TTarget, bool>>(body, parameter);
    }

    private static Expression<Func<TSource, TResult>> ConvertSelector<TSource, TResult>(Expression<Func<object, TResult>> expression)
    {
        var parameter = Expression.Parameter(typeof(TSource), "x");
        var body = ReplacingVisitor.Replace(expression.Parameters[0], parameter, expression.Body);
        return Expression.Lambda<Func<TSource, TResult>>(body, parameter);
    }

    private class ReplacingVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        private ReplacingVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        public static Expression Replace(ParameterExpression from, ParameterExpression to, Expression expression)
            => new ReplacingVisitor(from, to).Visit(expression);

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _from ? _to : base.VisitParameter(node);
    }
}