using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace YarikVor.Json.LazyLinq.JEnitites.Queryable;



public class JObjectCache(JObject jObject): IDisposable
{
    private struct JObjectCacheElement(JPropertyValue propertyValue)
    {
        public int HashCode;
        public JPropertyValue PropertyValue = propertyValue;

        public int GetPropertyHashCode()
        {
            if (HashCode != 0) 
                return HashCode;
            
            var span = PropertyValue.GetKeySpan();
            
            var hash = string.GetHashCode(span);
            HashCode = hash == 0 ? 1 : hash;

            return HashCode;
        }
    }

    public JBase? this[string key] => GetValue(key);

    public JBase? GetValue(string key, int keyHash)
    {
        var span = CollectionsMarshal.AsSpan(_items);
        for (int i = 0; i < span.Length; i++)
        {
            ref var refItem = ref span[i];
            var hash = refItem.GetPropertyHashCode();
            ref var refPropertyValue = ref refItem.PropertyValue;
            if (hash == keyHash && refPropertyValue.Equals(key))
            {
                return refPropertyValue.Value;
            }
        }
        
        if (_enumerator is null)
            return null;
        
        while (_enumerator.MoveNext())
        {
            var item = _enumerator.Current;
            var cacheElement = new JObjectCacheElement(item);
            var hash = cacheElement.GetPropertyHashCode();
            try
            {
                if (hash == keyHash && item.Equals(key))
                {
                    return cacheElement.PropertyValue.Value;
                }
            }
            finally
            {
                _items.Add(cacheElement);
            }
        }
        _enumerator.Dispose();
        _enumerator = null;
        return null;
    }
    
    public JBase? GetValue(string key)
    {
        var keyHash = key.GetHashCode();
        return GetValue(key, keyHash);
    }
    
    private readonly List<JObjectCacheElement> _items = new();
    private IEnumerator<JPropertyValue>? _enumerator = jObject.GetFastValues().GetEnumerator();

    public void Dispose()
    {
        _enumerator?.Dispose();
    }
}