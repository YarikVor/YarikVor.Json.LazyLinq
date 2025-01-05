using System.Diagnostics.CodeAnalysis;
using YarikVor.Json.LazyLinq.JEnitites.Extensions;

namespace YarikVor.Json.LazyLinq.JEnitites;

public sealed class JProperty(int startIndex, string source) : JBase(startIndex, source)
{
    public readonly JString JKey = new(startIndex, source);
    private JBase? _value;

    public override int SkipToNext()
    {
        var jBase = GetValue();
        return jBase.SkipToNext();
    }

    public JBase GetValue()
    {
        if (_value != null)
            return _value;
        
        var startIndex = JKey.SkipToNext() + 1;
        for (int i = startIndex;; i++)
        {
            var c = SourceText[i];
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
            var c = SourceText[i];
            if (c.IsExSpace())
                continue;

            _value = JDocument.GetAndConverted(SourceText, i);

            return _value;
        }
    }

    public bool TryGetRequiredValue<T>([NotNullWhen(true)] out T? value) where T : JBase
    {
        if (_value is null)
        {
            GetValue();
        }
        
        if (_value is T jValue)
        {
            value = jValue;
            return true;
        }
        
        value = default;
        return false;
    }

    public T GetRequiredValue<T>() where T : JBase
    {
        if (_value != null)
            return (T)_value;
        
        var startIndex = JKey.SkipToNext() + 1;
        for (int i = startIndex;; i++)
        {
            var c = SourceText[i];
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

        var creator = JInfo<T>.Creator;

        for (int i = startIndex; ; ++i)
        {
            var c = SourceText[i];
            if (c.IsExSpace())
                continue;

            var result = creator((SourceText, i));
            _value = result;
            return result;
        }
    }

}