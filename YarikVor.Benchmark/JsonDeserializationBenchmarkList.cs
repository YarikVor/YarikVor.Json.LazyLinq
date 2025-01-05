using System.Buffers;
using YarikVor.Json.LazyLinq;
using YarikVor.Json.LazyLinq.JEnitites;

namespace YarikVor.Benchmark;

using BenchmarkDotNet.Attributes;
using System.Text.Json;
using Newtonsoft.Json;


[MemoryDiagnoser] // Вимірює використання пам'яті
[DisassemblyDiagnoser]
public class JsonDeserializationBenchmarkList
{
    private readonly string JsonData;

    public JsonDeserializationBenchmarkList()
    {
        JsonData = File.ReadAllText("data.json");
    }

    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        
    }

    [Benchmark(Baseline = true)]
    public List<Item> SystemTextJson()
    {
        return System.Text.Json.JsonSerializer.Deserialize<List<Item>>(JsonData);
    }

    [Benchmark]
    public List<Item> NewtonsoftJson()
    {
        return JsonConvert.DeserializeObject<List<Item>>(JsonData);
    }

    [Benchmark]
    public List<Item> MyCustomJsonParser_UseCounter()
    {
        // Тут реалізація кастомного парсера
        var result = new List<Item>();
        var jArray = JDocument.GetAndConverted(JsonData, 0) as JArray;
        foreach (var jObject in jArray.GetRequiredValues<JObject>())
        {
            var item = new Item();
            int countProps = 2;
            foreach (var property in jObject.GetFastValues())
            {
                if (property.Equals("id") && property.TryGetRequiredValue<JNumericValue>(out var idValue))
                {
                    item.id = idValue.GetInt32();
                }
                else if (property.Equals("name") && property.TryGetRequiredValue<JString>(out var nameValue))
                {
                    item.name = nameValue.GetValue();
                }
                else
                {
                    continue;
                }

                --countProps;
                if (countProps == 0)
                    break;
            }
            result.Add(item);
        }
        return result;
    }


    [Benchmark]
    public List<Item> MyCustomJsonParser_UseCounterAndBool()
    {
        // Тут реалізація кастомного парсера
        var result = new List<Item>();
        var jArray = JDocument.GetAndConverted(JsonData, 0) as JArray;
        foreach (var jObject in jArray.GetRequiredValues<JObject>())
        {
            var item = new Item();
            var countProps = 2;
            var bId = true;
            var bName = true;
            foreach (var property in jObject.GetFastValues())
            {
                if (bId && property.Equals("id") && property.TryGetRequiredValue<JNumericValue>(out var idValue))
                {
                    item.id = idValue.GetInt32();
                    bId = false;
                }
                else if (bName && property.Equals("name") && property.TryGetRequiredValue<JString>(out var nameValue))
                {
                    item.name = nameValue.GetValue();
                    bName = false;
                }
                else
                {
                    continue;
                }

                --countProps;
                if (countProps == 0)
                    break;
            }
            result.Add(item);
        }
        return result;
    }
    
    
    
    [Benchmark]
    public List<Item> MyCustomJsonParser_UseStackalloc()
    {
        var result = new List<Item>();
        var jArray = JDocument.GetAndConverted(JsonData, 0) as JArray;
        Span<int> span = stackalloc int[Setters.Length];
        
        foreach (var jObject in jArray.GetRequiredValues<JObject>())
        {
            var item = new Item();
            var offset = 0;
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = i;
            }
            foreach (var property in jObject.GetFastValues())
            {
                for (var i = offset; i < span.Length; i++)
                {
                    var (key, value) = Setters[span[i]];
                    if (!property.Equals(key))
                        continue;
                    value(item, property.Value);
                    span[i] = -1;
                    if (i != offset)
                        span.Sort();
                    ++offset;
                    if (offset == span.Length)
                        goto add_item;
                    break;
                }
            }
            add_item:
            result.Add(item);
        }
        return result;
    }

    private static readonly KeyValuePair<string, Action<Item, JBase>>[] Setters =
    [
        new ("id", (i, j) => i.id = ((JNumericValue)j).GetInt32()),
        new ("name", (i, j) => i.name = ((JString)j).GetValue())
    ];
    
    [Benchmark]
    public List<Item> MyCustomJsonParser_UseStackalloc_2()
    {
        var result = new List<Item>();
        var jArray = JDocument.GetAndConverted(JsonData, 0) as JArray;
        Span<int> span = stackalloc int[Setters.Length];
        
        foreach (var jObject in jArray.GetRequiredValues<JObject>())
        {
            var item = new Item();
            var count = span.Length;
            for (var i = 0; i < count; i++)
            {
                span[i] = i;
            }
            foreach (var property in jObject.GetFastValues())
            {
                for (var i = 0; i < count; i++)
                {
                    var (key, value) = Setters[span[i]];
                    if (!property.Equals(key))
                        continue;
                    value(item, property.Value);
                    --count;
                    if (i != count)
                        span[i] = span[count];
                    
                    if (count == 0)
                        goto add_item;
                }
            }
            add_item:
            result.Add(item);
        }
        return result;
    }
}
