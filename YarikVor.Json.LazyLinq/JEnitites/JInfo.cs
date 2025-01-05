namespace YarikVor.Json.LazyLinq.JEnitites;

public static class JInfo
{
    public static readonly Dictionary<Type, Func<(string str, int position), JBase>> Converters =
        new()
        {
            {
                typeof(JProperty), t =>
                {
                    var c = t.str[t.position];
                    return c == '"'
                        ? new JProperty(t.position, t.str)
                        : throw new InvalidCastException($"Invalid character '{c}' for Property value.");
                }
            },
            {
                typeof(JBooleanValue), t =>
                {
                    var c = t.str[t.position];
                    return char.IsDigit(c) || c == 't' || c == 'f'
                        ? new JBooleanValue(t.position, t.str)
                        : throw new InvalidCastException($"Invalid character '{c}' for Boolean value.");
                }
            },
            {
                typeof(JNullValue), t =>
                {
                    var c = t.str[t.position];
                    return c == 'n'
                        ? new JNullValue(t.position, t.str)
                        : throw new InvalidCastException($"Invalid character '{c}' for Null value.");
                }
            },
            {
                typeof(JNumericValue), t =>
                {
                    var c = t.str[t.position];
                    return char.IsDigit(c) || c == '-'
                        ? JReader.ReadFrom(t.str, t.position)
                        : throw new InvalidCastException($"Invalid character '{c}' for Numeric value.");
                }
            },
            {
                typeof(JObject), t =>
                {
                    var c = t.str[t.position];
                    return c == JConstants.OpenObjectChar
                        ? new JObject(t.position, t.str)
                        : throw new InvalidCastException($"Invalid character '{c}' for Object value.");
                }
            },
            {
                typeof(JArray), t =>
                {
                    var c = t.str[t.position];
                    return c == JConstants.OpenArrayChar
                        ? new JArray(t.position, t.str)
                        : throw new InvalidCastException($"Invalid character '{c}' for Array value.");
                }
            },
            {
                typeof(JString), t =>
                {
                    var c = t.str[t.position];
                    return c == JConstants.QuoteChar
                        ? new JString(t.position, t.str)
                        : throw new InvalidCastException($"Invalid character '{c}' for String value.");
                }
            },
        };
}

public static class JInfo<T> where T : JBase
{
    public static readonly Func<(string str, int pos), T> Creator;
    public static readonly Func<(string str, int pos), JBase> BaseCreator;

    static JInfo()
    {
        BaseCreator = JInfo.Converters[typeof(T)];
        Creator = t => (T)BaseCreator(t);
    }
}