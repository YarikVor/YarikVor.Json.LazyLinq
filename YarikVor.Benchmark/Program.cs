using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Security.Cryptography;
using Newtonsoft.Json;
using YarikVor.Benchmark;
using YarikVor.Json.LazyLinq.JEnitites;
using YarikVor.Json.LazyLinq.JEnitites.Queryable;

public class BenchmarkTest
{

    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }

        public string description { get; set; }
        public IEnumerable<string> tags { get; set; }
    }
    
    public static void Main()
    {
        //var obj = JDocument.GetAndConvertedSkipSpace(json, 0);

        var text = File.ReadAllText("data.json");
        var data = JSerializer.Deserialize<List<Item>>(text);
        
        /*var a = new JsonDeserializationBenchmarkList_2();
        var sum = 0;

        for (int i = 0; i < 1000000; i++)
        {
            var result = a.MyCustomJsonParser_UseExpression();
            sum += result.Count;
        }
        Console.WriteLine(sum);
        return;*/
        var summary = BenchmarkRunner.Run<JsonDeserializationBenchmarkList_2>();
        return;
    }
}