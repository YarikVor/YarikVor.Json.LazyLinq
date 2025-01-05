using System.Linq.Expressions;

namespace YarikVor.Benchmark.Ex;

public class DictionaryQueryProvider<T> : IQueryProvider
{
    private readonly IEnumerable<Dictionary<string, object>> _source;

    public DictionaryQueryProvider(IEnumerable<Dictionary<string, object>> source)
    {
        _source = source;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new DictionaryQueryable<T>(this, expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new DictionaryQueryable<TElement>(this, expression);
    }

    public object Execute(Expression expression)
    {
        return Execute<IEnumerable<T>>(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        // Translate the expression tree and execute the query
        var translator = new DictionaryExpressionTranslator<T>(_source);
        return (TResult)translator.Translate(expression);
    }
}