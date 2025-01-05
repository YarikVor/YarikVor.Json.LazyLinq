using YarikVor.Json.LazyLinq.JEnitites;

namespace YarikVor.Common.PrimitiveConverters;

public static partial class PrimitiveConvertors
{
    public static bool FastParseBoolean(ReadOnlySpan<char> value)
    {
        var c = value[0];
        switch (c)
        {
            case 't' or 'T':
                return true;
            case 'f' or 'F':
                return false;
            default:
                ThrowHelper.ThrowFormat($"{value} is not a valid boolean value");
                return false;
        }
    }

    public static bool ParseBoolean(ReadOnlySpan<char> value)
    {
        var fastParse = FastParseBoolean(value);
        if (fastParse)
        {
            if (value[1..] is "rue")
                return true;
        }
        else
        {
            if (value[1..] is "alse")
                return false;
        }

        ThrowHelper.ThrowFormat($"{value} is not a valid boolean value");
        return false;
    }
}