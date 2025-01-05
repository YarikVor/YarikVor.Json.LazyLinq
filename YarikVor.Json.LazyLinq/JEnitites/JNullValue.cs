namespace YarikVor.Json.LazyLinq.JEnitites;

public sealed class JNullValue(int startIndex, string source) : JBase(startIndex, source)
{
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