using System.Diagnostics;
using YarikVor.Json.LazyLinq.JEnitites;
using YarikVor.Json.LazyLinq.JEnitites.Extensions;

namespace YarikVor.Json.LazyLinq;


public sealed class JObject(int startIndex, string source) : JBase(startIndex, source)
{
    private int _endIndex = -1;
    private bool _finally;
    
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
        var countCloseObject = 0;
        var offset = (_endIndex == -1) ? (StartIndex + 1) : _endIndex;
        var span = SourceText.AsSpan(offset);
        for (int i = 0; i < span.Length; i++)
        {
            var c = span[i];

            switch (c)
            {
                case JConstants.QuoteChar:
                {
                    ++i;
                    for (;; ++i)
                    {
                        c = span[i];
                        if (c is '\\')
                            ++i;
                        else if (c is JConstants.QuoteChar)
                            break;
                    }

                    continue;
                }
                case JConstants.CloseObjectChar when countCloseObject == 0:
                    return i + offset;
                case JConstants.CloseObjectChar:
                    --countCloseObject;
                    break;
                case JConstants.OpenObjectChar:
                    ++countCloseObject;
                    break;
            }
        }

        throw new FormatException();
    }

    public IEnumerable<JProperty> GetValues()
    {
        var requireComma = false;

        for (int i = StartBody;; ++i)
        {
            var c = SourceText[i];

            if (c.IsExSpace())
                continue;

            switch (c)
            {
                case JConstants.CommaChar when requireComma:
                    _endIndex = i + 1;
                    requireComma = false;
                    continue;
                case JConstants.CommaChar when !requireComma:
                    throw new InvalidOperationException("Unexpected end of comma");
                case JConstants.CloseObjectChar:
                    _endIndex = i;
                    _finally = true;
                    yield break;
                case JConstants.QuoteChar:
                {
                    var prop = new JProperty(i, SourceText);
                    yield return prop;
                    i = prop.SkipToNext();
                    requireComma = true;
                }
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected JValue type: {c}");
            }
        }
    }

    public IEnumerable<JPropertyValue> GetFastValues()
    {
        for (int i = StartBody;; ++i)
        {
            var c = SourceText[i];
            if (c.IsExSpace())
                continue;
            switch (c)
            {
                case JConstants.QuoteChar:
                {
                    var prop = JPropertyValue.Create(i, SourceText);
                    try
                    {
                        yield return prop;
                    }
                    finally
                    {
                       _endIndex = prop.SkipToNext() + 1;
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
                        else if (c is JConstants.CloseObjectChar)
                        {
                            _endIndex = i;
                            _finally = true;
                            yield break;
                        }

                        throw new InvalidOperationException($"Unexpected JValue type: {c}");
                    }
                } 
                    break;
                case JConstants.CloseObjectChar:
                    _endIndex = i;
                    _finally = true;
                    yield break;
                default:
                    throw new InvalidOperationException($"Unexpected JValue type: {c}");
            }
        }
    }
}