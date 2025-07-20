using System.Linq.Expressions;

public interface IGenericEntityQueryService<TPM>
{
    TPM GetSingle(Dictionary<string, string> keyParams, bool getComposition = true, bool getFromCache = false);
    List<TPM> GetMulti();
    object? GetFirst();
    List<TPM> GetMultiByParent<TParentKeys>(TParentKeys parentKeys, bool getFromCache = false, bool getComposition = true);
    List<TPM> GetMultiFromCache(string cacheKey, Expression<Func<object, bool>> predicate, string? include = null);
    List<TPM> GetMulti(Expression<Func<object, bool>> predicate);
    List<TPM> GetMulti(Expression<Func<object, bool>> predicate, Expression<Func<object, TPM>> selector);
    List<TPM> GetMulti(Expression<Func<object, bool>> predicate, Expression<Func<object, TPM>> selector, string include);
    List<TResult> GetMulti<TResult>(Expression<Func<object, bool>> predicate, Expression<Func<object, TResult>> selector);
    List<TResult> GetMulti<TResult>(Expression<Func<object, bool>> predicate, Expression<Func<object, TResult>> selector, string include);
}