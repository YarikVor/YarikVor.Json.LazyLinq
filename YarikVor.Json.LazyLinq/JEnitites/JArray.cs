using YarikVor.Json.LazyLinq.JEnitites.Extensions;

namespace YarikVor.Json.LazyLinq.JEnitites;

public sealed class JArray(int startIndex, string source) : JBase(startIndex, source)
{
    private int _endIndex = -1;
    private bool _finally;

    public IEnumerable<JBase> GetValues()
    {
        bool requireComma = false;

        for (int i = StartIndex + 1;; ++i)
        {
            var c = SourceText[i];
            if (c.IsExSpace())
                continue;

            switch (c)
            {
                case JConstants.CommaChar when requireComma:
                    requireComma = false;
                    continue;
                case JConstants.CommaChar when !requireComma:
                    throw new InvalidOperationException("Unexpected end of comma");
                case JConstants.CloseArrayChar:
                {
                    _finally = true;
                    _endIndex = i;
                }
                    yield break;
                default:
                {
                    var t = JDocument.GetAndConverted(SourceText, i);
                    yield return t;
                    i = t.SkipToNext();
                    requireComma = true;
                }
                 break;
            }
        }
    }

    public IEnumerable<TValue> GetRequiredValues<TValue>() where TValue : JBase
    {
        var creator = JInfo<TValue>.Creator;
        
        for (int i = StartIndex + 1;; ++i)
        {
            var c = SourceText[i];
            if (c.IsExSpace())
                continue;

            switch (c)
            {
                case JConstants.CloseArrayChar:
                    _finally = true;
                    _endIndex = i;
                    yield break;
                default:
                {
                    var t = creator((SourceText, i));
                    try
                    {
                        yield return t;
                    }
                    finally
                    {
                        _endIndex = t.SkipToNext() + 1;
                    }
                    i = _endIndex;
                    for (;; ++i)
                    {
                        c = SourceText[i];
                        if (c.IsExSpace())
                            continue;
                        if (c is JConstants.CommaChar)
                        {
                            _endIndex = i + 1;
                            break;
                        }
                        else if (c is JConstants.CloseArrayChar)
                        {
                            _endIndex = i;
                            _finally = true;
                            yield break;
                        }
                        throw new InvalidOperationException($"Unexpected JValue type: {c}");
                    }
                }
                break;
            }
        }
    }

    public override int SkipToNext()
    {
        if (!_finally)
        {
            _endIndex = ReadToEndObj();
            _finally = true;
        }

        return _endIndex;
    }

    private int ReadToEndObj()
    {
        var countOpenArray = 0;

        for (int i = _endIndex == -1 ? (StartIndex + 1) : _endIndex; i < SourceText.Length; ++i)
        {
            var c = SourceText[i];

            if (c is JConstants.QuoteChar)
            {
                ++i;
                for (;; ++i)
                {
                    c = SourceText[i];
                    if (c is '\\')
                    {
                        ++i;
                    }
                    else if (c is JConstants.QuoteChar)
                    {
                        break;
                    }
                }
            }
            else if (c is JConstants.CloseArrayChar)
            {
                if (countOpenArray == 0)
                {
                    return i;
                }
                --countOpenArray;
            }
            else if (c is JConstants.OpenArrayChar)
            {
                ++countOpenArray;
            }
        }

        throw new FormatException();
    }
}