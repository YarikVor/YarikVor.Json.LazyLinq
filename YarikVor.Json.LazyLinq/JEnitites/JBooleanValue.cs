namespace YarikVor.Json.LazyLinq.JEnitites;

public sealed class JBooleanValue(int startIndex, string source) : JBase(startIndex, source)
{
    public bool GetValue()
    {
        return SourceText[StartIndex] switch
        {
            JConstants.TrueFirstChar => true,
            JConstants.FalseFirstChar => false,
            _ => throw new FormatException()
        };
    }

    public override int SkipToNext()
    {
        for (var i = StartIndex; i < SourceText.Length; i++)
        {
            if (!char.IsAsciiLetter(SourceText[i]))
                return i - 1;
        }
        throw new FormatException();
    }
}