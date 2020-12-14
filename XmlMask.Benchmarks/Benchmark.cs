using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace XmlMask.Benchmarks
{
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    [GcForce(value: false)]
    [MediumRunJob]
    public class Benchmark
    {
        private string data;
        private static Dictionary<string, string> _contents = new Dictionary<string, string>
        {
            ["example-small.xml"] = File.ReadAllText("example-small.xml"),
            ["example.xml"] = File.ReadAllText("example.xml"),
            ["example-large.xml"] = File.ReadAllText("example-large.xml")
        };

        private static XmlMasker _xmlMasker = new XmlMasker();

        [Params("example-small.xml", "example.xml", "example-large.xml")]
        public string FileName { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            data = _contents[FileName];

            XmlMasker.CharArrayPool = ArrayPool<char>.Create(10 * 1024 * 1024, 5);
        }

        [Benchmark]
        public string XmlMasker_MaskByElementName() => _xmlMasker.MaskByElementName(data, "population");

        [Benchmark]
        public string XmlMasker_MaskByUnknownElementName() => _xmlMasker.MaskByElementName(data, "unknownElement");

        [Benchmark]
        public string XmlMasker_MaskByXpath() => _xmlMasker.MaskByXPath(data, "//*[local-name() = 'population']");

        [Benchmark]
        public string RegEx_Replace()
            => Regex.Replace(data, @"(<population.+>)[\s\S]*?(</population>)", "$1***$2");


        private static XslCompiledTransform _xslt = CreateXslCompiledTransform();

        private static XslCompiledTransform CreateXslCompiledTransform()
        {
            var xslt = new XslCompiledTransform();
            xslt.Load("example.xsl");
            return xslt;
        }

        [Benchmark]
        public string XSLT()
        {
            using (var textReader = new StringReader(data))
            using (var textWriter = new StringWriter())
            {
                var xmlReader = XmlReader.Create(textReader);
                var xPathDocument = new XPathDocument(xmlReader, XmlSpace.Preserve);
                var xmlWriter = new XmlTextWriter(textWriter)
                {
                    Formatting = Formatting.None
                };

                _xslt.Transform(xPathDocument, null, xmlWriter, null);

                return textWriter.ToString();
            }
        }

        [Benchmark]
        public string XDocument_ParseAndSerialize()
        {
            var document = XDocument.Parse(data);
            foreach (var node in document.Descendants().Where(e => e.Name.LocalName == "population"))
                node.Value = "***";
            return document.ToString();
        }
    }
}
