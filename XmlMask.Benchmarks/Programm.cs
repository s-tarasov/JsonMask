using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;
using System.Reflection;

namespace XmlMask.Benchmarks
{
    class Programm
    {
        public static void Main(string[] args)
        {
#if DEBUG
            var benchmark = new Benchmark();
            benchmark.FileName = "example-small.xml";
            benchmark.GlobalSetup();
            foreach (var method in typeof(Benchmark)
                .GetMethods()
                .Where(m => m.GetCustomAttribute<BenchmarkAttribute>() != null)) 
            {
                var methodReturn = method.Invoke(benchmark, Array.Empty<object>());
                Console.WriteLine($"{method.Name} : {methodReturn}");
            }

#else
            var summary = BenchmarkRunner.Run<Benchmark>();
#endif
        }

    }
}
