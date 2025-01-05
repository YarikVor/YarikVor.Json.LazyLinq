using System.Runtime.CompilerServices;

namespace YarikVor.Json.LazyLinq.JEnitites;

public static class JReader
{
    public static JNumericValue ReadFrom(string source, int position)
    {
        var index = position;
        var hasPoint = false;
        begin:
        for (;; ++index)
        {
            var c = source[index];
            if (!char.IsAsciiDigit(c))
                break;
        }

        if (source[index] == '.')
        {
            if (hasPoint)
                throw new FormatException();
            hasPoint = true;
            ++index;
            goto begin;
        }
        if (source[index] is 'e')
        {
            for (;; ++index)
            {
                var c = source[index];
                if (!char.IsAsciiDigit(c))
                    break;
            }
        }
        
        return new JNumericValue(position, index - 1, source);
    }
    
}