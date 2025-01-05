using YarikVor.Json.LazyLinq.JEnitites.Extensions;

namespace YarikVor.Json.LazyLinq.JEnitites.Legacy;

public abstract class JContainer<T>(int startIndex, string source) : JBase(startIndex, source)
    where T: JBase
{
    private bool _finally;
    private int _endIndex = -1;


    public sealed override int SkipToNext()
    {
        if (!_finally)
        {
            _endIndex = ReadToEndObj();
            _finally = true;
        }

        return _endIndex;
    }
    
    protected abstract char OpenChar { get; }
    protected abstract char CloseChar { get; }

    private int ReadToEndObj()
    {
        var countOpen = 0;
        var openChar = OpenChar;
        var closeChar = CloseChar;

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

                continue;
            }
            if (c == closeChar)
            {
                if (countOpen == 0)
                {
                    return i;
                }
                --countOpen;
                continue;
            }
            if (c == openChar)
            {
                ++countOpen;
            }
        }

        throw new FormatException();
    }
    
    public IEnumerable<T> GetValues()
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
            }

            if (c == CloseChar)
            {
                _finally = true;
                _endIndex = i;
                yield break;
            }

            var t = OrDefault(c, i);
            yield return t;
            i = t.SkipToNext();
            requireComma = true;
        }
    }

    protected abstract T OrDefault(char c, int i);
}