using BenchmarkDotNet.Attributes;
using JsonMasking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace JsonMask.Benchmarks
{
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    [GcForce(value: false)]
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
            JsonMasker.CharArrayPool = ArrayPool<char>.Create(10 * 1024 * 1024, 5);
        }

        [Benchmark]
        public string JsonMasker_MaskByPath() => _jsonMasker.Mask(data, f => f.Name == "name" && f.GetPath().Contains(".friends["));

        [Benchmark]
        public string JsonMasker_MaskByPropertyName() => _jsonMasker.MaskByPropertyName(data, "name");

        [Benchmark]
        public string RegEx_ReplaceByPropertyName() => MaskByRegex(data, "name");

        private static string MaskByRegex(string json, params string[] propertyName) 
            => Regex.Replace(json, "(" + string.Join("|", propertyName.Select(p => "\"" + p + "\"\\s*:\\s*\"")) + ")(.*?)(\")", "$1***$3");

        [Benchmark]
        public string JObject_ParseAndSerialize() => JObject.Parse(data).ToString(Formatting.None);

        [Benchmark]
        public string JsonMasking_MaskFieldsByPath() => data.MaskFields(new[] { "*.friends[*.name" }, "***");
    }
}
