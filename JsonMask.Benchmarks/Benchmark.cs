using BenchmarkDotNet.Attributes;
using JsonMasking;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace JsonMask.Benchmarks
{
    [MemoryDiagnoser]
    public class Benchmark
    {
        private string data;
        private static Dictionary<string, string> _contents = new Dictionary<string, string>
        {
            ["example-small.json"] = File.ReadAllText("example-small.json"),
            ["example.json"] = File.ReadAllText("example.json"),
            ["example-large.json"] = File.ReadAllText("example-large.json")
        };

        private static JsonMasker _jsonMasker = new JsonMasker();

        [Params("example-small.json", "example.json", "example-large.json")]
        public string FileName { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            data = _contents[FileName];
        }

        [Benchmark]
        public string JsonMasker() => _jsonMasker.Mask(data, f => f.Name == "name" && f.GetPath().Contains(".friends["));

        [Benchmark]
        public string RegExMasker()
        {
            return Mask(data, "name");

            string Mask(string json, params string[] propertyName)
                => Regex.Replace(json, "(" + string.Join("|", propertyName.Select(p => "\"" + p + "\"\\s*:\\s*\"")) + ")(.*?)(\")", "$1***$3");
        }

        [Benchmark]
        public string ParseJToken()
        {
            var obj = JObject.Parse(data);
            return "fake";
        }

        static string[] blacklist = new[] { "*.friends[*.name" };

        [Benchmark]
        public string MaskFields()
        {
            return data.MaskFields(blacklist, "***");
        }
    }
}
