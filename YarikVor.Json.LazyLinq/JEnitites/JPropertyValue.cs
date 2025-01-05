using System.Diagnostics.CodeAnalysis;
using System.Text;
using YarikVor.Json.LazyLinq.JEnitites.Extensions;

namespace YarikVor.Json.LazyLinq.JEnitites;

public struct JPropertyValue(IntRange range, JBase value, string? key = null)
{
    public readonly JBase Value = value;
    private string? _key = key;

    public string GetKey()
    {
        return _key ??= GetKeySpan().ToString();
    }
    
    public readonly ReadOnlySpan<char> GetKeySpan()
    {
        if (_key is not null)
        {
            return _key.AsSpan();
        }
        
        return Value.SourceText.AsSpan(range.Start + 1, range.End - range.Start - 1);
    }

    public bool Equals(string value)
    {
        return _key is null ? GetKeySpan().SequenceEqual(value) : value == GetKey();
    }

    public static JPropertyValue Create(int position, string source)
    {
        var index = position + 1;
        for (;; ++index)
        {
            var c = source[index];

            if (c is '"')
                return new JPropertyValue(new IntRange(position, index), GetValue(index, source));
            if (c is '\\')
                break;
        }

        var lastIndex = 0;
        var sb = new StringBuilder();
        
        for(;; ++index)
        {
            var c = source[index];
            if (c is '\\')
            {
                sb.Append(source.AsSpan(lastIndex, index - lastIndex));
                ++index;

                // TODO:
                lastIndex = index + 1;
                continue;
            }

            if (c is '"')
            {
                sb.Append(source.AsSpan(lastIndex, index - lastIndex));
                var text = sb.ToString();
                return new JPropertyValue(new IntRange(position, index), GetValue(index, source), text);
            }

            throw new Exception();
        }
    }

    public int SkipToNext()
    {
        return Value.SkipToNext();
    }

    private static JBase GetValue(int position, string source)
    {
        var startIndex = position + 1;
        
        for (int i = startIndex;; i++)
        {
            var c = source[i];
            if (c.IsExSpace())
                continue;
            if (c is ':')
            {
                startIndex = i + 1;
                break;
            }
            else
                throw new JException();
        }

        for (int i = startIndex; ; ++i)
        {
            var c = source[i];
            if (c.IsExSpace())
                continue;

            return JDocument.GetAndConverted(source, i);
        }
        
        throw new JException();
    }

    public bool TryGetRequiredValue<T>([NotNullWhen(true)] out T? value) where T : JBase
    {
        if (Value is T jValue)
        {
            value = jValue;
            return true;
        }
        
        value = default;
        return false;
    }

}