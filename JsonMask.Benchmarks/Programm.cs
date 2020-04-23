using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace JsonMask.Benchmarks
{
    class Programm
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmark>(/*new DebugInProcessConfig()*/);
        }
    }
}
