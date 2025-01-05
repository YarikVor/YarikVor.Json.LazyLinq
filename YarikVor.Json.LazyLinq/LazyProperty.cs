using System.Text.Json;

namespace YarikVor.Json.LazyLinq;

public class LazyProperty<T>
{
    public T Value { get; }

    public bool HasValue { get; }
    
    public LazyProperty(JsonProperty property)
    {
        
    }

    
}