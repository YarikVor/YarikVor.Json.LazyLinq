using System.Buffers;
using System.Runtime.CompilerServices;
using YarikVor.Json.LazyLinq;
using YarikVor.Json.LazyLinq.JEnitites;
using YarikVor.Json.LazyLinq.JEnitites.Queryable;

namespace YarikVor.Benchmark;

using BenchmarkDotNet.Attributes;
using System.Text.Json;
using Newtonsoft.Json;

[MemoryDiagnoser] // Вимірює використання пам'яті
[DisassemblyDiagnoser]
public class JsonDeserializationBenchmarkList_3
{
    private readonly string? data;
    
    private readonly string JsonData;

    public JsonDeserializationBenchmarkList_3()
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
        return System.Text.Json.JsonSerializer
            .Deserialize<IEnumerable<Item>>(JsonData)
            .Where(i => i.id > 5)
            .ToList();
    }

    [Benchmark]
    public List<Item> NewtonsoftJson()
    {
        return JsonConvert
            .DeserializeObject<IEnumerable<Item>>(JsonData)
            .Where(i => i.id > 5)
            .ToList();
    }

    /*[Benchmark]
    public List<Item> MyCustomJsonParser_UseExpression()
    {
        var jArray = JDocument.GetAndConverted(JsonData, 0) as JArray;

        return jArray
            .GetRequiredValues<JObject>()
            .Select(t => new JObjectCache(t))
            .Where(c => ((JNumericValue)c["id"]).GetInt32AndCached() > 5)
            .Select(c => new Item()
            {
                id = ((JNumericValue)c["id"]).GetInt32AndCached(),
                name = ((JString)c["name"]).GetValue(),
                description = ((JString)c["description"]).GetValue(),
            })
            .ToList();
    }*/
    
    [Benchmark]
    public List<Item> MyCustomJsonParser_UseExpression_WithoutCache()
    {
        var jArray = JDocument.GetAndConverted(JsonData, 0) as JArray;

        return jArray
            .GetRequiredValues<JObject>()
            .Select(t => new JObjectCache(t))
            .Where(c => ((JNumericValue)c["id"]).GetInt32() > 5)
            .Select(c => new Item()
            {
                id = ((JNumericValue)c["id"]).GetInt32(),
                name = ((JString)c["name"]).GetValue(),
                description = ((JString)c["description"]).GetValue(),
            })
            .ToList();
    }
}