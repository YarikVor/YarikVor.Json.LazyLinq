namespace YarikVor.Json.LazyLinq.JEnitites.Legacy;

public sealed class JObject0(int startIndex, string source) : JContainer<JProperty>(startIndex, source)
{
    protected override char OpenChar => JConstants.OpenObjectChar;
    protected override char CloseChar => JConstants.CloseObjectChar;
    protected override JProperty OrDefault(char c, int i)
    {
        if (c is JConstants.QuoteChar)
        {
            return new JProperty(i, SourceText);
        }
        throw new InvalidOperationException($"Unexpected JValue type: {c}");
    }
}