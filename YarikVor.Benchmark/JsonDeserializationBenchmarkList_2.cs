using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using YarikVor.Json.LazyLinq;
using YarikVor.Json.LazyLinq.JEnitites;

namespace YarikVor.Benchmark;

using BenchmarkDotNet.Attributes;
using System.Text.Json;
using Newtonsoft.Json;

[MemoryDiagnoser] // Вимірює використання пам'яті
[DisassemblyDiagnoser]
public class JsonDeserializationBenchmarkList_2
{
    private readonly string? data;
    
    private readonly string JsonData;

    public JsonDeserializationBenchmarkList_2()
    {
        JsonData = File.ReadAllText("data.json");
    }

    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }

        public string description { get; set; }
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
            int countProps = 3;
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
                else if (property.Equals("description") &&
                         property.TryGetRequiredValue<JString>(out var descriptionValue))
                {
                    item.description = descriptionValue.GetValue();
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
            var countProps = 3;
            var bId = true;
            var bName = true;
            var bDescription = true;
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
                else if (bDescription && property.Equals("description") &&
                         property.TryGetRequiredValue<JString>(out var descriptionValue))
                {
                    item.description = descriptionValue.GetValue();
                    bDescription = false;
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
    public List<Item> MyCustomJsonParser_UseCounterAndBool_2()
    {
        // Тут реалізація кастомного парсера
        var result = new List<Item>();
        var jArray = JDocument.GetAndConverted(JsonData, 0) as JArray;
        foreach (var jObject in jArray.GetRequiredValues<JObject>())
        {
            var item = new Item();
            var countProps = 3;
            var bId = true;
            var bName = true;
            var bDescription = true;
            foreach (var property in jObject.GetFastValues())
            {
                if (bId && property.Equals("id") && property.TryGetRequiredValue<JNumericValue>(out var idValue))
                {
                    item.id = idValue.GetInt32T();
                    bId = false;
                }
                else if (bName && property.Equals("name") && property.TryGetRequiredValue<JString>(out var nameValue))
                {
                    item.name = nameValue.GetValue();
                    bName = false;
                }
                else if (bDescription && property.Equals("description") &&
                         property.TryGetRequiredValue<JString>(out var descriptionValue))
                {
                    item.description = descriptionValue.GetValue();
                    bDescription = false;
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
    public List<Item> MyCustomJsonParser_UseCounterAndBool_3()
    {
        // Тут реалізація кастомного парсера
        var result = new List<Item>();
        var jArray = JDocument.GetAndConverted(JsonData, 0) as JArray;
        foreach (var jObject in jArray.GetRequiredValues<JObject>())
        {
            var item = new Item();
            var countProps = 3;
            var bId = true;
            var bName = true;
            var bDescription = true;
            foreach (var property in jObject.GetFastValues())
            {
                if (bId && property.Equals("id"))
                {
                    if (property.Value is JNumericValue idValue)
                    {
                        item.id = idValue.GetInt32();
                        bId = false;
                    }
                    else
                    {
                        Throw();
                    }
                }
                else if (bName && property.Equals("name"))
                {
                    if (property.Value is JString idValue)
                    {
                        item.name = idValue.GetValue();
                        bName = false;
                    }
                    else
                    {
                        Throw();
                    }
                }
                else if (bDescription && property.Equals("description"))
                {
                    if (property.Value is JString idValue)
                    {
                        item.description = idValue.GetValue();
                        bDescription = false;
                    }
                    else
                    {
                        Throw();
                    }
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

    [DoesNotReturn]
    public static void Throw()
    {
        throw new Exception();
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
        new("id", (i, j) => i.id = ((JNumericValue)j).GetInt32()),
        new("name", (i, j) => i.name = ((JString)j).GetValue()),
        new("description", (i, j) => i.description = ((JString)j).GetValue()),
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

    private static readonly Dictionary<string, Action<Item, JBase>> SettersDict =
        Setters.ToDictionary();


    [Benchmark]
    public List<Item> MyCustomJsonParser_UseDictionary()
    {
        var result = new List<Item>();
        var jArray = JDocument.GetAndConverted(JsonData, 0) as JArray;

        foreach (var jObject in jArray.GetRequiredValues<JObject>())
        {
            var item = new Item();
            var count = SettersDict.Count;

            foreach (var property in jObject.GetFastValues())
            {
                var propsName = property.GetKey();
                if (!SettersDict.TryGetValue(propsName, out var setter)) continue;
                setter(item, property.Value);
                --count;
                if (count == 0)
                    break;
            }

            add_item:
            result.Add(item);
        }

        return result;
    }

    [Benchmark]
    public List<Item> MyCustomJsonParser_UseExpression()
    {
        var jArray = JDocument.GetAndConverted(JsonData, 0) as JArray;

        return jArray
            .GetRequiredValues<JObject>()
            .Select(JSerializer.Deserialize<Item>)
            .ToList();
    }

    [Benchmark]
    public List<Item> JSerialize_Deserialize()
    {
        return JSerializer.Deserialize<List<Item>>(JsonData)!;
    }
}