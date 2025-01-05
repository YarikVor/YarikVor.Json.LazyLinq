using System.Collections;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using YarikVor.Common.PrimitiveConverters;
using YarikVor.Json.LazyLinq.JEnitites.Extensions;

namespace YarikVor.Json.LazyLinq.JEnitites;

public static class JSerializer
{
    public static JMetadata<T> GenerateGetter<T>()
    {
        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

        List<KeyValuePair<string, Action<T, JBase>>> setters = new();

        foreach (var propertyInfo in properties)
        {
            var name = propertyInfo.Name;
            var propertyType = propertyInfo.PropertyType;

            var pInstance = Expression.Parameter(typeof(T), "instance");
            var pJBase = Expression.Parameter(typeof(JBase), "jBase");

            if (propertyType == typeof(string))
            {
                var lambda = Expression.Lambda<Action<T, JBase>>(
                    Expression.Assign(
                        Expression.Property(pInstance, name),
                        Expression.Call(
                            Expression.Convert(pJBase, typeof(JString)),
                            nameof(JString.GetValue),
                            null
                        )
                    ),
                    pInstance,
                    pJBase
                );
                setters.Add(new KeyValuePair<string, Action<T, JBase>>(name, lambda.Compile()));
            }
            else if (propertyType.IsPrimitive)
            {
                if (propertyType == typeof(Int32))
                {
                    var lambda = Expression.Lambda<Action<T, JBase>>(
                        Expression.Assign(
                            Expression.Property(pInstance, name),
                            Expression.Call(
                                Expression.Convert(pJBase, typeof(JNumericValue)),
                                nameof(JNumericValue.GetInt32),
                                null
                            )
                        ),
                        pInstance,
                        pJBase
                    );
                    setters.Add(new KeyValuePair<string, Action<T, JBase>>(name, lambda.Compile()));
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                var lambda = Expression.Lambda<Action<T, JBase>>(
                    Expression.Assign(
                        Expression.Property(pInstance, name),
                        Expression.Convert(
                            Expression.Call(
                                typeof(JSerializer),
                                nameof(JSerializer.Deserialize),
                                null,
                                Expression.Convert(
                                    pJBase,
                                    typeof(JArray)
                                ),
                                Expression.Constant(propertyType)
                            ),
                            propertyType
                        )),
                    pInstance,
                    pJBase
                );
                setters.Add(new KeyValuePair<string, Action<T, JBase>>(name, lambda.Compile()));
            }
            else
            {
                throw new NotSupportedException($"{propertyType.FullName} is not supported");
            }
        }

        return new JMetadata<T>(setters.ToArray());
    }

    public class JQueryable<T>(Expression expression, IQueryProvider provider) : IQueryable<T>
    {
        public Type ElementType => typeof(T);

        public Expression Expression => expression;

        public IQueryProvider Provider => provider;

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)Array.Empty<T>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class JMetadata
    {
    }

    public class JMetadata<T>(KeyValuePair<string, Action<T, JBase>>[] setters) : JMetadata
    {
        public readonly KeyValuePair<string, Action<T, JBase>>[] Setters = setters;
    }

    private static readonly Dictionary<Type, JMetadata> Dict = new();

    private static JMetadata<T> GetGetters<T>() where T : class
    {
        if (Dict.TryGetValue(typeof(T), out var setters))
        {
            return (JMetadata<T>)setters;
        }
        else
        {
            var metadata = GenerateGetter<T>();
            Dict.Add(typeof(T), metadata);
            return metadata;
        }
    }

    public static T Deserialize<T>(JObject jObject) where T : class, new()
    {
        var instance = new T();
        var metadata = GetGetters<T>();
        var setters = metadata.Setters;
        var count = setters.Length;
        Span<int> span = stackalloc int[count];
        for (int i = 0; i < count; i++)
        {
            span[i] = i;
        }

        foreach (var property in jObject.GetFastValues())
        {
            for (var i = 0; i < count; i++)
            {
                var (key, value) = setters[span[i]];
                if (!property.Equals(key))
                    continue;
                value(instance, property.Value);
                --count;
                if (i != count)
                    span[i] = span[count];

                if (count == 0)
                    return instance;
            }
        }

        return instance;
    }


    public static string JDeserialize(JString jString)
    {
        return jString.GetValue();
    }

    public static T JDeserialize<T>(JNumericValue jValue) where T : INumberBase<T>
    {
        return jValue.GetValue<T>();
    }

    public static bool JDeserialize<T>(JBooleanValue jValue)
    {
        return jValue.GetValue();
    }

    public static T? Deserialize<T>(string source)
    {
        var index = 0;
        for (; index < source.Length; ++index)
        {
            var c = source[index];
            if (!c.IsExSpace())
                break;
        }

        if (index == source.Length)
            throw new KeyNotFoundException();

        var jBase = JDocument.GetAndConverted(source, index);

        if (jBase is JNullValue)
        {
            if (typeof(T).IsClass || typeof(T).IsAbstract || typeof(T).IsInterface)
            {
                return default;
            }

            throw new KeyNotFoundException();
        }

        if (typeof(T) == typeof(string) && jBase is JString jString)
        {
            return (T)(object)JDeserialize(jString);
        }

        if (typeof(T) == typeof(bool) && jBase is JBooleanValue jBoolean)
        {
            return (T)(object)jBoolean.GetValue();
        }


        if (jBase is JNumericValue jNumericValue &&
            typeof(T).IsAssignableFrom(typeof(INumberBase<>).MakeGenericType(typeof(T))))
        {
            var method = typeof(JNumericValue).GetMethod(nameof(JNumericValue.GetValue),
                BindingFlags.Instance | BindingFlags.Public);
            var generatedMethod = method.MakeGenericMethod(typeof(T));
            return (T)generatedMethod.Invoke(jNumericValue, null)!;
        }


        if (jBase is JArray jArray && typeof(IEnumerable).IsAssignableFrom(typeof(T)))
        {
            return (T)Deserialize(jArray, typeof(T));
        }

        if (jBase is JObject jObject)
        {
            var method = typeof(JSerializer).GetMethod(nameof(Deserialize), BindingFlags.Static | BindingFlags.Public,
                [typeof(JObject)]);
            var genericMethod = method.MakeGenericMethod(typeof(T));

            return (T)genericMethod.Invoke(null, [jObject])!;
        }

        throw new KeyNotFoundException();
    }

    private static IList Deserialize(JArray jArray, Type type)
    {
        var ienumerableInterface = type.GetInterface("IEnumerable`1");
        var subType = ienumerableInterface?.GetGenericArguments()[0] ?? type.GenericTypeArguments[0];

        if (subType == typeof(string))
        {
            return ToStringList(jArray);
        }
        else if (subType.IsClass)
        {
            var typeList = typeof(List<>).MakeGenericType(subType);
            var list = (IList)Activator.CreateInstance(typeList)!;
            var method = typeof(JSerializer).GetMethod(nameof(Deserialize), BindingFlags.Static | BindingFlags.Public,
                [typeof(JObject)]);
            var genericMethod = method.MakeGenericMethod(subType);
            foreach (var jObject in jArray.GetRequiredValues<JObject>())
            {
                var obj = genericMethod.Invoke(null, [jObject]);
                list.Add(obj);
            }

            jArray.GetRequiredValues<JObject>().ToArray();
            return list;
        }
        else if (subType.IsPrimitive)
        {
            if (subType == typeof(bool))
            {
                return jArray.GetRequiredValues<JBooleanValue>().Select(t => t.GetValue()).ToList();
            }
            else
            {
                var t = typeof(JNumericValue).GetMethod(nameof(JNumericValue.GetValue),
                    BindingFlags.Instance | BindingFlags.Public);
                var genericMethod = t.MakeGenericMethod(subType);
                var typeList = typeof(List<>).MakeGenericType(subType);
                var list = (IList)Activator.CreateInstance(typeList)!;
                foreach (var jNumericValue in jArray.GetRequiredValues<JNumericValue>())
                {
                    var obj = genericMethod.Invoke(jNumericValue, null);
                    list.Add(obj);
                }

                return list;
            }
        }
        

        return null;
    }

    private static List<string> ToStringList(JArray jArray)
    {
        return jArray.GetRequiredValues<JString>().Select(t => t.GetValue()).ToList();
    }
}