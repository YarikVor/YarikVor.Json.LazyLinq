using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace YarikVor.Benchmark.Ex;

public class MyExpressionVisitor<TReplace> : ExpressionVisitor
{
    private bool isChanged = false;

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Arguments[0] is ConstantExpression constantExpression && node.Object is null)
        {
            var first = Enumerable.Empty<Dictionary<string, object>>().AsQueryable();
            var second = Visit(node.Arguments[1]);
            var result = Expression.Call(null,
                node.Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(Dictionary<string, object>)),
                [Expression.Constant(first), second]);
            return result;
        }
        else if (node.Type == typeof(IQueryable<TReplace>))
        {
            var args = Visit(node.Arguments);
            var result = Expression.Call(null,
                node.Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(Dictionary<string, object>)),
                args);
            return result;
        }
        


        return base.VisitMethodCall(node);
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        if (typeof(TReplace) == node.GetType().GetGenericArguments()[0].GetGenericArguments()[0])
        {
            var param = Expression.Parameter(typeof(Dictionary<string, object>), "dict");
            var body = Visit(node.Body);
            return Expression.Lambda(body, param);
        }

        return base.VisitLambda(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is ParameterExpression param && param.Type == typeof(TReplace))
        {
            var index = Expression.Property(
                Expression.Parameter(typeof(Dictionary<string, object>), "dict"),
                "Item",
                [Expression.Constant(node.Member.Name)]
            );

            return Expression.Convert(index, node.Type);
        }

        return base.VisitMember(node);
    }
}

public class DictionaryExpressionTranslator<T>
{
    private readonly IEnumerable<Dictionary<string, object>> _source;
    private readonly ExpressionVisitor _visitor = new MyExpressionVisitor<T>();

    public DictionaryExpressionTranslator(IEnumerable<Dictionary<string, object>> source)
    {
        _source = source;
    }

    public object Translate(Expression expression)
    {
        var result = _visitor.Visit(expression);
        if (expression is MethodCallExpression methodCall)
        {
            if (methodCall.Method.Name == nameof(Queryable.Where))
            {
                var lambda = (LambdaExpression)((UnaryExpression)methodCall.Arguments[1]).Operand;

                var firstParam = lambda.Parameters[0];
                var body = lambda.Body;
                if (body is BinaryExpression binaryExpression)
                {
                    var left = binaryExpression.Left;
                    var a = left as MemberExpression;
                    var memberName = a.Member.Name;
                    var memberType = a.Type;
                    var right = binaryExpression.Right;
                    var dictionary = typeof(Dictionary<string, object>);
                    var pDict = Expression.Parameter(dictionary, "dict");
                    var getValueIndex = Expression.Property(pDict, "Item", Expression.Constant(memberName));


                    var lam = Expression.Lambda<Func<Dictionary<string, object>, bool>>(
                        Expression.MakeBinary(
                            binaryExpression.NodeType,
                            Expression.Convert(getValueIndex, memberType),
                            right
                        ),
                        pDict
                    );

                    var newLambda = lam.Compile();

                    return _source
                        .Where(dict => newLambda(dict))
                        .Select(ConvertDictionaryToType);
                }

                var predicate = (Func<T, bool>)lambda.Compile();


                // Compile the lambda to a Func<T, bool>

                // Apply the predicate to the source
                return _source.Where(dict => predicate(ConvertDictionaryToType(dict))).ToList();
            }
        }

        throw new NotSupportedException("Only Where is supported for now.");
    }

    private T ConvertDictionaryToType(Dictionary<string, object> dict)
    {
        var obj = Activator.CreateInstance(typeof(T));
        foreach (var prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                     BindingFlags.SetProperty))
        {
            if (dict.ContainsKey(prop.Name))
            {
                prop.SetValue(obj, Convert.ChangeType(dict[prop.Name], prop.PropertyType));
            }
        }

        return (T)obj;
    }
}