namespace YarikVor.Json.LazyLinq.JEnitites;

public abstract class JBase(int startIndex, string sourceText)
{
    public readonly string SourceText = sourceText;
    protected readonly int StartIndex = startIndex;
    protected int StartBody => StartIndex + 1;

    public abstract int SkipToNext();

    public JType GetJType()
    {
        return this switch
        {
            JProperty => JType.Property,
            JBooleanValue => JType.Boolean,
            JNullValue => JType.Null,
            JNumericValue => JType.Number,
            JObject => JType.Object,
            JArray => JType.Array,
            JString => JType.String,
            _ => JType.Null
        };
    }
}

