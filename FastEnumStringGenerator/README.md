# EnumToStringGenerator

Analyzer generating faster implementation of `Enum.ToString()`

## Prerequisites

1. .NET 5 SDK

## Quick start

1. Download source code
2. Reference it in a project (don't forget to add `OutputItemType="Analyzer" ReferenceOutputAssembly="false"`)
3. Add `OutputItemType="Analyzer" ReferenceOutputAssembly="false"` to it's `ProjectReference` e.g.
    ```xml
    <ProjectReference Include="..\FastEnumStringGenerator\FastEnumStringGenerator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false"/>
    ```
4. Enums in your project now contain `Enum.ToStringFast()`

## Purpose

`Enum.ToString()` can be done faster without any allocations by using constants.
For further solution, consider having an simple example `Value` enum
```csharp
public enum Value
{
    X,
    Y,
    Z
}
```

## Proposed solution

Construct extension fuction which returns constant value for `switch`, e.g.
```csharp
public static string ToStringFast(this Value val)
{
    switch (val)
    {
        case Value.X:
            return $"{nameof(Value.X)}";
        case Value.Y:
            return $"{nameof(Value.Y)}";
        case Value.Z:
            return $"{nameof(Value.Z)}";
        default:
            throw new System.NotSupportedException();
    }
}
```

Writing this code for every single enum in any project would be unreasonable, hence generating these functions by source generators intorduced in .NET 5 seems like a good match.

## Performance of the solution

Simple benchmarks, which were run to test this solution can be found [here](../FastEnumStringBenchmarks/).

Benchmarks were run on

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.1165 (20H2/October2020Update)
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=5.0.400
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  DefaultJob : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
```

With following results:

|               Method | Value |      Mean |     Error |    StdDev |  Gen 0 | Allocated |
|--------------------- |------ |----------:|----------:|----------:|-------:|----------:|
| **StandardEnumToString** |     **Q** | **32.262 ns** | **0.5707 ns** | **0.5339 ns** | **0.0057** |      **24 B** |
|     FastEnumToString |     Q |  2.174 ns | 0.0252 ns | 0.0211 ns |      - |         - |
| **StandardEnumToString** |     **W** | **33.555 ns** | **0.5733 ns** | **0.5362 ns** | **0.0057** |      **24 B** |
|     FastEnumToString |     W |  2.142 ns | 0.0160 ns | 0.0149 ns |      - |         - |
| **StandardEnumToString** |     **E** | **30.322 ns** | **0.3767 ns** | **0.3339 ns** | **0.0057** |      **24 B** |
|     FastEnumToString |     E |  2.154 ns | 0.0239 ns | 0.0224 ns |      - |         - |
| **StandardEnumToString** |     **R** | **33.187 ns** | **0.4530 ns** | **0.4237 ns** | **0.0057** |      **24 B** |
|     FastEnumToString |     R |  1.945 ns | 0.0408 ns | 0.0362 ns |      - |         - |
| **StandardEnumToString** |     **T** | **31.341 ns** | **0.5336 ns** | **0.4730 ns** | **0.0057** |      **24 B** |
|     FastEnumToString |     T |  1.985 ns | 0.0446 ns | 0.0395 ns |      - |         - |
| **StandardEnumToString** |     **Y** | **32.570 ns** | **0.5161 ns** | **0.4827 ns** | **0.0057** |      **24 B** |
|     FastEnumToString |     Y |  1.927 ns | 0.0241 ns | 0.0202 ns |      - |         - |

Reviewing the results, `Enum.ToStringFast()` does not allocate any memory. Averaging and approximating `StandardEnumToString` results to ~32 ns and `FastEnumToString` results to ~2.1 ns, it is confirmed that `Enum.ToStringFast()` runs approximately **15 times** faster than `Enum.ToString()`. 

Visually:

<img src="media/FastEnumStringBenchmarks.Benchmarks-barplot.png" alt="drawing" width="800"/>


