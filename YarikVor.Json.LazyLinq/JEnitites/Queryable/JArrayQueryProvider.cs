using System.Linq.Expressions;

namespace YarikVor.Json.LazyLinq.JEnitites.Queryable;

public class JArrayQueryProvider(JArray jArray) : IQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
    {
        var elementType = expression.Type.GetGenericArguments()[0];
        var queryType = typeof(JArrayQueryable<>).MakeGenericType(elementType);
        return (IQueryable)Activator.CreateInstance(queryType, jArray, this, expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new JArrayQueryable<TElement>(jArray, this, expression);
    }


    public object? Execute(Expression expression)
    {
        if (expression is MethodCallExpression mce)
        {
            if (mce.Method.Name == nameof(System.Linq.Queryable.Where))
            {
                var sourceExpression = mce.Arguments[0];
                var lambda = (LambdaExpression)mce.Arguments[1];

                var source = Execute(sourceExpression) as IEnumerable<JObject>;
                if (source == null) throw new InvalidOperationException("Source must be enumerable.");

                var compiledPredicate = lambda.Compile();
                return source.Where(item => (bool)compiledPredicate.DynamicInvoke(item));
            }
        }
        
        return null;
        // If not a MethodCallExpression, evaluate directly.
        //return jArray.Children<JObject>().Select(token => token.ToObject<T>());
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return (TResult)Execute(expression);
    }
}
