using System.Collections;
using System.Linq.Expressions;

namespace YarikVor.Benchmark.Ex;

public class DictionaryQueryable<T> : IQueryable<T>
{
    private readonly Expression _expression;
    private readonly IQueryProvider _provider;

    public DictionaryQueryable(IEnumerable<Dictionary<string, object>> source)
    {
        _provider = new DictionaryQueryProvider<T>(source);
        _expression = Expression.Constant(this);
    }

    public DictionaryQueryable(IQueryProvider provider, Expression expression)
    {
        _provider = provider;
        _expression = expression;
    }

    public Type ElementType => typeof(T);
    public Expression Expression => _expression;
    public IQueryProvider Provider => _provider;

    public IEnumerator<T> GetEnumerator() => Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}