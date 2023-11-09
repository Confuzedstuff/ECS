using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Benchmarks;



var arr = new float[2];

Console.WriteLine(arr.Length);
Array.Resize(ref arr, 3);
Console.WriteLine(arr.Length);
// var summary = BenchmarkRunner.Run<MemoryBenchmarkerDemo>();
