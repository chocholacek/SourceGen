using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FastEnumStringBenchmarks;
using Generated;


BenchmarkRunner.Run<Benchmarks>();


namespace FastEnumStringBenchmarks
{
    public enum TestEnum { Q, W, E, R, T, Y }

    [MemoryDiagnoser]
    [RPlotExporter]
    public class Benchmarks
    {

        [ParamsAllValues]
        public TestEnum Value;

        [Benchmark]
        public void StandardEnumToString()
        {
            Value.ToString();
        }

        [Benchmark]
        public void FastEnumToString()
        {
            Value.ToStringFast();
        }
    }
}
