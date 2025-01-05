using System.Collections;
using System.Linq.Expressions;

namespace YarikVor.Json.LazyLinq.JEnitites.Queryable;

public class JArrayQueryable<T> : IQueryable<T>
{
    private readonly JArrayQueryProvider _provider;
    private readonly Expression _expression;
    private readonly JArray _jArray; // Додано для збереження посилання на JArray

    // Конструктор із трьома параметрами
    public JArrayQueryable(JArray jArray, JArrayQueryProvider provider, Expression expression)
    {
        _jArray = jArray; // Збереження JArray
        _provider = provider;
        _expression = expression;
    }

    // Конструктор із двома параметрами
    public JArrayQueryable(JArray jArray, JArrayQueryProvider provider)
        : this(jArray, provider, Expression.Constant(jArray))
    {
    }

    public IEnumerator<T> GetEnumerator()
    {
        // Виконання запиту через провайдера
        return ((IEnumerable<T>)_provider.Execute<IEnumerable<T>>(_expression)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Type ElementType => typeof(T);
    public Expression Expression => _expression;
    public IQueryProvider Provider => _provider;

    // Доступ до JArray (за потреби)
    public JArray JArray => _jArray;
}
