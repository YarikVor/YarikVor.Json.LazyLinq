using YarikVor.Json.LazyLinq.JEnitites.Extensions;

namespace YarikVor.Json.LazyLinq.JEnitites;

public class JDocument
{
    public static JType GetByChar(char c)
    {
        if (char.IsDigit(c))
            return JType.Number;
        return c switch
        {
            JConstants.OpenArrayChar => JType.Array,
            JConstants.OpenObjectChar => JType.Object,
            '-' => JType.Number,
            JConstants.QuoteChar => JType.String,
            't' or 'f' => JType.Boolean,
            'n' => JType.Null,
            _ => JType.None
        };
    }

    public static JBase GetAndConverted(string source, int position)
    {
        var c = source[position];
        if (char.IsDigit(c))
            return JReader.ReadFrom(source, position);
        return c switch
        {
            JConstants.QuoteChar => new JString(position, source),
            '-' => JReader.ReadFrom(source, position),
            JConstants.OpenObjectChar => new JObject(position, source),
            JConstants.OpenArrayChar => new JArray(position, source),
            't' or 'f' => new JBooleanValue(position, source),
            'n' => new JNullValue(position, source),
            _ => throw new FormatException($"Invalid the char {c}")
        };
    }
}