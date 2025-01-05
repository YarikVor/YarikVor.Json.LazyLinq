namespace YarikVor.Json.LazyLinq.JEnitites.Legacy;

public sealed class JArray0(int startIndex, string source) : JContainer<JBase>(startIndex, source)
{
    protected override char OpenChar => JConstants.OpenArrayChar;
    protected override char CloseChar => JConstants.CloseArrayChar;
    
    protected override JBase OrDefault(char c, int i)
    {
        return JDocument.GetAndConverted(SourceText, i);
    }
}

